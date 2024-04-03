using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.ModaMailBox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;

namespace Management.Areas.MailBox.Controllers
{
    [Area("MailBox")]
    public class SpeedLogController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public SpeedLogController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// api Log
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(31);
            if (!CheckUserMenu(31).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }

            return View();
        }

        public IActionResult List(string cn , string strDate, string endDate, int p = 1, int displayCount = 10 )
        {
            var ListModels = new Models.SpeedLog.ListModel();
            var websiteId = "";
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = displayCount;
            pager.p = p;
            var lists = MailBoxService.GetSpeedLog(websiteId, strDate, endDate, cn, ref pager);

            ListModels.defaultPager = pager;
            ListModels.SpeedLogs = lists;
            return View(ListModels);
        }

        public IActionResult ExcelReport(string strDate, string endDate , string cn)
        {
            var websiteId = "";
            var fileName = $@"API紀錄表{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx";
            try
            {
                var path = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                DefaultPager pager = new DefaultPager();

                var list = MailBoxService.GetSpeedLog(websiteId, strDate, endDate,cn);
                var filePath = $@"{path}/Temp/UserReport.xlsx";
                var Info = new List<string>() {
                    $@"製表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                    $@"製表人: {UserData?.sysUser.UserName}",
                    $@"資料筆數: {list.Count}"
                };
                var DetailTitle = new List<string>() {
                    "案件編號",
                    "API請求",
                    "請求狀態",
                    "請求內容",
                    "API回應狀態",
                    "API回應訊息",
                    "API回應內容",
                    "請求時間",
                    "使用者"
                };
                var EModel = new ExcelModel();
                EModel.Title = "民意信箱API紀錄表";
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;
                EModel.ExcelDetails = list.Select(x => new ExcelDetailModel()
                {
                    a = x.CaseNo,
                    b = x.Action,
                    c = x.Success,
                    d = x.Requset,
                    e = x.ApiStatus,
                    f = x.ApiMessage,
                    g = x.Message,
                    h = x.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                    i = x.CreateUser
                }
                ).ToList();
                excelDatas = Utility.Output.ExampleReport(EModel, filePath);

                return File(excelDatas.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
            }
            catch (Exception ex)
            {
                IList<string> errmsgs = new List<string>();
                string error = "";
                error = ex.Message;
                errmsgs.Add(error);
                return File(System.Text.Encoding.UTF8.GetBytes(error), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
            }
        }
    }
}

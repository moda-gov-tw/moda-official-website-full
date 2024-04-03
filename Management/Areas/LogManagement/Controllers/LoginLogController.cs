using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.LogManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
namespace Management.Areas.LogManagement.Controllers
{
    [Area("LogManagement")]
    public class LoginLogController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public LoginLogController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(18);
            if (!CheckUserMenu(18).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" });}
            return View();
        }
        public IActionResult List(string mon = "", string userid = "", string ip = "", int p = 1, int DisplayCount = 10)
        {
            string str = mon + "-01 00:00:00";
            string end = Convert.ToDateTime(str).AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd 23:59:59").ToString();

            Models.LoginLog.ListModel listModel = new Models.LoginLog.ListModel();

            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;

            listModel.sysUserLogins = LoginLogService.GetUserLogins(str, end, ref pager, false, userid, ip);
            listModel.UserData = LoginLogService.GetUserName(listModel.sysUserLogins);
            listModel.defaultPager = pager;

            return View(listModel);
        }
        public FileContentResult ExcelReport(string mon = "", string userid = "", string ip = "")
        {
            var fileName = $@"使用者登入記錄{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx";
            try
            {
                string str = mon + "-01 00:00:00";
                string end = Convert.ToDateTime(str).AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd 23:59:59").ToString();

                var path = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                DefaultPager pager = new DefaultPager();

                var list = LoginLogService.GetUserLogins(str, end, ref pager, true, userid, ip);
                var UserName = LoginLogService.GetUserName(list);
                var filePath = $@"{path}/Temp/ExampleReport.xlsx";
                var Info = new List<string>() {
                $@"建表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"建表人: {UserData?.sysUser.UserName}",
                $@"資料筆數: {list.Count()}"
            };
                var DetailTitle = new List<string>() {
                "帳號",
                "使用者名稱",
                "狀態",
                "訊息",
                "IP",
                "時間"
            };
                var EModel = new ExcelModel();
                EModel.Title = "使用者登入記錄";
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;
                EModel.ExcelDetails = list.Select(x => new ExcelDetailModel()
                {
                    a = x.UserID,
                    b = UserName.Where(x => x.UserID == x.UserID).FirstOrDefault()?.UserName,
                    c = x.Status == "1" ? "成功" : "失敗",
                    d = x.Message,
                    e = x.ProcessIPAddress,
                    f = x.CreatedDate.ToString("G")
                }
                ).ToList();
                excelDatas = Utility.Output.ExampleReport(EModel, filePath);

                return File(excelDatas.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
            }
            catch(Exception ex)
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

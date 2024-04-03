using Management.Areas.LogManagement.Models.ErrorLog;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
namespace Management.Areas.LogManagement.Controllers
{
    [Area("LogManagement")]
    public class ErrorLogController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ErrorLogController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(16);
            var chkUM = CheckUserMenu(16);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area = "" });
            }

            return View();
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult List(string mon, int p = 1, int DisplayCount = 15)
        {
            string str = mon + "-01 00:00:00";
            string end = Convert.ToDateTime(str).AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd 23:59:59").ToString();

            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;

            var data = OperationStatisticsService.GetLogData(str, end, ref pager, 2);

            ListModel listModel = new ListModel();
            listModel.defaultPager = pager;
            listModel.operationStatisticsModels = data;

            return View(listModel);
        }
        /// <summary>
        /// 匯出紀錄
        /// </summary>
        /// <param name="mon"></param>
        /// <returns></returns>
        public IActionResult Export(string mon)
        {
            var fileName = $@"系統錯誤紀錄{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx";
            try
            {
                string str = mon + "-01 00:00:00";
                string end = Convert.ToDateTime(str).AddMonths(1).AddSeconds(-1).ToString("yyyy-MM-dd 23:59:59").ToString();

                var path = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                var list = OperationStatisticsService.GetLogData2(str, end, 2);
                var filePath = $@"{path}/Temp/ExampleReport.xlsx";
                var Info = new List<string>() {
                $@"建表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"建表人: {UserData?.sysUser.UserName}",
                $@"資料筆數: {list.Count()}",
             $@"月份：{mon}"
            };
                var DetailTitle = new List<string>() {
                "帳號",
                "部門",
                "IP",
                "異動狀態",
                "資料異動說明",
                "錯誤資訊",
                "資料表",
                "資料編號",
                "完整路徑",
                "時間"
            };
                var EModel = new ExcelModel();
                EModel.Title = "系統錯誤紀錄";
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;
                EModel.ExcelDetails = list.Select(x => new ExcelDetailModel()
                {
                    a = x.UserID,
                    b = x.DepartmentName,
                    c = x.ProcessIPAddress,
                    d = x.Action2,
                    e = x.MessageInput,
                    f = x.MessageResult,
                    g = x.SourceTable,
                    h = x.SourceSN.ToString(),
                    i = x.Webpath,
                    j = x.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")
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

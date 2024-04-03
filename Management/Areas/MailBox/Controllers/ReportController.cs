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
    public class ReportController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public ReportController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// 民意信箱報表
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var websiteId = UserData.WebSiteID;
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(29);
            if (!CheckUserMenu(29).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }

            var viewModel = new Management.Areas.MailBox.Models.Report.IndexModel();
            viewModel.casesModels = MailBoxService.GetCases("0").Where(x => x.WebSiteID == websiteId).ToList();

            return View(viewModel);
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="strDate"></param>
        /// <param name="endDate"></param>
        /// <param name="strDate2"></param>
        /// <param name="endDate2"></param>
        /// <param name="dep"></param>
        /// <param name="p"></param>
        /// <param name="displayCount"></param>
        /// <returns></returns>
        public IActionResult List(string strDate, string endDate, string strDate2, string endDate2, string caseclass, string dep, string originalDep, string replysource, int p = 1, int displayCount = 10)
        {
            int.TryParse(dep, out int intDep);
            int.TryParse(originalDep, out int intOriginalDep);
            int.TryParse(caseclass, out int _caseclass);
            var ListModels = new Management.Areas.MailBox.Models.Report.ListModel();
            var websiteId = UserData.WebSiteID;
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = displayCount;
            pager.p = p;
            var lists = MailBoxService.GetReport(websiteId, strDate, endDate, strDate2, endDate2, _caseclass, intDep, intOriginalDep, replysource, ref pager);
            ListModels.defaultPager = pager;
            ListModels.ReportModel = lists;
            return View(ListModels);
        }

        public IActionResult ExcelReport(string strDate, string endDate, string strDate2, string endDate2, string caseclass, string dep, string originalDep, string replysource)
        {
            int.TryParse(dep, out int intDep);
            int.TryParse(originalDep, out int intOriginalDep);
            int.TryParse(caseclass, out int _caseclass);
            var websiteId = UserData.WebSiteID;
            var fileName = $@"民意信箱統計報表{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx";
            try
            {
                var path = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                DefaultPager pager = new DefaultPager();

                var list = MailBoxService.GetReport(websiteId, strDate, endDate, strDate2, endDate2, _caseclass, intDep, intOriginalDep, replysource);
                var filePath = $@"{path}/Temp/UserReport.xlsx";
                var Info = new List<string>() {
                $@"製表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"製表人: {UserData?.sysUser.UserName}",
                $@"資料筆數: {list.Count}"
            };
                var DetailTitle = new List<string>() {
                "案號",
                "意見分類",
                "承辦單位",
                "主旨",
                "成立時間",
                "狀態",
                "回覆時間",
                "回覆狀態"
            };
                var EModel = new ExcelModel();
                EModel.Title = "民意信箱統計報表";
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;
                EModel.ExcelDetails = list.Select(x => new ExcelDetailModel()
                {
                    a = x.CaseNo,
                    b = x.ClassName,
                    c = x.depName,
                    d = x.Subject,
                    e = x.AcceptDate?.ToString("yyyy-MM-dd HH:mm"),
                    f = EnumTpye.GetEnumDescription(EnumTpye.GetEnum<Utility.MailBox.EnumCassApplyStatus>(x.Status)),
                    g = x.ReplyDate?.ToString("yyyy-MM-dd HH:mm"),
                    h = EnumTpye.GetEnumDescription(EnumTpye.GetEnum<Utility.MailBox.EnumReplySource>(x.ReplySource))
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

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
    public class SurveyController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public SurveyController(IWebHostEnvironment hostingEnvironment)
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
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(33);
            if (!CheckUserMenu(33).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }

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
        public IActionResult List(string strDate, string endDate, int p = 1, int displayCount = 10)
        {
            try
            {
                var ListModels = new Management.Areas.MailBox.Models.Survey.ListModel();
                var websiteId = UserData.WebSiteID;
                DefaultPager pager = new DefaultPager();
                pager.DisplayCount = displayCount;
                pager.p = p;
                var lists = MailBoxService.GetSurvey(websiteId, strDate, endDate, ref pager);

                ListModels.defaultPager = pager;
                ListModels.surveys = lists;
                ListModels.statistics = MailBoxService.GetStatistics(websiteId, strDate, endDate) ?? new Services.Models.ModaMailBox.Statistics();
                ListModels.DateRange = !string.IsNullOrEmpty(strDate) || !string.IsNullOrEmpty(endDate) ? $"{strDate}~{endDate}" : "-";
                return View(ListModels);
            }
            catch (Exception ex) 
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "資料異常請聯絡系統工程師");
            }
        }

        public IActionResult ExcelReport(string strDate, string endDate)
        {
            var websiteId = UserData.WebSiteID;
            var fileName = $@"滿意度調查報表{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx";
            try
            {
                var path = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                DefaultPager pager = new DefaultPager();
                
                var list = MailBoxService.GetSurvey(websiteId, strDate, endDate);
                var filePath = $@"{path}/Temp/UserReport.xlsx";
                var Info = new List<string>() {
                $@"製表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"製表人: {UserData?.sysUser.UserName}",
                $@"查詢日期區間: {strDate } ~ {endDate}  ",
                $@"回收問卷數: {list.Count}",
            };
                var DetailTitle = new List<string>() {
                "案號",
                "答覆內容滿意度",
                "問題是否解決",
                "不滿意之項目",
                "其他建議事項",
                "填表時間",
            };
                var EModel = new ExcelModel();
                EModel.Title = "滿意度調查報表";
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;
                EModel.ExcelDetails = list.Select(x => new ExcelDetailModel()
                {
                    a = x.CaseNo,
                    b = x.CaseSatisfy,
                    c = x.CaseSolved,
                    d = x.CaseDefect,
                    e = x.CaseProposal,
                    f = x.CreateDate.ToString("yyyy-MM-dd HH:mm")
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

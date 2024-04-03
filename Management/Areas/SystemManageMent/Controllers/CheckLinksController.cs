using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models.WebManagement;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Utility;

namespace Management.Areas.SystemManageMent.Controllers
{
    [Area("SystemManageMent")]
    public class CheckLinksController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CheckLinksController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(24);
            if (!CheckUserMenu(24).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            return View();
        }

        public IActionResult List(string websiteid = "", int p = 1, int DisplayCount = 10)
        {
            Models.CheckLinks.ListModel listModel = new Models.CheckLinks.ListModel();

            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;

            listModel.sysWebSites = WebsiteManagementService.GetSysWebSiteByWebSiteID(websiteid, ref pager);
            listModel.defaultPager = pager;
            return View(listModel);
        }

        public IActionResult GetLinks(string websiteid)
        {
            var links = CheckLinksService.GetCheckingLinks(websiteid).Where(x => x.URL.StartsWith("http")).ToList();
            CheckLinksService.UpdateCheckLinksDate(websiteid);
            var json = JsonSerializer.Serialize(links);
            return StatusResult(System.Net.HttpStatusCode.OK, json);
        }

        public IActionResult CreateExportFile(List<CheckLinkModel.CheckingLink> links, string websiteid)
        {
            try
            {
                var exportLinks = CheckLinksService.GetExportLinks(links);

                var fileName = $@"無效連結掃描{websiteid}{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx";
                var path = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();

                var filePath = $@"{path}/Temp/CheckLinksReport.xlsx";
                var Info = new List<string>() {
                    $@"建表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                    $@"建表人: {UserData?.sysUser.UserName}",
                    $@"資料筆數: {links?.Count() ?? 0}"
                };
                var DetailTitle = new List<string>() {
                    "序號",
                    "後台路徑",
                    "網址標題",
                    "無效連結名稱",
                    "無效連結URL",
                    "來源網址",
                    "維護單位",
                    "錯誤代碼"
                };
                var EModel = new ExcelModel();
                EModel.Title = $"無效連結掃描{websiteid}";
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;
                EModel.ExcelDetails = exportLinks.Select((x,index) => new ExcelDetailModel()
                {
                    a = (index + 1).ToString(),
                    b = x.Breadcrumb,
                    c = x.PageTitle,
                    d = x.Title,
                    e = x.URL,
                    f = x.WebURL,
                    g = x.DeptName,
                    h = x.error
                }
                ).ToList();
                excelDatas = Utility.Output.ExampleReport(EModel, filePath);


                SetSession("file", Convert.ToBase64String(excelDatas.ToArray()));

                excelDatas.Dispose();

                return StatusResult(System.Net.HttpStatusCode.OK, fileName);
            }
            catch (Exception ex)
            {
                IList<string> errmsgs = new List<string>();
                string error = "";
                error = ex.Message;
                errmsgs.Add(error);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, error);
            }
        }

        public IActionResult Export(string fileName)
        {
            byte[] data = Convert.FromBase64String(GetSession<string>("file"));

            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
    }
}

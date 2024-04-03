using Management.Areas.SystemManageMent.Models.FilePath;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;

namespace Management.Areas.SystemManageMent.Controllers
{

    [Area("SystemManageMent")]
    public class FilePathController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FilePathController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(36);
            if (!CheckUserMenu(36).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            return View();
        }

        public IActionResult List(string classType, string key = "", string websiteid = "", string type = "", int p = 1, int DisplayCount = 10)
        {
            FilePathViewModel filePathViewModel = new();
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            var data = BrowseStatisticsService.GetFilePath(classType, key, websiteid, type, ref pager);
            filePathViewModel.filePaths = data.Select(x => new FilePathModel()
            {
                FileTitle = x.FileTitle,
                FilePath = x.Path
            }).ToList();
            filePathViewModel.defaultPager = pager;
            return View(filePathViewModel);
        }
        public FileContentResult Export(string classType, string key = "", string websiteid = "", string type = "")
        {
            string _calssTpye = "類型：";
            string _key = "關鍵字：";
            string _websiteid = "站台：";
            string _type = "檔案類型：";
            switch (classType)
            {
                case "1": _calssTpye = $@"{_calssTpye}連結"; break;
                case "2": _calssTpye = $@"{_calssTpye}檔案"; break;
            }
            if (!string.IsNullOrWhiteSpace(key)) _key = $@"{_key}{key}";
            switch (websiteid)
            {
                case "MODA": _websiteid = $@"{_websiteid}數位發展部"; break;
                case "ADI": _websiteid = $@"{_websiteid}數位產業署"; break;
                case "ACS": _websiteid = $@"{_websiteid}資通安全署"; break;
            }
            if (!string.IsNullOrWhiteSpace(type)) _type = $@"{_type}{type}";
            var excelDatas = new MemoryStream();
            var fpath = _hostingEnvironment.WebRootPath;
            var filePath = $@"{fpath}/Temp/ExampleReport.xlsx";
            DefaultPager pager = new DefaultPager();
            pager = null;
            var data = BrowseStatisticsService.GetFilePath(classType, key, websiteid, type, ref pager);
            var info = new List<string>() {
                $@"建表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"建表人: {UserData?.sysUser.UserName}",
                _calssTpye,
                _key,
                _websiteid,
                _type
                };
            var title = "(檔案/圖檔/連結)查詢";
            var detailTitle = new List<string>() { "序號", "名稱", "麵包屑" };
            var excelModel = new ExcelModel();
            excelModel.Info = info;
            excelModel.Title = title;
            excelModel.SheetTitle = title;
            excelModel.DetailTitle = detailTitle;
            var i = 1;
            excelModel.ExcelDetails = data.Select(x => new ExcelDetailModel()
            {
                a = (i++).ToString(),
                b = x.FileTitle,
                c = x.Path
            }).ToList();
            excelDatas = Utility.Output.ExampleReport(excelModel, filePath);
            return File(excelDatas.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"(檔案/圖檔/連結)查詢{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx");
        }
    }
}

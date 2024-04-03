using DBModel;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.ModaMailBox;
using System;
using System.Collections.Generic;
using System.Linq;
using static Utility.Files;

namespace Management.Areas.MailBox.Controllers
{
    [Area("MailBox")]
    public class WebSiteController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public WebSiteController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(26);
            if (!CheckUserMenu(26).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            SetSession("WEBFile", new List<CommonFileModel>());
            var IndexModel = new Models.WebSite.IndexModel();
            IndexModel.caseApplyWeb = MailBoxService.GetCaseApplyWeb();
            var files = CommonUtility.GetFileByDB(IndexModel.caseApplyWeb.CaseApplyWebSN.ToString(), "CaseApplyWeb");
            if (files?.Count > 0)
            {
                foreach (var f in files)
                {
                    f.lan = "zh-tw";
                }
                SetSession("WEBFile", files);
                IndexModel.commonFileModels = files;
            }
            return View(IndexModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(CaseApplyWeb caseApplyWeb, List<CommonFileModel> fileinfo = null)
        {
            caseApplyWeb.ProcessDate = DateTime.UtcNow.AddHours(8);
            caseApplyWeb.ProcessUserID = UserData.sysUser.UserID;
            caseApplyWeb.ProcessIPAddress = ContextModel.ProcessIpaddress;

            SetLogActionModel(webPath: "民意信箱/網站維護", Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "CaseApplyWeb");
            var files = GetSession<List<CommonFileModel>>("WEBFile");
            if (fileinfo.Count() > 0 )
            {
                foreach (var file in files)
                {
                    var fileModel = fileinfo.FirstOrDefault(x => x.fileNewName == file.fileNewName);
                    if (fileModel != null)
                    {
                        file.fileTitle = fileModel.fileTitle;
                        file.FileSort = fileModel.FileSort;
                    }
                }
            }
            var _chk = MailBoxService.EditGetApplyWeb(caseApplyWeb, fileinfo);
            if (_chk) {
                return StatusResult(System.Net.HttpStatusCode.OK,"");
            } else {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "資料有問題請重新輸入");
            }

        }
    }
}

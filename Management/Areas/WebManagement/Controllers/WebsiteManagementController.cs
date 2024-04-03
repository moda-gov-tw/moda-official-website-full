using DBModel;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services.Static;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using Utility;
using static Utility.Files;

namespace Management.Areas.WebManagement.Controllers
{
    [Area("WebManagement")]
    public class WebsiteManagementController : BaseController
    {
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(14);
            if (!CheckUserMenu(14).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            return View();
        }
        public IActionResult List(string websiteid = "", string sts = "", int p = 1, int DisplayCount = 10)
        {
            Models.WebsiteManagement.ListModel listModel = new Models.WebsiteManagement.ListModel();

            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;

            listModel.sysWebSites = WebsiteManagementService.GetSysWebSiteByWebSiteID(websiteid, ref pager);
            listModel.defaultPager = pager;
            return View(listModel);
        }

        public IActionResult Mode(string key = "",string websiteid = "")
        {
            SetSession("WEBFile", new List<CommonFileModel>());
            Models.WebsiteManagement.ModeModel editModel = new Models.WebsiteManagement.ModeModel();
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (CommonUtility.UrlKey(ref key))
                {
                    var files = CommonUtility.GetFileByDB(key, "SysWebSite");
                    if (files != null)
                    {
                        SetSession("WEBFile", files);
                        editModel.commonFileModels = files;
                    }
                    editModel.sysWebSiteLangs = WebsiteManagementService.GetSysWebSiteID(websiteid);
                }
            }
            else
            {
                List<SysWebSiteLang> SysWebSiteLangs = new List<SysWebSiteLang>();
                SysWebSiteLangs.Add(new SysWebSiteLang() { Lang = "zh-tw" });
                editModel.sysWebSiteLangs = SysWebSiteLangs;
            }

            return View(editModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(SysWebSiteLang sysWebSites, List<CommonFileModel> fileinfo = null, List<WebSiteExtend> webExtend = null)
        {
            sysWebSites.GACode = sysWebSites.GACode;
            sysWebSites.ProcessDate = DateTime.UtcNow.AddHours(8);
            sysWebSites.ProcessUserID = UserData.sysUser.UserID;
            sysWebSites.ProcessIPAddress = UserData.sysUser.ProcessIPAddress;
            bool soure = true;
            if (sysWebSites.SysWebSiteLangSN == 0)
            {
                sysWebSites.CreatedDate = DateTime.UtcNow.AddHours(8);
                sysWebSites.CreatedUserID = UserData.sysUser.UserID;
                soure = WebsiteManagementService.Create(sysWebSites);
            }
            else
            {
                soure = WebsiteManagementService.Edit(sysWebSites, fileinfo, webExtend);
            }

            if (soure)
            {
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗");
            }
        }

        public IActionResult Delete(string key)
        {
            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key))
            {
                if (WebsiteManagementService.Delete(_key))
                {
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "刪除失敗");
                }
            }
            else
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "刪除失敗");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetStaticLink(string key)
        {
            if (CommonUtility.UrlKey(ref key))
            {
                try
                {
                    StaticLinkService.ResetStaticLink(key);
                    return StatusResult(System.Net.HttpStatusCode.OK, "請等待下一次更新");
                }
                catch (Exception ex)
                {
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗");
                }

            }
            return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗");
        }
    }

}

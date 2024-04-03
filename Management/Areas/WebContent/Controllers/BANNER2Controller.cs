using Management.Areas.WebContent.Models;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using System.Collections.Generic;
using System.Linq;
using static Utility.Files;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class BANNER2Controller : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.BANNERModel bannerModel = new Models.BANNERModel();
                bannerModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                bannerModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                bannerModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;

                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        bannerModel.wEBNews = WebLevelManagementService.GetWEBNew(_key2);
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        foreach (var langData in LangDataList)
                        {
                            var CommonModel = new NewCommonModel();
                            CommonModel.webNews = langData;
                            CommonModel.WebsiteID = langData.WebSiteID;
                            var files = CommonUtility.GetFileByDB(langData.WEBNewsSN.ToString(), "WEBNews");
                            if (files != null)
                            {
                                foreach (var f in files)
                                {
                                    f.lan = langData.Lang;
                                }
                                CommonModel.commonFileModels = files;
                                fileData.AddRange(files);
                            }
                            CommonModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                            bannerModel.newCommonModels.Add(CommonModel);
                        }
                        SetSession("WEBFile", fileData);
                        bannerModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);
                    }
                    else {
                        return View(null);
                    }
                }
                else
                {
                    bannerModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }
                return View(bannerModel);
            }
            return View(null);
        }
    }
}

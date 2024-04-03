using Management.Areas.WebContent.Models;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using System.Collections.Generic;
using static Utility.Files;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class OpendataNewsController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            string WEBSiteUrl = "https://" + new System.Uri(AppSettingHelper.GetAppsetting("WEBAPI")).Host + "/OpenData/Files/";
            if (int.TryParse(key, out _key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.OpenModel OpenModel = new Models.OpenModel();
                OpenModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                OpenModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);

                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        OpenModel.wEBNews = WebLevelManagementService.GetWEBNew(_key2);
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        foreach (var langData in LangDataList)
                        {
                            var CommonModel = new NewCommonModel();
                            CommonModel.webNews = langData;
                            var files = CommonUtility.GetFileByDB(langData.WEBNewsSN.ToString(), "WEBNews");
                            if (files != null)
                            {
                                foreach (var f in files)
                                {
                                    f.lan = langData.Lang;
                                }
                                CommonModel.commonFileModels = files;
                                fileData.AddRange(files);
                                CommonModel.webNews.URL = WEBSiteUrl + langData.WEBNewsSN;
                            }
                            CommonModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                            OpenModel.newCommonModels.Add(CommonModel);
                        }
                        SetSession("WEBFile", fileData);
                        OpenModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);
                    }
                    else
                    {
                        return View(null);
                    }
                }
                else
                {
                    OpenModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }
                return View(OpenModel);
            }
            return View(null);
        }
    }
}

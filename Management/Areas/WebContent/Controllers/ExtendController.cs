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
    public class ExtendController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.ExtendModel extendModel = new ExtendModel();
                extendModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                extendModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                extendModel.newCommonModels = new List<NewCommonModel>();
                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        extendModel.wEBNews = LangDataList.FirstOrDefault(x => x.WEBNewsSN == x.MainSN);
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
                            }
                            CommonModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                            extendModel.newCommonModels.Add(CommonModel);
                        }
                        SetSession("WEBFile", fileData);
                        extendModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);

                    }
                    else
                    {
                        return View(null);
                    }
                }
                else
                {
                    extendModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }
                return View(extendModel);
            }
            return View(null);
        }
    }
}

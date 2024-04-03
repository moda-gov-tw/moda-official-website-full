using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using System.Collections.Generic;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class TabController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                Models.TABModel tABModel = new Models.TABModel();
                tABModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                tABModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        tABModel.wEBNews = WebLevelManagementService.GetWEBNew(_key2);
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);

                        foreach (var langData in LangDataList)
                        {
                            var CommonModel = new Models.NewCommonModel();
                            CommonModel.webNews = langData;
                            CommonModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                            tABModel.newCommonModels.Add(CommonModel);

                            int rMSN = 0;
                            int.TryParse(langData.URL, out rMSN);
                            tABModel.relWebLevel.Add(WebLevelManagementService.GetWebLevel(rMSN));
                        }
                        tABModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);
                    }
                    else
                    {
                        return View(null);
                    }

                }
                else
                {
                    tABModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }

                return View(tABModel);
            }
            return View(null);
        }
    }
}

using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class TEXTController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {

            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                Models.TEXTModel textModel = new Models.TEXTModel();
                textModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                textModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                textModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        textModel.wEBNews = WebLevelManagementService.GetWEBNew(_key2);
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        foreach (var langData in LangDataList)
                        {
                            var CommonModel = new Models.NewCommonModel();
                            CommonModel.webNews = langData;
                            textModel.wEBNews = WebLevelManagementService.GetWEBNew(_key2);
                            CommonModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                            textModel.newCommonModels.Add(CommonModel);
                        }
                        textModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                    }
                    else
                    {
                        return View();
                    }

                }
                else
                {
                    textModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }
                return View(textModel);
            }
            return View();
        }
    }
}

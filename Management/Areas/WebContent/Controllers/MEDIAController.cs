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
    public class MEDIAController : BaseController
    {

        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.MEDIAModel mEDIAodel = new Models.MEDIAModel();
                mEDIAodel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                mEDIAodel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);

                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        mEDIAodel.wEBNews = WebLevelManagementService.GetWEBNew(_key2);
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        foreach (var langData in LangDataList)
                        {
                            var CommonModel = new Models.NewCommonModel();
                            CommonModel.webNews = langData;
                            var files = CommonUtility.GetFileByDB(langData.WEBNewsSN.ToString(), "WEBNews");
                            if (files != null)
                            {
                                foreach (var f in files)
                                {
                                    f.lan = langData.Lang;
                                }
                                CommonModel.commonFileModels = files;
                                CommonModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                                fileData.AddRange(files);
                            }
                            mEDIAodel.newCommonModels.Add(CommonModel);
                        }
                        mEDIAodel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);
                    }
                    else
                    {
                        mEDIAodel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                    }
                    return View(mEDIAodel);
                }
                else
                {
                    mEDIAodel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                    return View(mEDIAodel);
                }

            }
            return View(null);
        }

    }
}

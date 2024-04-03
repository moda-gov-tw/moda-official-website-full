using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Management.Areas.WebContent.Models;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using static Utility.Files;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class LINKController : BaseController
    {
        /// <summary>
        /// 資料頁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.LINKModel lINKModel = new Models.LINKModel();
                lINKModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                lINKModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                lINKModel.newCommonModels = new List<Models.NewCommonModel>();
                lINKModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        lINKModel.wEBNews = LangDataList.FirstOrDefault(x => x.WEBNewsSN == x.MainSN);
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
                            lINKModel.newCommonModels.Add(CommonModel);
                        }
                        SetSession("WEBFile", fileData);
                        lINKModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);
                    }
                    else {
                        return View(null);
                    }
                }
                else
                {
                    lINKModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }
                return View(lINKModel);
            }
            return View(null);

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModel;
using Management.Areas.WebContent.Models;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using static Utility.Files;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class CPController : BaseController
    {

        /// <summary>
        /// 資料頁
        /// </summary>
        /// <param name="key">WebLevelMSN</param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public IActionResult Index(string key = "", string key2 = "")
        {
            SetSession("WEBFile", new List<CommonFileModel>());
            var fileData = new List<CommonFileModel>();
            Models.CPModel viewModel = new Models.CPModel();
            if (CommonUtility.UrlKey(ref key))
            {
                viewModel.webLevel = WebLevelManagementService.GetWebLevel(int.Parse(key));
                viewModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                viewModel.AuthSysGroupWebLevels = UserData.webLevelAccessForGroups.Where(x => x.WebLevelSN == int.Parse(key)).ToList();
                var LangDataList = WebLevelManagementService.GetWebNewsByWebLevelSN(int.Parse(key));
                viewModel.wEBNews = LangDataList.FirstOrDefault(x => x.WEBNewsSN == x.MainSN);
                viewModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                foreach (var langData in LangDataList)
                {
                    var CommonModel = new NewCommonModel();
                    CommonModel.webNews = langData;
                    CommonModel.wEBNewsExtends = WebLevelManagementService.GetWEBNewsExtends(langData.WEBNewsSN);
                    CommonModel.AuthSysGroupWebLevels = UserData.webLevelAccessForGroups.Where(x => x.WebLevelSN == int.Parse(key)).ToList(); ;
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
                    viewModel.newCommonModels.Add(CommonModel);
                }
                SetSession("WEBFile", fileData);
                viewModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(int.Parse(key));
                viewModel.WebSiteId = UserData.WebSiteID;
            }
            return View(viewModel);
        }
    }
}

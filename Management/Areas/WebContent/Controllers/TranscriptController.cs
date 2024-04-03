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
    public class TranscriptController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.TranscriptModel viewModel = new Models.TranscriptModel();
                DBModel.WEBNews wEBNews = new DBModel.WEBNews();
                wEBNews.DepartmentID = UserData.sysUser.DepartmentID;
                wEBNews.ArticleType = "0";
                viewModel.wEBNews = wEBNews;
                viewModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                viewModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                viewModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        foreach (var langData in LangDataList)
                        {
                            var CommonModel = new NewCommonModel();
                            CommonModel.webNews = langData;
                            CommonModel.wEBNewsExtends = WebLevelManagementService.GetWEBNewsExtends(langData.WEBNewsSN);
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
                        viewModel.wEBNews = LangDataList.First();
                        viewModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);
                    }
                    else
                    {
                        return View(null);
                    }
                }
                else
                {
                    viewModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }
                SetSession("WEBFile", fileData);
                viewModel.WebSiteId = UserData.WebSiteID;
                return View(viewModel);
            }
            return View(null);


        }
    }
}

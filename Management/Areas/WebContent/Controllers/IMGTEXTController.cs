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
    public class IMGTEXTController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.IMGTEXTModel imgtextModel = new IMGTEXTModel();
                imgtextModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                imgtextModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                imgtextModel.newCommonModels = new List<NewCommonModel>();
                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        imgtextModel.wEBNews = LangDataList.FirstOrDefault(x => x.WEBNewsSN == x.MainSN);
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
                            imgtextModel.newCommonModels.Add(CommonModel);
                        }
                        SetSession("WEBFile", fileData);
                        imgtextModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);

                    }
                    else {
                        return View(null);
                    }
                }
                else
                {
                    imgtextModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }
                return View(imgtextModel);
            }
            return View(null);
        }
    }
}

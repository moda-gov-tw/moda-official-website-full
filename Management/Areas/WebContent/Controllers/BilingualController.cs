using Microsoft.AspNetCore.Mvc;
using Services.Authorization;
using Services;
using DBModel;
using Services.WebManagement;
using System;
using Management.Areas.WebContent.Models;
using System.Linq;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;
using static Utility.SysConst;
using static Utility.Files;
using System.Collections.Generic;
using Management.ManagementUtility;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class BilingualController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            SetSession("WEBFile", new List<CommonFileModel>());
            var fileData = new List<CommonFileModel>();
            BilingualModel bilingualModel = new Models.BilingualModel();
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                bilingualModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                bilingualModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                bilingualModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        bilingualModel.twWEBNews = LangDataList.FirstOrDefault(x => x.Lang == "zh-tw");
                        bilingualModel.enWEBNews = LangDataList.FirstOrDefault(x => x.Lang == "en");
                        bilingualModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                        bilingualModel.wEBNews = LangDataList.FirstOrDefault(x => x.Lang == "zh-tw");
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
                            CommonModel.wEBNewsExtends = WebLevelManagementService.GetWEBNewsExtends(langData.WEBNewsSN);
                            bilingualModel.newCommonModels.Add(CommonModel);
                        }
                        SetSession("WEBFile", fileData);
                    }
                    else
                    {
                        return View();
                    }

                }

                bilingualModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                return View(bilingualModel);
            }
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainsn"></param>
        /// <param name="weblevelsn"></param>
        /// <param name="module"></param>
        /// <param name="twtitle"></param>
        /// <param name="entitle"></param>
        /// <param name="isenable"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult WEBNewsSave(int mainsn, int weblevelsn, string module, string twtitle, string entitle, string isenable, string startdate, string enddate)
        {
            //   SetLogActionModel(webPath: "網站維護/" + OperationStatisticsService.GetWebLevelTree(wEBNews.WebLevelSN).FirstOrDefault()?.Path, Action2: wEBNews.WEBNewsSN == 0 ? Utility.Model.LoginModel.Action2.insert : Utility.Model.LoginModel.Action2.update, SourceTable: "WEBNews");
            
            WEBNews TWwEBNews = GetNewsData(mainsn, weblevelsn, module, startdate, enddate, isenable);
            WEBNews ENwEBNews = GetNewsData(mainsn, weblevelsn, module, startdate, enddate, isenable);
            TWwEBNews.Title = twtitle;
            TWwEBNews.Lang = "zh-tw";
            ENwEBNews.Title = entitle;
            ENwEBNews.Lang = "en";
            if (string.IsNullOrWhiteSpace(TWwEBNews.Title) || string.IsNullOrWhiteSpace(ENwEBNews.Title))
            {
                return StatusResult(System.Net.HttpStatusCode.InternalServerError, "請輸入中英文詞彙");
            }

            if (WebLevelManagementService.SaveWebNewsBilingual(TWwEBNews, ENwEBNews))
            {
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                return StatusResult(System.Net.HttpStatusCode.InternalServerError, "發生異常，請晚點再嘗試");
            }


        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult RegulationsSave(WEBNewsExtend wEBNewsExtend , string RegulationsType)
        {
            wEBNewsExtend.GroupID = "Regulations";
            //SaveWebNewsBilingualRegulations
            if (RegulationsType == "1") {
                if (string.IsNullOrWhiteSpace(wEBNewsExtend.Column_1) || string.IsNullOrWhiteSpace(wEBNewsExtend.Column_2))
                {
                    return StatusResult(System.Net.HttpStatusCode.InternalServerError, "請輸入法規相關資訊");
                }
                if (wEBNewsExtend.Column_1.Trim().Length < 8 || wEBNewsExtend.Column_1.Trim().Substring(0, 8) != "https://")
                {
                    return StatusResult(System.Net.HttpStatusCode.InternalServerError, "法規連結請以https開頭");
                }
            }
            
            if (WebLevelManagementService.SaveWebNewsBilingualRegulations(wEBNewsExtend, RegulationsType))
            {
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                return StatusResult(System.Net.HttpStatusCode.InternalServerError, "發生異常，請晚點再嘗試");
            }

        }
        WEBNews GetNewsData(int mainsn , int weblevelsn , string module , string startdate , string enddate , string isenable)
        {
            WEBNews wEBNews = new WEBNews();
            wEBNews.MainSN = mainsn;
            wEBNews.WebLevelSN = weblevelsn;
            wEBNews.ArticleType = Utility.EnumTpye.GetEnumNumberToSting(Utility.SYSConst.Content.Type.Bilingual);
            wEBNews.Module = module;
            wEBNews.WebSiteID = UserData.WebSiteID;
            if (wEBNews.MainSN == 0)
            {
                wEBNews.CreatedUserID = UserData.sysUser.UserID;
                wEBNews.CreatedDate = DateTime.UtcNow.AddHours(8);
            }
            wEBNews.ProcessUserID = UserData.sysUser.UserID;
            wEBNews.PublishDate = DateTime.UtcNow.AddHours(8);
            wEBNews.ProcessIPAddress = ContextModel.ProcessIpaddress;
            wEBNews.StartDate = string.IsNullOrWhiteSpace(startdate) ? null : DateTime.Parse(startdate);
            wEBNews.EndDate = string.IsNullOrWhiteSpace(enddate) ? null : DateTime.Parse(enddate);
            wEBNews.IsEnable = isenable;
            return wEBNews;
        }
    }
}

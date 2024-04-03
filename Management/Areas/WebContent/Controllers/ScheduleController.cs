using DBModel;
using Management.Areas.WebContent.Models;
using Management.ManagementUtility;
using Management.Models.Common;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using Services;
using Services.Authorization;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utility;
using static Utility.Files;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class ScheduleController : BaseController
    {
        public IActionResult Index(string key = "",string key2 = "")
        {
            var NewsKey = "press-releases"; //新聞發布
            var _key = 0;      

            if (int.TryParse(key, out _key))
            {

                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.ScheduleModel scheduleModel = new Models.ScheduleModel();
                DBModel.WEBNews wEBNews = new DBModel.WEBNews();
                scheduleModel.wEBNews = wEBNews;
                scheduleModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                scheduleModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                scheduleModel.AuthSysGroupWebLevels = UserData.webLevelAccessForGroups.Where(x => x.WebLevelSN == _key).ToList();
                scheduleModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                var webLevelDATA = new DBModel.WebLevel()
                {
                    WebSiteID = scheduleModel.webLevel.WebSiteID,
                    Lang = scheduleModel.webLevel.Lang,
                    WebLevelKey = NewsKey
                };
                var data = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA);
                var News = WebLevelManagementService.GetWebNewsByWebLevelSN(data.WebLevelSN).ToList();
                var NewsList = (from n in News
                                select new DBModel.WEBNews
                               {
                                WEBNewsSN = n.WEBNewsSN,
                                Title = n.Title == null ? n.Title : CommFun.JsonTransfer( Regex.Replace(n.Title, "'", " ")),
                                StartDate = n.StartDate,
                                EndDate = n.EndDate,
                            }).ToList();

                scheduleModel.News = NewsList;

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
                            CommonModel.webNewsSchedule = WebLevelManagementService.GetScheduleByWEBNews(langData.WEBNewsSN);
                            CommonModel.wEBNewsExtends = WebLevelManagementService.GetWEBNewsExtends(langData.WEBNewsSN);//LINK連結
                            CommonModel.AuthSysGroupWebLevels = UserData.webLevelAccessForGroups.Where(x => x.WebLevelSN == _key).ToList(); ;
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
                            scheduleModel.newCommonModels.Add(CommonModel);
                        }
                        scheduleModel.wEBNews = LangDataList.First();
                        scheduleModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key, _key2);
                    }
                    else
                    {
                        return View(null);
                    }
                }
                else
                {
                    scheduleModel.LevelBreadcrumb = CommonService.LevelBreadcrumb(_key);
                }
                SetSession("WEBFile", fileData);
                scheduleModel.WebSiteId = UserData.WebSiteID;
                return View(scheduleModel);
            }          
            return View(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult ScheduleSave (WEBNews wEBNews ,List<int> wEBNewsSN, List<string> chief, List<CommonFileModel> fileinfo = null, List<SelectTxt> relatedlink = null  )
        {
            try
            {
                SetLogActionModel(webPath: "網站維護/" + OperationStatisticsService.GetWebLevelTree(wEBNews.WebLevelSN).FirstOrDefault()?.Path, Action2: wEBNews.WEBNewsSN == 0 ? Utility.Model.LoginModel.Action2.insert : Utility.Model.LoginModel.Action2.update, SourceTable: "WEBNews");
                wEBNews.WebSiteID = UserData.WebSiteID;
                wEBNews.CreatedUserID = UserData.sysUser.UserID;
                wEBNews.CreatedDate = DateTime.UtcNow.AddHours(8);
                wEBNews.ProcessIPAddress = ContextModel.ProcessIpaddress;
                wEBNews.DepartmentID = wEBNews.DepartmentID;

                var ReviewerState = wEBNews.IsEnable;
                var MailProcessUserID = wEBNews.ProcessUserID;

                if (wEBNews.IsEnable != "-2")
                {
                    if (wEBNews.IsEnable == "4")
                    {
                        wEBNews.IsEnable = "1";
                    }
                    wEBNews.ProcessUserID = UserData.sysUser.UserID;
                }
                var WEBNewsExtendData = new List<WEBNewsExtend>();
                #region 擴充表資料整理
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(relatedlink, "relatedlink"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(chief, "chief"));

                #endregion
                var files = GetSession<List<CommonFileModel>>("WEBFile");
                if (fileinfo.Count() > 0 && fileinfo.Where(x => x.lan == wEBNews.Lang) != null && files.Where(x => x.lan == wEBNews.Lang) != null)
                {
                    foreach (var file in files.Where(x => x.lan == wEBNews.Lang))
                    {
                        var fileModel = fileinfo.FirstOrDefault(x => x.fileNewName == file.fileNewName);
                        if (fileModel != null)
                        {
                            file.fileTitle = fileModel.fileTitle;
                            file.FileSort = fileModel.FileSort;
                        }
                    }
                }
                List<string> ckEditorurl = new List<string>();
                var checkMissData = Services.CheckModel.CheckedData.CheckedWebNews(ref wEBNews, ref  ckEditorurl, fileinfo, null, null, WEBNewsExtendData);
                if (!checkMissData.chk)
                {
                    return StatusResult(System.Net.HttpStatusCode.InternalServerError, checkMissData.error);
                }


                if (WebLevelManagementService.ScheduleSave(ref wEBNews, wEBNewsSN, files, WEBNewsExtendData))
                {
                    if (ReviewerState == "3")
                    {
                        var levelForTrees = WebLevelManagementService.GetWebLevelList(Utility.EnumWeblevelType.WebLevelManagment, UserData.WebSiteID);
                        var NewsList = WebLevelManagementService.GetWebNewList(levelForTrees.Select(X => X.WebLevelSN).ToList(), wEBNews.Module, "3");
                        var AuthList = GroupManagementService.GetUserAuthList("Reviewer", wEBNews.WebSiteID,wEBNews.WebLevelSN);
                        if (AuthList.Rows.Count > 0)
                        {
                            MailUtility.SendReviewer(wEBNews, AuthList, out Exception ex);
                        }
                    }
                    else if (ReviewerState == "4")
                    {
                        var News = WebLevelManagementService.GetWEBNew(wEBNews.WEBNewsSN);
                        var MailUser = SYSUserService.GetUserData(MailProcessUserID);
                        if (News != null)
                        {
                            MailUtility.SendReviewerOK(News, MailUser, out Exception ex);
                        }
                    }
                    else if (ReviewerState == "-2")
                    {
                        var News = WebLevelManagementService.GetWEBNew(wEBNews.WEBNewsSN);
                        var MailUser = SYSUserService.GetUserData(MailProcessUserID);
                        if (News != null)
                        {
                            MailUtility.SendReturned(News, MailUser, out Exception ex);
                        }
                    }
                    logActionModel.SourceSN = wEBNews.WEBNewsSN;
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗，請洽管理者");
                }
            }
            catch(Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        /// <summary>
        /// 整理成WEBNewsExtend
        /// </summary>
        /// <param name="lis"></param>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        static List<WEBNewsExtend> GetWEBNewsExtend(List<SelectTxt> lis, string GroupID)
        {
            if (lis == null) return null;
            return lis.Where(x=>!string.IsNullOrWhiteSpace(x.txt) ).Select(x => new WEBNewsExtend()
            {
                Column_1 = x.txt.Trim(),
                Column_2 = x.val==null ? null : x.val.Trim(),
                GroupID = GroupID,
            }).ToList();
        }
        static List<WEBNewsExtend> GetWEBNewsExtend(List<string> lis, string GroupID)
        {
            return lis.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new WEBNewsExtend()
            {
                Column_1 = x.Trim(),
                SysCategoryKey = x.Trim(),
                GroupID = GroupID,
            }).ToList();
        }
    }
}

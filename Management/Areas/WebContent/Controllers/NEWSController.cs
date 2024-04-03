using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModel;
using Management.Areas.WebContent.Models;
using Management.Areas.WebContent.Models.WebLevelManagement;
using Management.ManagementUtility;
using Management.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using Services.Models;
using Services.Models.WebSite;
using Services.WebManagement;
using Services.WebSite;
using static Utility.Files;

namespace Management.Areas.WebContent.Controllers
{

    [Area("WebContent")]
    public class NEWSController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            var _key = 0;
            if (int.TryParse(key, out _key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());
                var fileData = new List<CommonFileModel>();
                Models.NEWSModel viewModel = new Models.NEWSModel();
                DBModel.WEBNews wEBNews = new DBModel.WEBNews();
                wEBNews.DepartmentID = UserData.sysUser.DepartmentID;
                wEBNews.ArticleType = "0";
                viewModel.wEBNews = wEBNews;
                viewModel.webLevel = WebLevelManagementService.GetWebLevel(_key);
                viewModel.sysWebSiteLangs = WebLevelManagementService.GetSysWebSiteLangs(UserData, "", 0);
                viewModel.AuthSysGroupWebLevels = UserData.webLevelAccessForGroups.Where(x => x.WebLevelSN == _key).ToList();
                viewModel.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
                if (!string.IsNullOrWhiteSpace(key2))
                {
                    var _key2 = 0;
                    if (int.TryParse(key2, out _key2))
                    {
                        var LangDataList = WebLevelManagementService.GetWEBNewByMainSN(_key2);
                        var ZH_Transcript = NewsService.GetWEBNewsTranscript(LangDataList.First(x=>x.Lang=="zh-tw").WEBNewsSN);
                        var en_Transcript = NewsService.GetWEBNewsTranscript(LangDataList.First(x => x.Lang == "en").WEBNewsSN);
                        SetSession($"MD_Datazh-tw", ZH_Transcript);
                        SetSession($"MD_Dataen", en_Transcript);
                        foreach (var langData in LangDataList)
                        {
                            var CommonModel = new NewCommonModel();
                            CommonModel.webNews = langData;
                            CommonModel.wEBNewsExtends = WebLevelManagementService.GetWEBNewsExtends(langData.WEBNewsSN);
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

        /// <summary>
        /// 中繼站
        /// </summary>
        /// <param name="key">WebLevelMSN</param>
        /// <param name="key2">WEBNewsSN</param>
        /// <returns></returns>
        public IActionResult NewsType(string key = "", string key2 = "", string key3 = "")
        {
            return RedirectToAction("Index", new { area = "WebContent", Controller = key3, key = key, key2 = key2 });
        }
        /// <summary>
        /// 公版WEBNES儲存功能
        /// </summary>
        /// <param name="wEBNews">webNEWS </param>
        /// <param name="fileinfo">檔案排序</param>
        /// <param name="linkinfo">這個要移除</param>
        /// <param name="tab">主題標籤</param>
        /// <param name="keyword">關鍵字</param>
        /// <param name="whole">整體績效</param>
        /// <param name="policy">政策計畫</param>
        /// <param name="business">業務分類</param>
        /// <param name="serve">服務對象</param>
        /// <param name="relatedlink">相關連結</param>
        /// <param name="relatedvideo">相關影片</param>
        /// <param name="relatedmoj">相關法規</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult WEBNewsSave(
            WEBNews wEBNews,
            List<CommonFileModel> fileinfo = null,
            List<WebLink> linkinfo = null,
            List<string> tab = null,
            List<SelectTxt> keyword = null,
            List<string> whole = null,
            List<string> policy = null,
            List<string> business = null,
            List<string> serve = null,
            List<SelectTxt> relatedlink = null,
            List<SelectTxt> relatedvideo = null,
            List<SelectTxt> relatedmoj = null
            )
        {
            try
            {
                SetLogActionModel(webPath: "網站維護/" + OperationStatisticsService.GetWebLevelTree(wEBNews.WebLevelSN).FirstOrDefault()?.Path, Action2: wEBNews.WEBNewsSN == 0 ? Utility.Model.LoginModel.Action2.insert : Utility.Model.LoginModel.Action2.update, SourceTable: "WEBNews");
                wEBNews.WebSiteID = UserData.WebSiteID;
                wEBNews.CreatedUserID = UserData.sysUser.UserID;
                var ReviewerState = wEBNews.IsEnable;
                var MailProcessUserID = wEBNews.ProcessUserID;

                if(wEBNews.IsEnable != "-2")
                {   
                    if(wEBNews.IsEnable == "4")
                    {
                        wEBNews.IsEnable = "1";
                    }
                    wEBNews.ProcessUserID = UserData.sysUser.UserID;
                }

                wEBNews.CreatedDate = DateTime.UtcNow.AddHours(8);
                wEBNews.PublishDate = DateTime.UtcNow.AddHours(8);
                wEBNews.ProcessIPAddress = ContextModel.ProcessIpaddress;
                wEBNews.DepartmentID = wEBNews.DepartmentID;
                var WEBNewsExtendData = new List<WEBNewsExtend>();

                #region 擴充表資料整理
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tab, "tab"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(keyword, "keyword"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(whole, "whole"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(policy, "policy"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(business, "business"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(serve, "serve"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(relatedlink, "relatedlink"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(relatedvideo, "relatedvideo"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(relatedmoj, "relatedmoj"));
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
                var Transcript = new List<WEBNewsTranscript>();
                Transcript = GetSession<List<WEBNewsTranscript>>($"MD_Data{wEBNews.Lang}");

                if (wEBNews.Module == Utility.EnumTpye.GetEnumName(Utility.SysConst.Module.NEWS) || wEBNews.Module == Utility.EnumTpye.GetEnumName(Utility.SysConst.Module.CP))
                {
                    string lang = wEBNews.Lang;
                    if (wEBNews.ArticleType != Utility.EnumTpye.GetEnumNumberToSting(Utility.SYSConst.Content.Type.PAGE))
                    {
                        wEBNews.PageViewType = null;
                        wEBNews.ContentText = null;
                        files.RemoveAll(x => x.GroupID == Utility.WebFileGroupID.News.InlineImgs && x.lan == lang);
                        files.RemoveAll(x => x.GroupID == Utility.WebFileGroupID.News.Files && x.lan == lang);
                        files.RemoveAll(x => x.GroupID == Utility.WebFileGroupID.News.Imgs && x.lan == lang);
                        WEBNewsExtendData.RemoveAll(x => x.GroupID == "relatedlink");
                        WEBNewsExtendData.RemoveAll(x => x.GroupID == "relatedvideo");
                        WEBNewsExtendData.RemoveAll(x => x.GroupID == "relatedmoj");
                    }

                    if (wEBNews.ArticleType != Utility.EnumTpye.GetEnumNumberToSting(Utility.SYSConst.Content.Type.DOWNLOAD))
                    {
                        files.RemoveAll(x => x.GroupID == Utility.WebFileGroupID.News.File && x.lan == lang);
                    }

                    if (wEBNews.ArticleType != Utility.EnumTpye.GetEnumNumberToSting(Utility.SYSConst.Content.Type.LINK))
                    {
                        wEBNews.URL = null;
                        wEBNews.target = null;
                    }

                    if (wEBNews.ArticleType != Utility.EnumTpye.GetEnumNumberToSting(Utility.SYSConst.Content.Type.Transcript))
                    {
                        Transcript = null;
                    }
                }

                List<string> ckEditorurl = new List<string>();
                var checkMissData = Services.CheckModel.CheckedData.CheckedWebNews(ref wEBNews, ref ckEditorurl, fileinfo, linkinfo , Transcript, WEBNewsExtendData);
                if (!checkMissData.chk)
                {
                    return StatusResult(System.Net.HttpStatusCode.InternalServerError, checkMissData.error);
                }
                if (WebLevelManagementService.SaveWebNews(ref wEBNews, files, linkinfo, WEBNewsExtendData , Transcript))
                {
                    switch (ReviewerState)
                    {
                        case "3":
                            var levelForTrees = WebLevelManagementService.GetWebLevelList(Utility.EnumWeblevelType.WebLevelManagment, UserData.WebSiteID);
                            var AuthList = GroupManagementService.GetUserAuthList("Reviewer", wEBNews.WebSiteID, wEBNews.WebLevelSN);
                            if (AuthList.Rows.Count > 0)
                            {
                                MailUtility.SendReviewer(wEBNews, AuthList, out Exception ex);
                            }
                            break;
                        case string a when a == "-2" || a == "4"  :
                            var News = WebLevelManagementService.GetWEBNew(wEBNews.WEBNewsSN);
                            var MailUser = SYSUserService.GetUserData(MailProcessUserID);
                            if (News != null)
                            {
                                switch (ReviewerState)
                                {
                                    case "-2":
                                        MailUtility.SendReturned(News, MailUser, out Exception ex1);
                                        break;
                                    case "4":
                                        MailUtility.SendReviewerOK(News, MailUser, out Exception ex);
                                        break;
                                }
                            }
                            break;
                        
                    }
                }
                else
                {}
                SetSession("WEBFile", new List<CommonFileModel>());
                SetSession($"MD_Data{wEBNews.Lang}", new List<WEBNewsTranscript>());
                WebLevelManagementService.SaveWebCntLink("WEBNews", wEBNews.WEBNewsSN, ckEditorurl);

                logActionModel.SourceSN = wEBNews.WEBNewsSN;
                if(wEBNews.IsEnable == "-2")
                {
                    logActionModel.userID = UserData.sysUser.UserID;
                    logActionModel.Action2 = Utility.Model.LoginModel.Action2.returned;
                }
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        /// <summary>
        /// 公版WEBNES刪除功能
        /// </summary>
        /// <param name="wEBNews"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult WEBNewsDelete(string key)
        {
            try
            {

                SetLogActionModel(Action2: Utility.Model.LoginModel.Action2.delete, SourceTable: "WEBNews");

                if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key))
                {
                    logActionModel.SourceSN = _key;

                    var wEBNews = new WEBNews();
                    wEBNews.WEBNewsSN = _key;
                    wEBNews.ProcessUserID = UserData.sysUser.UserID;
                    wEBNews.ProcessIPAddress = ContextModel.ProcessIpaddress;
                    if (WebLevelManagementService.DeleteWebNews(ref wEBNews))
                    {
                        logActionModel.webPath = "網站維護/" + OperationStatisticsService.GetWebLevelTree(wEBNews.WebLevelSN).FirstOrDefault()?.Path;
                        //Log(logActionModel);
                        return StatusResult(System.Net.HttpStatusCode.OK, "");
                    }
                    else
                    {
                        logActionModel.webPath = "網站維護/" + OperationStatisticsService.GetWebLevelTree(wEBNews.WebLevelSN).FirstOrDefault()?.Path;
                        logActionModel.status = Utility.Model.LoginModel.Status.Error;
                        logActionModel.response = "刪除失敗";
                        Log(logActionModel);
                        return StatusResult(System.Net.HttpStatusCode.BadRequest, "刪除失敗");
                    }
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "刪除失敗");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult WEBNewPageView(WEBNews wEBNews, List<CommonFileModel> fileinfo = null, List<WebLink> linkinfo = null)
        {
            var tempData = new PageViewModel.News();
            PageViewModel.News.WebNewsDetailModel webNewsDetailModel = new PageViewModel.News.WebNewsDetailModel();
            wEBNews.WebSiteID = UserData.WebSiteID;
            webNewsDetailModel.BasicData = wEBNews;

            tempData.localWebLevel = WebLevelManagementService.GetWebLevel(wEBNews.WebLevelSN);
            tempData.parentData = WebLevelManagementService.GetWebLevel(wEBNews.WebLevelSN);
            tempData.brotherData = NewsService.GetBrotherWebLevelM(tempData.parentData, wEBNews.WebLevelSN);
            tempData.Breadcrumb = HomeService.getBreadcrumbModel(wEBNews.WebLevelSN);
            SysDepartment dept = NewsService.GetDepName(wEBNews.DepartmentID);
            if (wEBNews.WebSiteID == "CEPP")
            {
                tempData.DepName = NewsService.GetDepName(wEBNews.DepartmentID)?.DepartmentName;
            }
            else
            {
                tempData.DepName = NewsService.GetDepName(wEBNews.DepartmentID)?.Description;
            }


            if (fileinfo != null)
            {
                #region files資料整理
                var files = GetSession<List<CommonFileModel>>("WEBFile");
                if (fileinfo != null && files != null)
                {
                    //重新定義名稱跟排序
                    foreach (var file in files)
                    {
                        var fileModel = fileinfo.FirstOrDefault(x => x.fileNewName == file.fileNewName);
                        if (fileModel != null)
                        {
                            file.fileTitle = fileModel.fileTitle;
                            file.FileSort = fileModel.FileSort;
                        }
                    }
                }
                #endregion
                var fileData = files.Select(x => new WebFileAndGroupIDModel()
                {
                    FilePath = x.filePath,
                    FileName = x.fileNewName,
                    FileType = x.fileExt,
                    GroupID = x.GroupID,
                    FileTitle = x.fileTitle,
                }).ToList();

                switch (wEBNews.Module)
                {
                    case "NEWS":
                        webNewsDetailModel.attachment.page.Files = fileData.Where(x => x.GroupID == Utility.WebFileGroupID.News.Files).ToList();
                        webNewsDetailModel.attachment.page.Imgs = fileData.Where(x => x.GroupID == Utility.WebFileGroupID.News.Imgs).ToList();
                        break;
                    case "CP":
                        webNewsDetailModel.attachment.page.Files = fileData.Where(x => x.GroupID == Utility.WebFileGroupID.CP.Files).ToList();
                        webNewsDetailModel.attachment.page.Imgs = fileData.Where(x => x.GroupID == Utility.WebFileGroupID.CP.Imgs).ToList();
                        break;
                }

            }
            if (linkinfo.Count > 0)
            {
                List<WebLink> webLinks = new List<WebLink>();
                foreach (var link in linkinfo)
                {
                    webLinks.Add(new WebLink { Title = link.Title, URL = link.URL });
                }
                webNewsDetailModel.webLinks = webLinks;
            }

            tempData.webNewsDetailModel = webNewsDetailModel;
            SetSession("webNewsDetailModel", tempData);
            // TempData["webNewsDetailModel"] = tempData;
            return StatusResult(System.Net.HttpStatusCode.OK, "");
        }

        /// <summary>
        /// 頁面預覽
        /// </summary>
        /// <returns></returns>
        public IActionResult PageView()
        {
            if (UserData.WebSiteID == "MODA")
            {
                var data = HomeService.getMasterModel();
                ViewData["WebSiteMaster"] = data;
            }
            else
            {
                var data = HomeService.getMasterModel(WebSiteID: "MODAEN", WebLevelMSN: 103);
                ViewData["ENWebSiteMaster"] = data;
            }

            var tempData = GetSession<PageViewModel.News>("webNewsDetailModel");

            return View(tempData);
        }

        /// <summary>
        /// WebNews
        /// </summary>
        /// <param name="key">WebNewsSN</param>
        /// <param name="sort">欲調整至序號</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult NewsReArrange(string key, string sort, string lan = "")
        {
            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key) && int.TryParse(sort, out int _sort))
            {
                WebLevelManagementService.NewsReArrangeByChild(_key, _sort, UserData.sysUser.UserID, UserData.sysUser.ProcessIPAddress, lan);
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";
                Log(logActionModel);
                return RedirectToAction("Index", "UserManagement", new { area = "Authorization", msg = "請別亂輸入測試" });
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult NewsReArrangeByIsTop(string key, string isTop, string lan)
        {
            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key) && int.TryParse(isTop, out int _isTop))
            {
                WebLevelManagementService.NewsReArrangeByIsTop(_key, _isTop, UserData.sysUser.UserID, UserData.sysUser.ProcessIPAddress, lan);
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";
                Log(logActionModel);
                return RedirectToAction("Index", "UserManagement", new { area = "Authorization", msg = "請別亂輸入測試" });
            }
        }

        /// <summary>
        /// 整理成WEBNewsExtend
        /// </summary>
        /// <param name="lis"></param>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        static List<WEBNewsExtend> GetWEBNewsExtend(List<string> lis, string GroupID)
        {
            return lis.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new WEBNewsExtend()
            {
                Column_1 = x.Trim(),
                SysCategoryKey = x.Trim(),
                GroupID = GroupID,
            }).ToList();
        }
        static List<WEBNewsExtend> GetWEBNewsExtend(List<SelectTxt> lis, string GroupID)
        {
            if (lis == null) return null;
            
            return lis.Where(x=> !string.IsNullOrWhiteSpace(x.txt) )
                .Select(x => new WEBNewsExtend()
            {
                Column_1 = x.txt.Trim(),
                Column_2 = x.val != null ?   x.val.Trim() :  null   ,
                GroupID = GroupID,
            }).ToList();
        }


    }
}

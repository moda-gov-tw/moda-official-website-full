using DBModel;
using Management.ManagementUtility;
using Management.Models.Common;
using ManagementManagement.Areas.WebContent.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using Services.Models.WebSite;
using Services.WebSite;
using System.Collections.Generic;
using System.Linq;
using static Utility.Files;

namespace Management.Areas.WebContent.Controllers
{
    [Area("WebContent")]
    public class DemoController : BaseController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult news(WEBNews wEBNews,
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
            List<SelectTxt> relatedmoj = null)
        {

            var tempData = wEBNews;
            var IsListData = false;
            #region 列表頁得預覽
            if (tempData.WEBNewsSN != 0 && tempData.WebLevelSN == 0 && wEBNews.Module == null)
            {
                tempData = Services.Authorization.WebLevelManagementService.GetWEBNewByMainSN(tempData.WEBNewsSN).Where(x => x.Lang == tempData.Lang).First();
                IsListData = true;
            }
            #endregion

            tempData.WebSiteID = UserData.WebSiteID;
            tempData.ProcessDate = Services.Authorization.WebLevelManagementService.GetWEBNew(tempData.WEBNewsSN) == null ? System.DateTime.UtcNow.AddHours(8) : Services.Authorization.WebLevelManagementService.GetWEBNew(tempData.WEBNewsSN).ProcessDate;

            #region WEBNewsExtend
            var WEBNewsExtendData = new List<WEBNewsExtend>();
            if (IsListData)
            {
                WEBNewsExtendData = WebLevelManagementService.GetWEBNewsExtends(tempData.WEBNewsSN);
            }
            else
            {
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, tab, "tab"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, keyword, "keyword"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, whole, "whole"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, policy, "policy"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, business, "business"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, serve, "serve"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, relatedlink, "relatedlink"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, relatedvideo, "relatedvideo"));
                WEBNewsExtendData.AddRange(GetWEBNewsExtend(tempData, relatedmoj, "relatedmoj"));
            }


            #endregion
            var files = GetSession<List<CommonFileModel>>("WEBFile");
            var NWMI = new List<WEBFile>();
            var NWMF = new List<WEBFile>();

            if (fileinfo.Count() > 0 && fileinfo.Where(x => x.lan == tempData.Lang) != null && files.Where(x => x.lan == tempData.Lang) != null)
            {
                foreach (var file in files.Where(x => x.lan == tempData.Lang))
                {
                    var fileModel = fileinfo.FirstOrDefault(x => x.fileNewName == file.fileNewName);
                    if (fileModel != null)
                    {
                        file.fileTitle = fileModel.fileTitle;
                        file.FileSort = fileModel.FileSort;
                    }
                }
            }
            else if (CommonUtility.GetFileByDB(tempData.WEBNewsSN.ToString(), "WEBNews") != null && CommonUtility.GetFileByDB(tempData.WEBNewsSN.ToString(), "WEBNews").Count > 0 && wEBNews.Module == null)
            {
                files = CommonUtility.GetFileByDB(tempData.WEBNewsSN.ToString(), "WEBNews");
                foreach (var f in files)
                {
                    f.lan = tempData.Lang;
                }
            }

            if (files != null && files.Count > 0)
            {
                var fileLangData = files.Where(x => x.lan == tempData.Lang).ToList();
                var FileGroupKey = files.Select(x => x.GroupID).Distinct().ToList();
                if (FileGroupKey.Count() > 0)
                {
                    if (FileGroupKey.FirstOrDefault(x => x.Contains("MI") && !x.Contains("MII")) != null) { NWMI = GetDemoFile(fileLangData, FileGroupKey.FirstOrDefault(X => X.Contains("MI") && !X.Contains("MII"))); }
                    if (FileGroupKey.FirstOrDefault(x => x.Contains("MF")) != null) { NWMF = GetDemoFile(fileLangData, FileGroupKey.FirstOrDefault(X => X.Contains("MF"))); }
                }

                foreach (var i in NWMI)
                {
                    i.FilePath = i.FilePath.Replace("..", "").Replace("/", @"\");
                }
                foreach (var i in NWMF)
                {
                    i.FilePath = i.FilePath.Replace("..", "").Replace("/", @"\");
                }
            }
            var sysCategories = Services.SystemManageMent.CodeManagementService.GetCategoryList(tempData.WebSiteID).Where(X => X.Lang == tempData.Lang).ToList();
            var viewModel = new NewsModel();
            var detail = new NewsDetailModel()
            {
                BasicData = tempData,
                newsExtends = WEBNewsExtendData,
                DepartmentName = Services.Authorization.DepartmentManagementService.GetDepartmentBySysDepartmentID(tempData.DepartmentID, tempData.WebSiteID).Where(x => x.Lang == tempData.Lang).FirstOrDefault()?.DepartmentName,
                sysCategories = sysCategories,
                Imgs = NWMI,
                Files = NWMF,
            };
            var masterData = HomeService.getMasterModel(tempData.WebSiteID, tempData.Lang);
            viewModel.Detail = detail;
            viewModel.langCategory = sysCategories;
            viewModel.SysWebSiteLang = masterData.SysWebSiteLang;
            viewModel.TitleBarModel = new TitleBarModel() { Title = tempData.Title == null ? "" : tempData.Title.Trim() };
            viewModel.webSiteBreadcrumbs = CommonService.GetWebSiteBreadcrumb(tempData.Lang, detail.BasicData.WebLevelSN);
            var TranscriptSession = GetSession<List<WEBNewsTranscript>>($"MD_Data{tempData.Lang}");
            if (TranscriptSession != null)
            {
                viewModel.WEBNewsTranscript = TranscriptSession;
            }
            else
            {
                viewModel.WEBNewsTranscript = WebLevelManagementService.GetWEBNewsTranscript(tempData.WEBNewsSN);
            }
            SetSession("NewsModel", viewModel);

            return StatusResult(System.Net.HttpStatusCode.OK, "");
        }
        public IActionResult PageView()
        {
            var viewData = GetSession<NewsModel>("NewsModel");
            var masterData = HomeService.getMasterModel(viewData.Detail.BasicData.WebSiteID, viewData.Detail.BasicData.Lang);
            ViewData["WebSiteMaster"] = masterData;
            return View(viewData);
        }
        public IActionResult PageViewTranscript()
        {

            return View();
        }

        NewsModel GetDetail(int WebLevelMainSN, int WebNewsMainSN, string Lang)
        {
            var detail = new NewsDetailModel();
            if (WebLevelMainSN != 0)
            {
                var webLevel = WebLevelManagementService.GetWebLevel(WebLevelMainSN);
                var LevelData = new WebLevel
                {
                    MainSN = WebLevelMainSN,
                    Lang = Lang
                };
            }
            else
            {
                var newData = new WEBNews
                {
                    MainSN = WebNewsMainSN,
                    Lang = Lang
                };
                detail = NewsService.GetWebSiteNewDetailData(null, newData);
            }
            var newsModel = new NewsModel()
            {
                WebLevel = WebLevelManagementService.GetWebLevel(detail.BasicData.WebLevelSN, Lang),
                Detail = detail,
                webSiteBreadcrumbs = CommonService.GetWebSiteBreadcrumb(Lang, detail.BasicData.WebLevelSN, detail.BasicData.MainSN.Value),
                langCategory = CommonService.GetWebSiteCategory(detail.BasicData.WebSiteID, detail.BasicData.Lang),
                SysWebSiteLang = CommonService.GetSysWebSiteLang(detail.BasicData.WebSiteID, detail.BasicData.Lang)
            };
            return newsModel;
        }
        static List<WEBNewsExtend> GetWEBNewsExtend(WEBNews wEBNews, List<string> lis, string GroupID)
        {
            if (lis.Count == 0 && wEBNews.Module == null)
            {
                var data = WebLevelManagementService.GetWEBNewsExtends(wEBNews.WEBNewsSN).Where(x => x.GroupID == GroupID).ToList();
                return data;
            }

            return lis.Select(x => new WEBNewsExtend()
            {
                Column_1 = x,
                SysCategoryKey = x,
                GroupID = GroupID,
            }).ToList();
        }
        static List<WEBNewsExtend> GetWEBNewsExtend(WEBNews wEBNews, List<SelectTxt> lis, string GroupID)
        {
            if (lis.Count == 0 && wEBNews.Module == null)
            {
                var data = WebLevelManagementService.GetWEBNewsExtends(wEBNews.WEBNewsSN).Where(
                x =>
                   !string.IsNullOrWhiteSpace(x.Column_1)
                && x.GroupID == GroupID).ToList();
                return data;
            }

            return lis.Where(x => !string.IsNullOrWhiteSpace(x.txt) && !string.IsNullOrWhiteSpace(x.val)).Select(x => new WEBNewsExtend()
            {
                Column_1 = x.txt.Trim(),
                Column_2 = x.val.Trim(),
                GroupID = GroupID,
            }).ToList();
        }

        static List<WEBFile> GetDemoFile(List<CommonFileModel> commonFileModels, string GroupID)
        {
            var file = (from a in commonFileModels.Where(x => x.GroupID == GroupID).OrderBy(x => x.FileSort)
                        select new WEBFile
                        {
                            FilePath = Utility.Files.PathTraversal(a.filePath),
                            FileTitle = a.fileTitle,
                            FileType = a.fileExt
                        }).ToList();
            return file;
        }
    }
}

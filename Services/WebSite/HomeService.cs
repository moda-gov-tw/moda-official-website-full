
using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Authorization;
using Services.Models;
using Services.Models.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace Services.WebSite
{
    public class HomeService
    {

        public static WebLevel EFGetWebLevelMByWebLevelMSN(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel.FirstOrDefault(X => X.WebLevelSN == key);
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    LogService.CreateLogAction(new LogAction()
                    {
                        Status = "0",
                        MessageResult = ex.ToString(),
                        ProcessIPAddress = "",
                        UserID = "",
                        WebSiteID = "",
                        WebPath = "",
                        ActionType = "1",
                        Action2 = "Select",
                        SourceTable = "WebLevel",
                        Action = "EFGetWebLevelMByWebLevelMSN",
                        Controller = "HomeService",
                        SourceSN = key,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                    return null;
                }
            }
        }

        public static List<WEBFile> getLogoImg(int? mainSN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var file = (from f in db.WEBFile
                                join r in db.RelWebFileContent.Where(x => x.SourceSN == mainSN) on f.WEBFileSN equals r.WEBFileSN
                                where r.GroupID == Utility.WebFileGroupID.Module.LogoImg
                                select f).ToList();
                    return file;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static WebLevel getWebLevelSNByKey(string lang, int MainSN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel.FirstOrDefault(x => x.Lang == lang
                                                && x.MainSN == MainSN
                                                && (x.StartDate == null || x.StartDate < DateTime.UtcNow.AddHours(8))
                                                && (x.EndDate == null || x.EndDate > DateTime.UtcNow.AddHours(8))
                                                && x.IsEnable == "1");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<WebLevel> EFgetHeadMenu(string WebSiteID = "MODA")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var list = db.WebLevel.Where(x =>
                            x.IsEnable == "1" && x.WeblevelType == "1" &&
                            x.MainMenuShow == "1" && x.WebSiteID == WebSiteID
                            ).Select(x => new WebLevel()
                            {
                                WebLevelSN = x.WebLevelSN,
                                Lang = x.Lang,
                                Module = x.Module,
                                ParentSN = x.ParentSN,
                                SortOrder = x.SortOrder.Value,
                                Title = x.Title,
                                WebSiteID = WebSiteID
                            }).ToList();
                    return list;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        #region Master

        public static WebSiteMasterModel getMasterModel(string WebSiteID = "MODA", string Lang = "zh-tw", int WebLevelMSN = 53)
        {
            var data = new WebSiteMasterModel();
            data.Master = getSysWebSite(WebSiteID);
            data.Lang = Lang;
            data.SysWebSiteLang = getSysWebSiteLang(WebSiteID, Lang);
            data.HeadMenu = getHeadMenu(WebSiteID);
            data.FatFooterMenu = getFatFooterMenu(WebSiteID);
            data.LeftMenu = getLeftMenu(WebSiteID);
            data.sysCategories = CommonService.GetWebSiteCategory(WebSiteID, Lang);
            data.PopularKeys = getPopularKeys(WebSiteID, Lang);
            data.WebsiteAnnouncementArea = getWebsiteAnnouncementArea(WebSiteID, Lang);
            data.ContactUsArea = getContactUsArea(WebSiteID, Lang);
            data.SocialMediaArea = getSocialMediaArea(WebSiteID, Lang);
            data.LogoImg = getLogo(WebSiteID, Lang);
            data.DarkLogoImg = getLogo(WebSiteID, Lang, true);
            return data;
        }

        private static WEBFile getLogo(string WebSiteID, string Lang, bool isDark = false)
        {
            var webLevelDATA = new WebLevel()
            {
                WebSiteID = WebSiteID,
                Lang = Lang,
                WebLevelKey = "header"
            };
            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA);

            var Logoes = getLogoImg(level?.WebLevelSN);
            if (isDark)
            {
                return Logoes.Skip(1).FirstOrDefault();
            }
            else 
            {
                return Logoes.FirstOrDefault();
            }
        }

        private static WEBFile getAAimg(string webSiteID,string Lang)
        {
            string sql = $@"SELECT F.*
                              FROM [dbo].[WEBFile] F
                             inner join [dbo].[RelWebFileContent] R on F.[WEBFileSN] = R.[WEBFileSN] and R.[GroupID] = 'SWSI' and R.[SourceTable] = 'SysWebSite'
                             inner join [dbo].[SysWebSiteLang] W on R.[SourceSN] = W.[SysWebSiteLangSN]
                             WHERE W.[WebSiteID] = @WebSiteID and W.[Lang] = @Lang and F.[IsEnable] = '1'";

            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@WebSiteID", webSiteID));
            sqlParams.Add(new SqlParameter("@Lang", Lang));

            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBFile.FromSqlRaw(sql, sqlParams.ToArray()).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private static List<WebSiteExtend> getNavBar(string webSiteID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebSiteExtend.Where(x => x.WebSiteID == webSiteID && x.IsEnable == "1").OrderBy(x => x.Sort).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static DateTime getNewsData()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBNews.OrderByDescending(x => x.ProcessDate).FirstOrDefault().ProcessDate;
                }
                catch (Exception)
                {

                    return DateTime.UtcNow.AddHours(8);
                }
            }

        }
        public static SysWebSite getSysWebSite(string WebSiteID = "MODA")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysWebSite.FirstOrDefault(x => x.WebSiteID == WebSiteID);
                }
                catch (Exception )
                {
                    return null;
                }
            }
        }
        public static SysWebSiteLang getSysWebSiteLang(string WebSiteID = "MODA", string Lang = "zh-tw")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysWebSiteLang.FirstOrDefault(x => x.WebSiteID == WebSiteID && x.Lang == Lang);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 取得Menu
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <returns></returns>
        public static List<WebLevelModel> getHeadMenu(string WebSiteID = "MODA")
        {
            var levels = getWebLevelMDModel(WebSiteID, 0);
            List<WebLevelModel> result = new List<WebLevelModel>();
            foreach (var x in levels)
            {
                var weblevel = new WebLevelModel()
                {
                    Title = x.Title,
                    WebLevelSN = x.WebLevelSN,
                    WebSiteID = x.WebSiteID,
                    ParentSN = x.ParentSN,
                    Module = x.Module,
                    Lang = x.Lang,
                    SortOrder = x.SortOrder ?? 0,
                    IsEnable = x.IsEnable,
                    MainSN = x.MainSN,
                };
                weblevel.DynamicURL = getDynamicURL(x, out string target, true);
                weblevel.target = target;
                result.Add(weblevel);
            }

            return result;
        }
        public static List<WebLevelModel> GetSiteMap(string WebSiteID = "MODA")
        {
            var levels = getWebLevelMDModel(WebSiteID, 0);
            List<WebLevelModel> result = new List<WebLevelModel>();
            foreach (var x in levels) 
            {
                var weblevel = new WebLevelModel()
                {
                    Title = x.Title,
                    WebLevelSN = x.WebLevelSN,
                    WebSiteID = x.WebSiteID,
                    ParentSN = x.ParentSN,
                    Module = x.Module,
                    Lang = x.Lang,
                    SortOrder = x.SortOrder ?? 0,
                    IsEnable = x.IsEnable,
                    MainSN = x.MainSN,
                };
                weblevel.DynamicURL = getDynamicURL(x, out string target, true);
                weblevel.target = target;

                result.Add(weblevel);
            }

            return result;
        }
        private static string getDynamicURL(WebLevel webLevel, out string target, bool IsStatic = false)
        {
            IsStatic = CommonService.IsStatic;
            var URL = "";
            target = "";
            try
            {
                switch (webLevel.Module)
                {
                    case "CP":
                        using (var db = new MODAContext())
                        {
                            var NewsData = db.WEBNews.FirstOrDefault(x => x.WebLevelSN == webLevel.MainSN && x.Lang == webLevel.Lang);
                            if (NewsData != null)
                            {
                                switch (NewsData.ArticleType)
                                {
                                    case "0": //文章 
                                        URL = IsStatic == true ? webLevel.StatesUrl : "~/" + webLevel.WebSiteID + "/" + webLevel.Lang + "/Level/" + webLevel.MainSN;
                                        break;
                                    case "1": //附件 
                                        break;
                                    case "2": //連結
                                        URL = NewsData.URL;
                                        target = NewsData.target;
                                        break;
                                    case "3":
                                        URL = @$"https://www.youtube.com/embed/{NewsData.URL}";
                                        break;
                                    default: break;
                                }
                            }
                        }
                        break;
                    case "DEPT":
                        URL = IsStatic == true ? webLevel.StatesUrl : "~/" + webLevel.WebSiteID + "/" + webLevel.Lang + "/Dept/" + webLevel.MainSN;
                        break;
                    case "PAGELIST":
                        URL = IsStatic == true ? webLevel.StatesUrl : "~/" + webLevel.WebSiteID + "/" + webLevel.Lang + "/Level/" + webLevel.MainSN;
                        break;
                    default:
                        URL = IsStatic == true ? webLevel.StatesUrl : "~/" + webLevel.WebSiteID + "/" + webLevel.Lang + "/Level/" + webLevel.MainSN;
                        break;
                }
            }
            catch (Exception)
            {
            }
            return URL;
        }
        /// <summary>
        /// 取得熱門字
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <returns></returns>
        public static List<string> getPopularKeys(string WebSiteID = "MODA", string Lang = "")
        {
            var result = new List<string>();

            var webLevelDATA = new WebLevel()
            {
                WebSiteID = WebSiteID,
                Lang = Lang,
                WebLevelKey = "Popular"
            };
            var Parent = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA);
            if (Parent != null)
            {
                SysConst.Module module = (SysConst.Module)Enum.Parse(typeof(SysConst.Module), Parent.Module);
                List<WEBNews> wEBNews = getNewsComm(WebSiteID, Lang, module, Parent.MainSN.Value);
                result = wEBNews.OrderBy(n => n.SortOrder).Select(n => n.Title).ToList();
            }
            return result;
        }

        /// <summary>
        /// 取得熱門字ByWebLevelMSN
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="WebLevelMSN">中文：53英文103</param>
        /// <returns></returns>
        public static List<string> getPopularKeys02(string WebSiteID = "MODA", string Lang = "", int WebLevelMSN = 0)
        {
            return getNewsComm(WebSiteID, Lang, SysConst.Module.TEXT, WebLevelMSN).Select(n => n.Title).ToList();
        }

        /// <summary>
        /// 取得FatFooter
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <returns></returns>
        public static List<WebLevelModel> getFatFooterMenu(string WebSiteID = "MODA")
        {
            var levels = getWebLevelMDModel(WebSiteID, 1);

            List<WebLevelModel> result = new List<WebLevelModel>();
            foreach (var x in levels)
            {
                var weblevel = new WebLevelModel()
                {
                    Title = x.Title,
                    WebLevelSN = x.WebLevelSN,
                    WebSiteID = x.WebSiteID,
                    ParentSN = x.ParentSN,
                    Module = x.Module,
                    Lang = x.Lang,
                    SortOrder = x.SortOrder ?? 0,
                    IsEnable = x.IsEnable,
                    MainSN = x.MainSN,
                };
                weblevel.DynamicURL = getDynamicURL(x, out string target, true);
                weblevel.target = target;

                result.Add(weblevel);
            }

            return result;
        }

        public static List<WebLevelModel> getLeftMenu(string WebSiteID = "MODA", bool IsStatic = false)
        {
            var levels = getWebLevelMDModel(WebSiteID, 2);

            List<WebLevelModel> result = new List<WebLevelModel>();
            foreach (var x in levels)
            {
                var weblevel = new WebLevelModel()
                {
                    Title = x.Title,
                    WebLevelSN = x.WebLevelSN,
                    WebSiteID = x.WebSiteID,
                    ParentSN = x.ParentSN,
                    Module = x.Module,
                    Lang = x.Lang,
                    SortOrder = x.SortOrder ?? 0,
                    IsEnable = x.IsEnable,
                    MainSN = x.MainSN,
                };
                weblevel.DynamicURL = getDynamicURL(x, out string target, true);
                weblevel.target = target;

                result.Add(weblevel);
            }

            return result;
        }

        /// <summary>
        /// 網站宣告
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <returns></returns>
        public static List<WebLevelModel> getWebsiteAnnouncementArea(string WebSiteID, string Lang)
        {
            var result = new List<WebLevelModel>();

            var webLevelDATA = new WebLevel()
            {
                WebSiteID = WebSiteID,
                Lang = Lang,
                WebLevelKey = "announcement"
            };
            var Parent = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA);
            if (Parent != null) 
            {
                var levels = getWebLevelMDModel(WebSiteID, 3, Parent.MainSN.Value, "3").Where(x => x.Lang == Lang).OrderBy(x => x.SortOrder).ToList();

                foreach (var x in levels)
                {
                    var weblevel = new WebLevelModel()
                    {
                        Title = x.Title,
                        WebLevelSN = x.WebLevelSN,
                        WebSiteID = x.WebSiteID,
                        ParentSN = x.ParentSN,
                        Module = x.Module,
                        Lang = x.Lang,
                        SortOrder = x.SortOrder ?? 0,
                        IsEnable = x.IsEnable,
                        MainSN = x.MainSN,
                    };
                    weblevel.DynamicURL = getDynamicURL(x, out string target, true);
                    weblevel.target = target;

                    result.Add(weblevel);
                }
            }
            return result;
        }

        /// <summary>
        /// 聯絡資訊
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <returns></returns>
        public static ContactUsModel getContactUsArea(string WebSiteID, string Lang)
        {
            var ContactUsArea = new ContactUsModel();

            var webLevelDATA = new WebLevel()
            {
                WebSiteID = WebSiteID,
                Lang = Lang,
                WebLevelKey = "footer"
            };

            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA);

            if (level != null) 
            {
                ContactUsArea.BasicLevel = level;

                var webFiles = getFileComm(ContactUsArea.BasicLevel.WebLevelSN, SysConst.SourceTable.WEBLEVEL);

                var Logoes = getLogoImg(level.WebLevelSN);

                ContactUsArea.LogoFile = Logoes.FirstOrDefault();
                ContactUsArea.DarkLogoFile = Logoes.Skip(1).FirstOrDefault();
            }

            return ContactUsArea;
        }

        /// <summary>
        /// 抓取社群分享資料
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <returns></returns>
        public static List<WebLinkModel> getSocialMediaArea(string WebSiteID, string Lang) 
        {
            var SocialMedia = new List<WebLinkModel>();

            var webLevelDATA = new WebLevel()
            {
                WebSiteID = WebSiteID,
                Lang = Lang,
                WebLevelKey = "socialmedia"
            };

            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA);

            if (level != null)
            {
                SysConst.Module module = (SysConst.Module)Enum.Parse(typeof(SysConst.Module), level.Module);
                List<WEBNews> wEBNews = getNewsComm(WebSiteID, Lang, module, level.MainSN.Value).OrderBy(x => x.SortOrder).ToList();
                foreach (WEBNews webNews in wEBNews) 
                {
                    var media = new WebLinkModel();
                    media.BasicData = webNews;
                    var webFiles = getFileComm(webNews.WEBNewsSN, SysConst.SourceTable.WEBNEWS);
                    media.Img = GetRelSingleFile(webFiles, Utility.WebFileGroupID.Link.Img);
                    media.Img2 = GetRelSingleFile(webFiles, Utility.WebFileGroupID.Link.Img1);
                    SocialMedia.Add(media);
                }
            }
            return SocialMedia;
        }
        private static List<WEBFile> getFiles(int wEBNewsSN, Utility.SysConst.SourceTable SourceTable, string groupID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return (from f in db.WEBFile
                            join r in db.RelWebFileContent.Where(
                                r => r.SourceTable == SourceTable.ToString()
                                        && r.SourceSN == wEBNewsSN
                                        && r.GroupID == groupID)
                            on f.WEBFileSN equals r.WEBFileSN

                            select f).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 共用模組
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="type">0- heand , 1- fatFooter , 2- leftMenu </param>
        /// <param name="ParentSN"></param>
        /// <param name="WeblevelType"></param>
        /// <returns></returns>
        static List<WebLevel> getWebLevelMDModel(string WebSiteID, int type = 0, int ParentSN = 0, string WeblevelType = "1")
        {
            using (var db = new MODAContext())
            {
                

                try
                {
                    var list = db.WebLevel.Where(x =>
                                                x.WebSiteID == WebSiteID
                                             && x.IsEnable == "1" //啟動
                                             && (x.StartDate == null || x.StartDate < DateTime.UtcNow.AddHours(8))
                                             && (x.EndDate == null || x.EndDate > DateTime.UtcNow.AddHours(8))
                                             && x.WeblevelType == WeblevelType
                                             && (type == 0 ? x.MainMenuShow == "1" : 1 == 1)
                                             && (type == 1 ? x.FatFooterShow == "1" : 1 == 1)
                                             && (type == 2 ? x.LeftMenuShow == "1" : 1 == 1)
                                             && (ParentSN != 0 ? x.ParentSN == ParentSN : 1 == 1)
                            ).Select(x => new WebLevel()
                            {
                                WebLevelSN = x.WebLevelSN,
                                Lang = x.Lang,
                                Module = x.Module,
                                ParentSN = x.ParentSN,
                                SortOrder = x.SortOrder.Value,
                                Title = x.Title,
                                WebSiteID = WebSiteID,
                                MainMenuShow = x.MainMenuShow,
                                MainSN = x.MainSN,
                                Parameter = x.Parameter,
                                TemplateValue = x.TemplateValue.Trim(),
                                StatesUrl = x.StatesUrl,
                                IsEnable = x.IsEnable
                            }).ToList();

                    return list;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 麵包屑
        /// </summary>
        /// <param name="WebLebelMSN"></param>
        /// <returns></returns>
        public static List<WebLevelModel> getBreadcrumbModel(int WebLevelSN)
        {
            var list = new List<WebLevelModel>();
            using (var db = new MODAContext())
            {
                try
                {
                    var firstCount = new List<int>() { 0, 1, 2, 50, 51, 49, 101 };
                    var sn = WebLevelSN;
                    var i = 1;
                    while (!firstCount.Contains(sn))
                    {
                        var detail = db.WebLevel.Where(x =>
                         x.WebLevelSN == sn
                        && x.IsEnable == "1" 
                        && (x.StartDate == null || x.StartDate < DateTime.UtcNow.AddHours(8))
                        && (x.EndDate == null || x.EndDate > DateTime.UtcNow.AddHours(8))
                        ).Select(x => new WebLevelModel()
                        {
                            WebLevelSN = x.WebLevelSN,
                            Lang = x.Lang,
                            Module = x.Module,
                            ParentSN = x.ParentSN,
                            SortOrder = i,
                            Title = x.Title,
                            WebSiteID = x.WebSiteID

                        }).FirstOrDefault();
                        i++;
                        if (detail != null)
                        {
                            list.Add(detail);
                        }
                        sn = detail == null ? 0 : detail.ParentSN;
                    }
                }
                catch (Exception)
                {

                    return list;
                }
            }
            return list;
        }
        public static int getLongInCount(string WebSiteID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.LogWebSite.Join(db.WebLevel, l => l.WebLevelSN, m => m.WebLevelSN, (l, m) => new { l, m }).Where(x => 1 == 1
                              && x.m.WebSiteID == WebSiteID
                              && x.l.WebFileSN == 0
                            ).Count();
                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }

        #endregion

        #region Dept

        /// <summary>
        /// 取得司模組關聯頁
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <returns></returns>
        public static WebLevel getDeptConnet(string Lang, int? WebLevelSN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel
                               .FirstOrDefault(
                                   x => x.WeblevelType == "1"
                                   && x.Module == "DEPT"
                                   && x.Parameter == WebLevelSN.ToString()
                                   && x.IsEnable == "1"
                                   && x.Lang == Lang
                               );
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        /// <summary>
        /// 取得司模組第一層
        /// parentSN 對應 MainSN，取得該Lang資料
        /// </summary>
        /// <param name="MainSN"></param>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <returns></returns>
        public static List<WebLevel> getDeptChild(int MainSN, string WebSiteID = "MODA", string Lang = "zh-tw")
        {
            return HomeService.getWebLevelMDModel(WebSiteID, 3, MainSN, "2").Where(x => x.Lang == Lang).ToList();
        }

        /// <summary>
        /// 取得司首頁Tab 第一筆News 
        /// </summary>
        /// <param name="key">weblevelSN</param>
        /// <returns></returns>
        public static WebTabModel getDeptTab(int key, string Lang)
        {
            var result = new WebTabModel();
            using (var db = new MODAContext())
            { 
                try
                {
                    result.BasicData = db.WEBNews
                                .Where(x => x.WebLevelSN == key && x.WEBNewsSN == x.MainSN && x.IsEnable == "1")
                                .Select(x => new WebTabModel.TabData()
                                {
                                    WebLevelSN = x.WebLevelSN,
                                    Lang = Lang,
                                    WEBNewsSN = x.WEBNewsSN,
                                    RelWebLevelMSN =  TryParse(x.URL),
                                    Title = x.Title,
                                    CreatedDate = x.CreatedDate,
                                    StartDate = x.StartDate,
                                    EndDate = x.EndDate,
                                    SortOrder = x.SortOrder,
                                    RelWebLevelModule = "TAB",
                                })
                                .FirstOrDefault();

                    if (result.BasicData != null)
                    {
                        result.TabNewList = new List<newsChildModel>();

                        var dbnews = db.WEBNews
                            .Where(x => x.WebLevelSN == result.BasicData.RelWebLevelMSN && x.Lang == Lang && x.IsEnable == "1");

                        var wEBNewss = dbnews.OrderBy(x => x.SortOrder).Take(10).ToList();

                        foreach (WEBNews o in wEBNewss)
                        {
                            newsChildModel n = new newsChildModel();
                            n.localData = o;

                            switch (o.ArticleType)
                            {
                                case "1":
                                    var cpfile = (
                                    from a in db.RelWebFileContent
                                    join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                    where a.SourceTable == "WEBNews"
                                    && a.GroupID == "NWSF"
                                    && b.IsEnable == "1"
                                    && a.SourceSN == o.WEBNewsSN
                                    select b).FirstOrDefault();
                                    n.DynamicURL = cpfile?.FilePath ?? "";
                                    break;
                                case "2":
                                    n.DynamicURL = o.URL;
                                    break;
                                case "3":
                                    n.DynamicURL = @$"https://www.youtube.com/embed/{o.URL}";
                                    break;
                                default:
                                    n.DynamicURL = "~/MODA/" + o.Lang + "/News/" + o.MainSN;
                                    break;
                            }
                            var file = (from f in db.WEBFile
                                        join r in db.RelWebFileContent.Where(
                                            r => r.SourceTable == SysConst.SourceTable.WEBNEWS.ToString()
                                                    && r.SourceSN == o.WEBNewsSN
                                                    && r.GroupID == Utility.WebFileGroupID.News.Logo)
                                        on f.WEBFileSN equals r.WEBFileSN
                                        select f).FirstOrDefault();
                            n.file = file;
                            result.TabNewList.Add(n);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }

        private static int TryParse(string s) 
        {
            int.TryParse(s, out int result);
            return result;
        }

        #endregion

        #region Index

        /// <summary>
        /// 取得首頁第一層
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <returns></returns>
        public static List<WebSiteChildModel> getIndexChild(string WebSiteID = "MODA", string Lang = "zh-tw")
        {
            var result = new List<WebSiteChildModel>();

            try
            {
                using (var db = new MODAContext())
                {
                    var levels =
                        from x in db.WebLevel.Where(x => x.WebSiteID == WebSiteID
                                                    && x.Lang == Lang
                                                    && x.WeblevelType == "2"
                                                    && (x.StartDate == null || x.StartDate < DateTime.UtcNow.AddHours(8))
                                                    && (x.EndDate == null || x.EndDate > DateTime.UtcNow.AddHours(8))
                                                    && x.IsEnable == "1"
                                                    && x.ParentSN != 0).OrderBy(x => x.WebLevelSN).Take(1)
                        join z in db.WebLevel.Where(z => z.WebSiteID == WebSiteID
                                                    && z.Lang == Lang
                                                    && (z.StartDate == null || z.StartDate < DateTime.UtcNow.AddHours(8))
                                                    && (z.EndDate == null || z.EndDate > DateTime.UtcNow.AddHours(8))
                                                    && z.IsEnable == "1")
                            on x.MainSN equals z.ParentSN
                        select z;

                    foreach (var level in levels.ToList())
                    {
                        var child = new WebSiteChildModel();

                        child = getChild(level);

                        result.Add(child);
                    }

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static WebSiteChildModel getChild(WebLevel level)
        {
            var child = new WebSiteChildModel();
            child.LevelView.BasicLevel = level;
            switch (level.Module.ToUpper())
            {
                case "BANNER":
                    child.NewsViews = getBanner(level.WebSiteID, level.Lang, level.MainSN.Value);
                    break;
                case "BANNER2":
                    child.NewsViews = getBanner2(level.WebSiteID, level.Lang, level.MainSN.Value);
                    break;
                case "MEDIA":
                    child.NewsViews = getMedia(level.WebSiteID, level.Lang, level.MainSN.Value);
                    break;
                case "LINK":
                    child.NewsViews = getLink(level.WebSiteID, level.Lang, level.MainSN.Value);
                    break;
                case "TAB":
                    child.NewsViews = getTab(level.WebSiteID, level.Lang, level.MainSN.Value);
                    break;
                case "NEWS":
                    child.NewsViews = getNews(level.WebSiteID, level.Lang, level.MainSN.Value);
                    break;
                case "TEXT":
                    child.NewsViews = getText(level.WebSiteID, level.Lang, level.MainSN.Value);
                    break;
				case "EXTEND":
					child.NewsViews = getExtend(level.WebSiteID, level.Lang, level.MainSN.Value);
					break;
				case "PAGELIST":
                    child.Childlevels = getlevel(level.WebSiteID, level.Lang, level.MainSN.Value);
                    break;
            }
            return child;
        }

        private static List<NewsViewModel> getText(string WebSiteID, string Lang, int WebLevelMainSN)
        {
            List<NewsViewModel> Texts = new List<NewsViewModel>();
            List<WEBNews> webNews = getNewsComm(WebSiteID, Lang, SysConst.Module.TEXT, WebLevelMainSN).OrderBy(x => x.SortOrder).ToList();

            foreach (WEBNews webNew in webNews)
            {
                NewsViewModel webLinkModel = new NewsViewModel();
                webLinkModel.BasicNews = webNew;

                Texts.Add(webLinkModel);
            }

            return Texts;
        }

        private static List<WebSiteChildModel> getlevel(string webSiteID, string lang, int WebLevelMainSN)
        {
            var result = new List<WebSiteChildModel>();

            try
            {
                using (var db = new MODAContext())
                {
                    var levels = db.WebLevel.Where(x => x.WebSiteID == webSiteID && x.Lang == lang && x.ParentSN == WebLevelMainSN && x.IsEnable == "1").OrderBy(x => x.SortOrder).ToList();

                    foreach (var level in levels)
                    {
                        var child = new WebSiteChildModel();

                        child = getChild(level);

                        result.Add(child);
                    }
                }

            }
            catch (Exception)
            {
            }
            return result;
        }

        private static List<NewsViewModel> getNews(string WebSiteID, string Lang, int WebLevelMainSN)
        {
            SysConst.Module module = SysConst.Module.NEWS;
            List<NewsViewModel> wNewsList = new List<NewsViewModel>();

            List<WEBNews> wEBNews = getNewsComm(WebSiteID, Lang, module, WebLevelMainSN).OrderBy(x => x.SortOrder).ToList();
            foreach(var item in wEBNews)
            {
                NewsViewModel wNews = new NewsViewModel();
                wNews.BasicNews = item;

                var webFiles = getFileComm(item.WEBNewsSN, SysConst.SourceTable.WEBNEWS);

                if (item.ArticleType == ((int)Utility.SYSConst.Content.Type.DOWNLOAD).ToString())
                {
                    var file = webFiles.FirstOrDefault(x => x.GroupID == Utility.WebFileGroupID.News.File);
                    wNews.DynamicURL = file?.FilePath;
                }
                else if (item.ArticleType == ((int)Utility.SYSConst.Content.Type.LINK).ToString())
                {
                    wNews.DynamicURL = item.URL;
                }
                else
                {
                    wNews.DynamicURL = $"/{WebSiteID}/{Lang}/News/{item.MainSN}";
                }

                wNewsList.Add(wNews);
            }

            return wNewsList;
        }

        /// <summary>
        /// 取得Banner
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <param name="WebLevelMSN"></param>
        /// <returns></returns>
        /// 
        public static List<NewsViewModel> getBanner(string WebSiteID, string Lang, int WebLevelMainSN)
        {
            SysConst.Module module = SysConst.Module.BANNER;
            List<NewsViewModel> wBanners = new List<NewsViewModel>();

            List<WEBNews> wEBNews = getNewsComm(WebSiteID, Lang, module, WebLevelMainSN).OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < wEBNews.Count; i++)
            {
                NewsViewModel webBanner = new NewsViewModel();
                webBanner.BasicNews = wEBNews[i];

                var webFiles = getFileComm(wEBNews[i].WEBNewsSN, SysConst.SourceTable.WEBNEWS);
                //取大圖
                webBanner.MainImg = GetRelSingleFile(webFiles, Utility.WebFileGroupID.Banner.BigImg);
                //取小圖
                webBanner.SubImg = GetRelSingleFile(webFiles, Utility.WebFileGroupID.Banner.SmallImg);

                wBanners.Add(webBanner);
            }

            return wBanners;
        }

        /// <summary>
        /// 取得Banner
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <param name="WebLevelMSN"></param>
        /// <returns></returns>
        /// 
        public static List<NewsViewModel> getBanner2(string WebSiteID, string Lang, int WebLevelMainSN)
        {
            SysConst.Module module = SysConst.Module.BANNER2;
            List<NewsViewModel> wBanners = new List<NewsViewModel>();

            List<WEBNews> wEBNews = getNewsComm(WebSiteID, Lang, module, WebLevelMainSN).OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < wEBNews.Count; i++)
            {
                NewsViewModel webBanner = new NewsViewModel();
                webBanner.BasicNews = wEBNews[i];

                var webFiles = getFileComm(wEBNews[i].WEBNewsSN, SysConst.SourceTable.WEBNEWS);
                //取大圖
                webBanner.MainImg = GetRelSingleFile(webFiles, Utility.WebFileGroupID.Banner.BigImg);

                wBanners.Add(webBanner);
            }

            return wBanners;
        }

		public static List<NewsViewModel> getExtend(string WebSiteID, string Lang, int WebLevelMainSN)
		{
			SysConst.Module module = SysConst.Module.Extend;
			List<NewsViewModel> extend = new List<NewsViewModel>();

			List<WEBNews> wEBNews = getNewsComm(WebSiteID, Lang, module, WebLevelMainSN).OrderBy(x => x.SortOrder).ToList();
			for (int i = 0; i < wEBNews.Count; i++)
			{
				NewsViewModel webExtend = new NewsViewModel();
				webExtend.BasicNews = wEBNews[i];

				extend.Add(webExtend);
			}

			return extend;
		}

		/// <summary>
		/// 取得影音
		/// </summary>
		/// <param name="WebSiteID"></param>
		/// <param name="WebLevelMSN"></param>
		/// <returns></returns>
		public static List<NewsViewModel> getMedia(string WebSiteID, string Lang, int WebLevelMainSN)
        {

            List<NewsViewModel> webMedias = new List<NewsViewModel>();
            List<WEBNews> wEBNews = getNewsComm(WebSiteID, Lang, SysConst.Module.MEDIA, WebLevelMainSN).OrderBy(x => x.SortOrder).ToList();
            for (int i = 0; i < wEBNews.Count; i++)
            {
                NewsViewModel webMedia = new NewsViewModel();
                webMedia.BasicNews = wEBNews[i];

                var webFiles = getFileComm(wEBNews[i].WEBNewsSN, SysConst.SourceTable.WEBNEWS);
                webMedia.MainImg = GetRelSingleFile(webFiles, Utility.WebFileGroupID.Media.Img);
                webMedias.Add(webMedia);
            }
            return webMedias;
        }

        /// <summary>
        /// 取得連結
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <param name="WebLevelMainSN"></param>
        /// <returns></returns>
        public static List<NewsViewModel> getLink(string WebSiteID, string Lang, int WebLevelMainSN)
        {
            List<NewsViewModel> webLinks = new List<NewsViewModel>();
            List<WEBNews> webNews = getNewsComm(WebSiteID, Lang, SysConst.Module.LINK, WebLevelMainSN).OrderBy(x => x.SortOrder).ToList();

            foreach (WEBNews webNew in webNews)
            {
                NewsViewModel webLinkModel = new NewsViewModel();
                webLinkModel.BasicNews = webNew;

                var webFiles = getFileComm(webNew.WEBNewsSN, SysConst.SourceTable.WEBNEWS);
                webLinkModel.MainImg = GetRelSingleFile(webFiles, Utility.WebFileGroupID.Link.Img);
                webLinkModel.SubImg = GetRelSingleFile(webFiles, Utility.WebFileGroupID.Link.Img1);
                webLinks.Add(webLinkModel);
            }

            return webLinks;
        }

        /// <summary>
        /// 取的tab
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <param name="WebLevelMSN"></param>
        /// <returns></returns>
        public static List<NewsViewModel> getTab(string WebSiteID, string Lang, int WebLevelMainSN)
        {
            List<NewsViewModel> webTabs = new List<NewsViewModel>();
            try
            {
                using (var db = new MODAContext())
                {
                    var Tabs = db.WEBNews.Where(x => x.WebSiteID == WebSiteID
                                                && x.Lang == Lang
                                                && x.WebLevelSN == WebLevelMainSN
                                                && (x.StartDate == null || x.StartDate < DateTime.UtcNow.AddHours(8))
                                                && (x.EndDate == null || x.EndDate > DateTime.UtcNow.AddHours(8))
                                                && x.IsEnable == "1"
                                                && x.Module == "TAB").OrderBy(x => x.SortOrder).ToList();
                    foreach (var item in Tabs)
                    {
                        NewsViewModel webTab = new NewsViewModel();
                        webTab.BasicNews = item;
                        webTab.TabNewsView = new List<TabNewsModel>();
                        if (!int.TryParse(item.URL, out int _URL))
                        {
                            break;
                        }
                        var level = db.WebLevel.FirstOrDefault(x => x.WebLevelSN == _URL
                                                                && x.Lang == item.Lang
                                                                && (x.StartDate == null || x.StartDate < DateTime.UtcNow.AddHours(8))
                                                                && (x.EndDate == null || x.EndDate > DateTime.UtcNow.AddHours(8))
                                                                && x.IsEnable == "1");
                        if (level == null) return webTabs;
                        webTab.DynamicURL = $"{level.StatesUrl}";
                        List<TabNews> TabNews = new List<TabNews>();
                        switch (level.Module)
                        {
                            case "PAGELIST":
                                var childrenlevel = db.WebLevel.Where(x => x.ParentSN == level.MainSN
                                                                        && x.Lang == level.Lang
                                                                        && (x.StartDate == null || x.StartDate < DateTime.UtcNow.AddHours(8))
                                                                        && (x.EndDate == null || x.EndDate > DateTime.UtcNow.AddHours(8))
                                                                        && x.IsEnable == "1").OrderBy(x => x.SortOrder);
                                foreach (var chidlevel in childrenlevel)
                                {
                                    if (chidlevel.Module == "CP")
                                    {
                                        var NewsList = getNewsComm(WebSiteID, Lang, SysConst.Module.CP, chidlevel.MainSN.Value);
                                        var TabCP = NewsList.OrderBy(x => x.SortOrder).Select(x => new Models.WebSite.TabNews()
                                        {
                                            Source = "CP",
                                            SourceSN = x.WEBNewsSN,
                                            Title = x.Title,
                                            SubTitle = x.SubTitle,
                                            URL = x.URL,
                                            ArticleType = x.ArticleType,
                                            MainSN = chidlevel.MainSN.Value,
                                            target = x.target,
                                            SortOrder = chidlevel.SortOrder.Value,
                                            PublishDate = x.PublishDate,
                                            strDate = x.StartDate,
                                            IsTop = false
                                        }).First();
                                        TabNews.Add(TabCP);
                                    }
                                    else
                                    {
                                        var Tablevel = new TabNews
                                        {
                                            Source = "LEVEL",
                                            SourceSN = chidlevel.WebLevelSN,
                                            Title = chidlevel.Title,
                                            SubTitle = "",
                                            target = "_self",
                                            SortOrder = chidlevel.SortOrder.Value,
                                            MainSN = chidlevel.MainSN.Value,
                                            PublishDate = null,
                                            strDate = chidlevel.StartDate,
                                            IsTop = false
                                        };
                                        TabNews.Add(Tablevel);
                                    }
                                }
                                break;
                            case "NEWS":
                                {
                                    var NewsList = getNewsComm(WebSiteID, Lang, SysConst.Module.NEWS, level.MainSN.Value);
                                    TabNews = NewsList.OrderBy(x => x.SortOrder).Select(x => new Models.WebSite.TabNews()
                                    {
                                        Source = "NEWS",
                                        SourceSN = x.WEBNewsSN,
                                        Title = x.Title,
                                        SubTitle = x.SubTitle,
                                        ContentText = x.ContentText,
                                        URL = x.URL,
                                        ArticleType = x.ArticleType,
                                        MainSN = x.MainSN.Value,
                                        target = x.target,
                                        SortOrder = x.SortOrder.Value,
                                        PublishDate = x.PublishDate,
                                        strDate = x.StartDate,
                                        IsTop = x.IsTop != null
                                    }).ToList();
                                }
                                break;
                        }
                        TabNews = TabNews.OrderBy(x => x.SortOrder).ToList();

                        foreach (TabNews news in TabNews)
                        {
                            TabNewsModel tabNews = new TabNewsModel();
                            tabNews.TabNews = news;
                            if (news.Source == "LEVEL")
                            {
                                tabNews.DynamicURL = $"/{WebSiteID}/{Lang}/Level/{news.MainSN}";
                                //Logo
                                tabNews.Logo = getFileComm(news.SourceSN, SysConst.SourceTable.WEBLEVEL).FirstOrDefault(x => x.GroupID == Utility.WebFileGroupID.Module.LogoImg);
                            }
                            else
                            {
                                var files = getFileComm(news.SourceSN, SysConst.SourceTable.WEBNEWS);
                                //Logo
                                tabNews.Logo = getFileComm(news.SourceSN, SysConst.SourceTable.WEBNEWS).FirstOrDefault(x => x.GroupID == Utility.WebFileGroupID.News.Logo);
                                //Accordion
                                if (level.ListType == "AccordionList" && level.WebSiteID == "MODA") 
                                {
                                    tabNews.DynamicURL = $"{level.StatesUrl}#qaH{news.SourceSN}";
                                }
                                //File
                                else if (news.ArticleType == ((int)Utility.SYSConst.Content.Type.DOWNLOAD).ToString())
                                {
                                    var file = files.FirstOrDefault(x => x.GroupID == Utility.WebFileGroupID.News.File);
                                    tabNews.DynamicURL = file?.FilePath;
                                    tabNews.fileType = file?.FileType.ToUpper();
                                    tabNews.fileTitle = file?.FileTitle;
                                }
                                //URL
                                else if (news.ArticleType == ((int)Utility.SYSConst.Content.Type.LINK).ToString())
                                {
                                    tabNews.DynamicURL = news.URL;
                                }
                                else if (news.Source == "CP")
                                {
                                    tabNews.DynamicURL = $"/{WebSiteID}/{Lang}/Level/{news.MainSN}";
                                }
                                else
                                {
                                    tabNews.DynamicURL = $"/{WebSiteID}/{Lang}/News/{news.MainSN}";
                                }
                                //Tag
                                tabNews.NewsTags = (from e in db.WEBNewsExtend.Where(x => x.WEBNewsSN == news.SourceSN && x.GroupID == "tab").ToList()
                                                    join c in db.SysCategory.Where(z => z.Lang == level.Lang)
                                                    on e.SysCategoryKey equals c.SysCategoryKey
                                                    select c).Select(x => new NewsTag()
                                                    {
                                                        WebSiteID = x.WebSiteID,
                                                        Lang = x.Lang,
                                                        Key = x.SysCategoryKey,
                                                        Value = x.Value
                                                    }).ToList();
                                //Media
                                tabNews.MediaKey = (from a in db.WEBNewsExtend
                                                    where a.WEBNewsSN == news.SourceSN
                                                    && a.GroupID == "relatedvideo"
                                                    select a.Column_1).FirstOrDefault();
                                //AccordionList
                                if (level.ListType == "AccordionList")
                                {
                                    //   相關檔案/圖片
                                    var relatedFile = WebSiteListService.GetWebNewFile(news.SourceSN, Utility.WebFileGroupID.News.Files, "WEBNews");
                                    var relatedImg = WebSiteListService.GetWebNewFile(news.SourceSN, Utility.WebFileGroupID.News.Imgs, "WEBNews");
                                    //   相關 /連結/法規/影片 GetWebnewsextend
                                    var relatedlink = WebSiteListService.GetWebnewsextend(news.SourceSN, "relatedlink");
                                    var relatedvideo = WebSiteListService.GetWebnewsextend(news.SourceSN, "relatedvideo");
                                    var relatedmoj = WebSiteListService.GetWebnewsextend(news.SourceSN, "relatedmoj");

                                    tabNews.relatedFile = relatedFile;
                                    tabNews.relatedImg = relatedImg;
                                    tabNews.relatedlink = relatedlink;
                                    tabNews.relatedvideo = relatedvideo;
                                    tabNews.relatedmoj = relatedmoj;
                                }
                            }
                            webTab.TabNewsView.Add(tabNews);
                        }

                        webTabs.Add(webTab);
                    }
                }

                //Tab排序
                webTabs = webTabs.OrderBy(x => x.BasicNews.SortOrder).ToList();
            }
            catch (Exception)
            {
            }
            return webTabs;
        }

        #endregion

        /// <summary>
        /// 取得WebLevel
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static WebLevel GetWebLevel(int MainSN, string Lang)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel.FirstOrDefault(X => X.MainSN == MainSN && X.Lang == Lang && X.IsEnable == "1");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 抓取Level下所有News資料
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <param name="enumModule"></param>
        /// <param name="MainSN"></param>
        /// <param name="topN"></param>
        /// <returns></returns>
        private static List<WEBNews> getNewsComm(string WebSiteID = "MODA", string Lang = "", SysConst.Module enumModule = SysConst.Module.TEXT, int MainSN = 0, int topN = 0)
        {
            return NewsService.getNewsComm(WebSiteID, Lang, enumModule, MainSN, topN);
        }
        /// <summary>
        /// 抓取WEBNEWS對應之所有附件檔案
        /// </summary>
        /// <param name="WebNewsMSN"></param>
        /// <param name="sourceTable"></param>
        /// <returns></returns>
        private static List<Services.Models.WebFileAndGroupIDModel> getFileComm(int WebNewsMSN = 0, SysConst.SourceTable sourceTable = SysConst.SourceTable.WEBNEWS)
        {

            return NewsService.getFileComm(WebNewsMSN, sourceTable);

        }
        /// <summary>
        /// 解析getFileComm抓取哪一類附件
        /// </summary>
        /// <param name="webFiles"></param>
        /// <param name="GroupID"></param>
        /// <returns>最新的附件</returns>
        private static Services.Models.WebFileAndGroupIDModel GetRelSingleFile(List<Services.Models.WebFileAndGroupIDModel> webFiles, string GroupID)
        {
            return NewsService.GetRelSingleFile(webFiles, GroupID);
        }



        /// <summary>
        /// 取的參數資料
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="Lang"></param>
        /// <param name="SysCategoryKey"></param>
        /// <returns></returns>
        public static SysCategory getCategory(string WebSiteID = "MODA", string Lang = "zh-tw", string SysCategoryKey = "")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysCategory.FirstOrDefault(x => x.WebSiteID == WebSiteID && x.Lang == Lang && x.SysCategoryKey == SysCategoryKey);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<NewsViewModel> getRSS(string WebSiteID, string Lang, int WebLevelMainSN, string host) 
        {
            SysConst.Module module = SysConst.Module.NEWS;
            List<NewsViewModel> wNewsList = new List<NewsViewModel>();

            List<WEBNews> wEBNews = getNewsComm(WebSiteID, Lang, module, WebLevelMainSN).OrderBy(x => x.SortOrder).ToList();
            foreach (var item in wEBNews)
            {
                NewsViewModel wNews = new NewsViewModel();
                wNews.BasicNews = item;

                var webFiles = getFileComm(item.WEBNewsSN, SysConst.SourceTable.WEBNEWS);

                if (item.ArticleType == ((int)Utility.SYSConst.Content.Type.DOWNLOAD).ToString())
                {
                    var file = webFiles.FirstOrDefault(x => x.GroupID == Utility.WebFileGroupID.News.File);
                    if (file != null)
                    {
                        wNews.DynamicURL = file.FilePath.StartsWith('/') ? host + file.FilePath : file.FilePath;
                    }
                    else 
                    {
                        wNews.DynamicURL = host;
                    }
                }
                else if (item.ArticleType == ((int)Utility.SYSConst.Content.Type.LINK).ToString())
                {
                    wNews.DynamicURL = item.URL;
                }
                else
                {
                    wNews.DynamicURL = host + item.StatesUrl;
                }

                wNewsList.Add(wNews);
            }

            return wNewsList;
        }

        public static List<SysWebSite> GetSysWebSite()
        {
            using (var db = new MODAContext())
            {
                return db.SysWebSite.Where(x => x.IsEnable == "1").OrderBy(x => x.SortOrder).ToList();
            }
        }
    }

}

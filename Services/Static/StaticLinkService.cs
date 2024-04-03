using DBModel;
using Services.Authorization;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Static
{
    public class StaticLinkService
    {
        public static string mainWebSiteID { get; set; } = "moda";

        #region 儲存
        /// <summary>
        /// 儲存完之後再把MODEL丟過來
        /// </summary>
        /// <param name="webLevel"></param>
        /// <param name="webSiteDNS">不公開的前台網址</param>
        public static void Save(WebLevel webLevel, string webSiteDNS)
        {
            try
            {
                var noNeedStaticModule = new List<string>() { "OpendataNews" };
                if (webLevel.WeblevelType != "1") return;
                if (noNeedStaticModule.FirstOrDefault(x => x == webLevel.Module) != null) return;
                var href = string.Empty;
                var staticModel = new StaticLink();
                staticModel.SourseSN = webLevel.WebLevelSN;
                staticModel.MainSN = webLevel.MainSN;
                if (webLevel.WebLevelKey != "sitemap") staticModel.SourseTable = "weblevel";
                staticModel.WebSiteID = webLevel.WebSiteID;
                staticModel.Lang = webLevel.Lang;
                href = GetStaticUrl(webLevel.MainSN.Value);
                var StaticLang = webLevel.Lang == "zh-tw" ? "" : $@"/{webLevel.Lang}";
                var StaticWebSiteID = webLevel.WebSiteID.ToUpper() == mainWebSiteID.ToUpper() ? "" : $@"/{webLevel.WebSiteID}";
                #region 連結處理
                var StaticUrl = $"{StaticLang}{StaticWebSiteID}{href}/{webLevel.MainSN}.html";
                var Link = $"{webSiteDNS}/{webLevel.WebSiteID}/{webLevel.Lang}/Level/{webLevel.MainSN}";
                var isUrlLink = false;
                if (webLevel.Module == "CP")
                {
                    using (var db = new MODAContext())
                    {
                        var NewsData = db.WEBNews.FirstOrDefault(x => x.WebLevelSN == webLevel.MainSN && x.Lang == webLevel.Lang);
                        if (NewsData != null)
                        {
                            switch (NewsData.ArticleType)
                            {
                                case "1":
                                    isUrlLink = true;
                                    break;
                                case "2":
                                    StaticUrl = NewsData.URL;
                                    isUrlLink = true;
                                    break;
                                case "3":
                                    StaticUrl = @$"https://www.youtube.com/embed/{NewsData.URL}";
                                    isUrlLink = true;
                                    break;
                                default: break;
                            }
                        }
                        else
                        {
                            isUrlLink = true;
                        }
                    }
                }
                if (!isUrlLink)
                {
                    //司的特殊
                    if (webLevel.Module == "DEPT") Link = $"{webSiteDNS}/{webLevel.WebSiteID}/{webLevel.Lang}/Dept/{webLevel.MainSN}";
                    //siteMap
                    if (webLevel.WebLevelKey == "sitemap") Link = $"{webSiteDNS}/{webLevel.WebSiteID}/{webLevel.Lang}/Home/sitemap";

                    staticModel.StaticUrl = StaticUrl;
                    staticModel.Link = Link;

                    staticModel.IsEnable = webLevel.IsEnable;
                    staticModel.StartDate = webLevel.StartDate;
                    staticModel.EndDate = webLevel.EndDate;
                    staticModel.CreatedUserID = webLevel.ProcessUserID;
                    staticModel.CreatedDate = DateTime.UtcNow.AddHours(8);
                    staticModel.ProcessUserID = webLevel.ProcessUserID;
                    staticModel.ProcessDate = DateTime.UtcNow.AddHours(8);
                    staticModel.ProcessIPAddress = webLevel.ProcessIPAddress;
                    SaveStatic(staticModel);
                }
                #endregion
                using (var db = new MODAContext())
                {
                    var LevelData = db.WebLevel.FirstOrDefault(x => x.WebLevelSN == staticModel.SourseSN);
                    if (LevelData != null)
                    {
                        LevelData.StatesUrl = StaticUrl;
                        db.WebLevel.Update(LevelData);
                        db.SaveChanges();
                    }
                }
                UpdateParentWebLevel(webLevel);
            }
            catch (Exception)
            {


            }
        }

        /// <summary>
        /// 靜態的頁面重置
        /// </summary>
        /// <param name="type"></param>
        public static void ResetStaticLink(string key)
        {
            var dt = DateTime.UtcNow.AddHours(8);
            using (var db = new MODAContext())
            {
                try
                {
                    var _list = db.StaticLink.Where(x => x.WebSiteID == key).ToList();
                    foreach (var _item in _list)
                    {
                        _item.IsLive = "0";
                        _item.ProcessDate = dt;
                    }
                    db.StaticLink.UpdateRange(_list);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
        }
        static void UpdateParentWebLevel(WebLevel webLevel)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var StaticLink = db.StaticLink.Where(x =>
                            x.MainSN == webLevel.ParentSN &&
                            x.SourseTable == "weblevel"
                           ).ToList();
                    if (StaticLink != null)
                    {
                        foreach (var parent in StaticLink)
                        {
                            parent.IsLive = "0";
                            parent.ProcessDate = DateTime.UtcNow.AddHours(8);
                            db.StaticLink.Update(parent);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        public static void Save(WEBNews wEB, string webSiteDNS)
        {
            try
            {

                var WebLevelSN = 0;
                var staticModel = new StaticLink();
                staticModel.SourseSN = wEB.WEBNewsSN;
                staticModel.MainSN = wEB.MainSN;
                staticModel.SourseTable = "webnews";
                WebLevelSN = wEB.WebLevelSN;
                staticModel.WebSiteID = wEB.WebSiteID;
                staticModel.Lang = wEB.Lang;
                var Link = "";
                var StaticUrl = "";
                var href = string.Empty;
                href = GetStaticUrl(WebLevelSN);

                var StaticLang = wEB.Lang == "zh-tw" ? "" : $@"/{wEB.Lang}";
                var StaticWebSiteID = wEB.WebSiteID == mainWebSiteID ? "" : $@"/{wEB.WebSiteID}";

                StaticUrl = $"{StaticLang}{StaticWebSiteID}{href}/{wEB.MainSN}.html";
                Link = $"{webSiteDNS}/{wEB.WebSiteID}/{wEB.Lang}/News/{wEB.MainSN}";
                var isUrlLink = false;
                if (wEB.Module == "NEWS")
                {
                    switch (wEB.ArticleType)
                    {
                        case "1":
                            //類型為附件 不用特別轉因為使用API
                            StaticUrl = $"";
                            Link = "";
                            isUrlLink = true;
                            break;
                        case "2":
                            //類型為超連結
                            StaticUrl = $"{wEB.URL}";
                            break;
                        case "3":
                            StaticUrl = @$"https://www.youtube.com/embed/{wEB.URL}";
                            break;
                    }
                }
                if (wEB.ArticleType == "2" || wEB.ArticleType == "3")
                {
                    isUrlLink = true;
                }
                if (!isUrlLink)
                {
                    staticModel.StaticUrl = StaticUrl;
                    staticModel.Link = Link;
                    staticModel.IsEnable = wEB.IsEnable;
                    staticModel.StartDate = wEB.StartDate;
                    staticModel.EndDate = wEB.EndDate;
                    staticModel.CreatedUserID = wEB.ProcessUserID;
                    staticModel.CreatedDate = DateTime.UtcNow.AddHours(8);
                    staticModel.ProcessUserID = wEB.ProcessUserID;
                    staticModel.ProcessDate = DateTime.UtcNow.AddHours(8);
                    staticModel.ProcessIPAddress = wEB.ProcessIPAddress;
                    SaveStatic(staticModel);
                }

                using (var db = new MODAContext())
                {
                    var newsData = db.WEBNews.FirstOrDefault(x => x.WEBNewsSN == staticModel.SourseSN);
                    if (newsData != null)
                    {
                        newsData.StatesUrl = StaticUrl;
                        db.WEBNews.Update(newsData);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {


            }
        }

        /// <summary>
        /// 取的WEBlevelGetStaticUrl
        /// </summary>
        /// <param name="webLevelSN"></param>
        /// <returns></returns>
        static string GetStaticUrl(int webLevelSN)
        {
            //
            try
            {
                var pages = new List<WebLevel>();
                var href = string.Empty;
                GetWebLevelUrlKey(webLevelSN, ref pages);
                href = $"{(pages.First().Lang == "" ? "" : $"/{pages.First().Lang}")}/{string.Join("/", pages.OrderByDescending(x => x.SortOrder).Select(x => x.WebLevelKey))}";
                return href;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 取得UrlKey
        /// </summary>
        /// <param name="webLevelSN"></param>
        /// <param name="pages"></param>
        /// <param name="sort"></param>
        static void GetWebLevelUrlKey(int webLevelSN, ref List<WebLevel> pages, int sort = 0)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Key = db.WebLevel.FirstOrDefault(x => x.WebLevelSN == webLevelSN);
                    if (Key.ParentSN == 0) return;
                    pages.Add(new WebLevel()
                    {
                        WebLevelKey = Key.WebLevelKey,
                        SortOrder = sort++,
                        Lang = Key.Lang == "zh-tw" ? "" : Key.Lang,
                    });
                    GetWebLevelUrlKey(Key.ParentSN, ref pages, sort);
                }
                catch (Exception)
                {
                }
            }
            return;
        }
        /// <summary>
        /// Static 儲存
        /// </summary>
        /// <param name="staticLink"></param>
        /// <returns></returns>
        static bool SaveStatic(StaticLink staticLink)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var data = db.StaticLink.FirstOrDefault(x => x.SourseSN == staticLink.SourseSN && x.SourseTable == staticLink.SourseTable);
                    staticLink.IsLive = "0";
                    if (data != null)
                    {
                        data.Link = staticLink.Link;
                        data.StaticUrl = staticLink.StaticUrl;
                        data.IsEnable = staticLink.IsEnable;
                        data.StartDate = staticLink.StartDate;
                        data.EndDate = staticLink.EndDate;
                        data.ProcessUserID = staticLink.ProcessUserID;
                        data.ProcessDate = DateTime.UtcNow.AddHours(8);
                        data.ProcessIPAddress = staticLink.ProcessIPAddress;
                        data.IsLive = "0";
                        db.StaticLink.Update(data);
                    }
                    else
                    {
                        db.StaticLink.Add(staticLink);
                    }
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
        #region 讀取
        /// <summary>
        /// 取得靜態化資料
        /// </summary>
        /// <returns></returns>
        public static List<StaticLink> GetStaticData()
        {
            try
            {
                BeforeCheckData();
                var allData = GetAllStaticLink();
                var TidyData = GetTidyData(allData);
                return TidyData;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static List<StaticLink> DoubleCheckData(List<StaticLink> data)
        {
            try
            {
                var list = data.Where(x => !string.IsNullOrWhiteSpace(x.SourseTable))
                       .GroupBy(x => new { x.StaticUrl })
                       .Select(x => new { x.Key.StaticUrl, count = x.Count() }).ToList();
                var _list = list.Where(x => x.count > 1 && !string.IsNullOrWhiteSpace(x.StaticUrl)).Select(x => x.StaticUrl).ToList();
                var _double = data.Where(x => x.SourseTable == "weblevel" && _list.Contains(x.StaticUrl)).ToList();
                return _double;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 首頁區塊&頁首頁尾不需要靜態化
        /// </summary>
        private static void BeforeCheckData()
        {
            using (var db = new MODAContext())
            {
                var needUpdateData = (from l in db.WebLevel
                                      join n in db.WEBNews.Where(x => x.Lang == "zh-tw") on l.WebLevelSN equals n.WebLevelSN
                                      join s in db.StaticLink.Where(x => x.SourseTable == "webnews") on n.MainSN equals s.MainSN
                                      where (l.WeblevelType == "2" || l.WeblevelType == "3") && s.IsEnable != "-99"
                                      select s).ToList();
                if (needUpdateData.Count() > 0)
                {
                    foreach (var d in needUpdateData)
                    {
                        d.IsEnable = "-99";
                        d.ProcessDate = DateTime.UtcNow.AddHours(8);
                        d.staticDate = DateTime.UtcNow.AddHours(8);
                        db.StaticLink.Update(d);
                        db.SaveChanges();
                    }
                }
            }
        }
        public static List<StaticLink> NoNeedSettingSiteMapData(string Module, string ListType, string ArticleType = "")
        {
            using (var db = new MODAContext())
            {
                switch (ArticleType)
                {
                    case "2": //特殊都要檢查
                        var data = (from a in db.WebLevel
                                    join b in db.WEBNews on a.WebLevelSN equals b.WebLevelSN
                                    join c in db.StaticLink.Where(x => x.SourseTable == "webnews") on b.MainSN equals c.MainSN
                                    where 1 == 1
                                    && b.ArticleType == ArticleType
                                    && c.IsEnable == "1"
                                    select c
                        ).ToList();
                        var data2 = (from a in db.WebLevel
                                     join b in db.WEBNews on a.WebLevelSN equals b.WebLevelSN
                                     join c in db.StaticLink.Where(x => x.SourseTable == "weblevel") on a.MainSN equals c.MainSN
                                     where 1 == 1
                                     && b.ArticleType == ArticleType
                                     && c.IsEnable == "1"
                                     select c
                            ).ToList();
                        data.Union(data2).ToList();
                        return data.Union(data2).ToList();
                    default:
                        var data3 = (from a in db.WebLevel
                                     join b in db.WEBNews on a.WebLevelSN equals b.WebLevelSN
                                     join c in db.StaticLink.Where(x => x.SourseTable == "webnews") on b.MainSN equals c.MainSN
                                     where 1 == 1
                                     && (string.IsNullOrWhiteSpace(Module) ? 1 == 1 : a.Module == Module)
                                     && (string.IsNullOrWhiteSpace(ListType) ? 1 == 1 : a.ListType == ListType)
                                     && (string.IsNullOrWhiteSpace(ArticleType) ? 1 == 1 : b.ArticleType == ArticleType)
                                     && c.IsEnable == "1"
                                     select c
                            ).ToList();
                        return data3;

                }

            }

        }

        /// <summary>
        /// 取的所有資料
        /// </summary>
        /// <returns></returns>
        static List<StaticLink> GetAllStaticLink()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.StaticLink.Where(x => x.Link != "").ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 整理資料
        /// </summary>
        /// <param name="staticLinks"></param>
        /// <returns></returns>
        static List<StaticLink> GetTidyData(List<StaticLink> staticLinks)
        {

            var falseData = staticLinks.Where(x =>
            x.IsEnable != "1" ||
            x.StartDate > DateTime.UtcNow.AddHours(8) ||
           (x.EndDate.HasValue ? x.EndDate.Value < DateTime.UtcNow.AddHours(8) : false)
            ).ToList();

            var UseWebLevl = new List<StaticLink>(); //webLevel 父層關閉所有子層資料 

            GetFalsLevelData(falseData, ref UseWebLevl);

            UseWebLevl = GetFalsNewsData(UseWebLevl);
            UseWebLevl.AddRange(falseData.Where(x => x.SourseTable == "webnews").ToList());
            foreach (var staticData in staticLinks)
            {
                if (UseWebLevl.Any(x => x.SourseSN == staticData.SourseSN && x.SourseTable == staticData.SourseTable))
                {
                    staticData.IsEnable = "0";

                }
            }
            return staticLinks;
        }
        /// <summary>
        /// 取得開啟的模型
        /// </summary>
        /// <param name="staticLinks"></param>
        /// <param name="UseWebLevl"></param>
        /// <param name="UseWebNews"></param>
        static void GetFalsLevelData(List<StaticLink> staticLinks, ref List<StaticLink> UseWebLevl)
        {
            var mianData = new List<MainData>(); //關閉的全部節點
            mianData = staticLinks.Where(x => x.SourseTable == "weblevel").Select(x => new MainData()
            {
                SN = x.SourseSN.Value,
                MainSN = x.MainSN.Value,
                Lang = x.Lang,
                IsEnable = 0
            }).ToList();

            var child = new List<MainData>();
            GetWebLevelChildData(mianData, ref child);
            mianData.AddRange(child);

            UseWebLevl = mianData.Select(x => new StaticLink()
            {
                SourseTable = "weblevel",
                SourseSN = x.SN,
                Lang = x.Lang,
                MainSN = x.MainSN
            }
            ).ToList();
        }
        static void GetWebLevelChildData(List<MainData> mainDatas, ref List<MainData> datas)
        {
            try
            {
                var list = new List<MainData>();
                using (var db = new MODAContext())
                {
                    foreach (var main in mainDatas.Select(x => new { x.Lang, x.MainSN }).Distinct())
                    {
                        var data = db.WebLevel.Where(x => x.ParentSN == main.MainSN && x.Lang == main.Lang).ToList().Select(
                            x => new MainData()
                            {
                                SN = x.WebLevelSN,
                                MainSN = x.MainSN.Value,
                                Lang = x.Lang,
                                IsEnable = 0
                            }
                            ).ToList().Distinct().ToList();

                        list.AddRange(data);
                        datas.AddRange(data);
                    }
                    if (list.Count() == 0) return;
                    GetWebLevelChildData(list, ref datas);
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 產生靜態化頁面-回寫判斷
        /// </summary>
        /// <param name="staticLinks"></param>
        /// <param name="_chk">是否需要產生</param>
        public static void IsLiveSaveStatic(List<StaticLink> staticLinks, bool _chk)
        {
            try
            {
                var StaticLinkSNs = new List<int>();
                var IsLive = "0";
                if (_chk)
                {
                    //生成
                    var IsLiveData = staticLinks.Where(x => !string.IsNullOrWhiteSpace(x.SourseTable)).ToList();
                    IsLive = "1";
                    StaticLinkSNs = IsLiveData.Select(x => x.StaticLinkSN).ToList();
                }
                else
                {
                    StaticLinkSNs = staticLinks.Select(x => x.StaticLinkSN).ToList();
                }
                using (var db = new MODAContext())
                {
                    //司的資料不能壓狀態需要略過
                    var manageSourseSNs = db.WebLevel.Where(x => x.Module == "DEPT" || x.Module == "RSS").Select(x => x.WebLevelSN).ToList();
                    var manageList = db.StaticLink.Where(x => x.SourseTable == "weblevel" && manageSourseSNs.Contains(x.SourseSN.Value)).Select(X => X.StaticLinkSN).ToList();
                    var source = db.StaticLink.Where(x => StaticLinkSNs.Contains(x.StaticLinkSN)).ToList();
                    foreach (var s in source.Where(x => !manageList.Contains(x.StaticLinkSN)))
                    {
                        s.IsLive = IsLive;
                        s.staticDate = DateTime.UtcNow.AddHours(8);
                    }
                    db.StaticLink.UpdateRange(source);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {


            }

        }
        /// <summary>
        /// 因WeblLevel關閉而關閉webNews節點的資料
        /// </summary>
        /// <param name="staticLinks"></param>
        /// <param name="datas"></param>
        static List<StaticLink> GetFalsNewsData(List<StaticLink> staticLinks)
        {
            try
            {
                var FalseData = new List<StaticLink>();
                FalseData.AddRange(staticLinks);
                using (var db = new MODAContext())
                {
                    foreach (var weblevel in staticLinks)
                    {
                        var s = db.WEBNews.Where(x => x.WebLevelSN == weblevel.MainSN && x.Lang == weblevel.Lang).ToList()
                            .Select(
                            x => new StaticLink()
                            {
                                SourseSN = x.WEBNewsSN,
                                SourseTable = "webnews",
                                Lang = x.Lang
                            }
                            ).ToList();
                        FalseData.AddRange(s);
                    }
                    return FalseData;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 取得各別的靜態化資料
        /// </summary>
        /// <param name="staticLink"></param>
        /// <returns></returns>
        public static StaticLink GetStaticLink(StaticLink staticLink)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.StaticLink.FirstOrDefault(x =>
                            x.WebSiteID == staticLink.WebSiteID &&
                            x.Lang == staticLink.Lang &&
                            x.SourseTable == staticLink.SourseTable &&
                            x.MainSN == staticLink.MainSN
                            );
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion

    }
}

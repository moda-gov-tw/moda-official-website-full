using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utility.YouTube.GetApi;

namespace Services.Youtube
{
    public class YoutubeService
    {
        /// <summary>
        /// 取的基本設定
        /// </summary>
        /// <returns></returns>
        static List<BaesData> GetBaesData()
        {
            List<BaesData> list = new List<BaesData>();
            using (var db = new MODAContext())
            {
                var apisettingData = db.SysCategory.Where(x => x.Lang == "zh-tw" && x.IsEnable == "1" && x.ParentKey == "Management-4").ToList();
                if (apisettingData != null)
                {
                    foreach (var apisetting in apisettingData)
                    {

                        var apisettingData2 = db.SysCategory.Where(x => x.Lang == "zh-tw" && x.IsEnable == "1" && x.ParentKey == apisetting.SysCategoryKey).ToList();
                        if (apisettingData2?.Count() > 2)
                        {
                            var apiKey = apisettingData2.FirstOrDefault(x => x.SortOrder == 1)?.Value ?? "";
                            var cannelId = apisettingData2.FirstOrDefault(x => x.SortOrder == 2)?.Value ?? "";
                            var state = apisettingData2.FirstOrDefault(x => x.SortOrder == 3)?.Value ?? "0";
                            var webLevelMainSN = apisettingData2.FirstOrDefault(x => x.SortOrder == 4)?.Value ?? "0";
                            var youtubeType = apisettingData2.FirstOrDefault(x => x.SortOrder == 5)?.Value ?? "";
                            var chkList = apisettingData2.FirstOrDefault(x => x.SortOrder == 6)?.Value ?? "";
                            int intwebLevelMainSN = 0;
                            int.TryParse(webLevelMainSN, out intwebLevelMainSN);
                            if (state == "1" && !string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(cannelId))
                            {
                                switch (youtubeType)
                                {
                                    case "0": // 
                                        list.Add(new BaesData()
                                        {
                                            webSiteId = apisetting.Value,
                                            apiKey = apiKey,
                                            channelld = cannelId,
                                            webLevelMainSN = intwebLevelMainSN,
                                            youtubeType = youtubeType,
                                        });
                                        break;
                                    case "1": // 
                                        var ParentKey = apisettingData2.FirstOrDefault(x => x.SortOrder == 6).SysCategoryKey;
                                        var list1 = db.SysCategory.Where(x => x.IsEnable == "1" && x.Lang == "zh-tw" && x.ParentKey == ParentKey
                                        ).ToList();
                                        if (list1.Count() > 0)
                                        {
                                            var items = new List<YouPageItemModel>();
                                            foreach (var item in list1)
                                            {
                                                var list2 = db.SysCategory.Where(x => x.IsEnable == "1" && x.Lang == "zh-tw" && x.ParentKey == item.SysCategoryKey).ToList();
                                                if (list2.Count() > 1)
                                                {
                                                    items.Add(new YouPageItemModel()
                                                    {
                                                        webLevelKey = list2.FirstOrDefault(x => x.SortOrder == 1)?.Value,
                                                        channelld = list2.FirstOrDefault(x => x.SortOrder == 2)?.Value,
                                                    });
                                                }
                                            }
                                            list.Add(new BaesData()
                                            {
                                                webSiteId = apisetting.Value,
                                                apiKey = apiKey,
                                                channelld = cannelId,
                                                webLevelMainSN = intwebLevelMainSN,
                                                youtubeType = youtubeType,
                                                youPageItemModels = items
                                            });
                                        }

                                        break;
                                }

                            }
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 頻道全部影片
        /// </summary>
        /// <param name="errormsg"></param>
        public static void GetAllApiData(out string youtubemsg, out string errormsg)
        {
            errormsg = "";
            youtubemsg = "";
            try
            {
                var data = new YouTubeVideoModel();
                var baseData = GetBaesData();
                foreach (var b in baseData)
                {
                    Utility.YouTube.GetApi.baesData = b;
                    Utility.YouTube.GetApi.GetVideo(ref data);
                    IUDforAllNews(b.webLevelMainSN, b.webSiteId, data, out string msg, out OutYoutubeMgs outYoutubeMgs);
                    youtubemsg = msg;
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.ToString();
            }
        }
        /// <summary>
        /// 列表影片
        /// </summary>
        /// <returns></returns>
        public static List<YouTubePlayItemModel> GetPlayListApiData(string DemoDNS  , out string msg , out string error , out List<OutYoutubeMgs> outYoutubeMgsList)
        {
            var list = new List<YouTubePlayItemModel>();
            outYoutubeMgsList = new List<OutYoutubeMgs>();
            var baseData = GetBaesData();
            msg = "";
            error = "";
            foreach (var b in baseData)
            {
                Utility.YouTube.GetApi.baesData = b;
                var youtubelist = Utility.YouTube.GetApi.GetPlayList();
                foreach (var item in youtubelist)
                {
                    var cycledata = b.youPageItemModels.FirstOrDefault(x => x.channelld == item.Id);
                    if (cycledata != null)
                    {
                        var returnData = item;
                        item.webLevelKey = cycledata.webLevelKey;
                        item.webLevelMainSN = b.webLevelMainSN;
                        Utility.YouTube.GetApi.GetPlayListItems(ref returnData ,out string err);
                        error += err;
                        list.Add(returnData);
                        IUDApiData(b.webSiteId, item , DemoDNS , out string m, out OutYoutubeMgs outYoutubeMgs);
                        msg += m;
                        outYoutubeMgsList.Add(outYoutubeMgs);
                    }
                }
            }
            return list;
        }

        static void IUDApiData(string webSiteID, YouTubePlayItemModel youTubePlayItemModel , string DemoDNS , out string  msg, out OutYoutubeMgs outYoutubeMgs)
        {
            msg = "";
            outYoutubeMgs = new OutYoutubeMgs();
            //webLevel
            var langs = new List<string>() { "zh-tw", "en" };
            using (var db = new MODAContext())
            {
                
                var brothData = db.WebLevel.Where(x => x.ParentSN == youTubePlayItemModel.webLevelMainSN && x.IsEnable == "1" && x.Lang == "zh-tw").ToList();
                var parentData = db.WebLevel.FirstOrDefault(x => x.MainSN == youTubePlayItemModel.webLevelMainSN && x.IsEnable == "1" && x.Lang == "zh-tw");
                if (parentData != null)
                {
                    var webLevelMainSN = 0;
                    var levelData = db.WebLevel.FirstOrDefault(x => x.IsEnable == "1" && x.WebLevelKey == youTubePlayItemModel.webLevelKey && x.WebSiteID == webSiteID && x.Lang == "zh-tw");
                    if (levelData != null)
                    {
                        webLevelMainSN = levelData.MainSN.Value;
                    }
                    else
                    {
                        var sort = brothData.Count() + 1;
                        var webLevelSN =new List<int>();
                        foreach (var l in langs)
                        {
                            var webLevelDATA = new WebLevel()
                            {
                                WebLevelKey = youTubePlayItemModel.webLevelKey,
                                ParentSN = youTubePlayItemModel.webLevelMainSN,
                                Lang = l,
                                WebSiteID = webSiteID.ToUpper(),
                                WeblevelType = "1",
                                Module = "NEWS",
                                Parameter = "0",
                                Title = youTubePlayItemModel.Title,
                                FatFooterShow = "1",
                                MainMenuShow = "1",
                                LeftMenuShow = "1",
                                RSSShow = "0",
                                StartDate = DateTime.UtcNow.AddHours(8),
                                ListType = "ImageTextList",
                                SortMethod = "0",
                                IsEnable = l =="zh-tw" ? "1" :"0",
                                CreatedUserID = "www-mgr",
                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                ProcessIPAddress = "::1",
                                ProcessUserID = "www-mgr",
                                ProcessDate = DateTime.UtcNow.AddHours(8),
                                DepartmentID = "M",
                                SortOrder = sort,
                            };
                            db.WebLevel.Add(webLevelDATA);
                            db.SaveChanges();
                            webLevelSN.Add(webLevelDATA.WebLevelSN);
                            if (l == "zh-tw") {
                                webLevelMainSN = webLevelDATA.WebLevelSN;
                            }
                        }
                        var setMainSN = db.WebLevel.Where(x => webLevelSN.Contains(x.WebLevelSN)).ToList();
                        foreach (var s in setMainSN)
                        {
                            s.MainSN = webLevelMainSN;
                        }
                        db.WebLevel.UpdateRange(setMainSN);
                        db.SaveChanges();
                        Services.Static.StaticLinkService.Save(setMainSN.FirstOrDefault(x => x.Lang == "zh-tw"), DemoDNS);
                    }

                    YouTubeVideoModel youTubeVideoModel = new();
                    
                    youTubeVideoModel.youTubePlaylistItemDetailModels = youTubePlayItemModel.youTubePlaylistItemDetailModels;
                    youTubeVideoModel.Title = youTubePlayItemModel.Title;
                    IUDforAllNews(webLevelMainSN, webSiteID, youTubeVideoModel, out string m, out OutYoutubeMgs outYoutubeMs);
                    msg += $"{m}";
                    outYoutubeMgs = outYoutubeMs;
                    Services.Static.StaticLinkService.Save(parentData, DemoDNS);
                }
            }
        }
        /// <summary>
        /// 新增修改刪除-資料
        /// </summary>
        /// <param name="webLevelMainSN"></param>
        /// <param name="webSiteID"></param>
        /// <param name="youTubeVideoModel"></param>
        static void IUDforAllNews(int webLevelMainSN, string webSiteID, YouTubeVideoModel youTubeVideoModel, out string msg , out OutYoutubeMgs outYoutubeMgs )
        {
            msg = "";
            var NewData = new List<WEBNews>();
            var sort = 1;
            var langs = new List<string>() { "zh-tw", "en" };
            using (var db = new MODAContext())
            {
                var oldData = (from a in db.WEBNews
                               join b in db.WEBNewsExtend on a.WEBNewsSN equals b.WEBNewsSN
                               where a.WebLevelSN == webLevelMainSN
                                && b.GroupID == "relatedvideo"
                                && a.IsEnable == "1"
                               select b.Column_1).Distinct().ToList();  //youtubeId

                var apiData = youTubeVideoModel.youTubePlaylistItemDetailModels.Where(x => !string.IsNullOrWhiteSpace(x.YoutubeId)).Select(x => x.YoutubeId).ToList();
                var insertData = apiData.Except(oldData);
                var deleteData = oldData.Except(apiData);
                msg = $@"播放清單：{youTubeVideoModel.Title},同步影音：{apiData.Count()}筆,其中新增：{insertData.Count()}筆,刪除：{deleteData.Count()}筆.";
                outYoutubeMgs = new OutYoutubeMgs();
                outYoutubeMgs.Title = $@"{youTubeVideoModel.Title},同步影音：{apiData.Count()}筆,其中新增：{insertData.Count()}筆,刪除：{deleteData.Count()}筆.";
                var insertList = new List<string>();
                var deleteList = new List<string>();

                #region step 1 delete
                var dData = (from a in db.WEBNews
                             join b in db.WEBNewsExtend on a.WEBNewsSN equals b.WEBNewsSN
                             where a.WebLevelSN == webLevelMainSN
                              && a.IsEnable == "1"
                              && b.GroupID == "relatedvideo"
                              && deleteData.Contains(b.Column_1)
                             select a).ToList();
                foreach (var d in dData)
                {
                    deleteList.Add(d.Title);
                    d.IsEnable = "-99";
                    d.PublishDate = DateTime.UtcNow.AddHours(8);
                }
                db.WEBNews.UpdateRange(dData);
                db.SaveChanges();
                #endregion
                #region step 2 insert
                foreach (var d in youTubeVideoModel.youTubePlaylistItemDetailModels.Where(x => insertData.Contains(x.YoutubeId)).OrderBy(x => x.Sort))
                {
                    var ns = new List<WEBNews>();
                    foreach (var l in d.YouTubeLanguageTitleModels)
                    {
                        var n = new WEBNews()
                        {
                            WebSiteID = webSiteID.ToUpper(),
                            WebLevelSN = webLevelMainSN,
                            Lang = l.Language.ToLower(),
                            Module = "NEWS",
                            ArticleType = "0",
                            Title = d.Title,
                            PublishDate = d.PushData,
                            ContentText = " ",
                            DepartmentID = "M",
                            StartDate = d.PushData,
                            IsEnable = l.HasData ? "1" : "0",
                            SortOrder = d.Sort,
                            CreatedDate = DateTime.UtcNow.AddHours(8),
                            ProcessUserID = "www-mgr",
                            ProcessDate = DateTime.UtcNow.AddHours(8),
                            CreatedUserID = "www-mgr",
                            ProcessIPAddress = "::1"
                        };
                        ns.Add(n);
                        insertList.Add(d.Title);
                    }
                    db.WEBNews.AddRange(ns);
                    db.SaveChanges();
                    var WEBNewsExtends = new List<WEBNewsExtend>();
                    #region updata main
                    foreach (var n in ns)
                    {
                        n.MainSN = ns.FirstOrDefault(x => x.Lang == "zh-tw").WEBNewsSN;
                        WEBNewsExtends.Add(new DBModel.WEBNewsExtend()
                        {
                            WEBNewsSN = n.WEBNewsSN,
                            GroupID = "relatedvideo",
                            Column_1 = d.YoutubeId,
                            Column_2 = d.Title,
                        });
                    }
                    db.WEBNewsExtend.AddRange(WEBNewsExtends);
                    db.SaveChanges();
                    #endregion
                    sort++;
                }
                #endregion
                #region step 3 Update
                foreach (var u in youTubeVideoModel.youTubePlaylistItemDetailModels.OrderBy(x => x.Sort))
                {
                    var source = (from a in db.WEBNews
                                   join b in db.WEBNewsExtend on a.WEBNewsSN equals b.WEBNewsSN
                                   where a.WebLevelSN == webLevelMainSN
                                    && b.GroupID == "relatedvideo"
                                    && b.Column_1 == u.YoutubeId
                                    select a).ToList();

                    foreach (var s in source)
                    {
                        var langData = u.YouTubeLanguageTitleModels.FirstOrDefault(x => x.Language.ToLower() == s.Lang.ToLower());
                        s.IsEnable = langData.HasData ? "1"  : "0" ;
                        s.Title = langData.Title;
                        s.Description = langData.Description;
                        s.SortOrder = u.Sort ;
                        s.PublishDate = u.PushData;
                        s.StartDate = u.PushData;
                        db.WEBNews.Update(s);
                        db.SaveChanges();
                    }
                }
                #endregion

                outYoutubeMgs.InestTitles = insertList;
                outYoutubeMgs.DelelteTitles= deleteList;
            }
        }

        public class OutYoutubeMgs
        {
            public string Title { get; set; } = "";
          
            public List<string> InestTitles { get; set; } = new List<string>();

            public List<string> DelelteTitles { get; set; } = new List<string>();

        }
    }
}

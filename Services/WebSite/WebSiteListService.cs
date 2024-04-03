using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Models.WebContent.WebLevelManagement;
using Services.Models.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utility;

namespace Services.WebSite
{
    public class WebSiteListService
    {
        /// <summary>
        /// 取得雙語詞彙
        /// </summary>
        /// <param name="webLevelMainSN"></param>
        /// <param name="lang"></param>
        /// <param name="key"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static List<WEBNewsListModel> GetBilingualListData(int webLevelMainSN, string lang, string key , ref Utility.DefaultPager pager, out List<WEBNewsListModel2> bigData , bool onlyRegulations = false)
        {
            var mainSNList = new List<int>();
            var whereSql = "";
            if (!string.IsNullOrWhiteSpace(key))
            {
                using (var db = new MODAContext())
                { 
                   mainSNList = db.WEBNews.Where(x=>
                    x.WebLevelSN == webLevelMainSN  && 
                    x.IsEnable == "1" &&
                    (x.StartDate == null || x.StartDate <= DateTime.UtcNow.AddHours(8)) &&
                    (x.EndDate == null || x.EndDate >= DateTime.UtcNow.AddHours(8)) &&
                    x.Title.Contains(key) 
                    ).Distinct().Select(x=>x.MainSN.Value).ToList();
                }
            }
            


            var listData = new List<WEBNewsListModel>();
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@WebLevelSN", webLevelMainSN));
            sqlParams.Add(new SqlParameter("@StartDate", DateTime.UtcNow.AddHours(8)));
            sqlParams.Add(new SqlParameter("@EndDate", DateTime.UtcNow.AddHours(8)));
            var sql = $@"
 	select 
	n.MainSN ,
	REPLACE ( n.Lang,'-','') as Lang ,
	Title ,
	case ArticleType
		when 11 then 1
		else 0 end ArticleType,
	case ArticleType
		when 0 then slink.StaticUrl
		when 1 then f.FilePath 
		when 2 then n.[URL] 
	    else  '' 
	end  [Url] ,
    n.target ,
    FileType ,
    ex.Column_1 as RegulationsURL ,
	ex.Column_2 as RegulationsTitle
	from  [WEBNews]  n
	left join StaticLink slink on n.WEBNewsSN = slink.SourseSN and slink.SourseTable='WEBNews'
	left join [dbo].[RelWebFileContent] relFile on n.WEBNewsSN = relFile.SourceSN and relFile.SourceTable= 'WEBNews' and relFile.GroupID='NWSF'
	left join [dbo].[WEBFile] f on relFile.WEBFileSN = f.WEBFileSN
    left join WEBNewsExtend ex on n.WEBNewsSN =ex.WEBNewsSN and ex.GroupID='Regulations'
	where WebLevelSN=@WebLevelSN 
	and n.IsEnable ='1'
    and (n.StartDate is null or n.StartDate <= @StartDate)
    and (n.EndDate is null or n.EndDate >= @EndDate)
";
            using (var db = new MODAContext())
            {
                var webNewsListModellist = new List<WEBNewsListModel>();
                var list = db.BilingualListModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                var groupByData = list.GroupBy(x => new { x.MainSN }).Select(x => new { x.Key, x }).ToList();
               
               if (!string.IsNullOrWhiteSpace(key))
               {
                   groupByData = groupByData.Where( x=> mainSNList.Contains(x.Key.MainSN)).ToList();
               }
                if (onlyRegulations)
                {
                   var RegulationsMain = (from a  in db.WEBNews  
                                         join b  in db.WEBNewsExtend.Where(x => x.GroupID == "Regulations") on a.WEBNewsSN equals b.WEBNewsSN
                                         select a.MainSN).Distinct().ToList();
                    groupByData = groupByData.Where(x => RegulationsMain.Contains(x.Key.MainSN)).ToList();
                }
                foreach (var g in groupByData)
                {
                    var endata = g.x.FirstOrDefault(x => x.Lang == "en");
                    var zhtwdata = g.x.FirstOrDefault(x => x.Lang == "zhtw");
                    var articleType = g.x.Any(x => x.ArticleType == 0) ? 0 : 1;

                    webNewsListModellist.Add(new WEBNewsListModel()
                    {
                        MainSN = g.Key.MainSN,
                        en = endata.Title,
                        zhtw = zhtwdata.Title,
                        ArticleType = articleType,
                        zhtwURL = zhtwdata.Url,
                        enURL = endata.Url,
                        zhtwtarget = zhtwdata.target,
                        entarget = endata.target,
                        zhtwfiletype = zhtwdata?.FileType?.Split('.')[1].ToUpper(),
                        enfiletype = endata?.FileType?.Split('.')[1].ToUpper(),
                        zhtw_filetypedisplay = string.IsNullOrWhiteSpace(zhtwdata.FileType) ? "none" : "inline",
                        en_filetypedisplay = string.IsNullOrWhiteSpace(endata.FileType) ? "none" : "inline",
                        bzhUrldisplay = string.IsNullOrWhiteSpace(zhtwdata.Url) ? "none" : (zhtwdata.Url.Contains("http") ? "inline" : "none"),
                        benUrldisplay = string.IsNullOrWhiteSpace(endata.Url) ? "none" : (endata.Url.Contains("http") ? "inline" : "none"),
                        zhtwregulationsURL = zhtwdata.RegulationsURL,
                        zhtwregulationsTitle = zhtwdata.RegulationsTitle,
                        zhtw_regulationsdisplay = string.IsNullOrWhiteSpace(zhtwdata.RegulationsURL) ? "none" : "inline",
                        enregulationsURL = endata.RegulationsURL,
                        enregulationsTitle = endata.RegulationsTitle,
                        en_regulationsdisplay = string.IsNullOrWhiteSpace(endata.RegulationsURL) ? "none" : "inline",
                    }
                   );
                }

                pager.TotalCount = groupByData.Count;
                pager.PageIndex = pager.p - 1;
                webNewsListModellist = webNewsListModellist.OrderBy(x=>x.ArticleType).ThenBy(x => x.en).ToList();
                
                bigData = webNewsListModellist.Select(x => new WEBNewsListModel2() { 
                    enTitle = x.en,
                    zhtwTitle = x.zhtw,
                    MainSN = x.MainSN,
                    ArticleType = x.ArticleType,
                    bzhURL= x.zhtwURL ,
                    benURL = x.enURL ,
                    bzhtwtarget = x.zhtwtarget ,
                    bentarget = x.entarget ,
                    benfiletype = x.enfiletype,
                    bzhfiletype = x.zhtwfiletype,
                    bzhfiledisplay = x.zhtw_filetypedisplay ,
                    benfiledisplay = x.en_filetypedisplay ,
                    bzhUrldisplay = x.bzhUrldisplay,
                    benUrldisplay = x.benUrldisplay,
                    bzhHasUrl = string.IsNullOrWhiteSpace( x.zhtwURL) ? "none":"inline",
                    bzhNoUrl = string.IsNullOrWhiteSpace(x.zhtwURL) ? "inline" : "none",
                    benHasUrl = string.IsNullOrWhiteSpace(x.enURL) ? "none" : "inline",
                    benNoUrl = string.IsNullOrWhiteSpace(x.enURL) ? "inline" : "none",
                    zhtwregulationsURL = x.zhtwregulationsURL,
                    zhtwregulationsTitle = x.zhtwregulationsTitle,
                    zhtw_regulationsdisplay = x.zhtw_regulationsdisplay,
                    enregulationsURL = x.enregulationsURL,
                    enregulationsTitle = x.enregulationsTitle,
                    en_regulationsdisplay = x.en_regulationsdisplay,
                }).ToList();
                listData = webNewsListModellist.Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                return listData;
            }

        }
        public static List<WEBNewsListModel> GetNewsListData(int webLevelMainSN, string lang, string str_date, string end_date, string key, string Condition4, string Condition5, string Condition6, string CustomizeTagSN, string SysZipCode, out List<WEBNewsListModel2> JSONModel, ref Utility.DefaultPager pager, string chief)
        {
            var isStatic = CommonService.IsStatic;
            var list = GetNewsList(webLevelMainSN, lang, str_date, end_date, key, Condition4, Condition5, Condition6, CustomizeTagSN, SysZipCode, chief)
                        .OrderBy(o => o.webNews.SortOrder).ToList();

            var LevelData = GetWebLevel(webLevelMainSN, lang);
            //首長行程特例排序
            if (LevelData?.Module == "Schedule")
            {
                list = list.OrderByDescending(x => x.webNews.PublishDate).ToList();
            }

            JSONModel = new List<WEBNewsListModel2>();
            foreach (var item in list)
            {
                var relatedFile = new List<webfile>();
                var relatedImg = new List<webfile>();
                var relatedlink = new List<webnewsextend>();
                var relatedvideo = new List<webnewsextend>();
                var relatedmoj = new List<webnewsextend>();
                var link = isStatic == true ? item.webNews.StatesUrl : $@"/{item.webNews.WebSiteID}/{item.webNews.Lang}/News/{item.webNews.MainSN}";
                item.webNews.StatesUrl = link;
                switch (item.webNews.ArticleType)
                {
                    case "1":
                        link = item.webFile != null ? item.webFile.FilePath : "";
                        break;
                    case "2":
                        link = item.webNews.URL;
                        break;
                }
                //手風琴 模式才需要撈取 -相關檔案/連結/法規/圖片/影片
                if (LevelData?.ListType == "AccordionList")
                {
                    //   相關檔案/圖片
                    relatedFile = GetWebNewFile(item.webNews.WEBNewsSN, Utility.WebFileGroupID.News.Files, "WEBNews");
                    relatedImg = GetWebNewFile(item.webNews.WEBNewsSN, Utility.WebFileGroupID.News.Imgs, "WEBNews");
                    //   相關 /連結/法規/影片 GetWebnewsextend
                    relatedlink = GetWebnewsextend(item.webNews.WEBNewsSN, "relatedlink");
                    relatedvideo = GetWebnewsextend(item.webNews.WEBNewsSN, "relatedvideo");
                    relatedmoj = GetWebnewsextend(item.webNews.WEBNewsSN, "relatedmoj");

                    item.relatedFile = relatedFile;
                    item.relatedImg = relatedImg;
                    item.relatedlink = relatedlink;
                    item.relatedvideo = relatedvideo;
                    item.relatedmoj = relatedmoj;
                }

                if (LevelData?.Module != "Schedule")
                {
                    JSONModel.Add(new WEBNewsListModel2()
                    {
                        mainsn = item.webNews.MainSN.ToString(),
                        href = link,
                        title = item.webNews.Title,
                        target = item.webNews.target,
                        filetypedisplay = item.webFile != null ? "inline" : "none",
                        filetype = item.webFile != null ? item.webFile.FileType.ToUpper().Split('.')[1] : "",
                        newstitle = item.webNews.Title,
                        newssubtitle = item.webNews.SubTitle,
                        istopdisplay = (item.webNews.IsTop == null || item.webNews.IsTop == "0") ? "none" : "inline",
                        startdate = item.webNews.StartDate.Value.ToString("yyyy-MM-dd"),
                        syscategoriesdisplay = item.sysCategories.Count() > 0 ? "block" : "none",
                        contenttext = LevelData?.ListType == "AccordionList" ? item.webNews.ContentText : "",
                        weblogopath = item.webLogo != null ? item.webLogo.FilePath : "~/assets/img/icon_dept4-2-1.svg",
                        webvideokey = item.webVideo != null ? item.webVideo.Column_1 : "",
                        tags = item.sysCategories.Count() > 0 ? item.sysCategories.Select(x => new WEBNewsTagModel2() { tag = x.Value }).ToList() : new List<WEBNewsTagModel2>(),
                        //  schedule = item.schedule,
                    });
                }
            }

            pager.TotalCount = list.Count();
            pager.PageIndex = pager.p - 1;
            list = list.Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();


            return list;
        }

        /// <summary>
        /// 撈取首長資料
        /// </summary>
        /// <param name="wEBNewsListModels"></param>
        /// <param name="categories"></param>
        public static List<WEBNewsListModel> GetGetNewsListDataSchedule(List<WEBNewsListModel> wEBNewsListModels, List<SysCategory> categories)
        {
            var defore = categories.Where(x => x.IsEnable == "1" && x.ParentKey == x.WebSiteID + "-13").OrderBy(x => x.SortOrder).FirstOrDefault();
            using (var db = new MODAContext())
            {
                foreach (var detail in wEBNewsListModels)
                {
                    var newsdata = db.WEBNewsExtend.FirstOrDefault(x => x.GroupID == "chief" && x.WEBNewsSN == detail.webNews.WEBNewsSN);
                    if (newsdata != null)
                    {
                        var sdefore = categories.FirstOrDefault(x => x.SysCategoryKey == newsdata.Column_1);
                        detail.scheduletTitle = $@"{sdefore?.Value}{sdefore?.Description}";
                        detail.scheduletChiefSort = sdefore.SortOrder.Value;
                    }
                    else
                    {
                        detail.scheduletTitle = $@"{defore?.Value}{defore?.Description}";
                        detail.scheduletChiefSort = defore.SortOrder.Value;
                    }

                }
            }
            return wEBNewsListModels.OrderByDescending(x => x.webNews.PublishDate).ThenBy(x => x.scheduletChiefSort).ToList();

        }
        public static List<WebSiteWebLevelPageListModel> GetRSSListData(string WebSiteID, string Lang)
        {
            var result = new List<WebSiteWebLevelPageListModel>();

            using (var db = new MODAContext())
            {
                var data = db.WebLevel.Where(x => x.RSSShow == "1" && x.IsEnable == "1" && x.WebSiteID == WebSiteID && x.Lang == Lang).ToList();

                foreach (var webLevel in data)
                {
                    result.Add(new WebSiteWebLevelPageListModel()
                    {
                        webLevel = webLevel,
                        url = $"{CommonService.WebAPIUrl}/RSS/RSSChannel/{webLevel.Lang}/{webLevel.MainSN}",
                        target = "_blank",
                    });
                }
            }

            return result;
        }

        private static List<WEBNewsListModel> GetNewsList(int webLevelMainSN, string lang, string str_date, string end_date, string key, string Condition4, string Condition5, string Condition6, string CustomizeTagSN, string SysZipCode, string Condition7)
        {
            var result = new List<WEBNewsListModel>();
            var LevelData = GetWebLevel(webLevelMainSN, lang);

            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.WEBNews.Where(x => x.WebLevelSN == webLevelMainSN
                            && x.Lang == lang
                            && x.IsEnable == "1"
                            && (x.StartDate == null || x.StartDate <= DateTime.UtcNow.AddHours(8))
                            && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.AddHours(8))
                            );

                    if (LevelData?.Module == "Schedule")
                    {
                        if (!string.IsNullOrWhiteSpace(str_date))
                        {
                            var _strdt = new DateTime();
                            if (DateTime.TryParse(str_date + " 00:00:00", out _strdt))
                            {
                                data = data.Where(x => x.PublishDate >= _strdt);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(end_date))
                        {
                            var _enddt = new DateTime();
                            if (DateTime.TryParse(end_date + " 23:59:59", out _enddt))
                            {
                                data = data.Where(x => x.PublishDate <= _enddt);
                            }
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(str_date))
                        {
                            var _strdt = new DateTime();
                            if (DateTime.TryParse(str_date + " 00:00:00", out _strdt))
                            {
                                data = data.Where(x => x.StartDate >= _strdt);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(end_date))
                        {
                            var _enddt = new DateTime();
                            if (DateTime.TryParse(end_date + " 23:59:59", out _enddt))
                            {
                                data = data.Where(x => x.StartDate <= _enddt);
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        var _key = key.Replace("台", "臺");

                        var fileExtend = (from n in data
                                          join a in db.RelWebFileContent on n.WEBNewsSN equals a.SourceSN
                                          join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                          join c in db.WebFileExtend on b.WEBFileSN equals c.WebFileExtendSN
                                          where a.SourceTable == "WEBNews"
                                          && b.IsEnable == "1"
                                          && c.FileContentText.Replace("台", "臺").Contains(_key)
                                          select n).ToList();

                        var newstag = (from n in data
                                       join e in db.WEBNewsExtend on n.WEBNewsSN equals e.WEBNewsSN
                                       join c in db.SysCategory on e.SysCategoryKey equals c.SysCategoryKey
                                       where c.IsEnable == "1"
                                       && c.ParentKey == n.WebSiteID + "-2"
                                       && c.Value.Replace("台", "臺").Contains(_key)
                                       select n).ToList();

                        var Transcript = (from n in data
                                          join e in db.WEBNewsTranscript on n.WEBNewsSN equals e.WEBNewsSN
                                          where e.TranscriptContent.Replace("台", "臺").Contains(_key) || e.TranscriptForm.Replace("台", "臺").Contains(_key)
                                          select n).ToList();

                        var keyword = (from n in data
                                       join e in db.WEBNewsExtend on n.WEBNewsSN equals e.WEBNewsSN
                                       where e.GroupID == "keyword"
                                       && e.Column_1.Replace("台", "臺").Contains(_key)
                                       select n).ToList();

                        var fe = fileExtend.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var nt = newstag.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var ts = Transcript.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var kw = keyword.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var keysInt = fe.Union(nt).Union(ts).Union(kw).ToList();


                        data = data.Where(x => x.ContentText.Replace("台", "臺").Contains(_key)
                        || x.Title.Replace("台", "臺").Contains(_key)
                        || keysInt.Contains(x.WEBNewsSN)
                        );

                    }
                    if (!string.IsNullOrEmpty(Condition4))
                    {
                        var extend = (from b in data
                                      join c in db.WEBNewsExtend
                                      on b.WEBNewsSN equals c.WEBNewsSN
                                      where c.SysCategoryKey == Condition4
                                      select c.WEBNewsSN).ToList();

                        data = data.Where(r => extend.Contains(r.WEBNewsSN));
                    }
                    if (!string.IsNullOrEmpty(Condition5))
                    {
                        var extend = (from b in data
                                      join c in db.WEBNewsExtend
                                      on b.WEBNewsSN equals c.WEBNewsSN
                                      where c.SysCategoryKey == Condition5
                                      select c.WEBNewsSN).ToList();

                        data = data.Where(r => extend.Contains(r.WEBNewsSN));
                    }
                    if (!string.IsNullOrEmpty(Condition6))
                    {
                        var extend = (from b in data
                                      join c in db.WEBNewsExtend
                                      on b.WEBNewsSN equals c.WEBNewsSN
                                      where c.SysCategoryKey == Condition6
                                      select c.WEBNewsSN).ToList();

                        data = data.Where(r => extend.Contains(r.WEBNewsSN));
                    }
                    if (!string.IsNullOrEmpty(Condition7))
                    {
                        var extend = (from b in data
                                      join c in db.WEBNewsExtend
                                      on b.WEBNewsSN equals c.WEBNewsSN
                                      where c.SysCategoryKey == Condition7
                                      select c.WEBNewsSN).ToList();

                        data = data.Where(r => extend.Contains(r.WEBNewsSN));
                    }
                    if (!string.IsNullOrEmpty(CustomizeTagSN))
                    {
                        if (int.TryParse(CustomizeTagSN, out int _TagSN))
                        {
                            data = data.Where(r => r.CustomizeTagSn == _TagSN);
                        }
                    }
                    if (!string.IsNullOrEmpty(SysZipCode))
                    {
                        if (int.TryParse(SysZipCode, out int _ZipSN))
                        {
                            data = data.Where(r => r.ZipCodeSn == _ZipSN);
                        }
                    }
                    //排序
                    var searchdata = data.OrderBy(o => o.SortOrder).ToList();

                    foreach (var item in searchdata)
                    {
                        var detail = new WEBNewsListModel();
                        detail.webNews = item;
                        var sysC = (from a in db.WEBNewsExtend
                                    join b in db.SysCategory on a.SysCategoryKey equals b.SysCategoryKey
                                    where a.WEBNewsSN == item.WEBNewsSN && a.GroupID == "tab" && b.Lang == item.Lang
                                    select b).ToList();
                        detail.sysCategories = sysC;
                        if (item.ArticleType == "1")
                        {
                            var file = (from a in db.RelWebFileContent
                                        join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                        where a.SourceSN == item.WEBNewsSN
                                        && a.SourceTable == "WEBNews"
                                        && a.GroupID == "NWSF"
                                        && b.IsEnable == "1"
                                        select b).FirstOrDefault();
                            detail.webFile = file;
                        }
                        else
                        {
                            detail.webFile = null;
                        }
                        var Logo = (from a in db.RelWebFileContent
                                    join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                    where a.SourceSN == item.WEBNewsSN
                                    && a.SourceTable == "WEBNews"
                                    && a.GroupID == "LOGO"
                                    && b.IsEnable == "1"
                                    select b).FirstOrDefault();
                        detail.webLogo = Logo;
                        var Video = (from a in db.WEBNewsExtend
                                     where a.WEBNewsSN == item.WEBNewsSN
                                     && a.GroupID == "relatedvideo"
                                     select a).FirstOrDefault();
                        detail.webVideo = Video;

                        if (detail.webNews.Module == "Schedule")
                        {
                            detail.schedule = GetSchedule(detail.webNews);
                        }

                        result.Add(detail);
                    }
                }
                catch (Exception)
                {

                }
                return result;
            }
        }

        private static Schedule GetSchedule(WEBNews wEBNews)
        {
            var result = new Schedule();
            result.scheduledate = wEBNews.PublishDate.Value.ToLongDateString();
            result.scheduletime = wEBNews.PublishDate.Value.ToString("tt hh:mm");

            using (var db = new MODAContext())
            {
                //相關新聞
                result.schedulenews = new List<ScheduleNews>();

                var schedulenews = (from a in db.WebNewsSchedule
                                    join b in db.WEBNews on a.FromWebNewsSn equals b.WEBNewsSN
                                    where a.ToWebNewsSn == wEBNews.WEBNewsSN
                                    && b.IsEnable == "1"
                                    && b.ArticleType == "0"
                                    select b).ToList();

                foreach (var news in schedulenews)
                {
                    var newsimages = (from a in db.RelWebFileContent
                                      join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                      where a.SourceTable == "WEBNews"
                                      && a.SourceSN == news.WEBNewsSN
                                      && a.GroupID == "NWMI"
                                      && b.IsEnable == "1"
                                      orderby a.SortOrder
                                      select new webfile
                                      {
                                          title = b.FileTitle,
                                          filename = b.FileTitle,
                                          filetype = b.FileType,
                                          url = b.FilePath,
                                      }).ToList();

                    var newsvideoes = (from a in db.WEBNewsExtend
                                       where a.WEBNewsSN == news.WEBNewsSN
                                       && a.GroupID == "relatedvideo"
                                       select new webnewsextend
                                       {
                                           column_1 = a.Column_1,
                                           column_2 = a.Column_2,
                                       }).ToList();
                    var context = StripHTML(news.ContentText);
                    context = String.IsNullOrWhiteSpace(context) ? "" : context.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\\", "\\\\").Replace("	", "").Replace("	", "").Replace(@"""", "'");
                    var SNews = new ScheduleNews
                    {
                        newstitle = news.Title,
                        newscontext = context.Length > 150 ? context.Substring(0, 149) + "......" : context,
                        newsurl = news.StatesUrl,
                        newsimage = newsimages,
                        imagedisplay = newsimages.Count > 0 ? "block" : "none",
                        newsvideo = newsvideoes,
                        videodisplay = newsvideoes.Count > 0 ? "block" : "none",
                    };

                    result.schedulenews.Add(SNews);
                }
                result.schedulenewsdisplay = result.schedulenews.Count() > 0 ? "block" : "none";

                //相關連結
                result.schedulelink = (from a in db.WEBNewsExtend
                                       where a.WEBNewsSN == wEBNews.WEBNewsSN
                                       && a.GroupID == "relatedlink"
                                       select new webnewsextend
                                       {
                                           column_1 = a.Column_1,
                                           column_2 = a.Column_2,
                                           isLocal = CommFun.CheckLocalUrl(a.Column_1, CommonService.WebSiteUrl, CommonService.WebAPIUrl),
                                       }).ToList();

                result.schedulelinkdisplay = result.schedulelink.Count() > 0 ? "block" : "none";

                //相關檔案
                result.schedulefile = (from a in db.RelWebFileContent
                                       join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                       where a.SourceTable == "WEBNews"
                                       && a.SourceSN == wEBNews.WEBNewsSN
                                       && a.GroupID == "NWMF"
                                       && b.IsEnable == "1"
                                       orderby a.SortOrder
                                       select new webfile
                                       {
                                           title = b.FileTitle,
                                           filename = b.FileTitle.Replace(b.FileType, ""),
                                           filetype = b.FileType.Replace(".", "").ToUpper(),
                                           url = b.FilePath,
                                       }).ToList();

                result.schedulefiledisplay = result.schedulefile.Count() > 0 ? "block" : "none";
            }

            return result;
        }

        private static DBModel.WebLevel GetWebLevel(int mainSN, string lang)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel.FirstOrDefault(X => X.MainSN == mainSN && X.Lang == lang && X.IsEnable == "1");
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 取得已確認存在檔案少串一個table
        /// </summary>
        /// <returns></returns>
        public static List<webfile> GetWebNewFile(int SourceSN, string FileGroupId, string SourceTable)
        {
            using (var db = new MODAContext())
            {
                var data = (from a in db.RelWebFileContent
                            join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                            where a.SourceSN == SourceSN
                            && a.SourceTable == SourceTable
                            && FileGroupId == a.GroupID
                            orderby a.SourceTable, a.SortOrder
                            select b).ToList();
                if (data?.Count() > 0)
                {
                    return data
                    ?.Select(x => new webfile()
                    {
                        filename = x.FileName,
                        filetype = x.FileType,
                        title = x.FileTitle,
                        url = x.FilePath,
                    }).ToList();
                }
                return null;
            }
        }

        public static List<webnewsextend> GetWebnewsextend(int WEBNewsSN, string GroupID)
        {
            using (var db = new MODAContext())
            {
                var data = db.WEBNewsExtend.Where(x => x.WEBNewsSN == WEBNewsSN && x.GroupID == GroupID).ToList();
                if (data?.Count() > 0)
                {
                    return data.Select(a => new webnewsextend()
                    {
                        column_1 = a.Column_1,
                        column_2 = a.Column_2,
                        isLocal = CommFun.CheckLocalUrl(a.Column_1, CommonService.WebSiteUrl, CommonService.WebAPIUrl)
                    }).ToList();
                }
                return null;
            }
        }

        private static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}

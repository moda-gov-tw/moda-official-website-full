using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utility.Model;

namespace Services.WebSite
{
    public class RSSService
    {
        //public static List<WebLevel> getRSSData(string WebSiteID, int WebLevelSN)
        //{
        //    var BigData = new List<WebLevel>();

        //    return RSSlist(WebSiteID);
        //}

        //static List<WebLevel> RSSlist(string WebSiteID)
        //{
        //    using (var db = new MODAContext())
        //    {
        //        var webLeveLDBigData = db.WebLevel.Where(x =>
        //        x.WebSiteID == WebSiteID
        //        && x.IsEnable == "1"
        //        && (x.StartDate == null || x.StartDate <= DateTime.Now)
        //        && (x.EndDate == null || x.EndDate >= DateTime.Now)
        //        && x.WeblevelType == "1"
        //        );

        //        var Items = new List<string>() { "CP", "NEWS", "JOURNAL", "BANKNOTE1", "BANKNOTE2" };
        //        var list = webLeveLDBigData.Where(x =>
        //                    Items.Contains(x.Module)
        //                    && x.WeblevelType == "1"
        //                    && x.RSSShow == "1"
        //        ).ToList();

        //        var Mdata = new List<WebLevel>();
        //        foreach (var data in list)
        //        {
        //            var source = webLeveLDBigData.FirstOrDefault(x => x.WebLevelSN == data.ParentSN);
        //            if (source != null)
        //            {
        //                if (data.ParentSN == 1)
        //                {
        //                    Mdata.Add(data);
        //                }
        //                else
        //                {
        //                    var source2 = webLeveLDBigData.FirstOrDefault(x => x.WebLevelSN == source.ParentSN);
        //                    if (source2 != null)
        //                    {
        //                        data.Title = $@"{source.Title}/{data.Title}";
        //                        if (source2.ParentSN == 0)
        //                        {
        //                            Mdata.Add(data);
        //                        }
        //                        else
        //                        {
        //                            var source3 = webLeveLDBigData.FirstOrDefault(x => x.WebLevelSN == source2.ParentSN);
        //                            if (source3.ParentSN == 0) { Mdata.Add(data); }
        //                            else
        //                            {
        //                                data.Title = $@"{source2.Title}/{data.Title}";
        //                                var source4 = webLeveLDBigData.FirstOrDefault(x => x.WebLevelSN == source3.ParentSN);
        //                                if (source4.ParentSN == 0) { Mdata.Add(data); }
        //                                else
        //                                {
        //                                    data.Title = $@"{source3.Title}/{data.Title}";
        //                                    var source5 = webLeveLDBigData.FirstOrDefault(x => x.WebLevelSN == source4.ParentSN);
        //                                    if (source5.ParentSN == 0) { Mdata.Add(data); }
        //                                    else
        //                                    {
        //                                        data.Title = $@"{source4.Title}/{data.Title}";
        //                                        Mdata.Add(data);
        //                                    }
        //                                }

        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        return Mdata;
        //    }


        //}

        //public static List<RSSModel> getRSSDetail(int WebLevelSN)
        //{
        //    using (var db = new MODAContext())
        //    {
        //        var weblebelM = db.WebLevel.FirstOrDefault(m => m.WebLevelSN == WebLevelSN);
        //        if (weblebelM != null)
        //        {
        //            switch (weblebelM.Module)
        //            {
        //                case "CP": return getRSSCP(WebLevelSN); 
        //                case "NEWS": return getRSSNews(WebLevelSN);
        //                default:
        //                    return null;
        //            }
        //        }
        //        return null;
        //    }
        //}
        ///// <summary>
        ///// cp資料
        ///// </summary>
        ///// <param name="WebLevelSN"></param>
        ///// <returns></returns>
        //static List<RSSModel> getRSSCP(int WebLevelSN)
        //{
        //    using (var db = new MODAContext())
        //    {
        //        var list = db.WebLevel.Where(m => m.WebLevelSN == WebLevelSN).ToList()
        //            .Select(x=> new RSSModel() {
        //              key = x.WebLevelSN.ToString(),
        //              title = x.Title,
        //              link = "/Page",
        //              description = Regex.Replace((x.ContentHeader ?? x.Title).Replace("\n", ""), "<[^>]*(>|$)", string.Empty),
        //              date = x.CreatedDate != null ? x.CreatedDate.Value.ToString("r") : null
        //            } ).ToList();
        //        return list;
        //    }
        //}
        ///// <summary>
        ///// News 資料
        ///// </summary>
        ///// <param name="WebLevelSN"></param>
        ///// <returns></returns>
        //static List<RSSModel> getRSSNews(int WebLevelSN)
        //{
        //    using (var db = new MODAContext())
        //    {
        //        var list = db.WEBNews.Where(m => 
        //        m.WebLevelSN == WebLevelSN
        //        && ( m.StartDate == null || m.StartDate <= DateTime.Now  )
        //        && (m.EndDate == null || m.EndDate >= DateTime.Now)
        //        && m.IsEnable == "1"
        //        ).OrderByDescending(x=>x.StartDate).Take(5).ToList()
        //        .Select(x => new RSSModel()
        //        {
        //            key = x.WebLevelSN.ToString(),
        //            key2 = x.WEBNewsSN.ToString(),
        //            title = x.Title,
        //            description = x.ContentText ?? x.Title,
        //            link = "/Page",
        //            date = x.PublishDate != null ? x.PublishDate.Value.ToString("r") : null
        //        }).ToList();
        //        return list;
        //    }

        //}

    }
}

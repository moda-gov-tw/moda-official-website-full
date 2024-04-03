using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Authorization;
using Services.Models.WebContent;
using Services.Models.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace Services.WebSite
{
    /// <summary>
    /// 指向WebNews.ArticleType
    /// </summary>
    public class NewsService
    {

        //ListData
        /// <summary>
        /// 找尋父層
        /// </summary>
        /// <param name="key">WebLevelSN</param>
        /// <returns></returns>
        public static WebLevel GetParentWebLevel(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var localData = db.WebLevel.First(x => x.WebLevelSN == key);
                    var ParentData = db.WebLevel.First(x => x.WebLevelSN == localData.ParentSN);
                    if (ParentData.ParentSN == 0)
                    {
                        return localData;
                    }
                    ParentData.Title = db.WebLevel.First(x => x.WebLevelSN == ParentData.WebLevelSN).Title;
                    return ParentData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 找尋兄弟層
        /// </summary>
        /// <param name="ParentData">父層資料</param>
        /// <param name="key">本身WebLevelSN</param>
        /// <returns></returns>
        public static List<PAGELISTModel> GetBrotherWebLevelM(WebLevel ParentData, int key)
        {
            var newList = new List<PAGELISTModel>();
            try
            {
                var list = new List<WebLevel>();
                if (ParentData.WebLevelSN == key)
                {
                    newList.Add(new PAGELISTModel()
                    {
                        Module = ParentData.Module,
                        WebLevelSN = ParentData.WebLevelSN,
                        SortOrder = ParentData.SortOrder.Value,
                        Title = ParentData.Title,
                        ParentSN = ParentData.ParentSN,
                        WeblevelType = ParentData.WeblevelType,
                    });
                }
                else
                {
                    using (var db = new MODAContext())
                    {
                        var data = db.WebLevel.Where(x =>
                        x.ParentSN == ParentData.WebLevelSN &&
                        x.IsEnable == "1"
                        ).ToList();
                        var sourseSN = data.Select(x => x.WebLevelSN).ToList();
                        foreach (var soure in data)
                        {
                            var newsData = db.WEBNews.FirstOrDefault(x => x.Module == "CP" && x.WebLevelSN == soure.WebLevelSN);
                            if (newsData == null)
                            {
                                newList.Add(new PAGELISTModel()
                                {
                                    Module = soure.Module,
                                    WebLevelSN = soure.WebLevelSN,
                                    SortOrder = soure.SortOrder.Value,
                                    Title = soure.Title,
                                    ParentSN = soure.ParentSN,
                                    WeblevelType = soure.WeblevelType,

                                });
                            }
                            else
                            {
                                if (newsData.IsEnable == "1")
                                {
                                    var chk = true;
                                    if (newsData.StartDate.HasValue)
                                    {
                                        if (newsData.StartDate > DateTime.UtcNow.AddHours(8))
                                        {
                                            chk = false;
                                        }
                                    }
                                    if (newsData.EndDate.HasValue)
                                    {
                                        if (newsData.EndDate < DateTime.UtcNow.AddHours(8))
                                        {
                                            chk = false;
                                        }
                                    }

                                    if (chk)
                                    {
                                        var WEBFileID = "";
                                        if (newsData.ArticleType == "1")
                                        {
                                            var file = (from a in db.RelWebFileContent.Where(x => x.SourceTable == "WEBNews" && x.GroupID == Utility.WebFileGroupID.CP.File && x.SourceSN == newsData.WEBNewsSN)
                                                        join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                                        select b).FirstOrDefault();
                                            WEBFileID = file?.WEBFileID;
                                        }
                                        newList.Add(new PAGELISTModel()
                                        {
                                            Module = soure.Module,
                                            WebLevelSN = soure.WebLevelSN,
                                            SortOrder = soure.SortOrder.Value,
                                            Title = soure.Title,
                                            ParentSN = soure.ParentSN,
                                            WeblevelType = soure.WeblevelType,
                                            ArticleType = newsData.ArticleType,
                                            WEBNewsSN = newsData.WEBNewsSN,
                                            Url = newsData.URL,
                                            WEBFileID = WEBFileID,
                                            target = newsData.target
                                        });
                                    }
                                }
                            }
                        }
                        return newList.OrderBy(X => X.SortOrder).ToList();
                    }
                }
            }
            catch (Exception)
            {
            }
            return newList;
        }
        /// <summary>
        /// 找尋孩子
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<newsChildModel> GetChildNews(int key)
        {
            var data = new List<newsChildModel>();
            using (var db = new MODAContext())
            {
                try
                {
                    var list = db.WEBNews.Where(x => x.WEBNewsSN == key && x.IsEnable == "1").OrderByDescending(x => x.StartDate);
                    foreach (var item in list)
                    {
                        var file = (from f in db.WEBFile
                                    join r in db.RelWebFileContent.Where(r => r.SourceSN == item.WEBNewsSN && r.SourceTable == "WEBNews") on f.WEBFileSN equals r.WEBFileSN
                                    where r.GroupID == Utility.WebFileGroupID.News.InlineImgs
                                    select f).FirstOrDefault();
                        data.Add(new newsChildModel()
                        {
                            localData = item,
                            file = file
                        });
                    }
                }
                catch (Exception)
                {
                }
            }
            return data;
        }

        public static List<WebLink> GetWebLink(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLink.Where(x => x.SourceTable == "WEBNews" && x.SourceSN == key && x.IsEnable == "1").OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }
        //取得部門資料
        public static SysDepartment GetDepName(string DepartmentID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysDepartment.Where(x => x.DepartmentID == DepartmentID).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }

            }
        }
        public static List<newsChildModel> GetChildNews(int key, string str, string end, string keyword, string depid, ref DefaultPager pager)
        {
            try
            {
                var data = new List<newsChildModel>();
                using (var db = new MODAContext())
                {
                    var WebLevel = db.WebLevel.FirstOrDefault(x => x.WebLevelSN == key);


                    var list = db.WEBNews.Where(user => true);
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        list = list.Where(x => x.Title.Contains(keyword) || x.SubTitle.Contains(keyword));
                    }

                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        var _str = DateTime.Parse(str + " 00:00:00");
                        list = list.Where(x => x.StartDate >= _str || x.StartDate == null);
                    }
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        var _end = DateTime.Parse(end + " 23:59:59");
                        list = list.Where(x => x.StartDate <= _end || x.StartDate == null);
                    }
                    if (!string.IsNullOrWhiteSpace(depid))
                    {
                        list = list.Where(x => x.DepartmentID == depid);
                    }
                    if (WebLevel.Module == "NEWS")
                    {
                        list = list.Where(x => x.Module == "NEWS");
                    }

                    list = list.Where(x => x.WebLevelSN == key && x.IsEnable == "1" && (x.StartDate <= DateTime.UtcNow.AddHours(8) || x.StartDate == null) && (x.EndDate >= DateTime.UtcNow.AddHours(8) || x.EndDate == null));

                    var allData = list.Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    var searchData = list.OrderByDescending(o => o.StartDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    foreach (var item in searchData)
                    {
                        var file = (from f in db.WEBFile
                                    join r in db.RelWebFileContent on f.WEBFileSN equals r.WEBFileSN
                                    where r.GroupID == Utility.WebFileGroupID.News.File
                                    && r.SourceSN == item.WEBNewsSN
                                    && r.SourceTable == "WEBNews"
                                    select f).FirstOrDefault();
                        var coverfile = (from f in db.WEBFile
                                         join r in db.RelWebFileContent on f.WEBFileSN equals r.WEBFileSN
                                         where r.GroupID == Utility.WebFileGroupID.News.Imgs
                                         && r.SourceSN == item.WEBNewsSN
                                         && r.SourceTable == "WEBNews"
                                         select f).FirstOrDefault();

                        data.Add(new newsChildModel()
                        {
                            localData = item,
                            file = file,
                            coverfile = coverfile
                        });

                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                var error = ex;
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
                    SourceTable = "WEBNews",
                    Action = "GetChildNews",
                    Controller = "NewsService",
                    SourceSN = key,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return null;
        }

        public static WebLevel GetWebLevelMbyWebLevelSN(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel.FirstOrDefault(x => x.WebLevelSN == key);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// USE CP
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static WEBNews GetWebNewsByWebLevelSN(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBNews.FirstOrDefault(x => x.WebLevelSN == key && x.IsEnable == "1");

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 抓取單筆資料 Services.Models.WebSite.WebNewsDetailModel webNewsDetail =  Services.WebSite.NewsService.getNew(146);
        /// </summary>
        /// <param name="WebNewsSN"></param>
        /// <param name="IsEnable"></param>
        /// <param name="chkSTET"></param>
        /// <returns></returns>
        public static WebNewsDetailModel getNew(int WebNewsSN = 0, string IsEnable = "1", bool chkSTET = true)
        {
            WebNewsDetailModel webNewsDetail = new WebNewsDetailModel();
            webNewsDetail.BasicData = getDataByWebNews(WebNewsSN, IsEnable, chkSTET);

            if (webNewsDetail.BasicData != null)
            {
                Utility.SYSConst.Content.Type enumArticleTypeCode = (Utility.SYSConst.Content.Type)Enum.Parse(typeof(Utility.SYSConst.Content.Type),
                    webNewsDetail.BasicData.ArticleType);

                Utility.SysConst.Module enumModule = (Utility.SysConst.Module)Enum.Parse(typeof(Utility.SysConst.Module),
                   webNewsDetail.BasicData.Module);

                var webFiles = getFileComm(WebNewsSN, SysConst.SourceTable.WEBNEWS);

                switch (enumArticleTypeCode)
                {
                    case Utility.SYSConst.Content.Type.PAGE:
                        string filesGrpID = (enumModule == SysConst.Module.NEWS) ? Utility.WebFileGroupID.News.Files : Utility.WebFileGroupID.CP.Files;
                        string imgsGrpID = (enumModule == SysConst.Module.NEWS) ? Utility.WebFileGroupID.News.Imgs : Utility.WebFileGroupID.CP.Imgs;
                        //取相關檔案
                        webNewsDetail.attachment.page.Files = GetRelMutileFiles(webFiles, filesGrpID);
                        //取相關圖檔
                        webNewsDetail.attachment.page.Imgs = GetRelMutileFiles(webFiles, imgsGrpID);
                        break;
                    case Utility.SYSConst.Content.Type.DOWNLOAD:
                        string downloadFileGrpID = (enumModule == SysConst.Module.NEWS) ? Utility.WebFileGroupID.News.File : Utility.WebFileGroupID.CP.File;
                        //下載檔案
                        webNewsDetail.attachment.download.File = GetRelSingleFile(webFiles, downloadFileGrpID);
                        break;
                }
            }
            return webNewsDetail;

        }

        /// <summary>
        /// WEBNEWS資料庫搜尋有效資料
        /// </summary>
        /// <param name="WebSiteID"></param>
        /// <param name="enumModule"></param>
        /// <param name="MainSN"></param>
        /// <returns></returns>
        public static List<WEBNews> getNewsComm(string WebSiteID = "MODA", string Lang = "", SysConst.Module enumModule = SysConst.Module.TEXT, int MainSN = 0, int topN = 0)
        {
            if (topN == 0) topN = 1000;

            string sql = $@"Select n.* from WEBNEWS n
            INNER JOIN WebLevel l on n.WebLevelSN = l.MainSN 
            where n.IsEnable = '1'																			
            and ( (n.StartDate is null) or (n.StartDate is not null and n.StartDate < getDate()))					
            and ( (n.EndDate is null) or (n.EndDate is not null and n.EndDate > getDate()))						
            and n.WebSiteID = @WebSiteID 
            and n.Lang = @Lang
            and l.WebSiteID = @WebSiteID 
            and l.Lang = @Lang
            and n.Module = @Module 
            and l.MainSN = @MainSN";

            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@WebSiteID", WebSiteID));
            sqlParams.Add(new SqlParameter("@Lang", Lang));
            sqlParams.Add(new SqlParameter("@Module", enumModule.ToString()));
            sqlParams.Add(new SqlParameter("@MainSN", MainSN));

            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.WEBNews.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    var serchData = data.OrderByDescending(o => o.StartDate)

                            .ThenByDescending(o => o.CreatedDate).Take(topN).ToList();
                    return serchData;
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
                        SourceTable = "WEBNEWS",
                        Action = "getNewsComm",
                        Controller = "HomeService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                    return null;
                }


            }

        }
        /// <summary>
        /// 取WebNews 資料
        /// </summary>
        /// <param name="WebNewsSN">WebNews流水號</param>
        /// <param name="IsEnable"></param>
        /// <param name="chkSTET">是否要判斷上下架日期</param>
        /// <returns></returns>
        public static WEBNews getDataByWebNews(int WebNewsSN = 0, string IsEnable = "", bool chkSTET = true)
        {

            string where = "";

            if (!string.IsNullOrWhiteSpace(IsEnable)) //是否發布
            {
                where += $@" and IsEnable = '{IsEnable}'";
            }
            if (chkSTET)                            //是否判斷上下架日期
            {
                where += $@" and ( (StartDate is null) or (StartDate is not null and StartDate < getDate()))";
                where += $@" and ( (EndDate is null) or (EndDate is not null and EndDate > getDate()))";
            }
            string sql = $@"Select * from WEBNEWS
            where WEBNewsSN = {WebNewsSN.ToString()} {where}";
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.WEBNews.FromSqlRaw(sql);
                    var serchData = data.OrderByDescending(o => o.SortOrder)
                            .ThenByDescending(o => o.StartDate)
                            .ThenByDescending(o => o.CreatedDate).ToList();
                    return serchData.FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 抓取WEBNEWS對應之所有附件檔案
        /// </summary>
        /// <param name="WebNewsMSN"></param>
        /// <param name="sourceTable"></param>
        /// <returns></returns>
        public static List<Services.Models.WebFileAndGroupIDModel> getFileComm(int WebNewsMSN = 0, SysConst.SourceTable sourceTable = SysConst.SourceTable.WEBNEWS)
        {

            try
            {
                String sql = $@"Select B.*
                                ,A.GroupID ,A.SortOrder from (
                                Select * from [dbo].[RelWebFileContent] 
                                where SourceTable = @SourceTable and SourceSN = @SourceSN
                                ) A
                                Join WEBFile B On A.WEBFileSN = B.WEBFileSN
                                where B.IsEnable = '1'";

                List<SqlParameter> sqlParams = new List<SqlParameter>();
                sqlParams.Add(new SqlParameter("@SourceTable", sourceTable.ToString()));
                sqlParams.Add(new SqlParameter("@SourceSN", WebNewsMSN));

                using (var db = new MODAContext())
                {
                    var data = db.Set<Models.WebFileAndGroupIDModel>().FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    return data;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        ///  解析getFileComm抓取哪一類附件
        /// </summary>
        /// <param name="webFiles"></param>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public static List<Services.Models.WebFileAndGroupIDModel> GetRelMutileFiles(List<Services.Models.WebFileAndGroupIDModel> webFiles, string GroupID)
        {
            //取GroupID = Utility.WebFileGroupID.Link.Img的清單列
            var webFilesData = webFiles.Where(x => x.GroupID == GroupID).ToList();
            return webFilesData;
        }

        /// <summary>
        /// 解析getFileComm抓哪一類附件，取第一筆
        /// </summary>
        /// <param name="webFiles"></param>
        /// <param name="GroupID"></param>
        /// <returns>最新的附件</returns>
        public static Services.Models.WebFileAndGroupIDModel GetRelSingleFile(List<Services.Models.WebFileAndGroupIDModel> webFiles, string GroupID)
        {
            var webFilesData = GetRelMutileFiles(webFiles, GroupID);
            //取第一筆
            var wEBFilesOB = webFilesData.OrderByDescending(o => o.CreatedDate).FirstOrDefault();
            return wEBFilesOB;
        }

        public static NewsDetailModel GetWebSiteNewDetailData(WebLevel webLevel, WEBNews wEBNews = null)
        {
            var fileGroupId = string.Empty;
            var NewData = new NewsDetailModel();
            using (var db = new MODAContext())
            {
                try
                {
                    if (webLevel != null)
                    {
                        fileGroupId = "CP";
                        //CP
                        wEBNews = db.WEBNews.FirstOrDefault(x => x.WebLevelSN == webLevel.MainSN && x.Lang == webLevel.Lang);
                        NewData.BasicData = wEBNews;
                    }
                    else
                    {
                        fileGroupId = "NW";
                        wEBNews = db.WEBNews.FirstOrDefault(x => x.MainSN == wEBNews.MainSN && x.Lang == wEBNews.Lang);
                        NewData.BasicData = wEBNews;
                    }
                    var SysCategoryData = db.SysCategory.Where(x => x.WebSiteID == wEBNews.WebSiteID && x.Lang == wEBNews.Lang).ToList();
                    var WEBNewsExtend = db.WEBNewsExtend.Where(x => x.WEBNewsSN == wEBNews.WEBNewsSN).ToList();
                    var imgs = (from a in db.RelWebFileContent
                                join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                where a.SourceTable == "WEBNews"
                                && a.SourceSN == wEBNews.WEBNewsSN
                                && a.GroupID == fileGroupId + "MI"
                                && b.IsEnable == "1"
                                orderby a.SortOrder
                                select b).ToList();
                    var files = (from a in db.RelWebFileContent
                                 join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                 where a.SourceTable == "WEBNews"
                                 && a.SourceSN == wEBNews.WEBNewsSN
                                 && a.GroupID == fileGroupId + "MF"
                                 && b.IsEnable == "1"
                                 orderby a.SortOrder
                                 select b).ToList();
                    var depModel = new SysDepartment() { WebSiteId = wEBNews.WebSiteID, Lang = wEBNews.Lang, DepartmentID = wEBNews.DepartmentID };
                    var DepartmentName = "";
                    GetDepName(depModel, ref DepartmentName);
                    if (string.IsNullOrWhiteSpace(DepartmentName)) GetBigDepName(depModel, ref DepartmentName);
                    NewData.DepartmentName = DepartmentName;
                    NewData.sysCategories = SysCategoryData;
                    NewData.newsExtends = WEBNewsExtend;
                    NewData.Imgs = imgs;
                    NewData.Files = files;
                }
                catch (Exception)
                {

                }
            }
            return NewData;
        }
        /// <summary>
        /// 抓取單位架構
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="DepartmentName"></param>
        /// <param name="webSiteSysDepartmentSN"></param>
        static void GetDepName(SysDepartment dep, ref string DepartmentName, int webSiteSysDepartmentSN = 0)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    if (webSiteSysDepartmentSN == 0)
                    {
                        webSiteSysDepartmentSN = db.SysDepartment.Where(x => x.Lang == "zh-tw" && x.WebSiteId == dep.WebSiteId).Min(x => x.SysDepartmentSN);
                    }
                    var depData = db.SysDepartment.FirstOrDefault(x => x.Lang == dep.Lang && x.DepartmentID == dep.DepartmentID && x.WebSiteId == dep.WebSiteId);
                    DepartmentName = depData.DepartmentName;
                    if (depData != null)
                    {
                        if (depData.ParentID == 0)
                        {
                            return;
                        }
                        else if (depData.ParentID != webSiteSysDepartmentSN)
                        {
                            var ParentData = db.SysDepartment.FirstOrDefault(x => x.SysDepartmentSN == depData.ParentID);
                            ParentData.Lang = dep.Lang;
                            GetDepName(ParentData, ref DepartmentName, webSiteSysDepartmentSN);
                        }
                    }
                }
                catch (Exception)
                {


                }
            }
        }
        /// <summary>
        /// 抓如有異動的時候抓取最大單位名稱
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="DepartmentName"></param>
        static void GetBigDepName(SysDepartment dep, ref string DepartmentName)
        {
            using (var db = new MODAContext())
            {
                DepartmentName= db.SysDepartment.FirstOrDefault(x => x.Lang == dep.Lang && x.WebSiteId == dep.WebSiteId && x.ParentID ==0 ).DepartmentName;
            }
        }
        public static List<WEBNewsTranscript> GetWEBNewsTranscript(int NewsSN)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    return db.WEBNewsTranscript.Where(x => x.WEBNewsSN == NewsSN).ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 取得NEWS資料
        /// </summary>
        /// <param name="NewsSN"></param>
        /// <param name="ft">0-需要判斷時間跟狀態   1-強制</param>
        /// <returns></returns>
        public static NewsDetailModel GetNews(int NewsSN  , string ft = "0" ) 
        {
            var NewsDetailModel = new NewsDetailModel();
            try
            {
                using (var db = new MODAContext())
                {
                    NewsDetailModel.BasicData = db.WEBNews
                        .FirstOrDefault(x => x.WEBNewsSN == NewsSN &&
                       (ft == "0" ?(x.StartDate == null || x.StartDate <= DateTime.UtcNow.AddHours(8))&& (x.EndDate == null || x.EndDate >= DateTime.UtcNow.AddHours(8)) :1==1)
                       );

                    NewsDetailModel.Files = (from n in db.WEBNews.Where(x => x.WEBNewsSN == NewsSN)
                                             join r in db.RelWebFileContent.Where(x => x.SourceTable.ToUpper() == "WEBNEWS") on n.WEBNewsSN equals r.SourceSN
                                             join f in db.WEBFile on r.WEBFileSN equals f.WEBFileSN
                                             where 
                                             (ft == "0" ? (n.IsEnable == "1" && f.IsEnable == "1") : 1==1)
                                             && (ft == "0" ? (n.StartDate == null || n.StartDate <= DateTime.UtcNow.AddHours(8)) : 1 == 1)
                                             && (ft == "0" ? (n.EndDate == null || n.EndDate >= DateTime.UtcNow.AddHours(8)) : 1 == 1)
                                             orderby r.SortOrder
                                             select f).ToList();

                    return NewsDetailModel;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

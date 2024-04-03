using DBModel;
using Services.Authorization;
using Services.Models.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.WebSite
{
    public class PAGELISTService
    {
        /// <summary>
        /// 找尋父層
        /// </summary>
        /// <param name="key">WebLevelMSN</param>
        /// <returns></returns>
        public static WebLevel GetParentWebLevelM(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var localData = db.WebLevel.First(x => x.WebLevelSN == key);
                    var ParentData = db.WebLevel.First(x => x.WebLevelSN == localData.ParentSN);
                    //假如爺爺是0 表示他是最上一層了
                    if (ParentData.ParentSN == 0)
                    {
                        localData.Title = localData.Title;
                        return localData;
                    }
                    ParentData.Title = ParentData.Title;
                    return ParentData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 找尋兄弟
        /// </summary>
        /// <param name="ParentData">父層資料</param>
        /// <param name="key">本身WebLevelMSN</param>
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
                            var newsData = db.WEBNews.FirstOrDefault(x => x.Module == soure.Module && x.WebLevelSN == soure.WebLevelSN && x.IsEnable == "1");
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
                                var chk = true;
                                if (newsData.StartDate.HasValue)
                                {
                                    if (newsData.StartDate > DateTime.Now)
                                    {
                                        chk = false;
                                    }
                                }
                                if (newsData.EndDate.HasValue)
                                {
                                    if (newsData.EndDate < DateTime.Now)
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
        public static List<childModel> GetChildWebLevelM(int key)
        {
            var data = new List<childModel>();
            try
            {
                using (var db = new MODAContext())
                {
                    var list = db.WebLevel.Where(x =>
                     x.ParentSN == key &&
                    x.IsEnable == "1"
                    ).OrderBy(x => x.SortOrder).ToList();

                    var webLevelMsnList = list.Select(x => x.WebLevelSN).ToList();
                    var details = db.WebLevel.Where(x => webLevelMsnList.Contains(x.WebLevelSN)).ToList();

                    foreach (var item in list)
                    {
                        try
                        {
                            var detail = details.FirstOrDefault(x => x.WebLevelSN == item.WebLevelSN);
                            item.Title = detail == null ? item.Title : detail.Title;
                            item.SortOrder = detail == null ? item.SortOrder : detail.SortOrder;
                            var file = (from f in db.WEBFile
                                        join r in db.RelWebFileContent.Where(x => x.SourceSN == detail.WebLevelSN) on f.WEBFileSN equals r.WEBFileSN
                                        where r.GroupID == Utility.WebFileGroupID.Module.LogoImg
                                        select f).FirstOrDefault();
                            var newList = new PAGELISTModel();
                            var newsData = db.WEBNews.FirstOrDefault(x => x.Module == "CP" && x.WebLevelSN == item.WebLevelSN);
                            if (newsData == null)
                            {
                                newList.Module = item.Module;
                                newList.ParentSN = item.ParentSN;
                                newList.SortOrder = item.SortOrder.Value;
                                newList.Title = item.Title;
                                newList.WebLevelSN = item.WebLevelSN;
                                newList.WeblevelType = item.WeblevelType;
                                newList.StartDate = detail?.StartDate;
                                data.Add(new childModel()
                                {
                                    localData = newList,
                                    file = file
                                });
                            }
                            else
                            {
                                if (newsData.IsEnable == "1")
                                {
                                    var chk = true;
                                    if (newsData.StartDate.HasValue)
                                    {
                                        if (newsData.StartDate > DateTime.Now)
                                        {
                                            chk = false;
                                        }
                                    }
                                    if (newsData.EndDate.HasValue)
                                    {
                                        if (newsData.EndDate < DateTime.Now)
                                        {
                                            chk = false;
                                        }
                                    }
                                    if (chk)
                                    {
                                        var WEBFileID = "";
                                        if (newsData.ArticleType == "1")
                                        {
                                            var CPfile = (from a in db.RelWebFileContent.Where(x => x.SourceTable == "WEBNews" && x.GroupID == Utility.WebFileGroupID.CP.File && x.SourceSN == newsData.WEBNewsSN)
                                                          join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                                          select b).FirstOrDefault();
                                            WEBFileID = file?.WEBFileID;
                                        }

                                        newList.Module = item.Module;
                                        newList.ParentSN = item.ParentSN;
                                        newList.SortOrder = item.SortOrder.Value;
                                        newList.Title = item.Title;
                                        newList.WebLevelSN = item.WebLevelSN;
                                        newList.WeblevelType = item.WeblevelType;
                                        newList.ArticleType = newsData.ArticleType;
                                        newList.WEBNewsSN = newsData.WEBNewsSN;
                                        newList.Url = newsData.URL;
                                        newList.StartDate = detail?.StartDate;
                                        newList.WEBFileID = WEBFileID;
                                        newList.target = newsData.target;
                                        data.Add(new childModel()
                                        {
                                            localData = newList,
                                            file = file
                                        });
                                    }
                                }

                            }


                        }
                        catch (Exception ex)
                        {
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
                                SourceTable = "WebLevelM",
                                Action = "GetChildWebLevelM",
                                Controller = "PAGELISTService",
                                SourceSN = 0,
                                CreatedDate = DateTime.Now
                            });
                        }
                    }
                    return data;

                }

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

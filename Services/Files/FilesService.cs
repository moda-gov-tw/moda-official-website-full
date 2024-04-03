using DBModel;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Services.Files
{
    public class FilesService
    {
        public static FilesActionModel Create(WEBFile wEBFile)
        {
            FilesActionModel result = new FilesActionModel();
            try
            {
                using (var db = new MODAContext())
                {
                    db.WEBFile.Add(wEBFile);
                    db.SaveChanges();

                    result.webfile = wEBFile;
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                result.ActionExption(ex.Message);
            }
            return result;
        }
        public static void CreateRelWebFileContent(RelWebFileContent relWebFileContent)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    db.RelWebFileContent.Add(relWebFileContent);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 取得檔案
        /// </summary>
        /// <param name="WEBFileID"></param>
        /// <param name="fileType">0-需要判斷狀態  1-強制</param>
        /// <returns></returns>
        public static FilesActionModel Get(string WEBFileID , string fileType = "0") 
        {
            FilesActionModel result = new FilesActionModel();
            try
            {
                var webfile = new WEBFile();
                using (var db = new MODAContext())
                {
                    webfile = db.WEBFile.FirstOrDefault(x => 
                    x.WEBFileID == WEBFileID &&
                    (fileType =="0" ?  x.IsEnable == "1" : x.IsEnable != "-99"));

                    if (webfile != null)
                    {
                        var relWebFileContent = db.RelWebFileContent.FirstOrDefault(x => x.WEBFileSN == webfile.WEBFileSN);
                        if (relWebFileContent == null)
                        {
                            result.ActionMessage = $" RelWebFileContent not find { webfile.WEBFileSN }";
                            result.ActionExption("查無資料。");
                        } 
                        else if (relWebFileContent.SourceTable.ToLower() == "webopendatamain" || relWebFileContent.SourceTable.ToLower() == "caseapply") 
                        {
                            result.webfile = webfile;
                            relWebFileContent.PageView = relWebFileContent.PageView == null ? 1 : relWebFileContent.PageView.Value + 1;
                            db.RelWebFileContent.Update(relWebFileContent);
                            if (fileType == "0") {
                                var download = new WebFileDownLoads()
                                {
                                    RelWebFileContentSN = relWebFileContent.RelWebFileContentSN,
                                    WEBFileSN = relWebFileContent.WEBFileSN,
                                    DownLoads = 1,
                                    CreatedDate = DateTime.UtcNow.AddHours(8)
                                };
                                db.WebFileDownLoads.Add(download);

                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var levelsn = 0;
                            if (relWebFileContent.SourceTable.ToLower() == "weblevel")
                            {
                                levelsn = relWebFileContent.SourceSN;
                            }
                            else if (relWebFileContent.SourceTable.ToLower() == "webnews")
                            {
                                var news = WebSite.NewsService.GetNews(relWebFileContent.SourceSN, fileType);

                                if (news?.BasicData != null && (fileType =="0"?  news.BasicData.IsEnable == "1" :1==1 ) )
                                {
                                    if (matchGroupArticle(news.BasicData.ArticleType, relWebFileContent.GroupID))
                                    {
                                        levelsn = news.BasicData.WebLevelSN;
                                    }
                                    else 
                                    {
                                        result.ActionMessage = $" matchGroupArticle error newssn {news.BasicData.WEBNewsSN}";
                                        result.ActionExption("查無資料。");
                                        return result;
                                    }                              
                                }
                                else
                                {
                                    levelsn = 0;
                                }
                            }
                            if (fileType == "0"? CommonService.CheckLevelByTree(levelsn) : 1==1)
                            {
                                relWebFileContent.PageView = relWebFileContent.PageView == null ? 1 : relWebFileContent.PageView.Value + 1;
                                db.RelWebFileContent.Update(relWebFileContent);
                                if (fileType == "0")
                                {                                 

                                    var download = new WebFileDownLoads()
                                    {
                                        RelWebFileContentSN = relWebFileContent.RelWebFileContentSN,
                                        WEBFileSN = relWebFileContent.WEBFileSN,
                                        DownLoads = 1,
                                        CreatedDate = DateTime.UtcNow.AddHours(8)
                                    };
                                    db.WebFileDownLoads.Add(download);

                                    db.SaveChanges();
                                }
                                result.webfile = webfile;
                            }
                            else
                            {
                                result.ActionMessage = $" CheckLevelByTree error levelsn { levelsn }";
                                result.ActionExption("查無資料。");
                            }
                        }
                    }
                    else 
                    {
                        result.ActionMessage = $" WEBFile not find { WEBFileID }";
                        result.ActionExption("查無資料。");
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                result.ActionMessage = $" Exception ex : { ex.Message }";
                result.ActionExption(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 對應現有新聞模組(Article)與檔案類型(GroupID)
        /// WebNews含CP頁
        /// </summary>
        /// <param name="article"></param>
        /// <param name="groupID"></param>
        /// <returns></returns>
        static bool matchGroupArticle(string article, string groupID) 
        {
            switch (article)
            {
                case "0":
                    return groupID == Utility.WebFileGroupID.News.InlineImgs || groupID == Utility.WebFileGroupID.News.Files || groupID == Utility.WebFileGroupID.News.Imgs
                        || groupID == Utility.WebFileGroupID.CP.InlineImgs || groupID == Utility.WebFileGroupID.CP.Files || groupID == Utility.WebFileGroupID.CP.Imgs;
                case "1":
                    return groupID == Utility.WebFileGroupID.News.File || groupID == Utility.WebFileGroupID.CP.File;
                case "10":
                    return groupID == Utility.WebFileGroupID.Transcript.MD;
                default: 
                    return false;
            }
        }

        #region 排程使用
        /// <summary>
        /// 抓取檔案
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="fileType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// 
        public static List<WEBFile> GetFiles(int minutes, string fileType, string type, string isFirstTimeRead)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    switch (type)
                    {
                        case "reader":
                            var listNewsA = (from a in db.WEBNews
                                             join b in db.RelWebFileContent.Where(b => b.SourceTable == "WEBNEWS") on a.WEBNewsSN equals b.SourceSN
                                             join c in db.WEBFile on b.WEBFileSN equals c.WEBFileSN
                                             where a.IsEnable == "1"
                                             && c.FileType == fileType
                                             && (isFirstTimeRead == "1" ? 1 == 1 : c.ProcessDate >= DateTime.UtcNow.AddHours(8).AddMinutes(-minutes))
                                             select c).ToList();
                            return listNewsA;
                        case "delete":
                            return db.WEBFile.Where(x => x.IsEnable != "1").ToList();
                        case "deleteFile":
                            var fileList = new List<WEBFile>();
                            var FilePathlist = db.WEBFile.Where(x => x.IsEnable == "1").Select(x => x.FilePath).ToList();
                            var list1 = db.WEBFile.Where(x => x.IsEnable != "1" && !FilePathlist.Contains(x.FilePath)).ToList();
                            var listNews = (from a in db.WEBNews.Where(x => x.IsEnable == "-99")
                                            join b in db.RelWebFileContent.Where(x => x.SourceTable == "WEBNews") on a.WEBNewsSN equals b.SourceSN
                                            join c in db.WEBFile on b.WEBFileSN equals c.WEBFileSN
                                            select c).ToList();
                            var listLevelD = (from a in db.WebLevel.Where(x => x.IsEnable == "-99")
                                              join b in db.RelWebFileContent.Where(x => x.SourceTable == "WebLevel") on a.WebLevelSN equals b.SourceSN
                                              join c in db.WEBFile on b.WEBFileSN equals c.WEBFileSN
                                              select c).ToList();
                            if (list1 != null) fileList.AddRange(list1);
                            if (listLevelD != null) fileList.AddRange(listLevelD);
                            return fileList;
                    }
                }
                catch (Exception )
                {
                    return null;
                }
            }
            return null;
        }
        public static void DeleteWebFileExtend(List<int> webFileSNs)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var list = db.WebFileExtend.Where(x => webFileSNs.Contains(x.WebFileExtendSN));
                    if (list != null)
                    {
                        db.WebFileExtend.RemoveRange(list);
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {

                }
            }
        }
        public static void SaveWebFileExtend(WebFileExtend webFileExtend)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.WebFileExtend.FirstOrDefault(x => x.WebFileExtendSN == webFileExtend.WebFileExtendSN);
                    if (data != null)
                    {
                        db.WebFileExtend.Remove(data);
                    }
                    db.WebFileExtend.Add(webFileExtend);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                    
                }
            }
        }
        #endregion
    }
}

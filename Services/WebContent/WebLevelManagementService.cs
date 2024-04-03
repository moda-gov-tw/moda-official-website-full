using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Models;
using Services.Models.WebContent;
using Services.Models.WebContent.WebLevelManagement;
using Services.Models.WebSite;
using Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using static Utility.Files;
using System.Linq.Dynamic.Core;

namespace Services.Authorization
{

    public class WebLevelManagementService
    {
        //這邊需調整設定 抓取webSite
        public static string DemoDNS { get; set; } = "";

        /// <summary>
        /// 取 WebLevel 樹
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<WebLevel> GetWebLevelList(Utility.EnumWeblevelType key, string website)
        {
            string _key = ((int)key).ToString();

            using (var db = new MODAContext())
            {
                try
                {
                    var list = db.WebLevel.Where(x =>
                           x.WeblevelType == _key &&
                           x.WebSiteID == website &&
                           x.IsEnable != "-99"
                            ).Select(x => new WebLevel()
                            {
                                WebSiteID = x.WebSiteID,
                                WeblevelType = x.WeblevelType,
                                Lang = x.Lang,
                                Module = x.Module,
                                ParentSN = x.ParentSN,
                                SortOrder = x.SortOrder.Value,
                                Title = x.Title,
                                WebLevelSN = x.WebLevelSN,
                                MainSN = x.MainSN,
                                WebLevelKey = x.WebLevelKey,
                                IsEnable = x.IsEnable
                            }).ToList();
                    return list;

                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    return null;
                }
            }
        }
        public static List<WebLevel> GetWebLevelList(Utility.EnumWeblevelType key, string website, string lan)
        {
            try
            {
                string _key = ((int)key).ToString();

                using (var db = new MODAContext())
                {

                    var list = db.WebLevel.Where(x =>
                   x.WeblevelType == _key &&
                   x.WebSiteID == website &&
                   x.IsEnable != "-99" && x.Lang == lan
                    ).Select(x => new WebLevel()
                    {
                        WebSiteID = x.WebSiteID,
                        WeblevelType = x.WeblevelType,
                        Lang = x.Lang,
                        Module = x.Module,
                        ParentSN = x.ParentSN,
                        SortOrder = x.SortOrder.Value,
                        Title = x.Title,
                        WebLevelSN = x.WebLevelSN,
                        MainSN = x.MainSN,
                        WebLevelKey = x.WebLevelKey,
                        IsEnable = x.IsEnable
                    }).ToList();
                    return list;

                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 撈取網站有的語系
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authType"></param>
        /// <param name="weblevelMsn"></param>
        /// <returns></returns>
        public static List<SysWebSiteLang> GetSysWebSiteLangs(sysUserModel user, string authType, int weblevelsn)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysWebSiteLang.Where(x => x.WebSiteID == user.WebSiteID).ToList();

                }
                catch (Exception)
                {
                    return null;
                }
            }


        }
        /// <summary>
        /// 修改群組對應節點
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool EditAuthSysGroupWebLevel(bool chk, AuthSysGroupWebLevel data)
        {
            using (var db = new MODAContext())
            {
                var oldData = db.AuthSysGroupWebLevel.FirstOrDefault(w =>
                w.WebSiteID == data.WebSiteID &&
                w.WebLevelSN == data.WebLevelSN &&
                w.SysGroupSN == data.SysGroupSN &&
                w.AuthType == data.AuthType
            );
                if (chk)
                {
                    try
                    {
                        if (oldData == null)
                        {
                            var newData = new AuthSysGroupWebLevel()
                            {
                                WebSiteID = data.WebSiteID,
                                WebLevelSN = data.WebLevelSN,
                                AuthType = data.AuthType,
                                Lang = data.Lang,
                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                CreatedUserID = data.CreatedUserID,
                                SysGroupSN = data.SysGroupSN
                            };
                            db.AuthSysGroupWebLevel.Add(newData);
                            db.SaveChanges();

                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        if (oldData != null)
                        {
                            db.AuthSysGroupWebLevel.Remove(oldData);
                            db.SaveChanges();
                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

        }
        /// <summary>
        /// 取得Auth 權限表
        /// </summary>
        /// <param name="webLevelSN"></param>
        /// <returns></returns>
        public static List<AuthSysGroupWebLevel> GetNotOwnerLevelAccessForGroupList(int webLevelSN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.AuthSysGroupWebLevel.Where(X => X.SysGroupSN != 1 && X.WebLevelSN == webLevelSN).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 過濾系統管理群組
        /// </summary>
        /// <returns></returns>
        public static List<SysGroup> GetNotOwnerSysGroups()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysGroup.Where(X => X.SysGroupSN != 1).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 過濾系統管理群組
        /// </summary>
        /// <returns></returns>
        public static List<SysGroup> GetNotOwnerSysGroups(string q, ref DefaultPager pager)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.SysGroup.Where(x => x.SysGroupSN != 1
                    && (string.IsNullOrWhiteSpace(q) ? 1 == 1 : (x.GroupName.Contains(q) || x.Description.Contains(q)))
                    );
                    var searchData = data.OrderBy(x => x.SortOrder).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    pager.TotalCount = data.Count();
                    pager.PageIndex = pager.p - 1;
                    return searchData;

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// GetWebLevel
        /// </summary>
        /// <param name="webLevelSN"></param>
        /// <param name="lan"></param>
        /// <returns></returns>
        public static WebLevel GetWebLevel(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel.FirstOrDefault(X => X.WebLevelSN == key);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 判斷是否有重置API功能
        /// </summary>
        /// <param name="webLevelSNMain"></param>
        /// <returns></returns>
        public static bool CheckYouTouApiKey(int webLevelSNMain)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysCategory.FirstOrDefault(X =>
                   X.Value == webLevelSNMain.ToString() &&
                   X.SysCategoryKey.Contains("Management-4")
                    ) != null ? true : false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

        }

        public static WebLevel GetWebLevel(int mainSN, string lang)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel.FirstOrDefault(X => X.MainSN == mainSN && X.Lang == lang);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 特殊 利用WebSiteID & Lang & WebLevelKey 取資料 <<sitemap使用>>
        /// </summary>
        /// <param name="webLevel"></param>
        /// <returns></returns>
        public static WebLevel GetWebLevelByWebLevelData(WebLevel webLevel, bool enableNeeded = true)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevel.FirstOrDefault(X =>
                                X.WebSiteID == webLevel.WebSiteID
                                && X.WebLevelKey == webLevel.WebLevelKey
                                && X.Lang == webLevel.Lang
                                && (X.IsEnable == "1" || !enableNeeded)
                            );
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }
        public static List<WEBNewsListModel2> GetAllWebNewsListData(int webLevelMainSN, string lang, string str_date, string end_date, string key, string Condition4, string Condition5, string Condition6)
        {
            var list = new List<WEBNewsListModel2>();
            using (var db = new MODAContext())
            {
                try
                {
                    var LevelData = db.WebLevel.FirstOrDefault(x => x.MainSN == webLevelMainSN && x.Lang == lang);

                    var data = db.WEBNews.Where(x => x.WebLevelSN == webLevelMainSN
                    && x.Lang == lang
                    && x.IsEnable == "1"
                    && (x.StartDate == null || x.StartDate <= DateTime.UtcNow.AddHours(8))
                    && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.AddHours(8))
                    );
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

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        var fileExtend = (from n in data
                                          join a in db.RelWebFileContent on n.WEBNewsSN equals a.SourceSN
                                          join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                          join c in db.WebFileExtend on b.WEBFileSN equals c.WebFileExtendSN
                                          where a.SourceTable == "WEBNews"
                                          && b.IsEnable == "1"
                                          && c.FileContentText.Contains(key)
                                          select n).ToList();

                        var newstag = (from n in data
                                       join e in db.WEBNewsExtend on n.WEBNewsSN equals e.WEBNewsSN
                                       join c in db.SysCategory on e.SysCategoryKey equals c.SysCategoryKey
                                       where c.IsEnable == "1"
                                       && c.ParentKey == n.WebSiteID + "-2"
                                       && c.Value.Contains(key)
                                       select n).ToList();

                        var Transcript = (from n in data
                                          join e in db.WEBNewsTranscript on n.WEBNewsSN equals e.WEBNewsSN
                                          where e.TranscriptContent.Contains(key) || e.TranscriptForm.Contains(key)
                                          select n).ToList();

                        var keyword = (from n in data
                                       join e in db.WEBNewsExtend on n.WEBNewsSN equals e.WEBNewsSN
                                       where e.GroupID == "keyword"
                                       && e.Column_1.Contains(key)
                                       select n).ToList();

                        var fe = fileExtend.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var nt = newstag.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var ts = Transcript.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var kw = keyword.Select(x => x.WEBNewsSN).Distinct().ToList();

                        data = data.Where(x => x.ContentText.Contains(key)
                        || x.Title.Contains(key)
                        || fe.Contains(x.WEBNewsSN)
                        || nt.Contains(x.WEBNewsSN)
                        || ts.Contains(x.WEBNewsSN)
                        || kw.Contains(x.WEBNewsSN));
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
                    var allData = data.Count();
                    //排序
                    data = data.OrderBy(o => o.SortOrder);

                    var searchData = data.ToList();

                    foreach (var item in searchData)
                    {
                        var detail = new WEBNewsListModel();
                        detail.webNews = item;
                        var sysC = (from a in db.WEBNewsExtend
                                    join b in db.SysCategory on a.SysCategoryKey equals b.SysCategoryKey
                                    where a.WEBNewsSN == item.WEBNewsSN && a.GroupID == "tab" && b.Lang == item.Lang
                                    select b).ToList();
                        detail.sysCategories = sysC;
                        var file = (from a in db.RelWebFileContent
                                    join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                    where a.SourceSN == item.WEBNewsSN
                                    && a.SourceTable == "WEBNews"
                                    && a.GroupID == "NWSF"
                                    && b.IsEnable == "1"
                                    select b).FirstOrDefault();
                        detail.webFile = file;
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
                        var link = item.StatesUrl;
                        switch (item.ArticleType)
                        {
                            case "1":
                                link = file != null ? file.FilePath : "";
                                break;
                            case "2":
                                link = item.URL;
                                break;
                        }
                        list.Add(new WEBNewsListModel2()
                        {
                            mainsn = item.MainSN.ToString(),
                            href = link,
                            title = item.Title,
                            target = item.target,
                            filetypedisplay = file != null ? "inline" : "none",
                            filetype = file != null ? file.FileType.ToUpper().Split('.')[1] : "",
                            newstitle = item.Title,
                            newssubtitle = item.SubTitle,
                            istopdisplay = (item.IsTop == null || item.IsTop == "0") ? "none" : "inline",
                            startdate = item.StartDate.Value.ToString("yyyy-MM-dd"),
                            syscategoriesdisplay = sysC.Count() > 0 ? "block" : "none",
                            contenttext = LevelData.ListType == "AccordionList" ? item.ContentText : "",
                            weblogopath = detail.webLogo != null ? detail.webLogo.FilePath : "~/assets/img/icon_dept4-2-1.svg",
                            webvideokey = detail.webVideo != null ? detail.webVideo.Column_1 : "",
                            tags = sysC.Count() > 0 ? sysC.Select(x => new WEBNewsTagModel2() { tag = x.Value }).ToList() : new List<WEBNewsTagModel2>()
                        });
                    }
                }
                catch (Exception)
                {
                }
                return list;
            }

        }
        public static List<WEBNewsListModel> GetWebNewsListData(int webLevelMainSN, string lang, string str_date, string end_date, string key, string Condition4, string Condition5, string Condition6, string CustomizeTagSN, ref Utility.DefaultPager pager)
        {
            var list = new List<WEBNewsListModel>();
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

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        var fileExtend = (from n in data
                                          join a in db.RelWebFileContent on n.WEBNewsSN equals a.SourceSN
                                          join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                          join c in db.WebFileExtend on b.WEBFileSN equals c.WebFileExtendSN
                                          where a.SourceTable == "WEBNews"
                                          && b.IsEnable == "1"
                                          && c.FileContentText.Contains(key)
                                          select n).ToList();

                        var newstag = (from n in data
                                       join e in db.WEBNewsExtend on n.WEBNewsSN equals e.WEBNewsSN
                                       join c in db.SysCategory on e.SysCategoryKey equals c.SysCategoryKey
                                       where c.IsEnable == "1"
                                       && c.ParentKey == n.WebSiteID + "-2"
                                       && c.Value.Contains(key)
                                       select n).ToList();

                        var Transcript = (from n in data
                                          join e in db.WEBNewsTranscript on n.WEBNewsSN equals e.WEBNewsSN
                                          where e.TranscriptContent.Contains(key) || e.TranscriptForm.Contains(key)
                                          select n).ToList();

                        var keyword = (from n in data
                                       join e in db.WEBNewsExtend on n.WEBNewsSN equals e.WEBNewsSN
                                       where e.GroupID == "keyword"
                                       && e.Column_1.Contains(key)
                                       select n).ToList();

                        var fe = fileExtend.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var nt = newstag.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var ts = Transcript.Select(x => x.WEBNewsSN).Distinct().ToList();
                        var kw = keyword.Select(x => x.WEBNewsSN).Distinct().ToList();

                        data = data.Where(x => x.ContentText.Contains(key)
                        || x.Title.Contains(key)
                        || fe.Contains(x.WEBNewsSN)
                        || nt.Contains(x.WEBNewsSN)
                        || ts.Contains(x.WEBNewsSN)
                        || kw.Contains(x.WEBNewsSN));

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
                    if (!string.IsNullOrEmpty(CustomizeTagSN))
                    {
                        if (int.TryParse(CustomizeTagSN, out int _TagSN))
                        {
                            data = data.Where(r => r.CustomizeTagSn == _TagSN);
                        }
                    }
                    var allData = data.Count();
                    //排序
                    data = data.OrderBy(o => o.SortOrder);

                    var searchData = data.Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();

                    foreach (var item in searchData)
                    {
                        var detail = new WEBNewsListModel();
                        detail.webNews = item;
                        var sysC = (from a in db.WEBNewsExtend
                                    join b in db.SysCategory on a.SysCategoryKey equals b.SysCategoryKey
                                    where a.WEBNewsSN == item.WEBNewsSN && a.GroupID == "tab" && b.Lang == item.Lang
                                    select b).ToList();
                        detail.sysCategories = sysC;
                        var file = (from a in db.RelWebFileContent
                                    join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                    where a.SourceSN == item.WEBNewsSN
                                    && a.SourceTable == "WEBNews"
                                    && a.GroupID == "NWSF"
                                    && b.IsEnable == "1"
                                    select b).FirstOrDefault();
                        detail.webFile = file;
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
                        list.Add(detail);
                    }
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                }
                catch (Exception)
                {
                }
                return list;
            }
        }

        /// <summary>
        /// 以MAINSN取得所有語系資料
        /// </summary>
        /// <param name="MainSN"></param>
        /// <returns></returns>
        public static List<WebLevel> GetWebLevelByMainSN(int mainSN, string websiteid = "")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.WebLevel.Where(x => 1 == 1);
                    data = data.Where(x => x.MainSN == mainSN).Count() > 0 ? data.Where(x => x.MainSN == mainSN) : data.Where(x => x.WebLevelSN == mainSN);
                    if (!string.IsNullOrWhiteSpace(websiteid))
                    {
                        data = data.Where(x => x.WebSiteID == websiteid);
                    }
                    return data.ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<WebLevel> GetWebLevelByParentSN(string sortTitle, string sortType, int key, ref List<int> sortData, ref DefaultPager pager, string isEnable = "")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Data = db.WebLevel.Where(x =>
                            x.ParentSN == key &&
                            (string.IsNullOrWhiteSpace(isEnable) ? x.IsEnable != "-99" : x.IsEnable == isEnable)
                            );
                    var allData = Data.Where(x => x.Lang == "zh-tw").Count();
                    sortData = Data.Where(x => x.Lang == "zh-tw").Select(X => X.SortOrder.Value).OrderBy(x => x).ToList();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    //
                    var searchData = Data.Where(x => x.Lang == "zh-tw").OrderBy($" {sortTitle} {sortType}").Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    //
                    var SelectMain = searchData.Select(x => x.MainSN).ToList();
                    var otherLanData = Data.Where((x) => x.Lang != "zh-tw" && SelectMain.Contains(x.MainSN)).ToList();
                    searchData.AddRange(otherLanData);
                    return searchData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public static List<WebLevel> GetWebLevelByParentSN(int key, string isEnable = "")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var webLevelData = db.WebLevel.FirstOrDefault(X => X.MainSN == key);
                    var sortOrder = webLevelData?.SortMethod;
                    var searchData = db.WebLevel.Where(x =>
                        x.ParentSN == key &&
                         (string.IsNullOrWhiteSpace(isEnable) ? x.IsEnable != "-99" : x.IsEnable == isEnable)
                        ).Select(x => new WebLevel()
                        {
                            WebSiteID = x.WebSiteID,
                            WeblevelType = x.WeblevelType,
                            Lang = x.Lang,
                            Module = x.Module,
                            ParentSN = x.ParentSN,
                            SortOrder = x.SortOrder.Value,
                            Title = x.Title,
                            WebLevelSN = x.WebLevelSN,
                            IsEnable = x.IsEnable,
                            ProcessDate = x.ProcessDate,
                            MainSN = x.MainSN,
                            StartDate = x.StartDate,
                        }).ToList();
                    if (sortOrder == "0")
                    {
                        searchData = searchData.OrderBy(x => x.SortOrder).ToList();
                    }
                    else
                    {
                        searchData = searchData.OrderByDescending(x => x.StartDate).ToList();
                    }
                    return searchData;
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }
        /// <summary>
        /// 前台類型:列表 資料
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static List<WebSiteWebLevelPageListModel> GetWebSiteLevleListData(int key, string lang)
        {
            var childData = WebLevelManagementService.GetWebLevelByParentSN(key, "1")
                                                        .Where(x => x.Lang == lang)
                                                        .OrderBy(x => x.SortOrder)
                                                        .ToList();
            return GetWelLevelDetail(childData);
        }
        static List<WebSiteWebLevelPageListModel> GetWelLevelDetail(List<WebLevel> webLevels)
        {
            var isStatic = CommonService.IsStatic;
            var WebLevelPageListModel = new List<WebSiteWebLevelPageListModel>();
            using (var db = new MODAContext())
            {
                try
                {
                    foreach (var webLevel in webLevels.OrderBy(x => x.SortOrder))
                    {
                        var url = "";
                        var youtubeLink = "";
                        var target = "";
                        url = @$"/{webLevel.WebSiteID}/{webLevel.Lang}/Level/{webLevel.MainSN}";
                        var icon = (from a in db.RelWebFileContent
                                    join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                    where a.SourceTable == "WebLevel"
                                    && a.GroupID == Utility.WebFileGroupID.Module.LogoImg
                                    && b.IsEnable == "1"
                                    && a.SourceSN == webLevel.WebLevelSN
                                    select b).FirstOrDefault();
                        var file = new WEBFile();
                        switch (webLevel.Module)
                        {
                            case "DEPT":
                                url = @$"/{webLevel.WebSiteID}/{webLevel.Lang}/Dept/{webLevel.MainSN}";
                                break;
                            case "CP":
                                var newsData = db.WEBNews.FirstOrDefault(x => x.WebLevelSN == webLevel.MainSN && x.Lang == webLevel.Lang);
                                if (newsData != null)
                                {
                                    switch (newsData.ArticleType)
                                    {
                                        case "1":
                                            file = (
                                            from a in db.RelWebFileContent
                                            join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                            where a.SourceTable == "WEBNews"
                                            && a.GroupID == "CPSF"
                                            && b.IsEnable == "1"
                                            && a.SourceSN == newsData.WEBNewsSN
                                            select b).FirstOrDefault();
                                            url = file?.FilePath != null ? file.FilePath : url;
                                            target = "_blank";
                                            break;
                                        case "2":
                                            url = newsData.URL;
                                            target = newsData.target;
                                            break;
                                        case "3":
                                            url = @$"https://www.youtube.com/embed/{newsData.URL}";
                                            youtubeLink = @$"https://www.youtube.com/embed/{newsData.URL}";
                                            break;
                                    }
                                    icon = (
                                            from a in db.RelWebFileContent
                                            join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                            where a.SourceTable == "WEBNews"
                                            && a.GroupID == "LOGO"
                                            && b.IsEnable == "1"
                                            && a.SourceSN == newsData.WEBNewsSN
                                            select b).FirstOrDefault();
                                }
                                break;
                        }
                        WebLevelPageListModel.Add(new WebSiteWebLevelPageListModel()
                        {
                            webLevel = webLevel,
                            url = url,
                            webFile = file, //這邊需要調整
                            webLogo = icon,
                            youtubeLink = youtubeLink,
                            target = target
                        });
                    }
                }
                catch (Exception)
                {
                }
            }
            return WebLevelPageListModel;
        }
        /// <summary>
        /// 同一父層下面 WebLevelKey 不能重複
        /// </summary>
        /// <param name="webLevel"></param>
        /// <returns></returns>
        public static bool CheckWebLevelKey(WebLevel webLevel)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var ck = db.WebLevel.FirstOrDefault(x =>
                             x.WebSiteID == webLevel.WebSiteID &&
                             x.WebLevelKey == webLevel.WebLevelKey.Trim().ToLower() &&
                             x.IsEnable != "-99"
                            );
                    return ck == null ? true : false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 先新增節點
        /// </summary>
        /// <param name="webLevelM"></param>
        /// <param name="webLevelD"></param>
        /// <returns></returns>
        public static int InsertWebLevel(WebLevel webLevel, List<SysWebSiteLang> sysWebSiteLangs)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var MainKey = 0;
                    int? SortOrder = 0;
                    foreach (var lan in sysWebSiteLangs)
                    {
                        if (lan.Lang == "zh-tw")
                        {
                            var sortOrder = db.WebLevel
                           .Where(m => m.ParentSN == webLevel.ParentSN && m.IsEnable != "-99")
                           .OrderByDescending(x => x.SortOrder).FirstOrDefault()?.SortOrder;
                            SortOrder = sortOrder == null ? 1 : sortOrder + 1;
                        }
                        var data = new WebLevel()
                        {
                            ParentSN = webLevel.ParentSN,
                            WebSiteID = lan.WebSiteID,
                            Lang = lan.Lang,
                            WeblevelType = webLevel.WeblevelType,
                            Module = webLevel.Module,
                            Title = MainKey == 0 ? webLevel.Title.Trim() : "",
                            ListType = webLevel.ListType,
                            SortMethod = "0",
                            IsEnable = webLevel.IsEnable,
                            ProcessUserID = webLevel.ProcessUserID,
                            ProcessDate = webLevel.ProcessDate,
                            ProcessIPAddress = webLevel.ProcessIPAddress,
                            CreatedUserID = webLevel.CreatedUserID,
                            CreatedDate = webLevel.CreatedDate,
                            SortOrder = SortOrder,
                            WebLevelKey = webLevel.WebLevelKey.Trim().ToLower(),
                            Condition = lan.Lang == "zh-tw" ? webLevel.Condition : null,
                            MainSN = MainKey,
                            DepartmentID = webLevel.DepartmentID,
                        };


                        db.WebLevel.Add(data);
                        db.SaveChanges();
                        if (MainKey == 0)
                        {
                            data.MainSN = data.WebLevelSN;
                            MainKey = data.MainSN.Value;
                            db.WebLevel.Update(data);
                            db.SaveChanges();
                        }
                    }
                    return MainKey;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 修改狀態
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool EditModule(WebLevel data, string module, List<CommonFileModel> commonFileModels)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var source = db.WebLevel.FirstOrDefault(x => x.WebLevelSN == data.WebLevelSN);
                    if (source != null)
                    {
                        //SortMethod不同記一筆更動排序
                        if (source.SortMethod != data.SortMethod)
                        {
                            LogService.CreateWebLevelSortLog(source.WebLevelSN, data.ProcessUserID, source.Lang, source.SortMethod, data.SortMethod);
                        }

                        source.Title = data.Title;
                        source.ProcessUserID = data.ProcessUserID;
                        source.ProcessDate = DateTime.UtcNow.AddHours(8);
                        source.ProcessIPAddress = data.ProcessIPAddress;
                        source.Title = data.Title.Trim();
                        source.FatFooterShow = data.FatFooterShow;
                        source.MainMenuShow = data.MainMenuShow;
                        source.LeftMenuShow = data.LeftMenuShow;
                        source.StartDate = data.StartDate;
                        source.EndDate = data.EndDate;
                        source.Parameter = data.Parameter;
                        source.SortMethod = data.SortMethod;
                        source.RSSShow = data.RSSShow;
                        source.Parameter = data.Parameter;
                        source.Description = data.Description;
                        source.TemplateValue = data.TemplateValue != null ? data.TemplateValue.Trim() : null;
                        source.ContentHeader = data.ContentHeader;
                        source.ContentFooter = data.ContentFooter;
                        source.IsEnable = data.IsEnable;
                        source.ProcessUserID = data.ProcessUserID;
                        source.ProcessDate = DateTime.UtcNow.AddHours(8);
                        source.ProcessIPAddress = data.ProcessIPAddress;
                        source.Module = data.Module;
                        source.ListType = data.ListType;
                        source.WeblevelType = data.WeblevelType;
                        source.Condition = data.Condition;
                        source.SEODescription = data.SEODescription != null ? data.SEODescription.Trim() : null;
                        source.SEOKeywords = data.SEOKeywords != null ? data.SEOKeywords.Trim() : null;
                        source.DepartmentID = data.DepartmentID;
                        source.AdditionalCSS = data.AdditionalCSS;
                        source.AdditionalScript = data.AdditionalScript;
                        db.WebLevel.Update(source);
                        db.SaveChanges();
                        StaticLinkService.Save(source, DemoDNS);
                        //
                        //if (source.Lang == "zh-tw")
                        //{
                        //    var sameMainDatas = db.WebLevel.Where(x => x.MainSN == source.MainSN && x.Lang != "zh-tw").ToList();

                        //    foreach (var main in sameMainDatas)
                        //    {
                        //        //SortMethod不同記一筆更動排序
                        //        if (main.SortMethod != data.SortMethod)
                        //        {
                        //            LogService.CreateWebLevelSortLog(data.WebLevelSN, data.ProcessUserID, main.Lang, main.SortMethod, data.SortMethod);
                        //        }

                        //        //中文其他語系一起關
                        //        if (data.IsEnable == "0") { main.IsEnable = "0"; }
                        //        //中文可以強制控制其他語系
                        //        main.Module = data.Module;
                        //        main.ListType = data.ListType;
                        //        main.WeblevelType = data.WeblevelType;
                        //        main.Parameter = data.Parameter;
                        //        main.EndDate = data.EndDate;
                        //        main.SortMethod = data.SortMethod;
                        //        main.ProcessUserID = data.ProcessUserID;
                        //        main.ProcessDate = DateTime.UtcNow.AddHours(8);
                        //        main.ProcessIPAddress = data.ProcessIPAddress;
                        //    }
                        //    db.WebLevel.UpdateRange(sameMainDatas);
                        //    db.SaveChanges();
                        //}
                        //
                        #region File
                        if (commonFileModels != null)
                        {
                            var nowNewsFileName = commonFileModels.Select(x => x.fileNewName).ToList();
                            var DBAllFile = (from a in db.RelWebFileContent
                                             join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                             where a.SourceTable == "WebLevel" &&
                                                   a.SourceSN == data.WebLevelSN
                                             select b).ToList();

                            var DBFileName = DBAllFile.Select(y => y.FileName).ToList();
                            //刪除的檔案
                            var NeedDeleteFiles = DBAllFile.Where(x => !nowNewsFileName.Contains(x.FileName)).ToList();
                            //新的檔案
                            var NewFiles = commonFileModels.Where(x => !DBFileName.Contains(x.fileNewName)).ToList();
                            // 刪除 先壓狀態
                            if (NeedDeleteFiles != null)
                            {
                                foreach (var file in NeedDeleteFiles)
                                {
                                    file.IsEnable = "0";
                                    var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                    db.WEBFile.Update(file);
                                    db.RelWebFileContent.Remove(RelWebFileContentData);
                                    db.SaveChanges();
                                }
                            }
                            //新增
                            if (NewFiles != null)
                            {
                                foreach (var file in NewFiles)
                                {
                                    var FileData = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                    var RelWebFileContentData = new RelWebFileContent()
                                    {
                                        WEBFileSN = FileData.WEBFileSN,
                                        SourceTable = "WebLevel",
                                        SourceSN = data.WebLevelSN,
                                        GroupID = file.GroupID.ToString(),
                                        CreatedUserID = data.CreatedUserID ?? data.ProcessUserID,
                                        CreatedDate = DateTime.UtcNow.AddHours(8),
                                        SortOrder = file.FileSort,
                                    };
                                    FileData.IsEnable = "1";
                                    db.WEBFile.Update(FileData);
                                    db.RelWebFileContent.Add(RelWebFileContentData);
                                    db.SaveChanges();
                                }
                            }
                            //修改
                            foreach (var file in commonFileModels)
                            {
                                var filedata = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                filedata.FileTitle = file.fileTitle;
                                var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == filedata.WEBFileSN);
                                db.WEBFile.Update(filedata);
                                db.RelWebFileContent.Update(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                    }

                    #region 發布日期 排序更新
                    if (data.SortMethod == "1" && data.Module == "NEWS")
                    {
                        string SQL = @"WITH CTE AS (
                                  SELECT WebLevelSN,MainSN, Title, Lang, SortOrder, StartDate, IsEnable, ROW_NUMBER() OVER(ORDER BY StartDate DESC) AS RN
                                  FROM WEBNews
                                  WHERE WebLevelSN = @WebLevelSN AND IsEnable != '-99' AND Lang = 'zh-tw'
                                  )
                                  UPDATE WEBNews SET SortOrder  = B.RN
								  FROM WEBNews AS A
								  JOIN CTE AS B ON A.MainSN = B.MainSN";

                        List<SqlParameter> sqlParams = new List<SqlParameter>();
                        sqlParams.Add(new SqlParameter("@WebLevelSN", data.WebLevelSN));
                        db.Database.ExecuteSqlRaw(SQL, sqlParams.ToArray());
                    }
                    #endregion
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 刪除子節點
        /// (IsEnable改成-99)
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteAericle(int key, string ProcessUserID, string ProcessIP)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var webLevel = db.WebLevel.Where(x => x.MainSN == key).ToList();
                    foreach (var level in webLevel)
                    {
                        level.IsEnable = "-99";
                        level.ProcessDate = DateTime.UtcNow.AddHours(8);
                        level.ProcessUserID = ProcessUserID;
                        level.ProcessIPAddress = ProcessIP;
                        StaticLinkService.Save(level, DemoDNS);
                    }
                    db.WebLevel.UpdateRange(webLevel);

                    db.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
        }
        #region WebNews
        /// <summary>
        /// 取得列表
        /// </summary>
        /// <param name="keyword">關鍵字</param>
        /// <param name="str">起始日期</param>
        /// <param name="end">結束日期</param>
        /// <param name="key">WebLevelMSN 不可以缺</param>
        /// <param name="isenable">狀態</param>
        /// /// <param name="dep">部門</param>
        /// <param name="pager">頁碼資訊</param>
        /// <returns></returns>
        public static List<WEBNews> GetWebNews(string sorttitle, string sorttype, string keyword, string str, string end, WebLevel webLevel, string isenable, string dep, string lan, ref DefaultPager pager)
        {
            try
            {
                using (var db = new MODAContext())
                {

                    var Data = db.WEBNews.Where(x => 1 == 1);
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        Data = Data.Where(x => x.Title.Contains(keyword) || x.SubTitle.Contains(keyword));
                    }
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        var _str = DateTime.Parse(str + " 00:00:00");
                        Data = Data.Where(x => x.StartDate >= _str);
                    }
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        var _end = DateTime.Parse(end + " 23:59:59");
                        Data = Data.Where(x => x.StartDate <= _end);
                    }
                    Data = Data.Where(x => x.WebLevelSN == webLevel.WebLevelSN && x.Module == webLevel.Module && x.IsEnable != "-99");

                    Data = Data.Where(x => x.IsEnable != ((int)Utility.SysConst.IsEnable.Code.Del).ToString());
                    if (!string.IsNullOrWhiteSpace(isenable))
                    {
                        if (isenable == "1")
                        {
                            Data = Data.Where(x => x.IsEnable.Contains(isenable));
                            Data = Data.Where(x => x.EndDate == null || x.EndDate > DateTime.UtcNow.AddHours(8));
                        }
                        else if (isenable == "0" || isenable == "2")
                        {
                            Data = Data.Where(x => x.IsEnable.Contains(isenable));
                        }
                        else
                        {
                            Data = Data.Where(x => x.IsEnable == "1");
                            Data = Data.Where(x => x.EndDate < DateTime.UtcNow.AddHours(8));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(lan))
                    {
                        Data = Data.Where(x => x.Lang == lan);
                    }

                    if (!string.IsNullOrWhiteSpace(dep))
                    {
                        Data = Data.Where(x => x.DepartmentID == dep);
                    }

                    var allData = Data.GroupBy(x => x.MainSN).Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    var searchData = Data.OrderBy($" {sorttitle}  {sorttype} ").Skip((pager.p - 1) * pager.DisplayCount)
                                         .Take(pager.DisplayCount).ToList();

                    //var searchData = Data.OrderBy(x => x.IsTop == null)
                    //                     .ThenBy(x => x.IsTop)
                    //                     .ThenBy(x => x.SortOrder)
                    //                     .ThenBy(x => x.MainSN).Skip((pager.p - 1) * pager.DisplayCount)
                    //                     .Take(pager.DisplayCount).ToList();

                    return searchData;
                }
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
                    Action2 = "SELECT",
                    SourceTable = "WEBNews",
                    Action = "GetWebNews",
                    Controller = "WebLevelManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                var error = ex;
            }
            return null;
        }
        /// <summary>
        /// 查詢所有News for sort
        /// </summary>
        /// <param name="webLevel"></param>
        /// <returns></returns>
        public static List<WEBNews> GetWebNews(WebLevel webLevel, string lan)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var Data = db.WEBNews.Where(x => x.WebLevelSN == webLevel.WebLevelSN
                            && x.Module == webLevel.Module
                            && x.IsEnable != "-99" && x.Lang == lan).OrderBy(x => x.SortOrder).ToList();
                    return Data;
                }
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
                    Action2 = "SELECT",
                    SourceTable = "WEBNews",
                    Action = "GetWebNews",
                    Controller = "WebLevelManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                var error = ex;
            }
            return null;
        }

        public static List<WEBNews> GetWebNewsForIsTop(WebLevel webLevel, string lan)
        {

            try
            {
                using (var db = new MODAContext())
                {
                    var Data = db.WEBNews.Where(x => x.WebLevelSN == webLevel.WebLevelSN
                            && x.Module == webLevel.Module
                            && x.IsEnable != "-99" && x.Lang == lan && x.IsTop != null).OrderBy(x => x.IsTop == null).ThenBy(x => x.IsTop).ToList();
                    return Data;
                }
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
                    Action2 = "SELECT",
                    SourceTable = "WEBNews",
                    Action = "GetWebNews",
                    Controller = "WebLevelManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                var error = ex;
            }
            return null;
        }

        public static List<WEBNews> GetWebNewList(List<int> WebLevelSN, string Module, string IsEnable = "")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    if (IsEnable != "")
                    {
                        return db.WEBNews.Where(x => x.Module == Module && WebLevelSN.Contains(x.WebLevelSN) && x.IsEnable == IsEnable).ToList();
                    }
                    else
                    {
                        return db.WEBNews.Where(x => x.Module == Module && WebLevelSN.Contains(x.WebLevelSN) && (x.IsEnable == "0" || x.IsEnable == "1")).ToList();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// GetWEBNew
        /// </summary>
        /// <param name="key">WEBNewsSN</param>
        /// <returns></returns>
        public static WEBNews GetWEBNew(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBNews.FirstOrDefault(x => x.WEBNewsSN == key);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// GetWebNewsByWebLevelSN
        /// </summary>
        /// <param name="key">LevelSN</param>
        /// <returns></returns>
        public static List<WEBNews> GetWebNewsByWebLevelSN(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBNews.Where(x => x.WebLevelSN == key && x.IsEnable != "-99").ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public static List<WEBNews> GetWEBNewByMainSN(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBNews.Where(x => x.MainSN == key).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 取得擴充表資料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<WEBNewsExtend> GetWEBNewsExtends(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBNewsExtend.Where(x => x.WEBNewsSN == key).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }
        public static WEBNews GetWEBNew(int WebLevelSN, string Module)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBNews.FirstOrDefault(x => x.WebLevelSN == WebLevelSN && x.Module == Module);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<WEBNewsTranscript> GetWEBNewsTranscript(int WEBNewsSN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WEBNewsTranscript.Where(x => x.WEBNewsSN == WEBNewsSN).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public static bool SaveWebNews(ref WEBNews wEBNews, List<CommonFileModel> commonFileModels, List<WebLink> webLinks, List<WEBNewsExtend> webNewsExtend = null, List<WEBNewsTranscript> wEBNewsTranscripts = null)
        {
            int _webLevelSN = wEBNews.WebLevelSN;
            int _MainSN = wEBNews.WEBNewsSN;
            var lang = wEBNews.Lang;
            var Module = wEBNews.Module;
            var IsTopSort = 0;

            try
            {
                using (var db = new MODAContext())
                {
                    var mainSN = 0;
                    if (wEBNews.WEBNewsSN == 0)
                    {
                        var maxOrder = db.WEBNews.Where(x => x.WebLevelSN == _webLevelSN && x.SortOrder != null)
                                                    .OrderByDescending(x => x.SortOrder)
                                                    .FirstOrDefault();
                        if (wEBNews.IsTop == "1")
                        {
                            var sort = db.WEBNews.Where(x => x.WebLevelSN == _webLevelSN && x.IsTop != null && x.Lang == lang && x.IsEnable != "99").OrderByDescending(x => x.IsTop).Take(1).FirstOrDefault();
                            IsTopSort = sort == null ? 1 : int.Parse(sort.IsTop) + 1;
                        }

                        wEBNews.SortOrder = maxOrder is null ? 1 : maxOrder.SortOrder + 1;
                        wEBNews.IsTop = IsTopSort == 0 ? null : IsTopSort.ToString();
                        wEBNews.IsEnable = wEBNews.IsEnable;
                        db.WEBNews.Add(wEBNews);
                        db.SaveChanges();
                        wEBNews.MainSN = wEBNews.WEBNewsSN;
                        db.WEBNews.Update(wEBNews);
                        db.SaveChanges();
                        if (commonFileModels != null)
                        {
                            commonFileModels = commonFileModels.Where(x => x.lan == lang).ToList();
                            foreach (var file in commonFileModels)
                            {
                                var FileData = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                var RelWebFileContentData = new RelWebFileContent()
                                {
                                    WEBFileSN = FileData.WEBFileSN,
                                    SourceTable = "WEBNews",
                                    SourceSN = wEBNews.WEBNewsSN,
                                    GroupID = file.GroupID.ToString(),
                                    CreatedUserID = wEBNews.CreatedUserID,
                                    CreatedDate = DateTime.UtcNow.AddHours(8),
                                    SortOrder = file.FileSort,
                                };
                                FileData.IsEnable = "1";
                                FileData.FileTitle = file.fileTitle;
                                db.WEBFile.Update(FileData);
                                db.RelWebFileContent.Add(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }
                        if (webNewsExtend != null)
                        {
                            foreach (var Extend in webNewsExtend)
                            {
                                Extend.WEBNewsSN = wEBNews.WEBNewsSN;
                            }
                            db.WEBNewsExtend.AddRange(webNewsExtend);
                            db.SaveChanges();
                        }
                        if (wEBNewsTranscripts != null && wEBNewsTranscripts.Count > 0)
                        {
                            foreach (var Transcript in wEBNewsTranscripts)
                            {
                                Transcript.WEBNewsSN = wEBNews.WEBNewsSN;
                            }
                            db.WEBNewsTranscript.AddRange(wEBNewsTranscripts);
                        }
                        //New 新增MAINsn
                        mainSN = wEBNews.WEBNewsSN;
                        wEBNews.MainSN = mainSN;
                        var WebSiteID = wEBNews.WebSiteID;
                        var UserID = wEBNews.CreatedUserID;
                        var ProcessIPAddress = wEBNews.ProcessIPAddress;
                        #region 以下共用*多語系應該一樣
                        var PageViewType = wEBNews.PageViewType;
                        var WebLevelSN = wEBNews.WebLevelSN;
                        var ArticleType = wEBNews.ArticleType;
                        var SortOrder = wEBNews.SortOrder;
                        var IsTop = wEBNews.IsTop;
                        #endregion
                        db.SaveChanges();

                        var otherLangData = db.SysWebSiteLang.Where(x => x.Lang != lang && x.WebSiteID == WebSiteID).ToList();
                        foreach (var langData in otherLangData)
                        {
                            var webNews = new WEBNews()
                            {
                                WebSiteID = WebSiteID,
                                MainSN = mainSN,
                                WebLevelSN = WebLevelSN,
                                Module = Module,
                                target = (ArticleType == "2" || Module == "BANNER" || Module == "BANNER2") ? wEBNews.target : null,
                                ArticleType = ArticleType,
                                PageViewType = PageViewType,
                                Lang = langData.Lang,
                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                SortOrder = SortOrder,
                                CreatedUserID = UserID,
                                ProcessDate = DateTime.UtcNow.AddHours(8),
                                ProcessUserID = UserID,
                                ProcessIPAddress = ProcessIPAddress,
                                IsEnable = "0"
                            };
                            db.WEBNews.Add(webNews);
                            db.SaveChanges();
                        }

                        ///查詢weblevel是否用最新日期排序
                        var weblevel = db.WebLevel.Where(x => x.WebLevelSN == _webLevelSN).FirstOrDefault();

                        if (weblevel != null)
                        {
                            var needSort = new List<string>() { "NEWS", "BANNER2", "TEXT", "LINK" };
                            switch (weblevel.SortMethod)
                            {
                                case "0":
                                    var LangData = db.WEBNews.Where(x => x.MainSN == mainSN).ToList();
                                    foreach (var data in LangData)
                                    {
                                        ReSortByIsTOP(data.WEBNewsSN, 1, wEBNews.ProcessUserID, data.Lang);
                                    }
                                    break;
                                default:
                                    if (needSort.Contains(wEBNews.Module))
                                    {
                                        string SQL = @"WITH CTE AS (
                                  SELECT WEBNewsSN,WebLevelSN,MainSN, Title, Lang, SortOrder, StartDate, IsEnable, ROW_NUMBER() OVER(ORDER BY CASE WHEN IsTop IS NULL THEN 'Z' ELSE IsTop END, StartDate DESC) AS RN
                                  FROM WEBNews
                                  WHERE WebLevelSN = @WebLevelSN AND IsEnable != '-99' AND Lang = 'zh-tw'
                                  )
                                  UPDATE WEBNews SET SortOrder  = B.RN
								  FROM WEBNews AS A
								  JOIN CTE AS B ON A.WEBNewsSN = B.WEBNewsSN";

                                        List<SqlParameter> sqlParams = new List<SqlParameter>();
                                        sqlParams.Add(new SqlParameter("@WebLevelSN", weblevel.WebLevelSN));
                                        db.Database.ExecuteSqlRaw(SQL, sqlParams.ToArray());
                                        //更動排序記一筆
                                        var Lang = new string[] { "zh-tw", "en" };
                                        foreach (var lan in Lang)
                                        {
                                            LogService.CreateWebLevelSortLog(weblevel.WebLevelSN, wEBNews.ProcessUserID, lan);
                                        }
                                    }
                                    break;
                            }
                        }
                        StaticLinkService.Save(wEBNews, DemoDNS);
                    }
                    else
                    {
                        //Update
                        #region News
                        var oldData = db.WEBNews.First(x => x.MainSN == _MainSN && x.Lang == lang);
                        if (wEBNews.IsTop == "1" && oldData.IsTop == null)
                        {
                            var sort = db.WEBNews.Where(x => x.WebLevelSN == _webLevelSN && x.IsTop != null && x.Lang == oldData.Lang && x.IsEnable != "-99").OrderByDescending(x => x.IsTop).Take(1).FirstOrDefault();
                            IsTopSort = sort == null ? 1 : int.Parse(sort.IsTop) + 1;
                        }
                        wEBNews.WEBNewsSN = oldData.WEBNewsSN;
                        oldData.ArticleType = wEBNews.ArticleType;
                        oldData.ContentText = wEBNews.ContentText;
                        oldData.Description = wEBNews.Description;
                        oldData.EndDate = wEBNews.EndDate;
                        oldData.IsEnable = wEBNews.IsEnable;
                        oldData.IsTop = wEBNews.IsTop == null ? null : IsTopSort.ToString();
                        oldData.ProcessDate = DateTime.UtcNow.AddHours(8);
                        oldData.ProcessIPAddress = wEBNews.ProcessIPAddress;
                        oldData.ProcessUserID = wEBNews.ProcessUserID;
                        oldData.PublishDate = wEBNews.PublishDate;
                        oldData.StartDate = wEBNews.StartDate;
                        oldData.SubTitle = wEBNews.SubTitle;
                        oldData.target = (wEBNews.ArticleType == "2" || wEBNews.Module == "BANNER" || wEBNews.Module == "BANNER2") ? wEBNews.target : null;
                        oldData.Title = wEBNews.Title;
                        oldData.URL = wEBNews.URL;
                        oldData.YoutubeID = wEBNews.YoutubeID;
                        oldData.DepartmentID = wEBNews.DepartmentID;
                        oldData.CustomizeTagSn = wEBNews.CustomizeTagSn;
                        oldData.SEODescription = wEBNews.SEODescription;
                        oldData.SEOKeywords = wEBNews.SEOKeywords;
                        //頁籤排序
                        if (oldData.Module == "TAB" && wEBNews.SortOrder != null)
                        {
                            //更動排序記一筆
                            LogService.CreateWebLevelSortLog(wEBNews.WebLevelSN, wEBNews.ProcessUserID, oldData.Lang);
                            //向上找出父節點的所有子節點
                            var Tree = from m in db.WEBNews.Where(m => m.WEBNewsSN == _MainSN)
                                       join d in db.WEBNews.Where(d => d.IsEnable != "-99")
                                        on m.WebLevelSN equals d.WebLevelSN
                                       select d;
                            //找出當前節點的SortOrder
                            int originalSort = (from t in Tree
                                                where t.WEBNewsSN == _MainSN
                                                select t.SortOrder).FirstOrDefault() ?? 0;
                            //插入新序號
                            foreach (var item in Tree)
                            {
                                if (wEBNews.SortOrder < originalSort)
                                {
                                    if (item.SortOrder >= wEBNews.SortOrder)
                                        item.SortOrder += 1;
                                    if (item.WEBNewsSN == _MainSN)
                                        item.SortOrder = wEBNews.SortOrder;
                                }
                                else
                                {
                                    if (item.SortOrder > wEBNews.SortOrder)
                                        item.SortOrder += 1;
                                    if (item.WEBNewsSN == _MainSN)
                                        item.SortOrder = wEBNews.SortOrder + 1;
                                }
                            }
                            //重新排序
                            int i = 1;
                            var timeNow = DateTime.UtcNow.AddHours(8);
                            foreach (var item in Tree.AsEnumerable().OrderBy(x => x.SortOrder))
                            {
                                item.SortOrder = i;
                                i++;
                                //item.ProcessDate = timeNow;
                                //item.ProcessUserID = wEBNews.ProcessUserID;
                                //item.ProcessIPAddress = wEBNews.ProcessIPAddress;
                            }
                            db.SaveChanges();
                        }
                        db.WEBNews.Update(oldData);
                        db.SaveChanges();


                        //新規則 - 共用並以中文為主
                        //if (oldData.Lang == "zh-tw")
                        //{
                        //    var otherLangData = db.WEBNews.Where(x => x.MainSN == oldData.MainSN && x.Lang != oldData.Lang).ToList();
                        //    foreach (var langData in otherLangData)
                        //    {

                        //        if (wEBNews.IsEnable == "0") { langData.IsEnable = "0"; }
                        //        langData.PageViewType = wEBNews.PageViewType;
                        //        langData.WebLevelSN = wEBNews.WebLevelSN;
                        //        langData.Module = wEBNews.Module;
                        //        langData.ArticleType = wEBNews.ArticleType;
                        //        langData.ProcessDate = DateTime.UtcNow.AddHours(8);
                        //        langData.ProcessIPAddress = wEBNews.ProcessIPAddress;
                        //        //if(wEBNews.Module != "NEWS") { langData.SortOrder = oldData.SortOrder; }
                        //        db.WEBNews.Update(langData);
                        //        db.SaveChanges();
                        //    }
                        //}

                        ///查詢weblevel是否用最新日期排序
                        var weblevel = db.WebLevel.Where(x => x.WebLevelSN == _webLevelSN).FirstOrDefault();

                        if (weblevel != null)
                        {
                            if (weblevel.SortMethod == "1" && wEBNews.Module == "NEWS")
                            {
                                string SQL = @"WITH CTE AS (
                                         SELECT WEBNewsSN,WebLevelSN,MainSN, Title, Lang, SortOrder, StartDate, IsEnable, ROW_NUMBER() OVER(ORDER BY CASE WHEN IsTop IS NULL THEN 'Z' ELSE IsTop END, StartDate DESC) AS RN
                                         FROM WEBNews
                                         WHERE WebLevelSN = @WebLevelSN AND IsEnable != '-99' AND Lang = @lang
                                         )
                                         UPDATE WEBNews SET SortOrder  = B.RN
								         FROM WEBNews AS A
                                         JOIN CTE AS B ON A.WEBNewsSN = B.WEBNewsSN";

                                List<SqlParameter> sqlParams = new List<SqlParameter>();
                                sqlParams.Add(new SqlParameter("@lang", oldData.Lang));
                                sqlParams.Add(new SqlParameter("@WebLevelSN", weblevel.WebLevelSN));
                                db.Database.ExecuteSqlRaw(SQL, sqlParams.ToArray());

                                //更動排序記一筆
                                LogService.CreateWebLevelSortLog(weblevel.WebLevelSN, wEBNews.ProcessUserID, oldData.Lang);
                            }
                            else if (weblevel.SortMethod == "0" && wEBNews.Module == "NEWS")
                            {
                                ReSortByIsTOP(oldData.WEBNewsSN, 0, wEBNews.ProcessUserID, oldData.Lang);
                            }
                        }


                        #endregion
                        if (commonFileModels != null)
                        {
                            commonFileModels = commonFileModels.Where(x => x.lan == lang).ToList();
                            if (commonFileModels != null)
                            {


                                if (commonFileModels.Count() > 0)
                                {
                                    var nowNewsFileName = commonFileModels.Select(x => x.fileNewName).ToList();
                                    var DBAllFile = (from a in db.RelWebFileContent
                                                     join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                                     where a.SourceTable == "WEBNews" &&
                                                           a.SourceSN == oldData.WEBNewsSN
                                                     select b).ToList();

                                    var DBFileName = DBAllFile.Select(y => y.FileName).ToList();
                                    //刪除的檔案
                                    var NeedDeleteFiles = DBAllFile.Where(x => !nowNewsFileName.Contains(x.FileName)).ToList();
                                    //新的檔案
                                    var NewFiles = commonFileModels.Where(x => !DBFileName.Contains(x.fileNewName)).ToList();
                                    // 刪除 先壓狀態
                                    if (NeedDeleteFiles != null)
                                    {
                                        foreach (var file in NeedDeleteFiles)
                                        {
                                            file.IsEnable = "0";
                                            var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                            db.WEBFile.Update(file);
                                            db.RelWebFileContent.Remove(RelWebFileContentData);
                                            db.SaveChanges();
                                        }
                                    }
                                    //新增
                                    if (NewFiles != null)
                                    {
                                        foreach (var file in NewFiles)
                                        {
                                            var FileData = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                            var RelWebFileContentData = new RelWebFileContent()
                                            {
                                                WEBFileSN = FileData.WEBFileSN,
                                                SourceTable = "WEBNews",
                                                SourceSN = oldData.WEBNewsSN,
                                                GroupID = file.GroupID.ToString(),
                                                CreatedUserID = wEBNews.CreatedUserID,
                                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                                SortOrder = file.FileSort,
                                            };
                                            FileData.IsEnable = "1";
                                            db.WEBFile.Update(FileData);
                                            db.RelWebFileContent.Add(RelWebFileContentData);
                                            db.SaveChanges();

                                        }
                                    }
                                    //修改fileTitle
                                    foreach (var file in commonFileModels)
                                    {
                                        var filedata = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                        filedata.FileTitle = file.fileTitle;
                                        var RelWebFileContent = db.RelWebFileContent.FirstOrDefault(x => x.WEBFileSN == filedata.WEBFileSN);
                                        RelWebFileContent.SortOrder = file.FileSort;

                                        db.WEBFile.Update(filedata);
                                        db.RelWebFileContent.Update(RelWebFileContent);
                                        db.SaveChanges();

                                    }
                                }
                                else
                                {
                                    var DBAllFile = (from a in db.RelWebFileContent
                                                     join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                                     where a.SourceTable == "WEBNews" &&
                                                           a.SourceSN == oldData.WEBNewsSN
                                                     select b).ToList();
                                    // 刪除 先壓狀態
                                    if (DBAllFile != null)
                                    {
                                        foreach (var file in DBAllFile)
                                        {
                                            file.IsEnable = "0";
                                            var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                            db.WEBFile.Update(file);
                                            db.RelWebFileContent.Remove(RelWebFileContentData);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        //WEBNewsExtend
                        var oldWEBNewsExtend = db.WEBNewsExtend.Where(x => x.WEBNewsSN == oldData.WEBNewsSN).ToList();
                        db.WEBNewsExtend.RemoveRange(oldWEBNewsExtend);
                        if (webNewsExtend != null)
                        {
                            foreach (var Extend in webNewsExtend)
                            {
                                Extend.WEBNewsSN = oldData.WEBNewsSN;
                            }
                            db.WEBNewsExtend.AddRange(webNewsExtend);
                            db.SaveChanges();
                        }
                        if (wEBNewsTranscripts != null && wEBNewsTranscripts.Count > 0)
                        {
                            if (wEBNewsTranscripts.FirstOrDefault().WEBNewsTranscriptSN == 0)
                            {
                                var delTranscript = db.WEBNewsTranscript.Where(x => x.WEBNewsSN == oldData.WEBNewsSN).ToList();
                                db.WEBNewsTranscript.RemoveRange(delTranscript);
                                db.SaveChanges();
                                foreach (var Transcript in wEBNewsTranscripts)
                                {
                                    Transcript.WEBNewsSN = wEBNews.WEBNewsSN;
                                }
                                db.WEBNewsTranscript.AddRange(wEBNewsTranscripts);
                                db.SaveChanges();
                            }

                        }
                        StaticLinkService.Save(oldData, DemoDNS);
                    }
                    //StaticLink
                    if (Module == "CP")
                    {
                        //CP頁資料需要一併更新: StaticLink
                        var webLevelData = db.WebLevel.FirstOrDefault(x => x.MainSN == _webLevelSN && x.Lang == lang);
                        if (webLevelData != null)
                        {
                            StaticLinkService.Save(webLevelData, DemoDNS);
                        }
                    }


                }
                return true;
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = wEBNews.ProcessIPAddress,
                    UserID = wEBNews.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "update",
                    SourceTable = "WEBNews",
                    Action = "SaveWebNews",
                    Controller = "WebLevelManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }

        /// <summary>
        /// 特殊-雙語詞彙
        /// </summary>
        /// <param name="TWwEBNews"></param>
        /// <param name="ENwEBNews"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool SaveWebNewsBilingual(WEBNews TWwEBNews, WEBNews ENwEBNews)
        {

            try
            {
                if (TWwEBNews.MainSN == 0)
                {//new
                    using (var db = new MODAContext())
                    {
                        TWwEBNews.SortOrder = 0;
                        ENwEBNews.SortOrder = 0;
                        db.WEBNews.Add(TWwEBNews);
                        db.SaveChanges();
                        TWwEBNews.MainSN = TWwEBNews.WEBNewsSN;
                        db.WEBNews.Update(TWwEBNews);
                        db.SaveChanges();
                        ENwEBNews.MainSN = TWwEBNews.MainSN;
                        db.WEBNews.Add(ENwEBNews);
                        db.SaveChanges();
                    }
                }
                else
                {
                    using (var db = new MODAContext())
                    {
                        var newsData = db.WEBNews.Where(x => x.WebSiteID == TWwEBNews.WebSiteID && x.MainSN == TWwEBNews.MainSN).ToList();
                        var twData = newsData.FirstOrDefault(x => x.Lang == "zh-tw");
                        var enData = newsData.FirstOrDefault(x => x.Lang == "en");
                        twData.Title = TWwEBNews.Title;
                        twData.StartDate = TWwEBNews.StartDate;
                        twData.EndDate = TWwEBNews.EndDate;
                        twData.IsEnable = TWwEBNews.IsEnable;
                        enData.Title = ENwEBNews.Title;
                        enData.StartDate = ENwEBNews.StartDate;
                        enData.EndDate = ENwEBNews.EndDate;
                        enData.IsEnable = ENwEBNews.IsEnable;
                        db.WEBNews.Update(twData);
                        db.WEBNews.Update(enData);
                        db.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 特殊- 雙語詞彙 法規
        /// </summary>
        /// <param name="wEBNewsExtend"></param>
        /// <returns></returns>
        public static bool SaveWebNewsBilingualRegulations(WEBNewsExtend wEBNewsExtend , string RegulationsType)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var oldData = db.WEBNewsExtend.FirstOrDefault(x => x.WEBNewsSN == wEBNewsExtend.WEBNewsSN && x.GroupID == wEBNewsExtend.GroupID);
                    if (oldData == null)
                    {
                        var newData = new WEBNewsExtend()
                        {
                            GroupID = wEBNewsExtend.GroupID,
                            WEBNewsSN = wEBNewsExtend.WEBNewsSN,
                            Column_1 = wEBNewsExtend.Column_1,
                            Column_2 = wEBNewsExtend.Column_2,
                        };
                        if (RegulationsType == "1") {
                            db.WEBNewsExtend.Add(newData);
                        }
                    }
                    else
                    {
                        if (RegulationsType == "0")
                        {
                            db.WEBNewsExtend.Remove(oldData);
                        }
                        else {
                            oldData.Column_1 = wEBNewsExtend.Column_1;
                            oldData.Column_2 = wEBNewsExtend.Column_2;
                            db.WEBNewsExtend.Update(oldData);
                        }
                        
                    }
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 刪除WebNews（將IsEnable改為-99）
        /// </summary>
        /// <param name="wEBNews"></param>
        /// <returns></returns>
        public static bool DeleteWebNews(ref WEBNews wEBNews)
        {
            try
            {
                var _WebNewsSN = wEBNews.WEBNewsSN;

                if (wEBNews.WEBNewsSN != 0)
                {
                    using (var db = new MODAContext())
                    {
                        var _Data = db.WEBNews.Where(x => x.MainSN == _WebNewsSN).ToList();
                        foreach (var oldData in _Data)
                        {
                            oldData.IsEnable = "-99";
                            oldData.ProcessUserID = wEBNews.ProcessUserID;
                            oldData.ProcessDate = DateTime.UtcNow.AddHours(8);
                            oldData.ProcessIPAddress = wEBNews.ProcessIPAddress;
                            db.WEBNews.Update(oldData);
                            db.SaveChanges();
                            StaticLinkService.Save(oldData, DemoDNS);
                            wEBNews = oldData;

                            var dtSchedule = db.WebNewsSchedule.Where(x => x.ToWebNewsSn == oldData.WEBNewsSN).ToList();
                            foreach (var oldate in dtSchedule)
                            {
                                db.WebNewsSchedule.Remove(oldate);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = wEBNews.ProcessIPAddress,
                    UserID = wEBNews.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Delete",
                    SourceTable = "WEBNews",
                    Action = "DeleteWebNews",
                    Controller = "WebLevelManagementService",
                    SourceSN = wEBNews.WEBNewsSN,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }

        /// <summary>
        /// 取得新增的WEBNewsSN
        /// </summary>
        /// <param name="wEBNews"></param>
        /// <returns></returns>
        public static int GetInsertWEBNewsSN(WEBNews wEBNews)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    db.WEBNews.Add(wEBNews);
                    db.SaveChanges();
                    var NewsData = db.WEBNews.First(x =>
                            x.CreatedDate == wEBNews.CreatedDate &&
                            x.CreatedUserID == wEBNews.CreatedUserID &&
                            x.WebSiteID == wEBNews.WebSiteID &&
                            x.Module == wEBNews.Module
                            );
                    return NewsData.WEBNewsSN;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        #endregion

        /// <summary>
        /// 記錄CKEditor裡的連結
        /// </summary>
        /// <param name="SourceTable"></param>
        /// <param name="SourceSN"></param>
        /// <param name="ckEditorurl"></param>
        /// <returns></returns>
        public static bool SaveWebCntLink(string SourceTable, int SourceSN, List<string> ckEditorurl)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var oldData = db.WebCntLink.Where(x => x.SourceTable == SourceTable && x.SourceSN == SourceSN);
                    db.WebCntLink.RemoveRange(oldData);
                    db.SaveChanges();

                    foreach (var s in ckEditorurl.Distinct())
                    {
                        WebCntLink webCntLink = new WebCntLink();
                        webCntLink.SourceTable = SourceTable;
                        webCntLink.SourceSN = SourceSN;
                        webCntLink.URL = s;
                        webCntLink.CreatedDate = DateTime.UtcNow.AddHours(8);
                        db.WebCntLink.Add(webCntLink);
                        db.SaveChanges();
                    }
                }
                return true;
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
                    Action2 = "Insert",
                    SourceTable = "WebCntLink",
                    Action = "SaveWebCntLink",
                    Controller = "WebLevelManagementService",
                    SourceSN = SourceSN,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }

        /// <summary>
        /// 查詢webLink
        /// </summary>
        /// <param name="key">SourceSN</param>
        /// <returns></returns>
        public static List<WebLink> GetWebLinks(int key, string Source)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLink.Where(x => x.SourceSN == key && x.SourceTable == Source && x.IsEnable != "-99").OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        /// <summary>
        /// 部門
        /// </summary>
        /// <returns></returns>
        public static List<SysDepartment> sysDepartments()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysDepartment.Where(x => x.IsEnable == "1").ToList();
                }
                catch (Exception)
                {

                    return null;
                }
            }

        }
        /// <summary>
        /// WebLevel重新排序
        /// </summary>
        /// <param name="key">子節點MainSN</param>
        /// <param name="sort">序列號</param>
        /// <param name="ProcessUserID"></param>
        /// <param name="ProcessIP"></param>
        public static void ReArrangeByChild(int key, int sort, string ProcessUserID, string ProcessIP)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    //向上找出父節點的所有子節點

                    var Tree = (from c in db.WebLevel.Where(d => d.WebLevelSN == key)
                                join p in db.WebLevel
                                 on c.ParentSN equals p.WebLevelSN
                                join pc in db.WebLevel
                                 on p.WebLevelSN equals pc.ParentSN
                                where pc.IsEnable != "-99"
                                select pc).ToList();

                    //找出當前節點的SortOrder
                    int originalSort = (from t in Tree
                                        where t.WebLevelSN == key
                                        select t.SortOrder).FirstOrDefault() ?? 0;


                    foreach (var mainSN in Tree.Where(x => x.Lang == "zh-tw"))
                    {
                        var _sort = 0;

                        #region Main一起更新
                        if (sort < originalSort)
                        {
                            if (mainSN.MainSN == key)
                                _sort = sort;
                            else if (mainSN.SortOrder >= sort)
                                _sort = mainSN.SortOrder.Value + 1;
                            else
                                _sort = mainSN.SortOrder.Value;
                        }
                        else
                        {
                            if (mainSN.MainSN == key)
                                _sort = sort + 1;
                            else if (mainSN.SortOrder > sort)
                                _sort = mainSN.SortOrder.Value + 1;
                            else
                                _sort = mainSN.SortOrder.Value;
                        }
                        #endregion

                        //插入新序號
                        foreach (var item in Tree.Where(x => x.MainSN == mainSN.MainSN))
                        {
                            item.SortOrder = _sort;
                        }
                    }

                    //重新排序
                    int i = 1;
                    var timeNow = DateTime.UtcNow.AddHours(8);
                    foreach (var item in Tree.AsEnumerable().OrderBy(x => x.SortOrder))
                    {
                        item.SortOrder = i;
                        i++;
                    }
                    db.WebLevel.UpdateRange(Tree);
                    db.SaveChanges();

                    foreach (var mainSN in Tree.AsEnumerable().Where(x => x.MainSN == key))
                    {
                        //更動排序記一筆
                        LogService.CreateWebLevelSortLog(mainSN.ParentSN, ProcessUserID, mainSN.Lang);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        ///抓取司的版型顯示
        public static List<WebLevel> GetWebLevelByWebLevelSN(int? Key, int lv1, string title = "")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    string SQL = @"WITH CTE (WebLevelSN,WebLevelKey,ParentSN,Title,Lang,WeblevelType,Module,IsEnable,lv1) AS (
                               SELECT WebLevelSN,WebLevelKey,ParentSN,Title,Lang,WeblevelType,Module,IsEnable,1 AS lv1 FROM WebLevel
                               WHERE Module != 'PAGELIST' AND WeblevelType = '2' AND IsEnable != '-99' AND MainSN = @MainSN
                               UNION ALL
                               SELECT A.WebLevelSN,A.WebLevelKey,A.ParentSN,A.Title,A.Lang,A.WeblevelType,A.Module,A.IsEnable,1+1 FROM  WebLevel A,CTE B
                               WHERE A.WebLevelSN = B.ParentSN
                               )
                               SELECT distinct(A.WebLevelSN),A.WebLevelKey,A.ParentSN,A.WebSiteID,A.Lang,A.WeblevelType,A.Description,A.Parameter,A.Title,A.ContentText,A.FatFooterShow,
                               A.MainMenuShow,A.SubMemuShow,A.LeftMenuShow,A.RSSShow,A.PageView,A.StartDate,A.EndDate,A.ContentHeader,A.ContentFooter,A.ListType,A.SortMethod,
                               A.IsEnable,A.ProcessUserID,A.ProcessDate,A.ProcessIPAddress,A.CreatedUserID,A.CreatedDate,A.SortOrder,A.MainSN,A.StatesUrl,A.Module,A.TemplateValue,A.Condition
                               FROM WebLevel AS A
                               JOIN CTE AS B ON A.WebLevelSN = B.WebLevelSN
                               WHERE lv1 = @lv1 AND A.ParentSN != 0 AND A.Title LIKE @title";

                    List<SqlParameter> sqlParams = new List<SqlParameter>();

                    sqlParams.Add(new SqlParameter("@MainSN", Key));
                    sqlParams.Add(new SqlParameter("@lv1", lv1));
                    sqlParams.Add(new SqlParameter("@title", '%' + title));

                    var rtn = db.Set<WebLevel>().FromSqlRaw(SQL, sqlParams.ToArray()).ToList();

                    return rtn;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// WebNews重新排序
        /// </summary>
        /// <param name="key">子節點WebNewsSN</param>
        /// <param name="sort">序列號</param>
        /// <param name="userID"></param>
        /// <param name="processIPAddress"></param>
        public static void NewsReArrangeByChild(int key, int sort, string ProcessUserID, string ProcessIP, string lan = "")
        {
            using (var db = new MODAContext())
            {
                try
                {

                    //向上找出父節點的所有子節點
                    var Tree = from m in db.WEBNews.Where(m => m.WEBNewsSN == key && m.Lang == lan)
                               join d in db.WEBNews.Where(d => d.IsEnable != "-99" && d.Lang == lan)
                                on m.WebLevelSN equals d.WebLevelSN
                               select d;
                    //找出當前節點的SortOrder
                    int originalSort = (from t in Tree
                                        where t.WEBNewsSN == key
                                        select t.SortOrder).FirstOrDefault() ?? 0;

                    //插入新序號
                    foreach (var item in Tree)
                    {
                        if (sort < originalSort)
                        {
                            if (item.SortOrder >= sort)
                                item.SortOrder += 1;
                            if (item.WEBNewsSN == key)
                                item.SortOrder = sort;
                        }
                        else
                        {
                            if (item.SortOrder > sort)
                                item.SortOrder += 1;
                            if (item.WEBNewsSN == key)
                                item.SortOrder = sort + 1;
                        }
                    }
                    //重新排序
                    int i = 1;
                    var timeNow = DateTime.UtcNow.AddHours(8);
                    foreach (var item in Tree.AsEnumerable().OrderBy(x => x.SortOrder))
                    {
                        item.SortOrder = i;
                        i++;
                    }
                    db.SaveChanges();

                    //更動排序記一筆
                    LogService.CreateWebLevelSortLog(Tree.AsEnumerable().Where(x => x.WEBNewsSN == key).First().WebLevelSN, ProcessUserID, lan);
                    #region 英文排序

                    //      var data = db.WEBNews.FirstOrDefault(x => x.WEBNewsSN == key);
                    //      if (data != null)
                    //      {
                    //          System.FormattableString SQL = $@"WITH CTE AS (
                    //                        SELECT WebLevelSN,MainSN ,Title, Lang, SortOrder, StartDate, IsEnable, SortOrder AS RN
                    //                        FROM WEBNews
                    //                        WHERE WebLevelSN = {data.WebLevelSN} AND IsEnable != '-99' AND Lang = 'zh-tw'
                    //)
                    //UPDATE WEBNews SET SortOrder  = B.RN
                    //FROM WEBNews AS A
                    //JOIN CTE AS B ON A.MainSN = B.MainSN";
                    //          db.Database.ExecuteSqlInterpolated(SQL);

                    //      }

                    #endregion
                }
                catch (Exception)
                {

                }
            }
        }

        public static void NewsReArrangeByIsTop(int key, int isTop, string ProcessUserID, string ProcessIP, string lan)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    //向上找出父節點的所有子節點
                    var Tree = from m in db.WEBNews.Where(m => m.WEBNewsSN == key && m.IsTop != null && m.Lang == lan)
                               join d in db.WEBNews.Where(d => d.IsEnable != "-99" && d.IsTop != null && d.Lang == lan)
                                on m.WebLevelSN equals d.WebLevelSN
                               select d;
                    //找出當前節點的IsTop
                    int originalSort = (from t in Tree
                                        where t.WEBNewsSN == key
                                        select int.Parse(t.IsTop)).FirstOrDefault();

                    //插入新序號
                    foreach (var item in Tree)
                    {
                        if (isTop < originalSort)
                        {
                            if (int.Parse(item.IsTop) >= isTop)
                                item.IsTop += 1;
                            if (item.WEBNewsSN == key)
                                item.IsTop = isTop.ToString();
                        }
                        else
                        {
                            if (int.Parse(item.IsTop) > isTop)
                                item.IsTop += 1;
                            if (item.WEBNewsSN == key)
                                item.IsTop = (isTop + 1).ToString();
                        }
                    }
                    //重新排序
                    int i = 1;
                    var timeNow = DateTime.UtcNow.AddHours(8);
                    foreach (var item in Tree.AsEnumerable().OrderBy(x => x.IsTop == null).ThenBy(x => x.IsTop).ThenBy(x => x.SortOrder))
                    {
                        item.IsTop = i.ToString();
                        item.SortOrder = i;
                        i++;
                    }
                    db.SaveChanges();

                    //更動排序記一筆
                    LogService.CreateWebLevelSortLog(Tree.AsEnumerable().Where(x => x.WEBNewsSN == key).First().WebLevelSN, ProcessUserID, lan);
                }
                catch (Exception)
                {
                }
            }
        }
        /// <summary>
        /// 找尋 file  
        /// </summary>
        /// <param name="key">WEBNewsSN or WEBlEBELD ...</param>
        /// <param name="SourceTable">關聯的table</param>
        /// <returns></returns>
        public static List<WebFileAndGroupIDModel> GetWEBFiles(string key, string SourceTable)
        {
            try
            {
                string _sql = $@"select  GroupID, f.* ,wfc.SortOrder
                from {SourceTable} n
                inner join [RelWebFileContent] wfc on n.{SourceTable}SN = wfc.SourceSN and wfc.SourceTable='{SourceTable}'
                inner join WEBFile f  on wfc.WEBFileSN = f.WEBFileSN
                where n.{SourceTable}SN = @key";

                var para = new SqlParameter("key", Convert.ToInt32(key));
                using (var db = new MODAContext())
                {
                    var list = db.Set<WebFileAndGroupIDModel>().FromSqlRaw(_sql, para).ToList();
                    return list;
                }

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
                    Action2 = "SELECT",
                    SourceTable = SourceTable,
                    Action = "GetWEBFiles",
                    Controller = "WebLevelManagementService",
                    SourceSN = Convert.ToInt32(key),
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return null;

            }
        }

        /// <summary>
        /// 置頂重整排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sort"></param>
        /// <param name="lan"></param>
        public static void ReSortByIsTOP(int key, int sort, string ProcessUserID, string lan)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    //向上找出父節點的所有子節點
                    var Tree = (from m in db.WEBNews.Where(m => m.WEBNewsSN == key && m.Lang == lan)
                                join d in db.WEBNews.Where(d => d.IsEnable != "-99" && d.Lang == lan)
                                 on m.WebLevelSN equals d.WebLevelSN
                                select d).OrderBy(d => d.IsTop == null).ThenBy(d => d.IsTop).ThenBy(d => d.SortOrder).ToList();

                    if (sort > 0)
                    {
                        //找出當前節點的SortOrder
                        int originalSort = (from t in Tree
                                            where t.WEBNewsSN == key
                                            select t.SortOrder).FirstOrDefault() ?? 0;

                        //插入新序號
                        foreach (var item in Tree)
                        {
                            if (sort < originalSort && item.IsTop == null)
                            {
                                if (item.SortOrder >= sort)
                                    item.SortOrder += 1;
                                if (item.WEBNewsSN == key)
                                    item.SortOrder = sort;
                            }
                            else
                            {
                                if (int.Parse(item.IsTop) >= sort)
                                {
                                    //item.SortOrder += 1;
                                    item.IsTop = (int.Parse(item.IsTop) + 1).ToString();
                                }
                                if (item.WEBNewsSN == key)
                                {
                                    //item.SortOrder = sort;
                                    item.IsTop = sort.ToString();
                                }
                            }
                        }
                    }

                    //重新排序
                    int i = 1;
                    foreach (var item in Tree.AsEnumerable().OrderBy(x => x.IsTop == null).ThenBy(x => x.IsTop).ThenBy(x => x.SortOrder))
                    {
                        item.SortOrder = i;
                        i++;
                    }
                    db.SaveChanges();

                    //更動排序記一筆
                    LogService.CreateWebLevelSortLog(Tree.AsEnumerable().Where(x => x.WEBNewsSN == key).First().WebLevelSN, ProcessUserID, lan);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 自訂義標籤
        /// </summary>
        /// <param name="key"></param>
        public static List<WebLevelCustomizeTag> GetCustomizeList(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevelCustomizeTag.Where(x => x.WebLevelSn == key).OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static bool EditWebLevelCustomizeTag(WebLevel data, Dictionary<int, string> customize, ref string msg)
        {
            using (var db = new MODAContext())
            {
                var oldData = db.WebLevelCustomizeTag.Where(x => x.WebLevelSn == data.WebLevelSN).ToLookup(x => x.WebLevelCustomizeTagSn, x => x.TagName).ToDictionary(x => x.Key, x => x.First());
                var NewData = customize.ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, x => x.First());
                var Cancel = oldData.Where(x => !NewData.ContainsKey(x.Key)).ToList();//比對刪除資料
                if (Cancel.Count > 0)
                {
                    foreach (var d in Cancel)
                    {

                        var check = db.WEBNews.Where(x => x.CustomizeTagSn == d.Key && x.IsEnable != "-99").FirstOrDefault();

                        if (check != null)
                        {
                            msg = "目前：分類" + d.Value + "有新聞資料正在使用中，請先檢查新聞資料資料";
                            return false;
                        }

                        System.FormattableString SQL = $@"DELETE WebLevelCustomizeTag
                                                                     WHERE WebLevelCustomizeTagSn = @WebLevelCustomizeTagSn";

                        List<SqlParameter> sqlParams = new List<SqlParameter>();

                        sqlParams.Add(new SqlParameter("@WebLevelCustomizeTagSn", d.Key));
                        db.Database.ExecuteSqlRaw(SQL.ToString(), sqlParams.ToArray());
                    }
                }

                var News = NewData.Where(x => !oldData.ContainsKey(x.Key)).ToList();//比對新增資料

                if (News.Count > 0)
                {

                    var mainSort = db.WebLevelCustomizeTag.Where(x => x.WebLevelSn == data.WebLevelSN).OrderByDescending(x => x.SortOrder).FirstOrDefault();
                    var i = mainSort is null ? 1 : mainSort.SortOrder + 1;

                    foreach (var N in News)
                    {
                        WebLevelCustomizeTag webLevelCustomizeTag = new WebLevelCustomizeTag();
                        webLevelCustomizeTag.WebLevelSn = data.WebLevelSN;
                        webLevelCustomizeTag.TagName = N.Value;
                        webLevelCustomizeTag.SortOrder = i;
                        webLevelCustomizeTag.IsEnable = "1";
                        webLevelCustomizeTag.CreatedUserId = data.ProcessUserID;
                        webLevelCustomizeTag.CreatedDate = DateTime.UtcNow.AddHours(8);
                        webLevelCustomizeTag.ProcessUserId = data.ProcessUserID;
                        webLevelCustomizeTag.ProcessDate = DateTime.UtcNow.AddHours(8);
                        webLevelCustomizeTag.ProcessIpaddress = data.ProcessIPAddress;
                        db.WebLevelCustomizeTag.Add(webLevelCustomizeTag);
                        db.SaveChanges();
                        i++;
                    }
                }

                #region 檢查是否就資料有更新
                if (customize.Count > 0)
                {
                    var i = 1; //重新排序
                    foreach (var s in customize)
                    {
                        var dt = db.WebLevelCustomizeTag.Where(x => x.WebLevelCustomizeTagSn == s.Key).FirstOrDefault();

                        if (dt != null)
                        {
                            dt.TagName = s.Value;
                            dt.ProcessUserId = data.ProcessUserID;
                            dt.ProcessDate = DateTime.UtcNow.AddHours(8);
                            dt.SortOrder = i;
                            db.WebLevelCustomizeTag.Update(dt);
                            db.SaveChanges();
                            i++;
                        }

                    }
                }
                #endregion

                return true;
            }
        }

        public static List<SysZipCode> GetCity()
        {
            using (var db = new MODAContext())
            {
                return db.SysZipCode.Where(x => x.IsEnable != "-99").OrderBy(x => x.SortOrder).ThenBy(x => x.SortOrder).ToList();
            }
        }

        public static bool ScheduleSave(ref WEBNews wEBNews, List<int> webNewsSN, List<CommonFileModel> commonFileModels, List<WEBNewsExtend> wEBNewsExtends = null)
        {
            int _webLevelSN = wEBNews.WebLevelSN;
            int _MainSN = wEBNews.WEBNewsSN;
            var lang = wEBNews.Lang;
            var Module = wEBNews.Module;

            try
            {
                using (var db = new MODAContext())
                {
                    var mainSN = 0;

                    if (wEBNews.WEBNewsSN == 0)
                    {
                        //SortOrder
                        var maxOrder = db.WEBNews.Where(x => x.WebLevelSN == _webLevelSN && x.SortOrder != null)
                                                    .OrderByDescending(x => x.SortOrder)
                                                    .FirstOrDefault();

                        wEBNews.SortOrder = maxOrder is null ? 1 : maxOrder.SortOrder + 1;

                        db.WEBNews.Add(wEBNews);
                        db.SaveChanges();

                        wEBNews.MainSN = wEBNews.WEBNewsSN;
                        db.WEBNews.Update(wEBNews);
                        db.SaveChanges();

                        //檔案
                        if (commonFileModels != null && commonFileModels.Count > 0)
                        {
                            commonFileModels = commonFileModels.Where(x => x.lan == lang).ToList();
                            foreach (var file in commonFileModels)
                            {
                                var FileData = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                var RelWebFileContentData = new RelWebFileContent()
                                {
                                    WEBFileSN = FileData.WEBFileSN,
                                    SourceTable = "WEBNews",
                                    SourceSN = wEBNews.WEBNewsSN,
                                    GroupID = file.GroupID.ToString(),
                                    CreatedUserID = wEBNews.CreatedUserID,
                                    CreatedDate = DateTime.UtcNow.AddHours(8),
                                    SortOrder = file.FileSort,
                                };
                                FileData.IsEnable = "1";
                                FileData.FileTitle = file.fileTitle;
                                db.WEBFile.Update(FileData);
                                db.RelWebFileContent.Add(RelWebFileContentData);
                                db.SaveChanges();
                            }
                        }

                        if (wEBNewsExtends != null && wEBNewsExtends.Count > 0)
                        {
                            foreach (var Extend in wEBNewsExtends)
                            {
                                Extend.WEBNewsSN = wEBNews.WEBNewsSN;
                            }
                            db.WEBNewsExtend.AddRange(wEBNewsExtends);
                            db.SaveChanges();
                        }

                        #region 以下共用*多語系應該一樣
                        mainSN = wEBNews.WEBNewsSN;
                        wEBNews.MainSN = mainSN;
                        var WebSiteID = wEBNews.WebSiteID;
                        var UserID = wEBNews.CreatedUserID;
                        var ProcessIPAddress = wEBNews.ProcessIPAddress;
                        var WebLevelSN = wEBNews.WebLevelSN;
                        var ArticleType = wEBNews.ArticleType;
                        var SortOrder = wEBNews.SortOrder;
                        db.SaveChanges();
                        #endregion

                        var otherLangData = db.SysWebSiteLang.Where(x => x.Lang != lang && x.WebSiteID == WebSiteID).ToList();

                        foreach (var langData in otherLangData)
                        {
                            var webNews = new WEBNews()
                            {
                                WebSiteID = WebSiteID,
                                MainSN = mainSN,
                                WebLevelSN = WebLevelSN,
                                Module = Module,
                                ArticleType = ArticleType,
                                Lang = langData.Lang,
                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                SortOrder = SortOrder,
                                CreatedUserID = UserID,
                                ProcessDate = DateTime.UtcNow.AddHours(8),
                                ProcessUserID = UserID,
                                ProcessIPAddress = ProcessIPAddress,
                                IsEnable = "0"
                            };
                            db.WEBNews.Add(webNews);
                            db.SaveChanges();
                        }

                        if (webNewsSN != null && webNewsSN.Count > 0)
                        {
                            foreach (var sn in webNewsSN)
                            {
                                WebNewsSchedule webNewsSchedule = new WebNewsSchedule();
                                webNewsSchedule.ToWebNewsSn = wEBNews.WEBNewsSN;
                                webNewsSchedule.FromWebNewsSn = sn;
                                db.WebNewsSchedule.Add(webNewsSchedule);
                                db.SaveChanges();
                            }
                        }

                        //調整最新一筆資料排序
                        var LangData = db.WEBNews.Where(x => x.MainSN == mainSN).ToList();
                        foreach (var data in LangData)
                        {
                            ReSortByPublishDate(wEBNews.WEBNewsSN, data.Lang);
                        }

                        //StaticLinkService.Save(wEBNews, DemoDNS);
                    }
                    else
                    {
                        var oldData = db.WEBNews.First(x => x.MainSN == _MainSN && x.Lang == lang);

                        wEBNews.WEBNewsSN = oldData.WEBNewsSN;
                        oldData.Title = wEBNews.Title;
                        oldData.SubTitle = wEBNews.SubTitle;
                        oldData.ContentText = wEBNews.ContentText;
                        oldData.Description = wEBNews.Description;
                        oldData.PublishDate = wEBNews.PublishDate;
                        oldData.IsEnable = wEBNews.IsEnable;
                        oldData.StartDate = wEBNews.StartDate;
                        oldData.EndDate = wEBNews.EndDate;
                        oldData.ZipCodeSn = wEBNews.ZipCodeSn;
                        oldData.ProcessUserID = wEBNews.ProcessUserID;
                        oldData.ProcessDate = DateTime.UtcNow.AddHours(8);
                        oldData.ProcessIPAddress = wEBNews.ProcessIPAddress;
                        oldData.DepartmentID = wEBNews.DepartmentID;
                        db.WEBNews.Update(oldData);
                        db.SaveChanges();

                        if (commonFileModels != null && commonFileModels.Count > 0)
                        {
                            commonFileModels = commonFileModels.Where(x => x.lan == lang).ToList();

                            if (commonFileModels.Count() > 0)
                            {
                                var nowNewsFileName = commonFileModels.Select(x => x.fileNewName).ToList();
                                var DBAllFile = (from a in db.RelWebFileContent
                                                 join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                                 where a.SourceTable == "WEBNews" &&
                                                       a.SourceSN == oldData.WEBNewsSN
                                                 select b).ToList();

                                var DBFileName = DBAllFile.Select(y => y.FileName).ToList();
                                //刪除的檔案
                                var NeedDeleteFiles = DBAllFile.Where(x => !nowNewsFileName.Contains(x.FileName)).ToList();
                                //新的檔案
                                var NewFiles = commonFileModels.Where(x => !DBFileName.Contains(x.fileNewName)).ToList();
                                // 刪除 先壓狀態
                                if (NeedDeleteFiles != null)
                                {
                                    foreach (var file in NeedDeleteFiles)
                                    {
                                        file.IsEnable = "0";
                                        var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                        db.WEBFile.Update(file);
                                        db.RelWebFileContent.Remove(RelWebFileContentData);
                                        db.SaveChanges();
                                    }
                                }
                                //新增
                                if (NewFiles != null)
                                {
                                    foreach (var file in NewFiles)
                                    {
                                        var FileData = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                        var RelWebFileContentData = new RelWebFileContent()
                                        {
                                            WEBFileSN = FileData.WEBFileSN,
                                            SourceTable = "WEBNews",
                                            SourceSN = oldData.WEBNewsSN,
                                            GroupID = file.GroupID.ToString(),
                                            CreatedUserID = wEBNews.CreatedUserID,
                                            CreatedDate = DateTime.UtcNow.AddHours(8),
                                            SortOrder = file.FileSort,
                                        };
                                        FileData.IsEnable = "1";
                                        db.WEBFile.Update(FileData);
                                        db.RelWebFileContent.Add(RelWebFileContentData);
                                        db.SaveChanges();

                                    }
                                }
                                //修改fileTitle
                                foreach (var file in commonFileModels)
                                {
                                    var filedata = db.WEBFile.First(x => x.FileName == file.fileNewName);
                                    filedata.FileTitle = file.fileTitle;
                                    var RelWebFileContent = db.RelWebFileContent.FirstOrDefault(x => x.WEBFileSN == filedata.WEBFileSN);
                                    RelWebFileContent.SortOrder = file.FileSort;

                                    db.WEBFile.Update(filedata);
                                    db.RelWebFileContent.Update(RelWebFileContent);
                                    db.SaveChanges();

                                }
                            }
                            else
                            {
                                var DBAllFile = (from a in db.RelWebFileContent
                                                 join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                                 where a.SourceTable == "WEBNews" &&
                                                       a.SourceSN == oldData.WEBNewsSN
                                                 select b).ToList();
                                // 刪除 先壓狀態
                                if (DBAllFile != null && DBAllFile.Count > 0)
                                {
                                    foreach (var file in DBAllFile)
                                    {
                                        file.IsEnable = "0";
                                        var RelWebFileContentData = db.RelWebFileContent.First(x => x.WEBFileSN == file.WEBFileSN);
                                        db.WEBFile.Update(file);
                                        db.RelWebFileContent.Remove(RelWebFileContentData);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        //WEBNewsExtend
                        var oldWEBNewsExtend = db.WEBNewsExtend.Where(x => x.WEBNewsSN == oldData.WEBNewsSN).ToList();
                        db.WEBNewsExtend.RemoveRange(oldWEBNewsExtend);
                        if (wEBNewsExtends != null && wEBNewsExtends.Count > 0)
                        {
                            foreach (var Extend in wEBNewsExtends)
                            {
                                Extend.WEBNewsSN = oldData.WEBNewsSN;
                            }
                            db.WEBNewsExtend.AddRange(wEBNewsExtends);
                            db.SaveChanges();
                        }

                        #region 比對選擇新聞
                        var oldSchedule = db.WebNewsSchedule.Where(x => x.ToWebNewsSn == oldData.WEBNewsSN).ToLookup(x => x.FromWebNewsSn).ToDictionary(x => x.Key);
                        var NewSchedule = webNewsSN.ToLookup(x => x).ToDictionary(x => x.Key);

                        var Cancel = oldSchedule.Where(x => !NewSchedule.ContainsKey((int)x.Key)).ToList();//比對刪除資料

                        if (Cancel.Count > 0)
                        {
                            foreach (var d in Cancel)
                            {
                                var del = db.WebNewsSchedule.Where(x => x.ToWebNewsSn == oldData.WEBNewsSN && x.FromWebNewsSn == d.Key).First();
                                db.WebNewsSchedule.Remove(del);
                                db.SaveChanges();
                            }
                        }

                        var News = NewSchedule.Where(x => !oldSchedule.ContainsKey(x.Key)).ToList();//比對新增資料

                        if (News.Count > 0)
                        {
                            foreach (var n in News)
                            {
                                WebNewsSchedule webNewsSchedule = new WebNewsSchedule();
                                webNewsSchedule.ToWebNewsSn = oldData.WEBNewsSN;
                                webNewsSchedule.FromWebNewsSn = n.Key;
                                db.WebNewsSchedule.Add(webNewsSchedule);
                                db.SaveChanges();
                            }
                        }
                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = wEBNews.ProcessIPAddress,
                    UserID = wEBNews.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "update",
                    SourceTable = "WEBNews",
                    Action = "ScheduleSave",
                    Controller = "WebLevelManagementService",
                    SourceSN = wEBNews.WEBNewsSN,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
            return true;
        }
        /// <summary>
        /// 取出行程內指定的新聞資料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<WEBNews> GetScheduleByWEBNews(int key)
        {
            using (var db = new MODAContext())
            {
                List<WEBNews> wEBNews = new List<WEBNews>();

                var dtWEBNews = db.WEBNews.Where(x => x.WEBNewsSN == key);

                #region 從MODULE 為Schedule SN取出與相關新聞SN做匹配
                var Schedule = (from N in dtWEBNews
                                join S in db.WebNewsSchedule
                                on N.WEBNewsSN equals S.ToWebNewsSn
                                select new WEBNews
                                {
                                    WEBNewsSN = (int)S.FromWebNewsSn
                                }).ToList();

                if (Schedule.Count > 0)
                {
                    foreach (var S in Schedule)
                    {
                        var News = (from N in db.WEBNews.Where(x => x.WEBNewsSN == S.WEBNewsSN)
                                    select new WEBNews
                                    {
                                        WEBNewsSN = N.WEBNewsSN,
                                        Title = N.Title,
                                        Lang = N.Lang,
                                        StartDate = N.StartDate,
                                        EndDate = N.EndDate,
                                        StatesUrl = N.StatesUrl,
                                    }).First();

                        wEBNews.Add(News);
                    }
                }
                #endregion

                return wEBNews;
            }
        }

        /// <summary>
        /// 行程日期排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lan"></param>
        public static void ReSortByPublishDate(int key, string lan)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    //向上找出父節點的所有子節點
                    var Tree = (from m in db.WEBNews.Where(m => m.WEBNewsSN == key && m.Lang == lan)
                                join d in db.WEBNews.Where(d => d.IsEnable != "-99" && d.Lang == lan)
                                on m.WebLevelSN equals d.WebLevelSN
                                select d).OrderByDescending(x => x.PublishDate).ToList();
                    //重新排序
                    int i = 1;
                    foreach (var item in Tree.AsEnumerable().OrderByDescending(x => x.PublishDate))
                    {
                        item.SortOrder = i;
                        i++;
                    }
                    db.SaveChanges();
                }
                catch (Exception) { }
            }
        }


        #region 移動節點
        public static void MoveTree(int parentSN, int mainSN)
        {
            using (var db = new MODAContext())
            {
                var moveData = db.WebLevel.Where(x => x.MainSN == mainSN).ToList();
                var brotherCount = db.WebLevel.Where(x => x.ParentSN == parentSN && x.Lang == "zh-tw" && x.IsEnable != "-99").Max(x => x.SortOrder);
                foreach (var le in moveData)
                {
                    le.ParentSN = parentSN;
                    le.IsEnable = "0";
                    le.SortOrder = brotherCount + 1;
                    Services.Static.StaticLinkService.Save(le, DemoDNS);
                }
                db.SaveChanges();
            }
            FindChildWebLevel(mainSN);
        }
        static void FindChildWebLevel(int mainSN)
        {
            using (var db = new MODAContext())
            {
                var leveldata = db.WebLevel.Where(x => x.ParentSN == mainSN).ToList();
                if (leveldata?.Count > 0)
                {
                    foreach (var le in leveldata)
                    {
                        Services.Static.StaticLinkService.Save(le, DemoDNS);
                        if (le.Lang != "zh-tw") return;
                        var module = Utility.EnumTpye.GetEnum<Utility.EnumWebLevelModuleLevel2>(le?.Module);
                        switch (module)
                        {
                            case EnumWebLevelModuleLevel2.PAGELIST:
                                FindChildWebLevel(le.MainSN.Value);
                                break;
                            case EnumWebLevelModuleLevel2.NEWS:
                            case EnumWebLevelModuleLevel2.CP:
                                FindChildWebNews(le.MainSN.Value);
                                break;
                        }
                    }
                }
            }
        }
        static void FindChildWebNews(int webLevelmainSN)
        {
            using (var db = new MODAContext())
            {
                var newsdata = db.WEBNews.Where(x => x.WebLevelSN == webLevelmainSN).ToList();
                foreach (var ne in newsdata)
                {
                    StaticLinkService.Save(ne, DemoDNS);
                }
            }
        }

        public static List<WebLevel> GetWebLevelTree(string webSiteID, string module)
        {
            using (var db = new MODAContext())
            {
                var webLevelData = db.WebLevel.Where(x =>
                x.WebSiteID == webSiteID &&
                x.Module == module &&
                x.IsEnable != "-99" &&
                x.WeblevelType == "1" &&
                x.Lang == "zh-tw"
                ).ToList();
                return webLevelData;
            }
        }



        #endregion
    }
}

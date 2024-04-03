using System;
using System.Collections.Generic;
using System.Text;
using DBModel;

using Utility;

using System.Linq;
using Services.Authorization;
using Services.Models;
using Services.Models.WebSite;

namespace Services
{
    public class CommonService
    {

        public static bool IsStatic { get; set; } = true;
        public static string WebSiteUrl { get; set; }

        public static string WebAPIUrl { get; set; }
        /// <summary>
        /// 依條件選擇使用者帳號
        /// 並排除群組現有人員
        /// </summary>
        /// <param name="group">群組SN</param>
        /// <param name="keyWord">姓名/帳號</param>
        /// <param name="depID">部門ID</param>
        /// <returns></returns>
        public static List<vw_UserLeftDep> UserSelectorGetUserList(int group, string keyWord, string depID)
        {
            try
            {

                using (var db = new MODAContext())
                {
                    var _group = group;

                    var lsit = (from m in db.vw_UserLeftDep
                                join d in db.RelSysUserGroup.Where(d => d.SysGroupSN == _group) on m.UserID equals d.UserID into ps
                                from o in ps.DefaultIfEmpty()
                                where o == null
                                     && (string.IsNullOrWhiteSpace(keyWord) ? 1 == 1 : m.UserID.Contains(keyWord) || m.UserName.Contains(keyWord))
                                     && (string.IsNullOrWhiteSpace(depID) ? 1 == 1 : m.DepartmentID == depID)
                                     && m.UserSatus == "1"
                                     && (m.DisableDate == null || m.DisableDate > DateTime.UtcNow.AddHours(8))
                                select m).ToList();
                    return lsit;
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
                    Action2 = "Select",
                    SourceTable = "vw_UserLeftDep",
                    Action = "UserSelectorGetUserList",
                    Controller = "CommonService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return null;
        }
        /// <summary>
        /// 依條件選擇使用者帳號 With Pager
        /// 並排除群組現有人員
        /// </summary>
        /// <param name="group">群組SN</param>
        /// <param name="keyWord">姓名/帳號</param>
        /// <param name="depID">部門ID</param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static List<vw_UserLeftDep> UserSelectorGetUserList(int group, string keyWord, string depID, ref DefaultPager pager)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var _group = group; 
                    var Data = (from m in db.vw_UserLeftDep
                                join d in db.RelSysUserGroup.Where(d => d.SysGroupSN == _group) on m.UserID equals d.UserID into ps
                                from o in ps.DefaultIfEmpty()
                                where o == null
                                   && (string.IsNullOrWhiteSpace(keyWord) ? 1 == 1 : m.UserID.Contains(keyWord) || m.UserName.Contains(keyWord))
                                   && (string.IsNullOrWhiteSpace(depID) ? 1 == 1 : m.DepartmentID == depID)
                                   && m.UserSatus == "1"
                                   && (m.DisableDate == null || m.DisableDate > DateTime.UtcNow.AddHours(8))
                                select m);
                    var allData = Data.Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    var searchData = Data.OrderByDescending(m => m.CreatedDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
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
                    Action2 = "Select",
                    SourceTable = "vw_UserLeftDep",
                    Action = "UserSelectorGetUserList(DefaultPager pager)",
                    Controller = "CommonService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }

            return null;
        }
        /// <summary>
        /// 取得所有群組
        /// </summary>
        /// <returns></returns>
        public static List<SysGroup> GetGroups()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysGroup.Where(x => x.IsEnable == "1").ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 取得區域代號
        /// </summary>
        /// <returns></returns>
        public static List<SysZipCode> GetZipCodes()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysZipCode.Where(x => x.IsEnable == "1" && x.ParentSn == 0).OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 發布單位
        /// </summary>
        /// <param name="webSiteID"></param>
        /// <param name="Lang"></param>
        /// <returns></returns>
        public static List<SysDepartment> GetDepartments(string webSiteID = "MODA", string Lang = "zh-tw")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysDepartment.Where(x => x.IsEnable == "1" && x.WebSiteId == webSiteID && x.Lang == Lang).OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 不分子站台抓取發布單位
        /// </summary>
        /// <param name="Lang"></param>
        /// <returns></returns>
        public static List<SysDepartment> GetAllDepartments(string Lang = "zh-tw")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysDepartment.Where(x => x.IsEnable == "1" && x.Lang == Lang).OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public static List<SysDepartment> GetFlatDepartments( )
        {
            var defaultData = GetAllDepartments();
            var depData = new List<SysDepartment>();
            int i = 1;
            foreach (var dep in defaultData.Where(x => x.ParentID == 0).OrderBy(x => x.SortOrder))
            {
                depData.Add(new SysDepartment() { SortOrder = i, DepartmentName = dep.DepartmentName  , SysDepartmentSN = dep.SysDepartmentSN });
                i++;
                foreach (var dep1 in defaultData.Where(x => x.ParentID == dep.SysDepartmentSN).OrderBy(x => x.SortOrder)) 
                {
                    depData.Add(new SysDepartment() { SortOrder = i, DepartmentName = dep.DepartmentName +"-"+ dep1.DepartmentName, SysDepartmentSN = dep1.SysDepartmentSN });
                    i++;
                    foreach (var dep2 in defaultData.Where(x => x.ParentID == dep1.SysDepartmentSN).OrderBy(x => x.SortOrder))
                    {
                        depData.Add(new SysDepartment() { SortOrder = i, DepartmentName = dep.DepartmentName + "-" + dep1.DepartmentName + "-" + dep2.DepartmentName, SysDepartmentSN = dep2.SysDepartmentSN });
                        i++;
                    }
                }
            }
            return depData;
        }

        public static List<WebLevel> GetWEBLevel(string Lang = "zh-tw")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var result = (from x in db.WebLevel.Where(x => x.IsEnable == "1" && x.Lang == Lang)
                                  select new WebLevel
                                  {
                                      WebLevelSN = x.WebLevelSN,
                                      ParentSN = x.ParentSN,
                                      WebSiteID = x.WebSiteID,
                                      Title = x.Title,
                                      SortOrder = x.SortOrder
                                  }).OrderBy(x => x.SortOrder).ToList();

                    return result;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 取得左側清單
        /// </summary>
        /// <returns></returns>
        public static List<DBModel.SysSection> GetSysSections()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var lsit = db.SysSection.Where(x => x.IsEnable == "1").OrderBy(x => x.SortOrder)
                               .ToList();
                    return lsit;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 取得參數表
        /// </summary>
        /// <param name="ParentKey"></param>
        /// <returns></returns>
        public static List<DBModel.SysCategory> GetSysCategories(string ParentKey)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysCategory.Where(x => x.IsEnable == "1" && x.ParentKey == ParentKey).OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 取得語系列表
        /// </summary>
        /// <param name="webSiteID"></param>
        /// <returns></returns>
        public static List<SysWebSiteLang> GetSysWebSiteLang(string webSiteID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysWebSiteLang.Where(x => x.WebSiteID == webSiteID).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        /// <summary>
        /// 前台取得麵包屑
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="levelMainSN"></param>
        /// <param name="newsMainSN"></param>
        /// <returns></returns>
        public static List<WebSiteBreadcrumb> GetWebSiteBreadcrumb(string lang, int levelMainSN = 0, int newsMainSN = 0)
        {
            var WebSiteBreadcrumb = new List<WebSiteBreadcrumb>();
            var newData = new WEBNews();
            try
            {
                if (newsMainSN != 0)
                {
                    using (var db = new MODAContext())
                    {
                        newData = db.WEBNews.First(x => x.MainSN == newsMainSN && x.Lang == lang);
                        if (newData.Module != "CP")
                        {
                            WebSiteBreadcrumb.Add(new Models.WebSiteBreadcrumb()
                            {
                                mainSN = newsMainSN,
                                sort = 1,
                                lang = lang,
                                SourseTable = "webnews",
                                WebSiteID = newData.WebSiteID,
                                Title = newData.Title,
                                IsActive = false,
                            });
                        }
                    }
                    levelMainSN = newData.WebLevelSN;
                }
                //webLevelBreadcrumb
                GetWebLevelData(levelMainSN, lang, ref WebSiteBreadcrumb);
                //Set Active Level
                WebSiteBreadcrumb.FirstOrDefault(x => x.mainSN == levelMainSN && x.SourseTable == "weblevel").IsActive = true;
                //取靜態化網址
                GetStaticLinkData(ref WebSiteBreadcrumb);
            }
            catch (Exception)
            {
            }
            return WebSiteBreadcrumb;
        }
        /// <summary>
        /// 取的節點資料
        /// </summary>
        /// <param name="mainSN"></param>
        /// <param name="lang"></param>
        /// <param name="webSiteBreadcrumbs"></param>
        static void GetWebLevelData(int mainSN, string lang, ref List<WebSiteBreadcrumb> webSiteBreadcrumbs, int sort = 2, int firstWebLevelSN = 0)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var weblevelData = db.WebLevel.Where(x => x.MainSN == mainSN && x.Lang == lang).ToList();
                    if (weblevelData.Count() == 0) return;
                    if (firstWebLevelSN == 0)
                    {
                        firstWebLevelSN = db.WebLevel.Where(x =>
                        x.WebSiteID == weblevelData.First().WebSiteID &&
                        x.WeblevelType == "1"
                        ).Min(x => x.WebLevelSN);
                    }

                    var levelData = weblevelData
                        .Select(x => new WebSiteBreadcrumb()
                        {
                            mainSN = x.MainSN.Value,
                            Title = x.Title,
                            ParentSN = x.ParentSN,
                            sort = sort++,
                            lang = lang,
                            WebSiteID = x.WebSiteID,
                            SourseTable = "weblevel",
                            IsActive = false,
                        });
                    webSiteBreadcrumbs.AddRange(levelData);
                    if (weblevelData.FirstOrDefault().ParentSN == firstWebLevelSN) return;
                    GetWebLevelData(levelData.FirstOrDefault().ParentSN, lang, ref webSiteBreadcrumbs, sort++, firstWebLevelSN);
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 取得靜態化網址
        /// </summary>
        /// <param name="webSiteBreadcrumb"></param>
        static void GetStaticLinkData(ref List<WebSiteBreadcrumb> webSiteBreadcrumb)
        {
            using (var db = new MODAContext())
            {
                var isStatic = CommonService.IsStatic;
                var domainUrl = db.StaticLink.FirstOrDefault(x => x.StaticUrl == "/index.html")?.Link;
                try
                {
                    foreach (var br in webSiteBreadcrumb)
                    {
                        var d = db.StaticLink.FirstOrDefault(x => x.MainSN == br.mainSN && x.Lang == br.lang && x.SourseTable == br.SourseTable);
                        br.Url = isStatic ? ( d?.StaticUrl ?? string.Empty)  : d.Link?.Replace(domainUrl, "")  ;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        public static ogModel GetOgData(string lang, int levelMainSN = 0, int newsMainSN = 0)
        {
            var og = new ogModel();
            try
            {
                var title = string.Empty;
                var Breadcrumb = GetWebSiteBreadcrumb(lang, levelMainSN, newsMainSN);
                using (var db = new MODAContext())
                {
                    var webSiteLangData = db.SysWebSiteLang.FirstOrDefault(x => x.WebSiteID == Breadcrumb.FirstOrDefault().WebSiteID && x.Lang == lang);
                    title = $@"｜{webSiteLangData.Title}";
                    if (newsMainSN != 0)
                    {
                        var newData = db.WEBNews.FirstOrDefault(x => x.MainSN == newsMainSN && x.Lang == lang);
                        var keywords = string.Join(',',
                                     (from a in db.WEBNewsExtend
                                      join b in db.WEBNews on a.WEBNewsSN equals b.WEBNewsSN
                                      where a.GroupID == "keyword" && b.MainSN == newsMainSN && b.Lang == lang
                                      select a).Select(x => x.Column_1).ToList());
                        var img = (from a in db.RelWebFileContent
                                   join b in db.WEBFile on a.WEBFileSN equals b.WEBFileSN
                                   where a.SourceTable == "WEBNews"
                                   && a.SourceSN == newData.WEBNewsSN
                                   && (a.GroupID == "NWMI" || a.GroupID == "CPMI")
                                   select b).FirstOrDefault();
                        og.keyword = keywords;
                        og.description = newData?.SubTitle ?? newData.Title;
                        if (img == null)
                        {
                            if (newData.WebSiteID == "MODA")
                            {
                                og.image = $"{WebSiteUrl}/assets/img/fbshare.jpg";
                                og.image_path = "/assets/img/fbshare.jpg";
                            }
                            else
                            {
                                og.image = $"{WebSiteUrl}/{newData.WebSiteID}/assets/img/fbshare.jpg";
                                og.image_path = $"/{newData.WebSiteID}/assets/img/fbshare.jpg";
                            }
                            og.image_type = "image/jpeg";
                        }
                        else
                        {
                            og.image = $"{WebSiteUrl}{img.FilePath}";
                            og.image_path = $"/{img.FilePath}";
                            og.image_type = Utility.Files.getcontenttype(img.OriginalFileName);
                        }
                    }
                    else
                    {
                        var webLeveLData = db.WebLevel.FirstOrDefault(x => x.MainSN == levelMainSN && x.Lang == lang);
                        if (webLeveLData.WebSiteID == "MODA")
                        {
                            og.image = $"{WebSiteUrl}/assets/img/fbshare.jpg";
                            og.image_path = "/assets/img/fbshare.jpg";
                        }
                        else
                        {
                            og.image = $"{WebSiteUrl}/{webLeveLData.WebSiteID}/assets/img/fbshare.jpg";
                            og.image_path = $"/{webLeveLData.WebSiteID}/assets/img/fbshare.jpg";
                        }
                        og.image_type = "image/jpeg";
                        og.description = webLeveLData.Description ?? webLeveLData.Title;
                    }
                }
                var lastTilte = string.Join(" - ", Breadcrumb.Select(x => x.Title));
                og.title = lastTilte + title;
            }
            catch (Exception)
            {
            }
            return og;
        }

        public static SysWebSiteLang GetSysWebSiteLang(string WebSiteID, string lang)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysWebSiteLang.FirstOrDefault(x => x.WebSiteID == WebSiteID && x.Lang == lang);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 取出站台參數列表
        /// </summary>
        /// <param name="webSiteID"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static List<SysCategory> GetWebSiteCategory(string webSiteID, string lang)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysCategory.Where(x => x.WebSiteID == webSiteID && x.Lang == lang).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 後台麵包屑
        /// </summary>
        /// <param name="LevelSN"></param>
        /// <param name="NewsSN"></param>
        /// <returns></returns>
        public static List<string> LevelBreadcrumb(int LevelSN = 0, int NewsSN = 0)
        {
            List<string> titleList = new List<string>();
            var levelData = new List<WebLevel>();
            GetWebLevelData(LevelSN, ref levelData);
            foreach (var level in levelData.OrderBy(x => x.WebLevelSN))
            {
                titleList.Add(level.Title);
            }
            if (NewsSN != 0)
            {
                using (var db = new MODAContext())
                {
                    try
                    {
                        var data = db.WEBNews.FirstOrDefault(x => x.WEBNewsSN == NewsSN);
                        if (data != null) titleList.Add(data.Title);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return titleList;
        }

        static void GetWebLevelData(int LevelSN, ref List<WebLevel> LevelSNData)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.WebLevel.FirstOrDefault(x => x.WebLevelSN == LevelSN);
                    if (data != null) LevelSNData.Add(data);
                    if (data.ParentSN == 0) return;
                    GetWebLevelData(data.ParentSN, ref LevelSNData);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public static List<WebLevelCustomizeTag> GetWebLevelCustomizeTags(int levelSN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebLevelCustomizeTag.Where(x => x.WebLevelSn == levelSN && x.IsEnable == "1").OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 查詢Level Tree是否啟用
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static bool CheckLevelByTree(int WeblevelSN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    bool result = true;
                    int cLevelSN = WeblevelSN;
                    bool flag = true;
                    while (flag)
                    {
                        var level = db.WebLevel.FirstOrDefault(x => x.WebLevelSN == cLevelSN
                                                                && (x.StartDate == null || x.StartDate <= DateTime.UtcNow.AddHours(8))
                                                                && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.AddHours(8)));
                        if (level != null)
                        {
                            if (level.IsEnable != "1")
                            {
                                result = false;
                                flag = false;
                            }
                            else if (level.ParentSN == 0)
                            {
                                flag = false;
                            }
                            else
                            {
                                cLevelSN = level.ParentSN;
                            }
                        }
                        else
                        {
                            result = false;
                            flag = false;
                        }
                    }

                    return result;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static bool CheckLocalUrl(string url) 
        {
            return Utility.CommFun.CheckLocalUrl(url, CommonService.WebSiteUrl, CommonService.WebAPIUrl);
        }

        /// <summary>
        /// 查詢單位階層樹
        /// </summary>
        public static string GetDeptTree(int deptSN) 
        {
            using (var db = new MODAContext())
            {
                SysDepartment dept = db.SysDepartment.FirstOrDefault(x => x.SysDepartmentSN == deptSN);
                String tree = dept?.DepartmentName ?? "";

                while (dept!= null && dept.ParentID != 0) 
                {
                    dept = db.SysDepartment.FirstOrDefault(x => x.SysDepartmentSN == dept.ParentID);
                    if (dept != null) 
                    {
                        tree = dept.DepartmentName + "-" + tree;
                    }
                }

                return tree;
            }
        }

        /// <summary>
        /// 查詢user所屬單位
        /// </summary>
        public static SysDepartment GetDeptByUser(string userID) 
        {
            using (var db = new MODAContext())
            {
                var result = (from a in db.SysUser
                             join b in db.SysDepartment on a.DepartmentID equals b.DepartmentID
                             where a.UserID == userID
                             select b).FirstOrDefault();

                return result;
            }
        }

        public static List<SysDepartment> GetWebsiteDept(string Lang = "zh-tw") 
        {
            using (var db = new MODAContext())
            {
                var result = (from a in db.SysDepartment.Where(x => x.Lang == Lang && x.ParentID == 0 && x.IsEnable == "1")
                              join b in db.SysWebSite on a.WebSiteId equals b.WebSiteID
                              orderby b.SortOrder ascending
                              select a).ToList();

                return result;
            }
        }
    }
}

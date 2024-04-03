using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Authorization;
using Utility;

namespace Services.SystemManageMent
{
    public class CodeManagementService
    {
        /// <summary>
        /// 全部-Category
        /// </summary>
        /// <returns></returns>
        public static List<SysCategory> GetCategoryList(string webSiteId, bool GodMode = false)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var WebSiteList = new List<string>();
                    WebSiteList.Add(webSiteId);
                    if (GodMode)
                    {
                        WebSiteList.Add("Management");
                    }
                    var rtn = db.SysCategory.Where(x => x.IsEnable != "-99"
                    && WebSiteList.Contains(x.WebSiteID)
                    ).OrderBy(x => x.SortOrder).ToList();
                    return rtn;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<SysCategory> GetCategoryByParentKeys(string ParentKey, string WebSiteID, string keyword, int p, int DisplayCount, List<SysWebSiteLang> language)
        {
            using (var db = new MODAContext())
            {
                DefaultPager pager = new DefaultPager();
                List<SysCategory> sysCategory = new List<SysCategory>();

                try
                {
                    var list = db.SysCategory.Where(x => x.IsEnable != "-99" && x.ParentKey == ParentKey && x.WebSiteID == WebSiteID);

                    if (!string.IsNullOrWhiteSpace(keyword) && keyword != "undefined")
                    {
                        list = list.Where(x => x.Value.Contains(keyword));
                    }

                    var allData = list.GroupBy(x => x.MainSN).Count();
                    pager.TotalCount = allData;
                    pager.DisplayCount = DisplayCount;
                    pager.p = p;
                    pager.PageIndex = pager.p - 1;

                    foreach (var lan in language)
                    {
                        var data = list.Where(x => x.Lang == lan.Lang).OrderBy(x => x.SortOrder).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount);
                        sysCategory.AddRange(data);
                    }
                    var searchData = sysCategory;
                    return searchData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static DefaultPager GetCategoryPage(string ParentKey, string WebSiteID, string keyword, string lan, int p, int DisplayCount)
        {
            using (var db = new MODAContext())
            {
                DefaultPager pager = new DefaultPager();
                try
                {
                    var list = db.SysCategory.Where(x => x.IsEnable != "-99" && x.ParentKey == ParentKey && x.WebSiteID == WebSiteID);

                    list = list.Where(x => x.Lang == lan);

                    if (!string.IsNullOrWhiteSpace(keyword) && keyword != "undefined")
                    {
                        list = list.Where(x => x.Value.Contains(keyword));
                    }

                    var allData = list.GroupBy(x => x.MainSN).Count();
                    pager.TotalCount = allData;
                    pager.DisplayCount = DisplayCount;
                    pager.p = p;
                    pager.PageIndex = pager.p - 1;
                    pager.Lang = lan;
                    return pager;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static void GetParentTitle(string ParentKey, ref List<SysCategory> tiltes, int sort = 0)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var CategoryData = db.SysCategory.OrderBy(x => x.SysCategorySN).Where(x => x.SysCategoryKey == ParentKey).First();
                    tiltes.Add(new SysCategory() { Description = CategoryData.Description, SortOrder = sort });
                    if (!string.IsNullOrWhiteSpace(CategoryData.ParentKey))
                    {
                        sort++;
                        GetParentTitle(CategoryData.ParentKey, ref tiltes, sort);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        public static bool Create(SysCategory sysCategory)
        {
            try
            {
                var WebSiteID = sysCategory.WebSiteID;
                if (sysCategory.WebSiteID == "Management") { WebSiteID = "MODA"; }
                var WEBSiteLang = (new MODAContext()).SysWebSiteLang.Where(x => x.WebSiteID == WebSiteID).ToList();
                if (WEBSiteLang.Count > 0)
                {
                    using (var db = new MODAContext())
                    {
                        var MainKey = 0;
                        var Data = db.SysCategory.Where(x => x.ParentKey == sysCategory.ParentKey && x.Lang == "zh-tw");
                        var allData = Data.Count();
                        var sortorder = Data.Count() > 0 ? (Data.OrderByDescending(x => x.SortOrder).Take(1).First().SortOrder + 1) : 1;
                        foreach (var lang in WEBSiteLang)
                        {
                            var _SysCategory = new SysCategory()
                            {
                                SysCategoryKey = $@"{sysCategory.ParentKey}-{allData + 1}",
                                ParentKey = sysCategory.ParentKey,
                                WebSiteID = sysCategory.WebSiteID,
                                Lang = lang.Lang,
                                Value = lang.Lang.Contains("zh-tw") ? sysCategory.Value : "",
                                Description = lang.Lang.Contains("zh-tw") ? sysCategory.Description : "",
                                ProcessUserID = sysCategory.ProcessUserID,
                                ProcessDate = sysCategory.ProcessDate,
                                ProcessIPAddress = sysCategory.ProcessIPAddress,
                                CreatedUserID = sysCategory.CreatedUserID,
                                CreatedDate = sysCategory.CreatedDate,
                                SortOrder = sortorder,
                                IsEnable = "1",
                                MainSN = MainKey
                            };

                            db.SysCategory.Add(_SysCategory);
                            db.SaveChanges();

                            if (MainKey == 0)
                            {
                                MainKey = _SysCategory.SysCategorySN;

                                System.FormattableString _sql = @$"Update  [SysCategory] set MainSN={MainKey}
                                WHERE SysCategorySN = {MainKey}
                                ";
                                db.Database.ExecuteSqlInterpolated(_sql);
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
                    ProcessIPAddress = sysCategory.ProcessIPAddress,
                    UserID = sysCategory.ProcessUserID,
                    WebSiteID = sysCategory.WebSiteID,
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Insert",
                    SourceTable = "SysCategory",
                    Action = "Create",
                    Controller = "CodeManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }
        public static bool Edit(SysCategory sysCategory)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    string SQL = @"UPDATE SysCategory
                                   SET Value = @Value
                                      ,IsEnable = @IsEnable
                                      ,Description = @Description
                                      ,ProcessUserID = @ProcessUserID
                                      ,ProcessDate = @ProcessDate
                                      ,ProcessIPAddress = @ProcessIPAddress
                                   WHERE SysCategorySN =@SysCategorySN";

                    List<SqlParameter> sqlParams = new List<SqlParameter>();

                    sqlParams.Add(new SqlParameter("@Value", sysCategory.Value));
                    sqlParams.Add(new SqlParameter("@IsEnable", sysCategory.IsEnable));
                    sqlParams.Add(new SqlParameter("@Description", sysCategory.Description));
                    sqlParams.Add(new SqlParameter("@ProcessUserID", sysCategory.ProcessUserID));
                    sqlParams.Add(new SqlParameter("@ProcessDate", sysCategory.ProcessDate));
                    sqlParams.Add(new SqlParameter("@ProcessIPAddress", sysCategory.ProcessIPAddress));
                    sqlParams.Add(new SqlParameter("@SysCategorySN", sysCategory.SysCategorySN));

                    db.Database.ExecuteSqlRaw(SQL, sqlParams.ToArray());
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
                    ProcessIPAddress = sysCategory.ProcessIPAddress,
                    UserID = sysCategory.ProcessUserID,
                    WebSiteID = sysCategory.WebSiteID,
                    WebPath = sysCategory.SysCategoryKey,
                    ActionType = "1",
                    Action2 = "Update",
                    SourceTable = "SysCategory",
                    Action = "Edit",
                    Controller = "CodeManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }

        public static List<SysCategory> GetCategory(string ParentKey, string WebSiteID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Data = db.SysCategory.Where(x => x.ParentKey == ParentKey && x.WebSiteID == WebSiteID);
                    Data = Data.Where(x => x.IsEnable != ((int)Utility.SysConst.IsEnable.Code.Del).ToString());

                    var searchData = Data.OrderBy(o => o.SortOrder).ToList();
                    return searchData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public static List<SysCategory> GetCategory(string ParentKey, string WebSiteID, string Lang)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Data = db.SysCategory.Where(x => x.ParentKey == ParentKey && x.WebSiteID == WebSiteID && x.Lang == Lang);
                    Data = Data.Where(x => x.IsEnable != ((int)Utility.SysConst.IsEnable.Code.Del).ToString());

                    var searchData = Data.OrderBy(o => o.SortOrder).ToList();
                    return searchData;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public static List<SysCategory> GetCategory(int MainSn)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Data = db.SysCategory.Where(x => x.MainSN == MainSn);
                    Data = Data.Where(x => x.IsEnable != ((int)Utility.SysConst.IsEnable.Code.Del).ToString());
                    var searchData = Data.OrderBy(o => o.SysCategorySN).ToList();
                    return searchData;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 抓取自身語系的資料
        /// </summary>
        /// <param name="CategoryKey"></param>
        /// <param name="Lang"></param>
        /// <returns></returns>
        public static SysCategory GetCategoryByCategoryKey(string CategoryKey, string Lang)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Data = db.SysCategory.FirstOrDefault(x => x.SysCategoryKey == CategoryKey && x.Lang == Lang);
                    return Data;
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        public static void CategoryReArrangeByChild(string SysCategoryKey, string WebSiteID, int sort, string ProcessUserID, string ProcessIP)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Tree = from m in db.SysCategory.Where(m => m.SysCategoryKey == SysCategoryKey && m.WebSiteID == WebSiteID)
                               join d in db.SysCategory.Where(m => m.IsEnable != "-99")
                                on new { m.ParentKey, m.WebSiteID } equals new { d.ParentKey, d.WebSiteID }
                               select d;
                    int originalSort = (from t in Tree
                                        where t.SysCategoryKey == SysCategoryKey
                                        select t.SortOrder).FirstOrDefault() ?? default(int);

                    foreach (var item in Tree)
                    {
                        if (sort < originalSort)
                        {
                            if (item.SortOrder >= sort)
                                item.SortOrder += 1;
                            if (item.SysCategoryKey == SysCategoryKey)
                                item.SortOrder = sort;
                        }
                        else
                        {
                            if (item.SortOrder > sort)
                                item.SortOrder += 1;
                            if (item.SysCategoryKey == SysCategoryKey)
                                item.SortOrder = sort + 1;
                        }
                    }
                    int i = 1;
                    var timeNow = DateTime.UtcNow.AddHours(8);
                    foreach (var item in Tree.AsEnumerable().OrderBy(x => x.SortOrder))
                    {
                        item.SortOrder = i;
                        i++;
                    }
                    db.SaveChanges();

                }
                catch (Exception) { }
            }
        }

        public static List<SysWebSiteLang> GetSysWebSiteLangs(Models.sysUserModel user)
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
        /// 判斷是否可以刪除
        /// </summary>
        /// <returns></returns>
        public static bool CheckCanDelete(string ParentID, string webSiteID)
        {
            using (var db = new MODAContext())
            {
                var canDeleteManager = db.SysCategory.Where(x => x.ParentKey == "Management-2" && x.Lang == "zh-tw")?.Select(x => x.Value).ToList();
                if (canDeleteManager.Count() == 0) return false;
                var chkParent = canDeleteManager.Select(x => webSiteID + x).ToList();
                if (chkParent.IndexOf(ParentID) > -1) return true;
                return false;
            }
        }


        public static bool Delate(string SysCategoryKey, string UserID)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var NeedDeleteData = db.SysCategory.Where(x => x.SysCategoryKey == SysCategoryKey).ToList();
                    foreach (var del in NeedDeleteData)
                    {
                        del.IsEnable = "-99";
                        del.ProcessUserID = UserID;
                        del.ProcessDate = DateTime.UtcNow.AddHours(8);
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void BindingNoLang()
        {
            using (var db = new MODAContext())
            {
                FormattableString q = $@" update SysCategory set Lang='zh-tw' where Lang = '' ;";
                db.Database.ExecuteSql(q);
                db.SaveChanges();
            }
        }
    }
}

using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Models;
using Services.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility;
using System.Linq.Dynamic.Core;

namespace Services.Authorization
{
    public class GroupManagementService
    {
        #region 主頁查詢條件

        /// <summary>
        /// 部門清單(抓取根)
        /// </summary>
        /// <returns></returns>
        public static List<SearchModel.SelectOptions> GetSysDepartments()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var lsit = db.SysDepartment.Where(x => x.IsEnable == "1").OrderBy(x => x.SortOrder)
                               .Select(d => new SearchModel.SelectOptions()
                               {
                                   Title = d.DepartmentName,
                                   Value = d.SysDepartmentSN.ToString()
                               }).ToList();
                    return lsit;
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
                        Action2 = "select",
                        SourceTable = "SysDepartment",
                        Action = "GetSysDepartments",
                        Controller = "GroupManagementService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                    return null;
                }
            }

        }

        public static List<SearchModel.SelectOptions> GetWebLevels()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var list = db.WebLevel.Where(x => x.IsEnable == "1").OrderBy(x => x.WebSiteID).ThenBy(x => x.SortOrder)
                                .Select(d => new SearchModel.SelectOptions()
                                {
                                    Title = d.Title,
                                    Value = d.WebLevelSN.ToString()
                                }).ToList();
                    return list;
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
                        Action2 = "select",
                        SourceTable = "WebLevel",
                        Action = "GetWebLevels",
                        Controller = "GroupManagementService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                    return null;
                }
            }
        }
        #endregion

        /// <summary>
        /// 取得群組列表
        /// </summary>
        /// <param name="key">群組名稱</param>
        /// <param name="states">啟用與否</param>
        /// <param name="dep">部門代號</param>
        /// <param name="keyword">關鍵字</param>
        /// <param name="sec">選單</param>
        /// <param name="pager">分頁模組</param>
        /// <returns></returns>
        public static List<GroupModel> GetGroupList(string sortTitle , string sortType, string key, string states, string dep, string keyword, string sec, ref DefaultPager pager , ref List<int> sortData )
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var Seatch1 = new List<RelSysUserGroup>();  //查詢人員或部門
                    var Search2 = new List<AuthSysGroupSysSection>(); //查詢選單
                    if (!string.IsNullOrWhiteSpace(keyword) || !string.IsNullOrWhiteSpace(dep))
                    {
                        Seatch1 = (from a in db.RelSysUserGroup
                                   join b in db.SysUser on a.UserID equals b.UserID
                                   where b.UserSatus == "1"
                                   && ((string.IsNullOrWhiteSpace(keyword) ? 1 == 1 : b.UserName.Contains(keyword)) ||
                                   (string.IsNullOrWhiteSpace(keyword) ? 1 == 1 : b.UserID.Contains(keyword)))
                                   && (string.IsNullOrWhiteSpace(dep) ? 1 == 1 : b.DepartmentID == dep)
                                   select new RelSysUserGroup() { SysGroupSN = a.SysGroupSN }
            ).ToList();


                    }
                    if (!string.IsNullOrWhiteSpace(sec))
                    {
                        int _sec = int.Parse(sec);
                        Search2 = db.AuthSysGroupSysSection.Where(x => x.SysSectionSN == _sec).ToList();
                    }
                    var groupList = new List<int>();
                    if (Seatch1.Count() > 0) { groupList.AddRange(Seatch1.Select(x => x.SysGroupSN.Value)); }
                    if (Search2.Count() > 0) { groupList.AddRange(Search2.Select(x => x.SysGroupSN)); }

                    System.FormattableString sql = $@"
                    select G.[SysGroupSN]
                          ,G.[GroupName]
                          ,G.[Description]
                          ,G.[IsEnable]
                          ,IIF(G.[CanDelete] = '0' , '0' ,IIF(UG.UGcount > 0 , '0' , '1')) as [CanDelete]
                          ,G.[ProcessUserID]
                          ,G.[ProcessDate]
                          ,G.[ProcessIPAddress]
                          ,G.[CreatedUserID]
                          ,G.[CreatedDate]
                          ,G.[SortOrder],isnull(GS.GScount,0) as SectionCount
	                      ,isnull(UG.UGcount,0) as UsersCount
                    from [SysGroup] G
                    left join  (
                        select [SysGroupSN] , sum(1) as GScount
                        from [AuthSysGroupSysSection]
                        group by [SysGroupSN]
                    ) GS on G.SysGroupSN = GS.SysGroupSN
                    left join (
                        select [SysGroupSN] , sum(1) as UGcount
                        from [RelSysUserGroup]
                        group by [SysGroupSN]
                    ) UG on G.SysGroupSN = UG.SysGroupSN
                    ";

                    var list = db.GroupModels.FromSqlInterpolated(sql).Where(x =>
                      (!string.IsNullOrWhiteSpace(key) ? x.GroupName.Contains(key) : 1 == 1)
                    && (!string.IsNullOrWhiteSpace(states) ? x.IsEnable == states : 1 == 1)
                    && ((!string.IsNullOrWhiteSpace(keyword) || !string.IsNullOrWhiteSpace(dep) || !string.IsNullOrWhiteSpace(sec)) ? groupList.Contains(x.SysGroupSN) : 1 == 1)
                    );

                    var all0Data = list.Count();
                    pager.TotalCount = all0Data;
                    pager.PageIndex = pager.p - 1;
                    sortData =  list.Select(x => x.SortOrder.Value).ToList();
                    //可以下ORDER BY 條件
                    var searchData0 = new List<GroupModel>();
                    if (!string.IsNullOrWhiteSpace(sortTitle) && !string.IsNullOrWhiteSpace(sortType))
                    {
                        searchData0 = list.OrderBy($"{sortTitle} {sortType}").Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    }
                    else {
                        searchData0 = list.OrderBy(x => x.SortOrder).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    }
                    return searchData0;
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
                    SourceTable = "SysGroup",
                    Action = "GetGroupList",
                    Controller = "GroupManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return null;
        }

        

        /// <summary>
        /// 修改群組狀態(啟用/停用)
        /// </summary>
        /// <param name="SysGroupSN"></param>
        public static void SetGroupAbility(int SysGroupSN,out string IsEnable)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var deleteData = db.SysGroup.FirstOrDefault(x => x.SysGroupSN == SysGroupSN);
                    deleteData.IsEnable = (deleteData.IsEnable == "0" ? "1" : "0");
                    IsEnable = deleteData.IsEnable;
                    db.SysGroup.Update(deleteData);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    IsEnable = "0";
                }
            }
        }

        /// <summary>
        /// 刪除群組
        /// </summary>
        /// <param name="SysGroupSN"></param>
        public static void DeleteGroup(int SysGroupSN)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var deleteData = db.SysGroup.FirstOrDefault(x => x.SysGroupSN == SysGroupSN);
                    db.SysGroup.Remove(deleteData);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                }
            }
        }

        public static sysGroupModel CreateUser(SysGroup group)
        {
            sysGroupModel sysGroupModel = new sysGroupModel();
            try
            {
                using (var db = new MODAContext())
                {
                    if (db.SysGroup.FirstOrDefault(x => x.GroupName == group.GroupName.Trim()) != null)
                    {
                        sysGroupModel.check = false;
                        sysGroupModel.message = "群組名稱已存在";
                        return sysGroupModel;
                    }
                    group.SortOrder = db.SysGroup?.Max(x=>x.SortOrder) + 1;
                    db.SysGroup.Add(group);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                sysGroupModel.check = false;
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = group.ProcessIPAddress,
                    UserID = group.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Insert",
                    SourceTable = "SysGroup",
                    Action = "CreateUser",
                    Controller = "GroupManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return sysGroupModel;
        }

        /// <summary>
        /// 搜尋群組資料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static SysGroup GetSysGroup(string key)
        {
            try
            {
                var _key = int.Parse(key);
                using (var db = new MODAContext())
                {
                    return db.SysGroup.FirstOrDefault(x => x.SysGroupSN == _key);
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 群組列表資料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<GroupSectionByGroupModel> GetSysSectionList(string key)
        {

            using (var db = new MODAContext())
            {
                try
                {
                    var _key = int.Parse(key);
                    var list = (from a in db.SysSection
                                join b in db.AuthSysGroupSysSection.Where(b => b.SysGroupSN == _key) on a.SysSectionSN equals b.SysSectionSN into ps
                                from o in ps.DefaultIfEmpty()
                                where a.IsEnable == "1"
                                select new GroupSectionByGroupModel
                                {
                                    ActionPath = a.ActionPath,
                                    haveAuthorization = o != null ? true : false,
                                    WebSiteID = o != null ? o.WebSiteID : "",
                                    Title = a.Title,
                                    SysSectionSN = a.SysSectionSN,
                                    ParentSN = a.ParentSN,
                                    Icon = a.Icon,
                                    SortOrder = a.SortOrder,
                                    CreatedDate = null,
                                    CreatedUserID = "",
                                    Description = "",
                                    IsEnable = "",
                                    Parameter = "",
                                    Path = "",
                                    ProcessDate = null,
                                    ProcessIPAddress = "",
                                    ProcessUserID = ""
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
        public static List<SysSection> GetAllSysSection(int key)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    return db.SysSection.Where(x => x.ParentSN == key).OrderBy(x => x.SortOrder).ToList();
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 取得站台列表
        /// </summary>
        /// <returns></returns>
        public static List<SysWebSite> GetAllSysWebSites()
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysWebSite.Where(x => x.IsEnable == "1").OrderBy(x => x.SortOrder).ToList();
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    return null;
                }
            }
        }
        /// <summary>
        /// 更新MENU
        /// </summary>
        /// <param name="data"></param>
        /// <param name="SysGroupSN"></param>
        public static void UpdateSysGroupAccess(List<GroupSectionByGroupModel> data, int SysGroupSN, string UserID)
        {
            //Delete
            var de = data.Select(x => x.SysSectionSN).ToList();
            //Insert
            var inData = data.Where(x => x.haveAuthorization).ToList();
            using (var db = new MODAContext())
            {
                try
                {
                    if (data.Count() > 0)
                    {
                        var deleteData = db.AuthSysGroupSysSection.Where(x => x.SysGroupSN == SysGroupSN).ToList();
                        if (deleteData.Count() > 0)
                        {
                            db.AuthSysGroupSysSection.RemoveRange(deleteData);
                        }
                    }
                    if (inData.Count() > 0)
                    {
                        var list = inData.Select(x => new AuthSysGroupSysSection()
                        {
                            CreatedDate = DateTime.UtcNow.AddHours(8),
                            CreatedUserID = UserID,
                            SysGroupSN = SysGroupSN,
                            WebSiteID = x.WebSiteID,
                            SysSectionSN = x.SysSectionSN
                        });
                        db.AuthSysGroupSysSection.AddRange(list);
                    }
                    db.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
        }
        public static sysGroupModel Edit(SysGroup group)
        {
            sysGroupModel sysGroupModel = new sysGroupModel();
            try
            {
                using (var db = new MODAContext())
                {
                    var oldData = db.SysGroup.FirstOrDefault(x => x.SysGroupSN == group.SysGroupSN);
                    if (oldData != null)
                    {
                        oldData.GroupName = group.GroupName;
                        oldData.Description = group.Description;
                        oldData.IsEnable = group.IsEnable;
                        oldData.ProcessIPAddress = group.ProcessIPAddress;
                        oldData.ProcessUserID = group.ProcessUserID;
                        oldData.ProcessDate = DateTime.UtcNow.AddHours(8);
                        db.SysGroup.Update(oldData);
                        db.SaveChanges();
                        sysGroupModel.check = true;
                        sysGroupModel.sysGroup = oldData;
                    }
                }

            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                sysGroupModel.check = false;
                sysGroupModel.message = "修改失敗";
            }
            return sysGroupModel;
        }

        public static List<EditModel.GroupUser> GetGroupUsers(string key, ref DefaultPager pager)
        {
            var _key = int.Parse(key);
            using (var db = new MODAContext())
            {
                var lsit = (from m in db.RelSysUserGroup
                            join d in db.vw_UserLeftDep on m.UserID equals d.UserID into ps
                            from o in ps.DefaultIfEmpty()
                            where m.SysGroupSN == _key
                            select new EditModel.GroupUser()
                            {
                                UserID = o.UserID,
                                UserName = o.UserName,
                                DepartmentName = o.DepartmentName,
                                JobTitle = o.JobTitle,
                                RelSysGroupUserSN = m.RelSysGroupUserSN,
                                CreatedDate = m.CreatedDate
                            });
                var allData = lsit.Count();
                pager.TotalCount = allData;
                pager.PageIndex = pager.p - 1;
                try
                {
                    var searchData = lsit.OrderByDescending(m => m.CreatedDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                    return searchData;
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
                        SourceTable = "SysGroup",
                        Action = "GetGroupUsers",
                        Controller = "GroupManagementService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                    return null;
                    throw;
                }
            }

        }
        /// <summary>
        /// 新增群組中的使用者
        /// </summary>
        /// <param name="userGroup"></param>
        public static void CreateSysUserGroup(RelSysUserGroup userGroup)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    db.RelSysUserGroup.Add(userGroup);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 刪除群組中的使用者
        /// </summary>
        /// <param name="userGroup"></param>
        public static void DeleteSysUserGroup(RelSysUserGroup userGroup)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var deleteData = db.RelSysUserGroup.FirstOrDefault(x => x.RelSysGroupUserSN == userGroup.RelSysGroupUserSN);
                    if (deleteData != null)
                    {
                        db.RelSysUserGroup.Remove(deleteData);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 刪除某群群組中的所有使用者
        /// </summary>
        /// <param name="userGroup"></param>
        public static void DeleteSysUserGroups(int grpid)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var deleteData = db.RelSysUserGroup.Where(x => x.SysGroupSN == grpid);
                    db.RelSysUserGroup.RemoveRange(deleteData);
                    db.SaveChanges();
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
                    Action2 = "Delete",
                    SourceTable = "RelSysUserGroup",
                    Action = "DeleteSysUserGroups",
                    Controller = "GroupManagementService",
                    SourceSN = grpid,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
        }

        /// <summary>
        /// 刪除某群組中的所有單元維護權限
        /// </summary>
        /// <param name="userGroup"></param>
        public static void DeleteAuthSysGroupWebLevels(int grpid)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var deleteData = db.AuthSysGroupWebLevel.Where(x => x.SysGroupSN == grpid);
                    db.AuthSysGroupWebLevel.RemoveRange(deleteData);
                    db.SaveChanges();
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
                    Action2 = "Delete",
                    SourceTable = "AuthSysGroupWebLevel",
                    Action = "DeleteAuthSysGroupWebLevels",
                    Controller = "GroupManagementService",
                    SourceSN = grpid,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
        }

        /// <summary>
        /// 刪除某群組中的所有功能維護權限
        /// </summary>
        /// <param name="userGroup"></param>
        public static void DeleteAuthSysGroupSysSections(int grpid)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var deleteData = db.AuthSysGroupSysSection.Where(x => x.SysGroupSN == grpid);
                    db.AuthSysGroupSysSection.RemoveRange(deleteData);
                    db.SaveChanges();
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
                    Action2 = "Delete",
                    SourceTable = "AuthSysGroupSysSection",
                    Action = "DeleteAuthSysGroupSysSections",
                    Controller = "GroupManagementService",
                    SourceSN = grpid,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
        }


        /// <summary>
        /// 群組重新排序
        /// </summary>
        /// <param name="key">SysGroupSN</param>
        /// <param name="sort">欲調整至序號</param>
        /// <param name="ProcessUserID"></param>
        /// <param name="ProcessIP"></param>
        public static void GroupReArrange(int key, int sort, string ProcessUserID, string ProcessIP)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Tree = from m in db.SysGroup
                               select m;
                    int originalSort = (from t in Tree
                                        where t.SysGroupSN == key
                                        select t.SortOrder).FirstOrDefault() ?? 0;
                    foreach (var item in Tree)
                    {
                        if (sort < originalSort)
                        {
                            if (item.SortOrder >= sort)
                                item.SortOrder += 1;
                            if (item.SysGroupSN == key)
                                item.SortOrder = sort;
                        }
                        else
                        {
                            if (item.SortOrder > sort)
                                item.SortOrder += 1;
                            if (item.SysGroupSN == key)
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
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 使用者權限
        /// </summary>
        /// <param name="AuthType"></param>
        /// <param name="WebSiteID"></param>
        /// <returns></returns>
        public static DataTable GetUserAuthList(string AuthType,string WebSiteID,int WebLevelSN)
        {
            DataTable data = new DataTable();

            using (var db = new MODAContext())
            {
                using (var connection = db.Database.GetDbConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT Auth.SysGroupSN,Us.UserID,Auth.WebSiteID,Us.Email,Us.DepartmentID,Us.Email,Us.UserSatus 
                                                FROM AuthSysGroupWebLevel AS Auth
                                                JOIN RelSysUserGroup AS Rel ON Auth.SysGroupSN = Rel.SysGroupSN
                                                JOIN SysUser AS Us ON Rel.UserID = Us.UserID
                                                WHERE Auth.WebSiteID = @WebSiteID AND Auth.AuthType = @AuthType
                                                AND Auth.WebLevelSN = @WebLevelSN AND Us.UserSatus = '1'";

                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@WebSiteID", WebSiteID));
                        command.Parameters.Add(new SqlParameter("@AuthType", AuthType));
                        command.Parameters.Add(new SqlParameter("@WebLevelSN", WebLevelSN));

                        using (var reader = command.ExecuteReader())
                        {
                            data.Load(reader);
                            reader.Close();
                        }

                    }
                    connection.Close();
                }

            }
            return data;
        }

        /// <summary>
        /// 群組權限查詢
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static RelSysUserGroup GetSysUserGroup (string UserID,int Key)
        {
            using (var db = new MODAContext())
            {
                return db.RelSysUserGroup.Where(x => x.UserID == UserID && x.SysGroupSN == Key).FirstOrDefault();
            }
        }
    }
}

using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Models.Authorization;

namespace Services.Authorization
{
    public class AccessManagmentService
    {

        /// <summary>
        /// 需要重新命名 匯出報表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<UserGroupSysSectionModel> GetExcel1(UserGroupSysSectionModel data)
        {
            var where = "";
            if (data != null)
            {
                using (var db = new MODAContext())
                {
                    string sql = $@"
SELECT 
u.UserId , 
u.UserName , 
g.GroupName, g.IsEnable as GroupIsEnable, 
Cast(g.SysGroupSn as varchar(50)) as GroupSN,
ss.Title  as SectionTitle , 
Cast(ss.SysSectionSN as varchar(50)) as SectionSN , 
d.DepartmentID,
d.DepartmentName,
u.UserSatus as IsEnable,WebLevelTitle='', LevelPath='',
ModulePath='', AtricPath='', AuthPath=''
 FROM [dbo].[SysUser]　u
LEFT join [dbo].[SysDepartment]　d on u.DepartmentID = d.DepartmentID
LEFT join [dbo].[RelSysUserGroup]　ug on u.UserId = ug.UserId
LEFT join [dbo].[SysGroup]　g on ug.SysGroupSn = g.SysGroupSn
LEFT JOIN [dbo].[AuthSysGroupSysSection]　asgss  on g.SysGroupSn = asgss.SysGroupSn
LEFT JOIN [SysSection] ss on asgss.SysSectionSN = ss.SysSectionSN
WHERE 1=1  ";
                    List<SqlParameter> sqlParams = new List<SqlParameter>();
                    if (!string.IsNullOrWhiteSpace(data.UserID))
                    {
                        where += $@" and ( u.UserId like '%' + @UserID + '%' or u.UserName like '%' + @UserID + '%' ) ";
                        sqlParams.Add(new SqlParameter("@UserID", data.UserID));

                    }
                    if (!string.IsNullOrWhiteSpace(data.DepartmentID))
                    {
                        where += $@" and u.DepartmentID = @DepartmentID";
                        sqlParams.Add(new SqlParameter("@DepartmentID", data.DepartmentID));
                    }
                    if (!string.IsNullOrWhiteSpace(data.GroupSN))
                    {
                        where += $@" and g.SysGroupSn = @SysGroupSn";
                        sqlParams.Add(new SqlParameter("@SysGroupSn", data.GroupSN));
                    }
                    sql += where;
                    sql += " and g.IsEnable<>'-99'";
                    sql += " order by u.UserID , DepartmentName";

                    try
                    {
                        var rtn = db.UserGroupSysSectionModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                        return rtn;
                    }
                    catch (System.Exception ex)
                    {
                        LogService.CreateLogAction(new LogAction()
                        {
                            Status = "0",
                            MessageResult = ex.ToString(),
                            ProcessIPAddress = "",
                            UserID = data.UserID,
                            WebSiteID = "",
                            WebPath = "",
                            ActionType = "1",
                            Action2 = "select",
                            SourceTable = "UserGroupSysSection",
                            Action = "GetExcel1",
                            Controller = "AccessManagment",
                            SourceSN = 0,
                            CreatedDate = DateTime.UtcNow.AddHours(8)
                        });
                        Utility.Mail.Error(ex.ToString());
                        return null;
                    }

                }
            }
            return null;
        }

        public static List<UserGroupSysSectionModel> GetExcel2(UserGroupSysSectionModel data)
        {
            var where = "";
            if (data != null)
            {
                using (var db = new MODAContext())
                {
                    string sql = $@"
SELECT 
u.UserId , 
u.UserName , 
g.GroupName, g.IsEnable as GroupIsEnable, 
Cast(g.SysGroupSn as varchar(50)) as GroupSN,
ss.Title  as SectionTitle , 
Cast(ss.SysSectionSN as varchar(50)) as SectionSN , 
d.DepartmentID,
d.DepartmentName,
u.UserSatus as IsEnable,
access.Title as WebLevelTitle, access.LevelPath,
ISNULL(ModulePath,'') as ModulePath, ISNULL(ArticlePath,'') as AtricPath, ISNULL(AuthPath,'') as AuthPath
 FROM [dbo].[SysUser]　u
LEFT join [dbo].[SysDepartment]　d on u.DepartmentID = d.DepartmentID
LEFT join [dbo].[RelSysUserGroup]　ug on u.UserId = ug.UserId
LEFT join [dbo].[SysGroup]　g on ug.SysGroupSn = g.SysGroupSn
LEFT JOIN [dbo].[AuthSysGroupSysSection]　asgss  on g.SysGroupSn = asgss.SysGroupSn
LEFT JOIN [SysSection] ss on asgss.SysSectionSN = ss.SysSectionSN
LEFT JOIN 
(
	select 
	case when Lv1.WebLevelSN is not Null
		then Lv1.WebLevelSN else Auth.WebLevelSN end as WebLevelSN,
		case when Lv1.Title is not Null
		then Lv1.Title else Auth.Title end as Title,
		case when Lv1.SysGroupSN is not Null
		then Lv1.SysGroupSN else Auth.SysGroupSN end as SysGroupSN,
		case when Lv1.SysSectionSN is not Null
		then Lv1.SysSectionSN else Auth.SysSectionSN end as SysSectionSN,
		case when ModulePath is not null
		then ModulePath
		when ArticlePath is not null then ArticlePath
		else
		Auth.Path
		end LevelPath,
		ModulePath, ArticlePath, Auth.Path as AuthPath
	from
	(
	select 
		case when Module.WebLevelSN is not Null
		then Module.WebLevelSN else Article.WebLevelSN end as WebLevelSN,
		case when Module.Title is not Null
		then Module.Title else Article.Title end as Title,
		case when Module.SysGroupSN is not Null
		then Module.SysGroupSN else Article.SysGroupSN end as SysGroupSN,
		case when Module.SysSectionSN is not Null
		then Module.SysSectionSN else Article.SysSectionSN end as SysSectionSN,
		Module.Path as ModulePath, Article.Path as ArticlePath
		from
		(
			select a.WebLevelSN, a.SysGroupSN, b.Title, b.Path, c.SysSectionSN from AuthSysGroupWebLevel as a
			inner join [vw_WebLevelTree] as b
			on a.WebLevelSN = b.WebLevelSN	and AuthType='Module' 
			inner join AuthSysGroupSysSection as c
			on a.SysGroupSN = c.SysGroupSN
			where ((c.SysSectionSN=6 and (path like '全球資訊網%' or Path like '網站單元維護%'))
			   or (c.SysSectionSN=7 and path like '首頁區塊維護%')
			   or (c.SysSectionSN=22 and path like '頁首頁底區塊維護%'))

		) as Module
		full join
		(
			select a.WebLevelSN, a.SysGroupSN, b.Title, b.Path, c.SysSectionSN from AuthSysGroupWebLevel as a
			inner join [vw_WebLevelTree] as b
			on a.WebLevelSN = b.WebLevelSN and  AuthType='Article' 
			inner join AuthSysGroupSysSection as c
			on a.SysGroupSN = c.SysGroupSN
			where ((c.SysSectionSN=6 and (path like '全球資訊網%' or Path like '網站單元維護%'))
			   or (c.SysSectionSN=7 and path like '首頁區塊維護%')
			   or (c.SysSectionSN=22 and path like '頁首頁底區塊維護%'))

		) as Article
		on Module.WebLevelSN = Article.WebLevelSN and Module.SysGroupSN = Article.SysGroupSN and Module.SysSectionSN= Article.SysSectionSN
	) as Lv1
	full join
	(
		select a.WebLevelSN, a.SysGroupSN, b.Title, b.Path, c.SysSectionSN from AuthSysGroupWebLevel as a
		inner join [vw_WebLevelTree] as b
		on a.WebLevelSN = b.WebLevelSN and AuthType='Auth' 
		inner join AuthSysGroupSysSection as c
		on a.SysGroupSN = c.SysGroupSN
		where ((c.SysSectionSN=6 and (path like '全球資訊網%' or Path like '網站單元維護%'))
		   or (c.SysSectionSN=7 and path like '首頁區塊維護%')
		   or (c.SysSectionSN=22 and path like '頁首頁底區塊維護%'))

	) as Auth
	on  Lv1.WebLevelSN = Auth.WebLevelSN and Lv1.SysGroupSN = Auth.SysGroupSN and Lv1.SysSectionSN=Auth.SysSectionSN
)  AS Access
on  Access.SysSectionSN =ss.SysSectionSN and asgss.SysGroupSN=Access.SysGroupSN and asgss.SysSectionSN in (6,7,22) 
WHERE 1=1  ";
                    List<SqlParameter> sqlParams = new List<SqlParameter>();
                    if (!string.IsNullOrWhiteSpace(data.UserID))
                    {
                        where += $@" and ( u.UserId like '%' + @UserID + '%' or u.UserName like '%' + @UserID + '%' ) ";
                        sqlParams.Add(new SqlParameter("@UserID", data.UserID));

                    }
                    if (!string.IsNullOrWhiteSpace(data.DepartmentID))
                    {
                        where += $@" and u.DepartmentID = @DepartmentID";
                        sqlParams.Add(new SqlParameter("@DepartmentID", data.DepartmentID));
                    }
                    if (!string.IsNullOrWhiteSpace(data.GroupSN))
                    {
                        where += $@" and g.SysGroupSn = @SysGroupSn";
                        sqlParams.Add(new SqlParameter("@SysGroupSn", data.GroupSN));
                    }
                    sql += where;
                    sql += " and g.IsEnable<>'-99'";
                    try
                    {
                        var rtn = db.UserGroupSysSectionModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                        return rtn;
                    }
                    catch (System.Exception ex)
                    {
                        Utility.Mail.Error(ex.ToString());
                        LogService.CreateLogAction(new LogAction()
                        {
                            Status = "0",
                            MessageResult = ex.ToString(),
                            ProcessIPAddress = "",
                            UserID = data.UserID,
                            WebSiteID = "",
                            WebPath = "",
                            ActionType = "1",
                            Action2 = "select",
                            SourceTable = "UserGroupSysSection",
                            Action = "GetExcel2",
                            Controller = "AccessManagment",
                            SourceSN = 0,
                            CreatedDate = DateTime.UtcNow.AddHours(8)
                        });
                        return null;
                    }
                }
            }
            return null;
        }
    }
}

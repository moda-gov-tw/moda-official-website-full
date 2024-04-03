using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Authorization;
using Services.Models.WebManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace Services.WebManagement
{
    public class OperationStatisticsService
    {
        /// <summary>
        /// 報表
        /// </summary>
        /// <param name="str"></param>
        /// <param name="end"></param>
        /// <param name="type">0- 操作紀錄   1-sysUser, 2- 系統錯誤紀錄(status=0), 3- 使用者操作紀錄(status=1)</param>
        /// <returns></returns>
        public static List<OperationStatisticsModel> GetLogData(string str, string end, int type = 0,string websiteid ="")
        {
            var SourceTable = "";
            var Status = "";
            switch (type)
            {
                case 0:
                    SourceTable = @$" and SourceTable in ('WEBNews' , 'WebLevel' )  and l.Status=1 ";
                    break;
                case 1:
                    SourceTable = @$" and SourceTable not in ('WEBNews' , 'WebLevel'  )  and l.Status=1 ";
                    break;
                case 2:
                    Status = " and l.Status=0";
                    break;
                case 3:
                    Status = " and l.Status=1";
                    break;
            }

            List<SqlParameter> sqlParams = new List<SqlParameter>();
            var sql = @$"
  select 
  u.UserID ,
  u.UserName ,
  d.DepartmentName,
  d.DepartmentID,
  l.ProcessIPAddress,
  Action2 ,
  MessageInput,
  l.SourceTable,
   case when 　l.SourceSN  is null then 0 else l.SourceSN end  SourceSN,
  l.CreatedDate
  from [LogAction] l
  inner join sysUser u on l.UserID =u.UserID
  left join SysDepartment d on u.DepartmentID = d.DepartmentID 
  where Controller !='Home'　
  and l.UserID is not　null 
  and Action2 != ''
 {SourceTable}
 {Status}
  and l.CreatedDate >=@StartDate 
  and l.CreatedDate <= @EndDate
  Order by l.CreatedDate desc
";
            sqlParams.Add(new SqlParameter("@StartDate", str));
            sqlParams.Add(new SqlParameter("@EndDate", end));
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.OperationStatisticsModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    return data;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 有分頁
        /// </summary>
        /// <param name="str"></param>
        /// <param name="end"></param>
        /// <param name="pager"></param>
        /// <param name="type">0- 操作紀錄   1-sysUser, 2- 系統錯誤紀錄(status=0), 3- 使用者操作紀錄(status=1)</param>
        /// <returns></returns>
        public static List<OperationStatisticsModel> GetLogData(string str, string end, ref DefaultPager pager, int type = 0, string userid = "", string ip = "",string websiteid = "",string key ="",string sn = "",string departmentID = "")
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            var whereP = "";
            if (!string.IsNullOrWhiteSpace(userid))
            {
                whereP += @" and u.UserID=@userid   ";
                sqlParams.Add(new SqlParameter("@userid", userid.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(ip))
            {
                whereP += @" and l.ProcessIPAddress=@ip   ";
                sqlParams.Add(new SqlParameter("@ip", ip.Trim()));
            }
            if(!string.IsNullOrWhiteSpace(websiteid))
            {
                whereP += @" and l.websiteid=@websiteid  ";
                sqlParams.Add(new SqlParameter("@websiteid", websiteid.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                whereP += @" and l.WebPath like '%'+ @key +'%'";
                sqlParams.Add(new SqlParameter("@key", key.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(sn))
            {
                whereP += @" and w.WebLevelSN = @sn";
                sqlParams.Add(new SqlParameter("@sn", sn));
            }

            if (!string.IsNullOrWhiteSpace(departmentID))
            {
                whereP += @" and w.DepartmentID = @departmentID";
                sqlParams.Add(new SqlParameter("@departmentID", departmentID));
            }

            var SourceTable = "";
            var Status = "";
            switch (type)
            {
                case 0:
                    SourceTable = @$" and SourceTable in ('WEBNews' , 'WebLevel'  ) and l.Status=1";
                    break;
                case 1:
                    SourceTable = @$" and SourceTable not in ('WEBNews' , 'WebLevel'  ) and l.Status=1";
                    break;
                case 2:
                    Status = " and l.Status=0";
                    break;
                case 3:
                    Status = " and l.Status=1";
                    break;
            }



            var sql = @$"
  select 
  u.UserID ,
  u.UserName ,
  d.DepartmentName,
  case  when　w.DepartmentID  is null then lv.DepartmentID else w.DepartmentID end DepartmentID   ,
  l.ProcessIPAddress,
  Action2 ,
  MessageInput,
  l.SourceTable,
  case when 　l.SourceSN  is null then 0 else l.SourceSN end  SourceSN,
  case when l.SourceTable = 'WebLevel' then l.WebPath + '/' + lv.Title 
       when l.SourceTable = 'WEBNews' then  l.WebPath + '/' + w.Title
       else l.WebPath
  end WebPath,
  l.CreatedDate,
  case when MessageInput like '%zh-tw%' then 'zh-tw'
       when MessageInput like '%en%' then 'en'
	   else 'zh-tw'
	   end Lang
  from [LogAction] l
  inner join sysUser u on l.UserID =u.UserID
  left join WEBNews w on l.SourceSN = w.WEBNewsSN and l.SourceTable = 'WEBNews'
  left join WebLevel lv on l.SourceSN = lv.WebLevelSN  and l.SourceTable = 'WebLevel'
  left join SysDepartment d on u.DepartmentID = d.DepartmentID 
  where Controller !='Home'　
  and l.UserID is not　null 
  and SourceTable is not　null 
  and SourceSN != 0 
  and Action2 != ''
  and d.Lang = 'zh-tw'
 {whereP}
 {SourceTable}
 {Status}
  and l.CreatedDate >=@StartDate 
  and l.CreatedDate <= @EndDate
  Order by l.CreatedDate desc
";
            sqlParams.Add(new SqlParameter("@StartDate", str));
            sqlParams.Add(new SqlParameter("@EndDate", end));


            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.OperationStatisticsModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    var allData = data.Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    return data.OrderByDescending(o => o.CreatedDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
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
                        SourceTable = "LogAction",
                        Action = "getLogData",
                        Controller = "OperationStatisticsService",
                        SourceSN = 0,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    });
                    return null;
                }

            }
        }


        public static List<PathModel> GetWebLevelTree(int WebLevelSN)
        {
            var sql = $@"
with CTE as (
         select T1.WebLevelSN,T1.ParentSN,T1.Title,convert(nvarchar(200), T1.Title) as Path
         from WebLevel T1
         where ParentSN = 0
         union all
         select
         T1.WebLevelSN,T1.ParentSN,T1.Title,convert(nvarchar(200), T2.Path + '/' + T1.Title) as Path
         from WebLevel T1
         join CTE T2 on T1.ParentSN = T2.WebLevelSN
             )
 SELECT * FROM CTE
   where WebLevelSN = @WebLevelSN
";
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@WebLevelSN", WebLevelSN));

            using (var db = new MODAContext())
            {
                try
                {
                    return db.PathModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<OperationStatisticsModel2> GetLogData2(string str, string end, int type = 0, string userid = "", string ip = "", string path = "",string websiteid = "",string key = "",string sn = "",string departmentID = "")
        {
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                var whereP = "";
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    whereP += @" and u.UserID=@userid   ";
                    sqlParams.Add(new SqlParameter("@userid", userid.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    whereP += @" and l.ProcessIPAddress=@ip   ";
                    sqlParams.Add(new SqlParameter("@ip", ip.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(path))
                {
                    whereP += @" and WebPath like '%' + @path  +'%' ";
                    sqlParams.Add(new SqlParameter("@path", path.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(key))
                {
                    whereP += @" and l.WebPath like '%' + @key + '%'";
                    sqlParams.Add(new SqlParameter("@key", key.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(sn))
                {
                    whereP += @" and w.WebLevelSN = @sn OR lv.ParentSN = @sn";
                    sqlParams.Add(new SqlParameter("@sn", sn));
                }

                if (!string.IsNullOrWhiteSpace(departmentID))
                {
                    whereP += @" and w.departmentID = @departmentID";
                    sqlParams.Add(new SqlParameter("@departmentID", departmentID));
                }

                whereP += @" and l.WebSiteID = @WebSiteID";
                sqlParams.Add(new SqlParameter("@WebSiteID", websiteid));

                var SourceTable = "";
                var Status = "";
                switch (type)
                {
                    case 0:
                        SourceTable = @$" and SourceTable in ('WEBNews' , 'WebLevel'  )  and l.Status=1";
                        break;
                    case 1:
                        SourceTable = @$" and SourceTable not in ('WEBNews' , 'WebLevel'  )  and l.Status=1";
                        break;
                    case 2:
                        Status = " and l.Status=0";
                        break;
                    case 3:
                        Status = " and l.Status=1";
                        break;
                }


                var sql = @$"
  
with LogData as  (
  select 
  u.UserID ,
  u.UserName ,
  d.DepartmentName,
  w.DepartmentID,
  l.ProcessIPAddress,
  Action2 ,
  MessageInput,
  MessageResult,
  l.SourceTable,
  case when 　l.SourceSN  is null then 0 else l.SourceSN end  SourceSN,
  case when l.SourceTable = 'WebLevel' then l.WebPath + '/' + lv.Title 
       else l.WebPath
  end WebPath,
  l.CreatedDate,
  case when MessageInput like '%zh-tw%' then 'zh-tw'
       when MessageInput like '%en%' then 'en'
	   else 'zh-tw'
	   end Lang,
  l.WebSiteID,
  w.WebLevelSN as ParentSN
  from [LogAction] l
  inner join sysUser u on l.UserID =u.UserID
  left join WEBNews w on l.SourceSN = w.WEBNewsSN and l.SourceTable = 'WEBNews'
  left join WebLevel lv on l.SourceSN = lv.WebLevelSN  and l.SourceTable = 'WebLevel'
  left join SysDepartment d on u.DepartmentID = d.DepartmentID 
  where Controller !='Home'　
  and l.UserID is not　null 
  and Action2 != ''
  and SourceTable is not　null 
  and SourceSN != 0 
  and l.CreatedDate >=@StartDate 
  and l.CreatedDate <= @EndDate
  and d.Lang = 'zh-tw'
  {whereP}
  {SourceTable}
  {Status}
 )
 


 select
 UserID , 
 UserName ,
 DepartmentName,
 DepartmentID,
 ProcessIPAddress,
 Action2 ,
 MessageInput,
MessageResult,
 SourceTable,
 SourceSN,
 CreatedDate ,
 isnull( WebPath,'') + _1Title+ _2Title+ _3Title+ _4Title+ _5Title+ _6Title+ _7Title+ _8Title as Webpath,
 Lang,
 WebSiteTitle,
 ParentSN ,
 ParentTitle
  from 
 (
 select 
 l.* ,sws.Title as webSiteTitle ,dp.Title as ParentTitle ,

   case when  sysd.DepartmentName is null then '' else '/'+sysd.DepartmentName  end _1Title ,
   case when  sysg.GroupName is null then '' else '/'+sysg.GroupName  end _2Title ,
   case when  u.UserID is null then '' else '/('+u.UserID + ')'+u.UserName end _3Title ,
   case when  d.Title is null then '' else '/'+ d.Title  end _4Title ,
   case when  n.Title is null then '' else '/'+ n.Title  end _5Title ,
   case when  asgs.GroupName is null then '' else '/(選單)'+ asgs.GroupName  end _6Title ,
   case when  asgsM.GroupName is null then '' else '/(權限)'+ asgsM.GroupName  end _7Title ,
   case when  rsg.GroupName is null then '' else '/(人員)'+ rsg.GroupName  end _8Title 
 from LogData l

 left join [dbo].[SysDepartment] sysd on sysd.SysDepartmentSN = l.SourceSN and l.SourceTable='SysDepartment'
 left join dbo.SysGroup sysg on sysg.SysGroupSN = l.SourceSN and l.SourceTable='SysGroup'
 left join SysUser u on u.SysUserSN = l.SourceSN and l.SourceTable='SysUser'
 left join [dbo].[WebLevel] d on  d.WebLevelSN = l.SourceSN and l.SourceTable='WebLevel'
 left join [dbo].[WEBNews] n on n.WEBNewsSN  = l.SourceSN and l.SourceTable='WEBNews'
 left join dbo.SysGroup asgs on asgs.SysGroupSN = l.SourceSN and l.SourceTable='AuthSysGroupSysSection'  --注意這邊關係(選單)
 left join dbo.SysGroup asgsM on asgsM.SysGroupSN = l.SourceSN and l.SourceTable='AuthSysGroupWebLevel'  --注意這邊關係(權限)
 left join [dbo].[SysGroup] rsg on rsg.SysGroupSN = l.SourceSN and l.SourceTable='RelSysUserGroup'  --注意這邊關係(人員)
 inner join SysWebSite  sws on sws.WebSiteID = l.WebSiteID 
 left join [dbo].[WebLevel] dp on  l.SourceTable='WebLevel' and d.ParentSN = dp.WebLevelSN
 ) a
";
                sqlParams.Add(new SqlParameter("@StartDate", str));
                sqlParams.Add(new SqlParameter("@EndDate", end));
                using (var db = new MODAContext())
                {
                    var data = db.OperationStatisticsModel2s.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    return data;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 統計更新數
        /// </summary>
        /// <param name="str"></param>
        /// <param name="end"></param>
        /// <param name="type"></param>
        /// <param name="userid"></param>
        /// <param name="ip"></param>
        /// <param name="path"></param>
        /// <param name="websiteid"></param>
        /// <param name="key"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public static List<OperationStatisticsModel3> GetLogData3(string str, string end, int type = 0, string userid = "", string ip = "", string path = "", string websiteid = "", string key = "", string sn = "",string departmentID ="")
        {
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                var whereP = "";
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    whereP += @" and u.UserID=@userid   ";
                    sqlParams.Add(new SqlParameter("@userid", userid.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(ip))
                {
                    whereP += @" and l.ProcessIPAddress=@ip   ";
                    sqlParams.Add(new SqlParameter("@ip", ip.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(path))
                {
                    whereP += @" and WebPath like '%' + @path  +'%' ";
                    sqlParams.Add(new SqlParameter("@path", path.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(key))
                {
                    whereP += @" and l.WebPath like '%' + @key + '%'";
                    sqlParams.Add(new SqlParameter("@key", key.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(sn))
                {
                    whereP += @" and w.WebLevelSN = @sn OR lv.ParentSN = @sn";
                    sqlParams.Add(new SqlParameter("@sn", sn));
                }
                if (!string.IsNullOrWhiteSpace(departmentID))
                {
                    whereP += @" and w.DepartmentID = @departmentID";
                    sqlParams.Add(new SqlParameter("@departmentID", departmentID));
                }

                whereP += @" and l.WebSiteID = @WebSiteID";
                sqlParams.Add(new SqlParameter("@WebSiteID", websiteid));

                var SourceTable = "";
                var Status = "";
                switch (type)
                {
                    case 0:
                        SourceTable = @$" and SourceTable in ('WEBNews' , 'WebLevel'  )  and l.Status=1";
                        break;
                    case 1:
                        SourceTable = @$" and SourceTable not in ('WEBNews' , 'WebLevel'  )  and l.Status=1";
                        break;
                    case 2:
                        Status = " and l.Status=0";
                        break;
                    case 3:
                        Status = " and l.Status=1"; break;
                    case 4:
                        Status = @$" and SourceTable in ('WEBNews'  )  and l.Status=1";
                        break;
                }


                var sql = @$"
  
with LogData as  (
  select 
  u.UserID ,
  u.UserName ,
  d.DepartmentName,
  case when l.SourceTable = 'WebLevel' then lv.DepartmentID 
       when l.SourceTable = 'WEBNews' then  w.DepartmentID
       end DepartmentID,
  case when w.Title is null then　SUBSTRING(l.WebPath,1,CHARINDEX('/',l.WebPath)-1)　else w.Title end Title,
  l.ProcessIPAddress,
  Action2 ,
  MessageInput,
  MessageResult,
  l.SourceTable,
  case when 　l.SourceSN  is null then 0 else l.SourceSN end  SourceSN,
  case when l.SourceTable = 'WebLevel' then l.WebPath 
       when l.SourceTable = 'WEBNews' then  l.WebPath
       else l.WebPath
  end WebPath,
  l.CreatedDate,
  case when MessageInput like '%zh-tw%' then 'zh-tw'
       when MessageInput like '%en%' then 'en'
	   else 'zh-tw'
	   end Lang,
  l.WebSiteID
  from [LogAction] l
  inner join sysUser u on l.UserID =u.UserID
  left join WEBNews w on l.SourceSN = w.WEBNewsSN and l.SourceTable = 'WEBNews'
  left join WebLevel lv on l.SourceSN = lv.WebLevelSN  and l.SourceTable = 'WebLevel'
  left join SysDepartment d on u.DepartmentID = d.DepartmentID 
  where Controller !='Home'　
  and l.UserID is not　null 
  and Action2 != ''
  and SourceTable is not　null 
  and SourceSN != 0 
  and l.CreatedDate >=@StartDate 
  and l.CreatedDate <= @EndDate
  and d.Lang = 'zh-tw'
  {whereP}
  {SourceTable}
  {Status}
 )
 

select UserID,UserName,DepartmentName,DepartmentID,SourceTable,SourceSN,MIN(CreatedDate) as StrDate,MAX(CreatedDate) as EndDate,Webpath,Lang,WebSiteID,Title,
SUM(CASE WHEN Action2 = 'insert' THEN 1 ELSE 0 END) as InsertSUM,SUM(CASE WHEN Action2 = 'update' THEN 1 ELSE 0 END) as UpdateSUM,SUM(CASE WHEN Action2 = 'delete' THEN 1 ELSE 0 END) as DeleteSUM
from
(
 select
 UserID , 
 UserName ,
 DepartmentName,
 DepartmentID,
 Title,
 ProcessIPAddress,
 Action2 ,
 MessageInput,
MessageResult,
 SourceTable,
 SourceSN,
 CreatedDate ,
 isnull( WebPath,'') + _1Title+ _2Title+ _3Title+ _4Title+ _5Title+ _6Title+ _7Title+ _8Title as Webpath,
 Lang,
 WebSiteID
  from 
 (
 select 
 l.* ,

   case when  sysd.DepartmentName is null then '' else '/'+sysd.DepartmentName  end _1Title ,
   case when  sysg.GroupName is null then '' else '/'+sysg.GroupName  end _2Title ,
   case when  u.UserID is null then '' else '/('+u.UserID + ')'+u.UserName end _3Title ,
   case when  d.Title is null then '' else '/'+ d.Title  end _4Title ,
   case when  n.Title is null then '' else '/'+ n.Title  end _5Title ,
   case when  asgs.GroupName is null then '' else '/(選單)'+ asgs.GroupName  end _6Title ,
   case when  asgsM.GroupName is null then '' else '/(權限)'+ asgsM.GroupName  end _7Title ,
   case when  rsg.GroupName is null then '' else '/(人員)'+ rsg.GroupName  end _8Title 
 from LogData l

 left join [dbo].[SysDepartment] sysd on sysd.SysDepartmentSN = l.SourceSN and l.SourceTable='SysDepartment'
 left join dbo.SysGroup sysg on sysg.SysGroupSN = l.SourceSN and l.SourceTable='SysGroup'
 left join SysUser u on u.SysUserSN = l.SourceSN and l.SourceTable='SysUser'
 left join [dbo].[WebLevel] d on  d.WebLevelSN = l.SourceSN and l.SourceTable='WebLevel'
 left join [dbo].[WEBNews] n on n.WEBNewsSN  = l.SourceSN and l.SourceTable='WEBNews'
 left join dbo.SysGroup asgs on asgs.SysGroupSN = l.SourceSN and l.SourceTable='AuthSysGroupSysSection'  --注意這邊關係(選單)
 left join dbo.SysGroup asgsM on asgsM.SysGroupSN = l.SourceSN and l.SourceTable='AuthSysGroupWebLevel'  --注意這邊關係(權限)
 left join [dbo].[SysGroup] rsg on rsg.SysGroupSN = l.SourceSN and l.SourceTable='RelSysUserGroup'  --注意這邊關係(人員)
 ) a
 ) b
GROUP BY 
 b.UserID
,b.UserName
,b.DepartmentName
,b.DepartmentID
,b.SourceTable
,b.SourceSN
,b.Webpath
,b.Lang
,b.WebSiteID
,b.Title
";
                sqlParams.Add(new SqlParameter("@StartDate", str));
                sqlParams.Add(new SqlParameter("@EndDate", end));
                using (var db = new MODAContext())
                {
                    var data = db.OperationStatisticsModel3s.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    return data;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 統計建立單位跟發布單位不同
        /// </summary>
        public static List<OperationStatisticsModel4> GetLogData4(string str, string end , string websiteid)
        {
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                sqlParams.Add(new SqlParameter("@StartDate", str));
                sqlParams.Add(new SqlParameter("@EndDate", end));
                sqlParams.Add(new SqlParameter("@WebSiteID", websiteid));
                var sql = @"
SELECT　s.Title　as WebSiteTitle , s.SortOrder, d1.DepartmentName as CreateDepName　, d2.DepartmentName as ReleaseDepName 　, count(*)　as HelpOtherCount
FROM WEBNews n
inner join SysUser  u on n.CreatedUserID =  u .UserID
inner join SysDepartment d1 on u.DepartmentID = d1.DepartmentID and d1.Lang='zh-tw'
inner join SysDepartment d2 on  SUBSTRING( n.DepartmentID,1,3 ) = d2.DepartmentID　and d2.Lang='zh-tw'
inner join SysWebSite s on n.WebSiteID = s.WebSiteID
where 
n.Lang='zh-tw'
and n.WebSiteID = @WebSiteID
and n.CreatedDate >=@StartDate 
and n.CreatedDate <= @EndDate
and n.DepartmentID != d1.DepartmentID
and n.IsEnable !='-99'  
Group by s.Title　,　s.SortOrder ,d1.DepartmentName　 , d2.DepartmentName 
order by s.SortOrder , CreateDepName ,HelpOtherCount desc ,ReleaseDepName 
";
                using (var db = new MODAContext())
                {
                    var data = db.OperationStatisticsModel4s.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    return data;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static OperationStatisticsModel GetLogByPrevious(string datetime,int sn, string tablename= "")
        {
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();

                var sql = @"SELECT A.LogActionSN,A.UserID,A.WebSiteID,A.Controller,A.Action,A.ActionType,A.Status,A.SourceTable
                                  ,A.SourceSN,A.Action2,A.MessageInput,A.MessageResult,A.WebPath,A.DepartmentID
                                  ,A.CreatedDate,A.ProcessIPAddress,'' AS Lang,'' AS DepartmentName, '' AS UserName FROM (
                                   SELECT * FROM (
                                   SELECT TOP 1 * FROM LogAction
                                   WHERE CreatedDate < @Sd AND SourceSN = @SourceSN1 AND SourceTable = @SourceTable1
                                   ORDER BY CreatedDate  desc) Previous
                                   UNION
                                   SELECT * FROM (
                                   SELECT TOP 1 * FROM LogAction
                                   WHERE CreatedDate > @Ed AND SourceSN = @SourceSN2 AND SourceTable = @SourceTable2
                                   ORDER BY CreatedDate  desc) Next) A
                                   WHERE CreatedDate < @d";

                sqlParams.Add(new SqlParameter("@SourceSN1", sn));
                sqlParams.Add(new SqlParameter("@SourceSN2", sn));
                sqlParams.Add(new SqlParameter("@SourceTable1", tablename));
                sqlParams.Add(new SqlParameter("@SourceTable2", tablename));
                sqlParams.Add(new SqlParameter("@Sd", datetime));
                sqlParams.Add(new SqlParameter("@Ed", datetime));
                sqlParams.Add(new SqlParameter("@d", datetime));

                using (var db = new MODAContext())
                {
                    var data = db.OperationStatisticsModels.FromSqlRaw(sql, sqlParams.ToArray()).FirstOrDefault();
                    return data;
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
                    SourceTable = "LogAction",
                    Action = "getLogData",
                    Controller = "OperationStatisticsService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return null;
            }

        }
    }
}

using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Models;
using Services.Models.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using static Utility.Files;

namespace Services.WebManagement
{
    public class BrowseStatisticsService
    {

        public static List<BrowseStatisticsModel> GetFilesCount(  string WebSiteID, string str, string end, string key, ref DefaultPager pager)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            var whereP = "";
            if (!string.IsNullOrWhiteSpace(key))
            {
                whereP += @" and F.FileTitle like '%' + @key  + '%' ";
                sqlParams.Add(new SqlParameter("@key", key.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(WebSiteID))
            {
                whereP += @" and W.WebSiteID=@websiteid  ";
                sqlParams.Add(new SqlParameter("@websiteid", WebSiteID.Trim()));
            }

            var sql = @$"with CTE (WebLevelSN,ParentSN,Title,Lang,IsEnable,Lvl)
                        as(
                           select WebLevelSN,ParentSN,Title,Lang,IsEnable,1 as lvl
                           from WebLevel AS A 
						   WHERE A.IsEnable != '-99' AND A.ParentSN = 0
                           union all
                           select a.WebLevelSN,a.ParentSN,CAST(b.Title + ' / ' + a.Title AS nvarchar(200)) as Title,A.Lang,b.IsEnable,lvl+1
                           from WebLevel a,CTE b
                           where b.WebLevelSN = a.ParentSN AND A.IsEnable != '-99'
                          )

                        select  w.WebSiteID,F.FileTitle,F.OriginalFileName,CAST(CTE.Title + ' / ' + w.Title AS nvarchar(200)) AS Path,SUM(DL.DownLoads) AS DownLoadCount,w.Lang
                        from CTE
						LEFT JOIN WEBNews AS W ON CTE.WebLevelSN = W.WebLevelSN
						JOIN RelWebFileContent AS rl on w.WEBNewsSN = rl.SourceSN
						LEFT JOIN WebLevel l ON rl.SourceTable = 'WebLevel' AND l.WebLevelSN = rl.SourceSN
						JOIN WEBFile AS F ON rl.WEBFileSN = F.WEBFileSN
                        JOIN WebFileDownLoads DL ON DL.WEBFileSN = F.WEBFileSN AND DL.RelWebFileContentSN = rl.RelWebFileContentSN
						WHERE 1 = 1
                        {whereP}
                        AND  DL.CreatedDate >=@str AND DL.CreatedDate <=@end
                        GROUP BY w.WebSiteID,F.FileTitle,CTE.Title,w.Title,F.OriginalFileName,w.Lang
             ";

            sqlParams.Add(new SqlParameter("@str", str));
            sqlParams.Add(new SqlParameter("@end", end));

            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.BrowseStatisticsModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    var allData = data.Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    return data.OrderByDescending(x => x.DownLoadCount).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<BrowseStatisticsModel> GetFileData(string WebSiteID, string str, string end, string key)
        {
            try
            {
                List<SqlParameter> sqlParams = new List<SqlParameter>();
                var whereP = "";

                if (!string.IsNullOrWhiteSpace(key))
                {
                    whereP += @" and F.OriginalFileName like '%' + @key + '%' ";
                    sqlParams.Add(new SqlParameter("@key", key.Trim()));
                }

                if (!string.IsNullOrWhiteSpace(WebSiteID))
                {
                    whereP += @" and W.WebSiteID=@websiteid  ";
                    sqlParams.Add(new SqlParameter("@websiteid", WebSiteID.Trim()));
                }

                var sql = @$"with CTE (WebLevelSN,ParentSN,Title,Lang,IsEnable,Lvl)
                        as(
                           select WebLevelSN,ParentSN,Title,Lang,IsEnable,1 as lvl
                           from WebLevel AS A 
						   WHERE A.IsEnable != '-99' AND A.ParentSN = 0
                           union all
                           select a.WebLevelSN,a.ParentSN,CAST(b.Title + ' / ' + a.Title AS nvarchar(200)) as Title,A.Lang,b.IsEnable,lvl+1
                           from WebLevel a,CTE b
                           where b.WebLevelSN = a.ParentSN AND A.IsEnable != '-99'
                          )

                        select  w.WebSiteID,F.FileTitle,F.OriginalFileName,CAST(CTE.Title + ' / ' + w.Title AS nvarchar(200)) AS Path,SUM(DL.DownLoads) AS DownLoadCount,w.Lang
                        from CTE
						LEFT JOIN WEBNews AS W ON CTE.WebLevelSN = W.WebLevelSN
						JOIN RelWebFileContent AS rl on w.WEBNewsSN = rl.SourceSN
						LEFT JOIN WebLevel l ON rl.SourceTable = 'WebLevel' AND l.WebLevelSN = rl.SourceSN
						JOIN WEBFile AS F ON rl.WEBFileSN = F.WEBFileSN
                        JOIN WebFileDownLoads DL ON DL.WEBFileSN = F.WEBFileSN AND DL.RelWebFileContentSN = rl.RelWebFileContentSN
						WHERE 1 = 1
                        {whereP}
                        AND  DL.CreatedDate >=@str AND DL.CreatedDate <=@end
                        GROUP BY w.WebSiteID,F.FileTitle,CTE.Title,w.Title,F.OriginalFileName,w.Lang
             ";

                sqlParams.Add(new SqlParameter("@str", str));
                sqlParams.Add(new SqlParameter("@end", end));

                using (var db = new MODAContext())
                {
                    var data = db.BrowseStatisticsModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();

                    return data;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static List<BrowseStatisticsModel> GetFilePath(string classType ,  string key,string WebSiteID,string type, ref DefaultPager pager)
        {
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            var cteSql = @"with CTE (WebLevelSN,ParentSN,Title,Lang,IsEnable,Lvl)
                        as(
                           select WebLevelSN,ParentSN,Title,Lang,IsEnable,1 as lvl
                           from WebLevel AS A 
						   WHERE A.IsEnable != '-99' AND A.ParentSN = 0
                           union all
                           select a.WebLevelSN,a.ParentSN,CAST(b.Title + ' / ' + a.Title AS nvarchar(200)) as Title,A.Lang,b.IsEnable,lvl+1
                           from WebLevel a,CTE b
                           where b.WebLevelSN = a.ParentSN AND A.IsEnable != '-99'
                          )";

            var whereP = "";
            var sql = "";
            switch (classType)
            {
                case "1":
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        whereP += @" and (F.FileTitle like '%' + @key + '%' OR F.OriginalFileName like '%' + @key1 + '%')";
                        sqlParams.Add(new SqlParameter("@key", key.Trim()));
                        sqlParams.Add(new SqlParameter("@key1", key.Trim()));
                    }
                    if (!string.IsNullOrWhiteSpace(WebSiteID))
                    {
                        whereP += @" and W.WebSiteID=@websiteid  ";
                        sqlParams.Add(new SqlParameter("@websiteid", WebSiteID.Trim()));
                    }
                    if (!string.IsNullOrWhiteSpace(type))
                    {
                        whereP += @" and F.FileType like '%' + @type + '%'";
                        sqlParams.Add(new SqlParameter("@type", type.ToLower()));
                    }
                    sql = @$" {cteSql}
                        select distinct  w.WebSiteID,F.FileTitle,F.OriginalFileName,CAST(CTE.Title + ' / ' + w.Title AS nvarchar(200)) AS Path,0 AS DownLoadCount,w.Lang
                        from CTE
						LEFT JOIN WEBNews AS W ON CTE.WebLevelSN = W.WebLevelSN
						JOIN RelWebFileContent AS rl on w.WEBNewsSN = rl.SourceSN
						LEFT JOIN WebLevel l ON rl.SourceTable = 'WebLevel' AND l.WebLevelSN = rl.SourceSN
						JOIN WEBFile AS F ON rl.WEBFileSN = F.WEBFileSN
						WHERE 1 = 1
                        {whereP}
                        AND W.IsEnable != '-99' AND F.IsEnable ='1' 
                        Order by WebSiteID desc, Path , Lang
                        
             ";
                    break;
                case "2":
                    var whereP2 = "";
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        whereP += @" and  ( ne.Column_1 like '%' + @key + '%')";
                        whereP2 += @" and ( cl.URL  like '%' + @key + '%'   )";
                        sqlParams.Add(new SqlParameter("@key", key.Trim()));
                    }
                    if (!string.IsNullOrWhiteSpace(WebSiteID))
                    {
                        whereP += @" and n.WebSiteID=@websiteid  ";
                        whereP2 += @" and n.WebSiteID=@websiteid  ";
                        sqlParams.Add(new SqlParameter("@websiteid", WebSiteID.Trim()));
                    }
                    sql = @$" 
                        {cteSql}
					   select 
					   a.WebSiteID ,  
                         FileTitle ,
                         OriginalFileName ,
                        CAST(CTE.Title + ' / ' + a.Title AS nvarchar(200)) AS Path ,
                        0 as DownLoadCount ,
                        a.Lang
					   from (
                        select distinct n.WebSiteID , webLevelSN,
						ne.Column_1 as FileTitle ,
						ne.Column_2 as OriginalFileName ,
						0 as DownLoadCount ,
						n.Title,
						n.Lang
                        from WebNews n 
                        left join WEBNewsExtend ne on n.WEBNewsSN = ne.WEBNewsSN
                        where 1=1
                        and n.IsEnable !='-99'
                        and ne.GroupId='relatedlink'
                        {whereP}
						union 
						select distinct n.WebSiteID , webLevelSN,
						cl.Url as FileTitle ,
						'內容編輯器' as OriginalFileName ,
						0 as DownLoadCount ,
						n.Title,
						n.Lang
                        from WebNews n 
                        left join WebCntLink cl on n.WEBNewsSN = cl.SourceSN and cl.SourceTable='WEBNews'
                        where 1=1
                        and n.IsEnable !='-99'
                        and ContentText != null
                        {whereP2}
                      ) a join CTE on a.webLevelSN = CTE.WebLevelSN
                       Order by a.WebSiteID desc, Path , a.Lang
                        ";
                    break;
            }
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.BrowseStatisticsModels.FromSqlRaw(sql, sqlParams.ToArray()).ToList();
                    if (pager == null) {
                        return data;
                    }
                    var allData = data.Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    return data.Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

    }
}

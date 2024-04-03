using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Services.Authorization
{
    public class LogService
    {

        /// <summary>
        /// 紀錄動作
        /// </summary>
        /// <param name="lOGAction"></param>
        public static void CreateLogAction(LogAction lOGAction)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(lOGAction.SourceTable))
                {
                    using (var db = new Services.MODAContext())
                    {
                        var logAction = new LogAction()
                        {
                            ProcessIPAddress = lOGAction.ProcessIPAddress,
                            UserID = lOGAction.UserID,
                            WebSiteID = lOGAction.WebSiteID,
                            Controller = lOGAction.Controller,
                            Action = lOGAction.Action,
                            ActionType = lOGAction.ActionType.ToString(),
                            DepartmentID = lOGAction.DepartmentID,
                            MessageInput = lOGAction.MessageInput,
                            MessageResult = lOGAction.MessageResult,
                            Status = lOGAction.Status.ToString(),
                            WebPath = lOGAction.WebPath,
                            Action2 = lOGAction.Action2.ToString(),
                            SourceSN = lOGAction.SourceSN,
                            SourceTable = lOGAction.SourceTable,
                            CreatedDate = DateTime.UtcNow.AddHours(8)
                        };
                        db.LogAction.Add(logAction);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
            }
        }


        /// <summary>
        /// 登入紀錄
        /// </summary>
        /// <param name="sUL"></param>
        public static void SysUserLogin(SysUserLogin sUL)
        {
            using (var db = new Services.MODAContext())
            {
                try
                {
                    db.SysUserLogin.Add(sUL);
                    db.SaveChanges();
                }
                catch (Exception)
                {

                }
            }
        }


        /// <summary>
        /// 取得更新資訊
        /// </summary>
        /// <param name="lOGAction"></param>
        public static DataTable GetLogActionByWEBNews(string SourceSN, string WebSiteID)
        {
            var data = new DataTable();

            using (var db = new MODAContext())
            {
                using (var connection = db.Database.GetDbConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT L.UserID,U.UserName,W.Lang,L.Action2,L.CreatedDate,L.ProcessIPAddress,L.SourceSN,L.MessageInput,L.SourceTable FROM LogAction AS L
                                                JOIN SysUser AS U ON L.UserID = U.UserID
                                                JOIN WEBNews AS W ON L.SourceSN = W.WEBNewsSN
                                                WHERE L.SourceSN = @SourceSN
                                                AND L.WebSiteID = @websiteid";

                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@SourceSN", SourceSN));
                        command.Parameters.Add(new SqlParameter("@websiteid", WebSiteID));

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

        public static DataTable GetLogActionByWEBLevel(string SourceSN, string WebSiteID)
        {
            var data = new DataTable();

            using (var db = new MODAContext())
            {
                using (var connection = db.Database.GetDbConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT L.UserID,U.UserName,W.Lang,L.Action2,L.CreatedDate,L.ProcessIPAddress,L.SourceSN,L.MessageInput,L.SourceTable FROM LogAction AS L
                                                JOIN SysUser AS U ON L.UserID = U.UserID
                                                JOIN WEBLevel AS W ON L.SourceSN = W.WebLevelSN
                                                WHERE L.SourceSN = @SourceSN
                                                AND L.WebSiteID = @websiteid";

                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SqlParameter("@SourceSN", SourceSN));
                        command.Parameters.Add(new SqlParameter("@websiteid", WebSiteID));

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

        public static List<LogAction> GetLogAction(int SourceSN, string SourceTable, string Action2 = "")
        {
            using (var db = new MODAContext())
            {
                var data = db.LogAction.Where(x=> 1==1
                &&  x.SourceSN == SourceSN 
                && x.SourceTable == SourceTable 
                && (string.IsNullOrWhiteSpace(Action2) ? 1== 1 : x.Action2 == Action2)
                ).OrderByDescending(x=>x.CreatedDate).Take(50).ToList();
                return data;
            }
        }

        public static List<LogAction> GetAction(string UserID, string action2, int weblevelsn)
        {
            using (var db = new MODAContext())
            {
                var data = db.WEBNews.Where(x => x.WebLevelSN == weblevelsn && x.IsEnable != "-99");

                var result = (from a in db.LogAction.Where(x => x.Action2.ToLower() == action2 && x.UserID == UserID)
                              join b in data on a.SourceSN equals b.WEBNewsSN
                              select new LogAction
                              {
                                  UserID = a.UserID,
                                  SourceSN = a.SourceSN,
                                  Action = a.Action2,
                                  CreatedDate = a.CreatedDate

                              }).ToList();

                return result;
            }
        }

        public static string GetActionMessageInput(int LogActionSN)
        {
            using (var db = new MODAContext())
            {
                return db.LogAction.FirstOrDefault(x => x.LogActionSN == LogActionSN)?.MessageInput;
            }
        }
        /// <summary>
        /// 排序紀錄
        /// </summary>
        /// <param name="WebLevelSN"></param>
        /// <param name="UserID"></param>
        /// <param name="Lang"></param>
        /// <param name="BeforeType"></param>
        /// <param name="AfterType"></param>
        public static void CreateWebLevelSortLog(int WebLevelSN, string UserID, string Lang, string BeforeType = "", string AfterType = "")
        {
            using (var db = new MODAContext())
            {
                var old = db.WebLevel.Where(x => x.MainSN == WebLevelSN && x.Lang == Lang).First();

                var data = new WebLevelSortLog
                {
                    WebSiteID = old.WebSiteID,
                    Lang = Lang,
                    WebLevelParentSN = (int)old.MainSN,
                    CreatedUserID = UserID,
                    CreatedDate = DateTime.UtcNow.AddHours(8),
                    BeforeSortType = old.SortMethod,
                    AfterSortType = ((AfterType == "" && BeforeType == "" || AfterType == BeforeType) ? old.SortMethod : AfterType),
                    SortMethod = ((AfterType == "" && BeforeType == "" || AfterType == BeforeType) ? "2" : AfterType),
                };
                db.WebLevelSortLog.Add(data);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 取得ErrorAccount
        /// </summary>
        /// <returns></returns>
        public static string GetErroEmailAccount()
        {
            using (var db = new MODAContext())
            {
                var data = (from a in db.SysGroup
                            join b in db.RelSysUserGroup on a.SysGroupSN equals b.SysGroupSN
                            join c in db.SysUser on b.UserID equals c.UserID
                            where a.CanDelete =="0" && a.SysGroupSN !=1
                            select c.Email).ToList();
                if (data.Count() == 0) return "";
                return String.Join(";", data);

            }
        }

    }
}

using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Models;
using Services.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;
using System.Linq.Dynamic.Core;
using Services.SystemManageMent;

namespace Services.Authorization
{
    public class UserManagementService
    {
        /// <summary>
        /// 登入機制
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static sysUserModel Login(SysUser user , string aesKeyUp)
        {
            sysUserModel sysUserModel = new sysUserModel();
            try
            {
                user.p_w_d = AES.AesEncrypt(user.p_w_d, aesKeyUp + user.UserID.Trim());
                using (var db = new MODAContext())
                {
                    var ad = db.SysUser.FirstOrDefault(x => x.UserID == user.UserID);
                    if (ad == null)
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "帳號密碼錯誤";
                        return sysUserModel;
                    }
                    #region 驗證機制
                    else if (ad.ErrLoginnum > 1 && ad.LastLoginDate.Value.AddMinutes(15) > DateTime.UtcNow.AddHours(8))
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "帳號密碼錯誤超過3次無法登入，15分鐘內無法登入";
                    }
                    else if (ad.p_w_d != user.p_w_d)
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "帳號密碼錯誤";
                    }
                    else if (ad.UserSatus == "0")
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "請與資訊處聯繫";
                    }

                    else if (ad.DisableDate.HasValue)
                    {
                        if (ad.DisableDate.Value < DateTime.UtcNow.AddHours(8))
                        {
                            sysUserModel.check = false;
                            sysUserModel.message = "帳號已停用";
                        }
                    }
                    #endregion
                    if (sysUserModel.check)
                    {
                        ad.ErrLoginnum = 0;
                        #region menu
                        var GroupData = (from g in db.SysGroup
                                         join ga in db.RelSysUserGroup on g.SysGroupSN equals ga.SysGroupSN
                                         where ga.UserID == ad.UserID
                                         select g).Where(g => g.IsEnable == "1").ToList();
                        var GroupSNs = GroupData.Select(x => x.SysGroupSN).Distinct().ToList();
                        if (GroupData.Any(x => x.GroupName.Contains("系統管理")))
                        {
                            sysUserModel.GodMode = true;
                            sysUserModel.menu = db.vw_SysSection.Where(x => x.IsEnableC == "1").Where(y => (y.IsEnableP == null || y.IsEnableP == "1")).ToList();
                            #region website
                            var sysWebSites = db.SysWebSite.Where(x => x.IsEnable == "1").ToList();
                            sysUserModel.sysWebSites = sysWebSites.OrderBy(x => x.SortOrder).ToList();
                            sysUserModel.WebSiteID = sysWebSites.First().WebSiteID;
                            #endregion
                        }
                        else
                        {
                            var sysWebSites = (from a in db.SysWebSite
                                               join b in db.AuthSysGroupSysSection on a.WebSiteID equals b.WebSiteID
                                               where GroupSNs.Contains(b.SysGroupSN)
                                               select a).Distinct().ToList();
                            sysUserModel.sysWebSites = sysWebSites.OrderBy(x => x.SortOrder).ToList();
                            sysUserModel.WebSiteID = sysWebSites.First().WebSiteID;
                            ;
                            //全部人都有<<網站維護>> 基本功能
                            //var PeopelHaveMenu = db.vw_SysSection.Where(x => x.SysSectionSN == 1 || x.ParentSN == 1).ToList();
                            var PeopelHaveMenu = db.vw_SysSection.Where(x => x.SysSectionSN == 1 || x.SysSectionSN == 6).ToList();

                            string sql2 = $@"select vw.* from [dbo].[SysUser] sysu 
                                          inner join [dbo].[RelSysUserGroup] sysg on sysu.UserID = sysg.UserID
                                          inner join [dbo].[AuthSysGroupSysSection] sysga on sysg.SysGroupSN = sysga.SysGroupSN
                                          inner join vw_SysSection vw on sysga.SysSectionSN = vw.SysSectionSN
                                          inner Join SysGroup g on sysg.SysGroupSN = g.SysGroupSN
                                        where sysu.UserID = @user 
                                        and vw.SysSectionSN != 6
                                        and vw.ParentSN = 1
                                        and (vw.IsEnableC = '1' and (vw.IsEnableP = '1' Or vw.IsEnableP = null))
                                        and g.IsEnable = '1'
                                        ";

                            string sql = $@"select vw.* from [dbo].[SysUser] sysu 
                                          inner join [dbo].[RelSysUserGroup] sysg on sysu.UserID = sysg.UserID
                                          inner join [dbo].[AuthSysGroupSysSection] sysga on sysg.SysGroupSN = sysga.SysGroupSN
                                          inner join vw_SysSection vw on sysga.SysSectionSN = vw.SysSectionSN
                                          inner Join SysGroup g on sysg.SysGroupSN = g.SysGroupSN
                                        where sysu.UserID = @user 
                                        and vw.SysSectionSN != 1
                                        and vw.ParentSN != 1
                                        and (vw.IsEnableC = '1' and (vw.IsEnableP = '1' Or vw.IsEnableP = null))
                                        and g.IsEnable = '1'
                                        ";

                            var para = new SqlParameter("user", ad.UserID);

                            //子節點2
                            var GroupMenuChi2 = db.vw_SysSection.FromSqlRaw(sql2, para).ToList();
                            //子節點
                            var GroupMenuChi = db.vw_SysSection.FromSqlRaw(sql, para).ToList();
                            //父節點
                            var Parnts = GroupMenuChi.Select(x => x.ParentSN).ToList();
                            var GroupMenuParnt = db.vw_SysSection.Where(x => Parnts.Contains(x.SysSectionSN)).ToList();
                            //全部節點合起來
                            PeopelHaveMenu.AddRange(GroupMenuChi2);
                            PeopelHaveMenu.AddRange(GroupMenuChi);
                            PeopelHaveMenu.AddRange(GroupMenuParnt);
                            var newList = PeopelHaveMenu.Select(x => new
                            {
                                x.SysSectionSN,
                                x.ParentSN,
                                x.ActionPath,
                                x.Title,
                                x.Icon,
                                x.IsEnableC,
                                x.SortOrderC,
                                x.ParentSNP,
                                x.ActionPathP,
                                x.TitleP,
                                x.IconP,
                                x.IsEnableP,
                                x.SortOrderP
                            }).Distinct();
                            PeopelHaveMenu = newList.Select(x => new vw_SysSection()
                            {
                                SysSectionSN = x.SysSectionSN,
                                ParentSN = x.ParentSN,
                                ActionPath = x.ActionPath,
                                Title = x.Title,
                                Icon = x.Icon,
                                IsEnableC = x.IsEnableC,
                                SortOrderC = x.SortOrderC,
                                ParentSNP = x.ParentSNP,
                                ActionPathP = x.ActionPathP,
                                TitleP = x.TitleP,
                                IconP = x.IconP,
                                IsEnableP = x.IsEnableP,
                                SortOrderP = x.SortOrderP
                            }).ToList();


                            sysUserModel.menu = PeopelHaveMenu;
                        }
                        #endregion
                        #region power - WebLevelAccessForGroup
                        var SysGroupSNList = GroupData.Select(x => x.SysGroupSN).ToList();
                        var webLevelAccessForGroupData = db.AuthSysGroupWebLevel
                           .Where(x => SysGroupSNList.Contains(x.SysGroupSN)).ToList();
                        sysUserModel.webLevelAccessForGroups = webLevelAccessForGroupData;
                        #endregion
                        #region sysGroups
                        sysUserModel.sysGroups = GroupData;
                        #endregion
                        #region MyRegion
                        sysUserModel.sysUserSysDepartmentSN = db.SysDepartment.FirstOrDefault(x => x.DepartmentID == ad.DepartmentID)?.SysDepartmentSN;
                        #endregion

                        sysUserModel.sysUser = ad;
                        ad.LastLoginDate = DateTime.UtcNow.AddHours(8);
                    }
                    else
                    {
                        ad.ErrLoginnum++;
                        if (ad.ErrLoginnum == 1)
                        {
                            ad.LastLoginDate = DateTime.UtcNow.AddHours(8);
                        }
                    }
                    ad.ProcessUserID = ad.UserID;
                    ad.ProcessIPAddress = user.ProcessIPAddress;
                    sysUserModel.errorCount = ad.ErrLoginnum.HasValue ? ad.ErrLoginnum.Value : 0;
                    db.SysUser.Update(ad);
                    db.SaveChanges();

                    return sysUserModel;
                }
            }
            catch (Exception)
            {
                sysUserModel.check = false;
                sysUserModel.message = "您未有全球資訊網後台權限，請與資訊處聯繫！";
                return sysUserModel;
            }
        }

        /// <summary>
        /// AAD 登入
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static sysUserModel ADDLogin(SysUser user)
        {
            sysUserModel sysUserModel = new sysUserModel();
            try
            {

                using (var db = new MODAContext())
                {
                    var ad = db.SysUser.FirstOrDefault(x => x.UserID == user.UserID);
                    if (ad == null)
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "您未有全球資訊網後台權限，請與資訊處聯繫！";
                        return sysUserModel;
                    }
                    #region 驗證機制
                    else if (ad.UserSatus == "0")
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "您未有全球資訊網後台權限，請與資訊處聯繫！";
                    }
                    else if (ad.DisableDate.HasValue)
                    {
                        if (ad.DisableDate.Value < DateTime.UtcNow.AddHours(8))
                        {
                            sysUserModel.check = false;
                            sysUserModel.message = "帳號已停用";
                        }
                    }

                    #endregion
                    if (sysUserModel.check)
                    {
                        ad.ErrLoginnum = 0;
                        #region menu
                        var GroupData = (from g in db.SysGroup
                                         join ga in db.RelSysUserGroup on g.SysGroupSN equals ga.SysGroupSN
                                         where ga.UserID == ad.UserID
                                         select g).Where(g => g.IsEnable == "1").ToList();
                        var GroupSNs = GroupData.Select(x => x.SysGroupSN).Distinct().ToList();
                        if (GroupData.Any(x => x.GroupName.Contains("系統管理")))
                        {
                            sysUserModel.GodMode = true;
                            sysUserModel.menu = db.vw_SysSection.Where(x => x.IsEnableC == "1").Where(y => (y.IsEnableP == null || y.IsEnableP == "1")).ToList();
                            #region website
                            var sysWebSites = db.SysWebSite.Where(x => x.IsEnable == "1").ToList();
                            sysUserModel.sysWebSites = sysWebSites.OrderBy(x => x.SortOrder).ToList();
                            sysUserModel.WebSiteID = sysWebSites.First().WebSiteID;
                            #endregion
                        }
                        else
                        {
                            var sysWebSites = (from a in db.SysWebSite
                                               join b in db.AuthSysGroupSysSection on a.WebSiteID equals b.WebSiteID
                                               where GroupSNs.Contains(b.SysGroupSN)
                                               select a).Distinct().ToList();
                            sysUserModel.sysWebSites = sysWebSites.OrderBy(x => x.SortOrder).ToList();
                            sysUserModel.WebSiteID = sysWebSites.First().WebSiteID;
                            ;
                            var PeopelHaveMenu = db.vw_SysSection.Where(x => x.SysSectionSN == 1 || x.SysSectionSN == 6).ToList();
                            string sql2 = $@"select vw.* from [dbo].[SysUser] sysu 
                                          inner join [dbo].[RelSysUserGroup] sysg on sysu.UserID = sysg.UserID
                                          inner join [dbo].[AuthSysGroupSysSection] sysga on sysg.SysGroupSN = sysga.SysGroupSN
                                          inner join vw_SysSection vw on sysga.SysSectionSN = vw.SysSectionSN
                                          inner Join SysGroup g on sysg.SysGroupSN = g.SysGroupSN
                                        where sysu.UserID = @user 
                                        and vw.SysSectionSN != 6
                                        and vw.ParentSN = 1
                                        and (vw.IsEnableC = '1' and (vw.IsEnableP = '1' Or vw.IsEnableP = null))
                                        and g.IsEnable = '1'
                                        ";

                            string sql = $@"select vw.* from [dbo].[SysUser] sysu 
                                          inner join [dbo].[RelSysUserGroup] sysg on sysu.UserID = sysg.UserID
                                          inner join [dbo].[AuthSysGroupSysSection] sysga on sysg.SysGroupSN = sysga.SysGroupSN
                                          inner join vw_SysSection vw on sysga.SysSectionSN = vw.SysSectionSN
                                          inner Join SysGroup g on sysg.SysGroupSN = g.SysGroupSN
                                        where sysu.UserID = @user 
                                        and vw.SysSectionSN != 1
                                        and vw.ParentSN != 1
                                        and (vw.IsEnableC = '1' and (vw.IsEnableP = '1' Or vw.IsEnableP = null))
                                        and g.IsEnable = '1'
                                        ";

                            var para = new SqlParameter("user", ad.UserID);

                            //子節點2
                            var GroupMenuChi2 = db.vw_SysSection.FromSqlRaw(sql2, para).ToList();
                            //子節點
                            var GroupMenuChi = db.vw_SysSection.FromSqlRaw(sql, para).ToList();
                            //父節點
                            var Parnts = GroupMenuChi.Select(x => x.ParentSN).ToList();
                            var GroupMenuParnt = db.vw_SysSection.Where(x => Parnts.Contains(x.SysSectionSN)).ToList();
                            //全部節點合起來
                            PeopelHaveMenu.AddRange(GroupMenuChi2);
                            PeopelHaveMenu.AddRange(GroupMenuChi);
                            PeopelHaveMenu.AddRange(GroupMenuParnt);
                            var newList = PeopelHaveMenu.Select(x => new
                            {
                                x.SysSectionSN,
                                x.ParentSN,
                                x.ActionPath,
                                x.Title,
                                x.Icon,
                                x.IsEnableC,
                                x.SortOrderC,
                                x.ParentSNP,
                                x.ActionPathP,
                                x.TitleP,
                                x.IconP,
                                x.IsEnableP,
                                x.SortOrderP
                            }).Distinct();
                            PeopelHaveMenu = newList.Select(x => new vw_SysSection()
                            {
                                SysSectionSN = x.SysSectionSN,
                                ParentSN = x.ParentSN,
                                ActionPath = x.ActionPath,
                                Title = x.Title,
                                Icon = x.Icon,
                                IsEnableC = x.IsEnableC,
                                SortOrderC = x.SortOrderC,
                                ParentSNP = x.ParentSNP,
                                ActionPathP = x.ActionPathP,
                                TitleP = x.TitleP,
                                IconP = x.IconP,
                                IsEnableP = x.IsEnableP,
                                SortOrderP = x.SortOrderP
                            }).ToList();


                            sysUserModel.menu = PeopelHaveMenu;
                        }
                        #endregion
                        #region power - WebLevelAccessForGroup
                        var SysGroupSNList = GroupData.Select(x => x.SysGroupSN).ToList();
                        var webLevelAccessForGroupData = db.AuthSysGroupWebLevel
                           .Where(x => SysGroupSNList.Contains(x.SysGroupSN)).ToList();
                        sysUserModel.webLevelAccessForGroups = webLevelAccessForGroupData;
                        #endregion
                        #region sysGroups
                        sysUserModel.sysGroups = GroupData;
                        #endregion
                        sysUserModel.sysUser = ad;
                        ad.LastLoginDate = DateTime.UtcNow.AddHours(8);
                    }
                    else
                    {
                        ad.ErrLoginnum++;
                        if (ad.ErrLoginnum == 1)
                        {
                            ad.LastLoginDate = DateTime.UtcNow.AddHours(8);
                        }
                    }
                    ad.ProcessUserID = ad.UserID;
                    ad.ProcessIPAddress = user.ProcessIPAddress;
                    sysUserModel.errorCount = ad.ErrLoginnum.HasValue ? ad.ErrLoginnum.Value : 0;
                    db.SysUser.Update(ad);
                    db.SaveChanges();

                    return sysUserModel;
                }
            }
            catch (Exception)
            {
                sysUserModel.check = false;
                sysUserModel.message = "您未有全球資訊網後台權限，請與資訊處聯繫！";
                return sysUserModel;
            }
        }

        /// <summary>
        /// 建立會員資料
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static sysUserModel CreateUser(SysUser user , string askKey)
        {
            sysUserModel sysUserModel = new sysUserModel();
            try
            {
                using (var db = new MODAContext())
                {
                    user.p_w_d = Regular.GetRandomString(2, RegularType.special) + Regular.GetRandomString(4, RegularType.big_en) + Regular.GetRandomString(4, RegularType.smail_en) + Regular.GetRandomString(2, RegularType.number);
                    if (Regular.checkId(user.UserID.Trim()))
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "帳號不能為身分字號";
                        return sysUserModel;
                    }
                    var askKey1 = "";
                    var IsOpenASEKey = CodeManagementService.GetCategoryByCategoryKey("Management-5-1", "zh-tw");
                    if (IsOpenASEKey?.Value == "1") { askKey1 = askKey.ToUpper(); }
                    user.p_w_d = AES.AesEncrypt(user.p_w_d, askKey1 + user.UserID.Trim());
                    var ad = db.SysUser.FirstOrDefault(x => x.UserID == user.UserID);
                    if (ad != null)
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "帳號已存在";
                        return sysUserModel;
                    }
                    user.DateCreated = DateTime.UtcNow.AddHours(8);
                    user.ProcessDate = DateTime.UtcNow.AddHours(8);
                    db.SysUser.Add(user);
                    var pwdHistoryModel = new SysPwdHistory()
                    {
                        UserID = user.UserID,
                        p_w_d = user.p_w_d,
                        CreateDate = DateTime.UtcNow.AddHours(8),
                        CreateUserID = user.ProcessUserID
                    };
                    db.SysPwdHistory.Add(pwdHistoryModel);
                    db.SaveChanges();
                    sysUserModel.sysUser = user;
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                sysUserModel.check = false;
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = user.ProcessIPAddress,
                    UserID = user.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Insert",
                    SourceTable = "SysUser",
                    Action = "CreateUser",
                    Controller = "UserManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            //
            return sysUserModel;
        }
        /// <summary>
        /// 修改會員基本資料
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static sysUserModel EditUser(SysUser user)
        {
            sysUserModel sysUserModel = new sysUserModel();
            try
            {
                using (var db = new MODAContext())
                {
                    var data = db.SysUser.FirstOrDefault(X => X.UserID == user.UserID);
                    if (data == null)
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "查無此會員資料";
                        return sysUserModel;
                    }
                    data.UserName = user.UserName.Trim();
                    data.EngName = user.EngName;
                    data.NickName = user.NickName;
                    data.Tel = user.Tel;
                    data.Mobile = user.Mobile;
                    data.Gender = user.Gender;
                    data.UserSatus = user.DisableDate < DateTime.UtcNow.AddHours(8) ? "0" : user.UserSatus;
                    data.Email = user.Email;
                    data.EmpID = user.EmpID;
                    data.JobTitle = user.JobTitle;
                    data.OfficePhone = user.OfficePhone;
                    data.DepartmentID = user.DepartmentID;
                    data.ProcessDate = user.ProcessDate;
                    data.ProcessIPAddress = user.ProcessIPAddress;
                    data.ProcessUserID = user.ProcessUserID;
                    if (data.UserSatus == "0")
                    {
                        data.DisableDate = DateTime.UtcNow.AddHours(8);
                    }
                    else
                    {
                        data.DisableDate = user.DisableDate;
                    }
                    db.SysUser.Update(data);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                sysUserModel.check = false;
                sysUserModel.message = "資料出現錯誤，請聯絡系統管理人員";
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = user.ProcessIPAddress,
                    UserID = user.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Update",
                    SourceTable = "SysUser",
                    Action = "EditUser",
                    Controller = "UserManagementService",
                    SourceSN = user.SysUserSN,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return sysUserModel;
            }
            return sysUserModel;
        }
        public static sysUserModel EditModeUser(SysUser user , string aeskey)
        {

            sysUserModel sysUserModel = new sysUserModel();
            try
            {
                using (var db = new MODAContext())
                {
                    var data = db.SysUser.FirstOrDefault(X => X.UserID == user.UserID);
                    if (data == null)
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "查無此會員資料";
                        return sysUserModel;
                    }
                    if (!string.IsNullOrEmpty(user.p_w_d))
                    {

                        var requertData = UpdatePwd(user, aeskey);
                        data.p_w_d = user.p_w_d;
                        data.PwdLastUpdate = DateTime.UtcNow.AddHours(8);
                        if (!requertData.check)
                        {
                            sysUserModel.check = false;
                            sysUserModel.message = requertData.message;
                            return sysUserModel;
                        }
                    }

                    data.UserName = user.UserName;
                    data.EngName = user.EngName;
                    data.NickName = user.NickName;
                    data.Tel = user.Tel;
                    data.Mobile = user.Mobile;
                    data.Email = user.Email;
                    data.OfficePhone = user.OfficePhone;
                    data.ProcessDate = user.ProcessDate;
                    data.ProcessIPAddress = user.ProcessIPAddress;
                    data.ProcessUserID = user.ProcessUserID;

                    db.SysUser.Update(data);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                sysUserModel.check = false;
                sysUserModel.message = "資料出現錯誤，請聯絡系統管理人員";
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = user.ProcessIPAddress,
                    UserID = user.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Update",
                    SourceTable = "SysUser",
                    Action = "EditModeUser",
                    Controller = "UserManagementService",
                    SourceSN = user.SysUserSN,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return sysUserModel;
            }

            return sysUserModel;


        }

        public static sysUserModel CheckForgetUser(string UserId, string email)
        {
            sysUserModel sysUserModel = new sysUserModel();
            using (var db = new MODAContext())
            {
                try
                {
                    var cus = db.SysUser.FirstOrDefault(x => x.UserID == UserId
                            && x.Email == email);
                    if (cus == null)
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "帳號或信箱錯誤";
                    }
                    else
                    {
                        if (cus.DisableDate.HasValue)
                        {
                            if (cus.DisableDate.Value <= DateTime.UtcNow.AddHours(8))
                            {
                                sysUserModel.check = false;
                                sysUserModel.message = "密碼已停用，請聯絡管理員";
                            }
                        }
                        sysUserModel.check = true;
                        sysUserModel.sysUser = cus;
                        cus.ProcessDate = DateTime.UtcNow.AddHours(8);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    return sysUserModel;
                }
                return sysUserModel;
            }
        }
        /// <summary>
        /// 確認是否有這個User
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="key"></param>
        /// <param name="type">0-第一次 1-後續設定take</param>
        /// <returns></returns>
        public static bool CheckUser(string UserID, string key, int type = 0)
        {

            using (var db = new MODAContext())
            {
                try
                {
                    key = key.Replace(" ", "+");
                    var data = db.SysUser.FirstOrDefault(x => x.UserID == UserID
                           && x.p_w_d == key
                           && (x.DisableDate == null || x.DisableDate <= DateTime.UtcNow.AddHours(8))
                           && (type == 0 ? 1 == 1 : x.ProcessDate.Value.AddMinutes(10) >= DateTime.UtcNow.AddHours(8))
                         );
                    return data == null ? false : true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 更新密碼
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static sysUserModel UpdatePwd(SysUser user , string aeskey)
        {

            //
            sysUserModel sysUserModel = new sysUserModel();
            if (!Regular.CheckTxt(new Regular.RegularModel() { minLin = 12, txt = user.p_w_d }))
            {
                sysUserModel.check = false;
                sysUserModel.message = "密碼強度不夠";
                return sysUserModel;
            }
           
            user.p_w_d = AES.AesEncrypt(user.p_w_d, aeskey + user.UserID.Trim());
            using (var db = new MODAContext())
            {
                try
                {
                    var ad = db.SysPwdHistory.FirstOrDefault(x => x.UserID == user.UserID && x.p_w_d == user.p_w_d);
                    if (ad != null)
                    {
                        sysUserModel.check = false;
                        sysUserModel.message = "密碼跟前三次的重複，請修正";
                        return sysUserModel;
                    }
                    var uModel = db.SysUser.FirstOrDefault(x => x.UserID == user.UserID);
                    uModel.p_w_d = user.p_w_d;
                    uModel.ProcessDate = DateTime.UtcNow.AddHours(8);
                    uModel.PwdLastUpdate = DateTime.UtcNow.AddHours(8);
                    uModel.ProcessUserID = user.ProcessUserID;
                    uModel.ErrLoginnum = 0;
                    db.SysUser.Update(uModel);
                    //
                    var pwdHistory = db.SysPwdHistory.Where(x => x.UserID == user.UserID).ToList();
                    if (pwdHistory.Count < 4)
                    {
                        var pwdHistoryModel = new SysPwdHistory()
                        {
                            UserID = user.UserID,
                            p_w_d = user.p_w_d,
                            CreateDate = DateTime.UtcNow.AddHours(8),
                            CreateUserID = user.ProcessUserID
                        };
                        db.SysPwdHistory.Add(pwdHistoryModel);
                    }
                    else
                    {
                        var minpwdHistory = pwdHistory.OrderBy(x => x.CreateDate).First();
                        minpwdHistory.p_w_d = user.p_w_d;
                        minpwdHistory.CreateDate = DateTime.UtcNow.AddHours(8);
                        minpwdHistory.CreateUserID = user.ProcessUserID;
                        db.SysPwdHistory.Remove(minpwdHistory);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    return sysUserModel;
                }
            }
            return sysUserModel;
        }

        /// <summary>
        /// 取得會員列表
        /// </summary>
        /// <param name="key">關鍵字</param>
        /// <param name="dep">部門</param>
        /// <param name="states">狀態</param>
        /// <param name="pager">頁碼資料</param>
        /// <returns></returns>
        public static List<vw_UserLeftDep> GetUserList(string sortTitle, string sortType, string key, string dep, string states, ref DefaultPager pager, bool isExport = false)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var Data = db.vw_UserLeftDep.Where(x => 1 == 1);
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        Data = Data.Where(x => x.UserID.Contains(key) || x.UserName.Contains(key));
                    }
                    if (!string.IsNullOrWhiteSpace(dep))
                    {
                        Data = Data.Where(x => x.DepartmentID.Equals(dep));
                    }
                    if (!string.IsNullOrWhiteSpace(states))
                    {
                        switch (states)
                        {
                            case "0":
                                Data = Data.Where(x => x.UserSatus.Equals(states));
                                break;
                            case "1":
                                Data = Data.Where(x => x.UserSatus.Equals("1") && x.PwdLastUpdate != null);
                                break;
                            case "2":
                                Data = Data.Where(x => x.UserSatus.Equals("1") && x.PwdLastUpdate == null);
                                break;
                        }
                    }
                    var list = Data;

                    if (!isExport)
                    {
                        var allData = list.Count();
                        pager.TotalCount = allData;
                        pager.PageIndex = pager.p - 1;
                        //可以下ORDER BY 條件
                        var searchData = new List<vw_UserLeftDep>();
                        if (!string.IsNullOrWhiteSpace(sortTitle) && !string.IsNullOrWhiteSpace(sortTitle))
                        {
                            searchData = list.OrderBy($" {sortTitle} {sortType}").Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                        }
                        else
                        {
                            searchData = list.OrderByDescending(o => o.CreatedDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                        }
                        return searchData;
                    }
                    else
                    {
                        return list.ToList();
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
                    ProcessIPAddress = "",
                    UserID = "",
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Select",
                    SourceTable = "vw_UserLeftDep",
                    Action = "GetUserList",
                    Controller = "UserManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
            return null;
        }

        /// <summary>
        /// 會員資料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static SysUser GetUserData(string key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysUser.FirstOrDefault(x => x.UserID == key);
                }
                catch (Exception ex) { Utility.Mail.Error(ex.ToString()); return null; }
            }
        }
        /// <summary>
        /// 快速停用會員
        /// 改為切換啟用/停用狀態
        /// </summary>
        /// <param name="sysUser"></param>
        /// <returns></returns>
        public static bool StopUser(SysUser sysUser, out int sn, out int IsEnable)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.SysUser.FirstOrDefault(x => x.UserID == sysUser.UserID);
                    if (data == null)
                    {
                        sn = 0;
                        IsEnable = 0;
                        return false;
                    }
                    sn = data.SysUserSN;
                    if (data.UserSatus == "0")
                    {
                        data.DisableDate = null;
                        data.UserSatus = "1";
                        IsEnable = 1;
                    }
                    else
                    {
                        data.DisableDate = DateTime.UtcNow.AddHours(8);
                        data.UserSatus = "0";
                        IsEnable = 0;
                    }
                    data.ProcessIPAddress = sysUser.ProcessIPAddress;
                    data.ProcessDate = DateTime.UtcNow.AddHours(8);
                    data.ProcessUserID = sysUser.ProcessUserID;

                    db.SysUser.Update(data);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Utility.Mail.Error(ex.ToString());
                    sn = 0;
                    IsEnable = 0;
                    return false;
                }
            }
        }
        /// <summary>
        /// 刪除帳號-沒有登入記錄過的帳號才能刪除
        /// </summary>
        /// <param name="sysUser"></param>
        /// <returns></returns>
        public static bool DelUser(SysUser sysUser , out int sn)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var data = db.SysUser.FirstOrDefault(x => x.UserID == sysUser.UserID);
                    if (data == null)
                    {
                        sn = 0;
                        return false;
                    }
                    if (data.LastLoginDate != null)
                    {
                        sn = 0;
                        return false;
                    }
                    sn = data.SysUserSN;
                    db.SysUser.Remove(data);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    sn = 0;
                    return false;
                }

            }
        }

        /// <summary>
        /// 群組對應User關係
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static List<sysGroupToUserModel> GetUserToGroup(string UserID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Alldata = db.SysGroup.ToList();
                    string sqlUserData = $@"
                        select 
                          g.SysGroupSN 
                        , g.GroupName
                        , g.Description
                        , g.IsEnable
                        , cast(case 
                            when u.UserID is null then 0
                            else 1
                          end as bit) IsSelect
                       from [dbo].[SysGroup] g
                        join [dbo].[RelSysUserGroup] ug on g.SysGroupSN = ug.SysGroupSN
                        join  [dbo].[SysUser]  u  on u.UserID = ug.UserID 
                        where u.UserID  = @UserID";
                    List<SqlParameter> sqlParams = new List<SqlParameter>();
                    sqlParams.Add(new SqlParameter("@UserID", UserID.ToString()));

                    var UserData = db.SysGroupToUserModels.FromSqlRaw(sqlUserData, sqlParams.ToArray()).ToList();
                    var data = (from a in Alldata
                                join b in UserData on a.SysGroupSN equals b.SysGroupSN into ps
                                from o in ps.DefaultIfEmpty()
                                select new sysGroupToUserModel
                                {
                                    SysGroupSN = a.SysGroupSN,
                                    GroupName = a.GroupName,
                                    Description = a.Description,
                                    IsEnable = a.IsEnable,
                                    IsSelect = o != null ? true : false
                                }).ToList();
                    return data;
                }
                catch (Exception ex) { Utility.Mail.Error(ex.ToString()); return null; }
            }
        }
        /// <summary>
        /// 修改USER對應群組資料
        /// </summary>
        /// <param name="editSysUserGroupModels"></param>
        public static void EditUserToGroup(List<EditSysUserGroupModel> editSysUserGroupModels)
        {
            try
            {
                var deleteData = editSysUserGroupModels.Where(x => x.isselect == false).ToList();

                var insertData = editSysUserGroupModels.Where(x => x.isselect == true).ToList();
                using (var db = new MODAContext())
                {
                    //先將沒有打勾的資料刪除
                    foreach (var data in deleteData)
                    {
                        var source = db.RelSysUserGroup.FirstOrDefault(X => X.UserID == data.UserID && X.SysGroupSN == data.groupsn);
                        if (source != null)
                        {
                            db.RelSysUserGroup.Remove(source);
                        }
                    }
                    //新增資烙
                    foreach (var data in insertData)
                    {
                        var source = db.RelSysUserGroup.FirstOrDefault(X => X.UserID == data.UserID && X.SysGroupSN == data.groupsn);
                        if (source == null)
                        {
                            var newData = new RelSysUserGroup()
                            {
                                SysGroupSN = data.groupsn,
                                CreatedDate = DateTime.UtcNow.AddHours(8),
                                CreatedIPAddress = data.CreatedIPAddress,
                                CreatedUserID = data.CreatedUserID,
                                SortOrder = 1,
                                UserID = data.UserID
                            };
                            db.RelSysUserGroup.Add(newData);
                        }
                    }
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
                    Action2 = "Update",
                    SourceTable = "RelSysUserGroup",
                    Action = "EditUserToGroup",
                    Controller = "UserManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
        }

        /// <summary>
        /// 撈取麵包資料
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static vw_SysSection Breadcrumb(int key)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.vw_SysSection.FirstOrDefault(x => x.SysSectionSN == key);
                }
                catch (Exception ex) { Utility.Mail.Error(ex.ToString()); return null; }
            }
        }





    }
}

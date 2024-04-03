using DBModel;
//using Management.Areas.Authorization.Models.GroupManagement;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.Authorization;
using Services.Models;
using Services.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Management.Areas.Authorization
{
    [Area("Authorization")]
    public class GroupManagementController : BaseController
    {
        #region Page 
        /// <summary>
        /// 群組-首頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(9);
            var chkUM = CheckUserMenu(9);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area = "" });
            }
            return View();
        }
        /// <summary>
        /// 群組-列表
        /// </summary>
        public IActionResult List(string sorttitle="", string sorttype="", string key = "", string states = "", string dep = "", string keyword = "", string sec = "", int p = 1, int DisplayCount = 10)
        {
            var sortData = new List<int>();
            var ListModels = new Management.Areas.Authorization.Models.GroupManagement.ListModel();
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            var lists = GroupManagementService.GetGroupList(sorttitle, sorttype,key, states, dep, keyword, sec, ref pager , ref sortData);
            ListModels.defaultPager = pager;
            ListModels.SysGroups = lists;
            ListModels.SortData = sortData.OrderBy(x=>x).ToList();
            return View(ListModels);
        }
        /// <summary>
        /// 新增 頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(9);
            var chkUM = CheckUserMenu(9);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area = "" });
            }
            return View();
        }
        /// <summary>
        /// 修改頁面
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IActionResult Edit(string key)
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(9);
            var chkUM = CheckUserMenu(9);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area = "" });
            }

            if (CommonUtility.UrlKey(ref key))
            {
                EditModel editModel = new EditModel();
                editModel.sysGroup = GroupManagementService.GetSysGroup(key);
                editModel.groupSectionByGroupModels = GroupManagementService.GetSysSectionList(key);
                editModel.sysSections = GroupManagementService.GetAllSysSection(1);

                editModel.sysWebSites = GroupManagementService.GetAllSysWebSites();
                return View(editModel);
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";
                return RedirectToAction("Index", "UserManagement", new { area = "Authorization", msg = "請別亂輸入測試" });
            }
        }
        /// <summary>
        /// 使用者對應群組列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult UserGroupList(string key, int p = 1, int DisplayCount = 10)
        {
            if (CommonUtility.UrlKey(ref key))
            {
                EditModel editModel = new EditModel();
                DefaultPager pager = new DefaultPager();
                pager.DisplayCount = DisplayCount;
                pager.p = p;
                editModel.GroupUsers = GroupManagementService.GetGroupUsers(key, ref pager);
                editModel.defaultPager = pager;

                return View(editModel);
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";

                return RedirectToAction("Index", "UserManagement", new { area = "Authorization", msg = "請別亂輸入測試" });
            }
        }
        #endregion
        #region HttpPost event 動作
        /// <summary>
        /// 新增基本群組資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SysGroup data)
        {
            try
            {
                SetLogActionModel(webPath: CommonUtility.Breadcrumb(9), Action2: Utility.Model.LoginModel.Action2.insert, SourceTable: "SysGroup");

                #region SysGroup
                data.CreatedUserID = UserData.sysUser?.UserID;
                data.CreatedDate = DateTime.UtcNow.AddHours(8);
                data.ProcessDate = DateTime.UtcNow.AddHours(8);
                data.ProcessUserID = UserData.sysUser?.UserID;
                data.ProcessIPAddress = ContextModel.ProcessIpaddress;
                #endregion
                var soure = GroupManagementService.CreateUser(data);
                if (!soure.check)
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = soure.message;
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, soure.message);
                }
                logActionModel.response = soure.message;
                logActionModel.SourceSN = data.SysGroupSN;
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }
        /// <summary>
        /// 修改群組基本資料
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SysGroup data, string sn)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(9), Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "SysGroup");

            try
            {
                if (CommonUtility.UrlKey(ref sn))
                {
                    logActionModel.SourceSN = data.SysGroupSN;
                    data.SysGroupSN = int.Parse(sn);
                    data.ProcessIPAddress = ContextModel.ProcessIpaddress;
                    data.CreatedUserID = UserData != null ? UserData.sysUser.UserID : "";
                    EditModel editModel = new EditModel();
                    var source = GroupManagementService.Edit(data);
                    if (source.check)
                    {
                        Log(logActionModel);
                        return StatusResult(System.Net.HttpStatusCode.OK, "");
                    }
                    else
                    {
                        logActionModel.status = Utility.Model.LoginModel.Status.Error;
                        logActionModel.response = source.message;
                        return StatusResult(System.Net.HttpStatusCode.BadRequest, source.message);
                    }
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = "請別亂輸入測試";
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string key)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(9), Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "SysGroup");
            logActionModel.SourceTable = "SysGroup";
            logActionModel.Action2 = Utility.Model.LoginModel.Action2.delete;
            try
            {
                if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key))
                {

                    GroupManagementService.DeleteAuthSysGroupSysSections(_key);
                    GroupManagementService.DeleteAuthSysGroupWebLevels(_key);
                    GroupManagementService.DeleteGroup(_key);

                    logActionModel.SourceSN = _key;
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.OK, "群組刪除成功!");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = "請別亂輸入測試";
                    Log(logActionModel);
                    return RedirectToAction("Index", "UserManagement", new { area = "Authorization", msg = "請別亂輸入測試" });
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        /// <summary>
        /// 修改群組與SysSection關係
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditGroup(List<GroupSectionByGroupModel> data, string sn)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(9), Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "AuthSysGroupSysSection");
            try
            {
                if (CommonUtility.UrlKey(ref sn))
                {
                    int _sn = int.Parse(sn);
                    GroupManagementService.UpdateSysGroupAccess(data, _sn, UserData != null ? UserData.sysUser.UserID : "");
                    logActionModel.SourceSN = _sn;

                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = "請別亂輸入測試";
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }
        /// <summary>
        /// 新增群組人員
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddGroupUser(List<RelSysUserGroup> data, string sn)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(9), Action2: Utility.Model.LoginModel.Action2.insert, SourceTable: "SysGroup");

            try
            {
                if (CommonUtility.UrlKey(ref sn))
                {
                    var _sn = int.Parse(sn);
                    logActionModel.SourceSN = _sn;
                    foreach (var userGroup in data)
                    {
                        var userID = userGroup.UserID;
                        if (CommonUtility.UrlKey(ref userID))
                        {
                            userGroup.UserID = userID;
                            userGroup.SysGroupSN = _sn;
                            userGroup.CreatedUserID = UserData.sysUser.UserID;
                            userGroup.CreatedDate = DateTime.UtcNow.AddHours(8);
                            userGroup.CreatedIPAddress = ContextModel.ProcessIpaddress;
                            userGroup.SortOrder = 1;
                            GroupManagementService.CreateSysUserGroup(userGroup);
                        }
                    }
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = "請別亂輸入測試";
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }
        /// <summary>
        /// 刪除群組人員
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteGroupUser(string sn)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(9), Action2: Utility.Model.LoginModel.Action2.delete, SourceTable: "RelSysUserGroup");
            var _sn = 0;
            try
            {
                if (CommonUtility.UrlKey(ref sn))
                {
                    _sn = int.Parse(sn);
                    logActionModel.SourceSN = _sn;
                    var userGroup = new RelSysUserGroup
                    {
                        RelSysGroupUserSN = _sn
                    };
                    GroupManagementService.DeleteSysUserGroup(userGroup);
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = "請別亂輸入測試";
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }
        /// <summary>
        /// Group重新排序
        /// </summary>
        /// <param name="key">SysGroupSN</param>
        /// <param name="sort">欲調整至序號</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReArrange(string key, string sort)
        {
            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key) && int.TryParse(sort, out int _sort))
            {
                GroupManagementService.GroupReArrange(_key, _sort, UserData.sysUser.UserID, UserData.sysUser.ProcessIPAddress);
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ability(string key)
        {
            if (CommonUtility.UrlKey(ref key))
            {
                logActionModel.SourceTable = "SysGroup";
                logActionModel.Action2 = Utility.Model.LoginModel.Action2.update;
                GroupManagementService.SetGroupAbility(int.Parse(key), out string IsEnable);
                logActionModel.SourceSN = int.Parse(key);
                Log(logActionModel);

                return StatusResult(System.Net.HttpStatusCode.OK, IsEnable == "1" ? "已啟用此群組" : "已停用此群組");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂測試";
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂測試");
            }
        }
        #endregion

        

    }
}

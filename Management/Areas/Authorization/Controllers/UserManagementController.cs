using DBModel;
using Management.Areas.Authorization.Models.UserManagement;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.Authorization;
using Services.Models;
using Services.Models.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
using Utility.Model;

namespace Management.Areas.Authorization
{
    [Area("Authorization")]
    public class UserManagementController : BaseController
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;
        public UserManagementController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        #region Page
        /// <summary>
        /// 帳號首頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(8);
            var chkUM = CheckUserMenu(8);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area = "" });
            }

            Models.UserManagement.IndexModel indexModel = new IndexModel();
            return View(indexModel);
        }
        /// <summary>
        /// 帳號 首頁-列表
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dep"></param>
        /// <param name="states"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult List(string sorttitle = "", string sorttype = "", string key = "", string dep = "", string states = "", int p = 1, int DisplayCount = 10)
        {
            var ListModels = new ListModel();
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            var UserLeftDeps = UserManagementService.GetUserList(sorttitle, sorttype, key, dep, states, ref pager);
            ListModels.defaultPager = pager;
            ListModels.UserLeftDeps = UserLeftDeps;
            return View(ListModels);
        }
        /// <summary>
        /// 帳號模式
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IActionResult Mode(string key = "")
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(8);
            var chkUM = CheckUserMenu(8);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area = "" });
            }
            var viewData = new ModeModel();
            viewData.UserID = UserData.sysUser.UserID;
            if (string.IsNullOrWhiteSpace(key))
            {
                return View(viewData);
            }
            else
            {
                var _key = CommonUtility.GetUrlAesDecrypt(key);
                if (!string.IsNullOrWhiteSpace(_key))
                {
                    viewData.sysUser = UserManagementService.GetUserData(_key);
                    viewData.sysGroupToUserModels = UserManagementService.GetUserToGroup(_key);
                }
                return View(viewData);
            }
        }

        public IActionResult EditMode(string key)
        {
            if (CommonUtility.UrlKey(ref key))
            {
                ViewData["Title"] = "Moda";
                var editModeModel = new EditModeModel();
                var data = UserManagementService.GetUserData(key);
                editModeModel.sysUser = data;
                return View(editModeModel);
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入帳號測試";

                return RedirectToAction("Index", "WebLevelManagement", new { area = "WebContent", msg = "請別亂輸入帳號測試" });
            }
        }


        #endregion
        #region HttpPost
        /// <summary>
        /// 儲存帳號資料
        /// </summary>
        /// <param name="data">帳號基本資料</param>
        /// <param name="isNews">1- 新增 0-更新</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUser(SysUser data)
        {
            try
            {
                SetLogActionModel(webPath: CommonUtility.Breadcrumb(8), Action2: data.SysUserSN == 0 ? Utility.Model.LoginModel.Action2.insert : Utility.Model.LoginModel.Action2.update, SourceTable: "SysUser");
                var askKey = AppSettingHelper.GetAppsetting("AESKey");
                #region SysUser 資料整理
                data.ProcessDate = DateTime.UtcNow.AddHours(8);
                data.ProcessIPAddress = ContextModel.ProcessIpaddress;
                data.ProcessUserID = UserData.sysUser?.UserID;
                string createmsg = "";
                #endregion
                var soure = new sysUserModel();
                if (data.SysUserSN == 0)
                {
                    #region 新增
                    data.UserSatus = "1";
                    data.DateCreated = DateTime.UtcNow.AddHours(8);
                    createmsg = "/目前此使用者尚未設置權限群組，請確認相關群組";
                    soure = UserManagementService.CreateUser(data, askKey);
                    #endregion
                }
                else
                {
                    #region 更新
                    logActionModel.SourceSN = data.SysUserSN;
                    var UserID = data.UserID;
                    if (!CommonUtility.UrlKey(ref UserID))
                    {
                        logActionModel.status = Utility.Model.LoginModel.Status.Error;
                        logActionModel.response = "請別亂輸入帳號測試";
                        return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入帳號測試");
                    }
                    else
                    {
                        data.UserID = UserID;
                        soure = UserManagementService.EditUser(data);
                        logActionModel.response = soure.message;
                    }
                    #endregion
                }
                if (!soure.check)
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, soure.message);
                }
                //if (logActionModel.Action2 == Utility.Model.LoginModel.Action2.insert)
                //{
                //    //發信寄給對方 通知對方 需要補添加
                //    MailUtility.Sendpwd(MailType.pwdfirst, _hostingEnvironment.WebRootPath, out Exception ex, data);
                //}
                logActionModel.SourceSN = data.SysUserSN;
                return StatusResult(System.Net.HttpStatusCode.OK, createmsg);
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.Message;
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }
        /// <summary>
        /// 停止此帳號
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StopUser(string key)
        {

            logActionModel.SourceTable = "SysUser";
            logActionModel.Action2 = Utility.Model.LoginModel.Action2.update;

            if (CommonUtility.UrlKey(ref key))
            {
                var data = new SysUser();
                data.UserID = key;
                data.ProcessDate = DateTime.UtcNow.AddHours(8);
                data.ProcessIPAddress = ContextModel.ProcessIpaddress;
                data.ProcessUserID = UserData != null ? UserData.sysUser.UserID : "";
                UserManagementService.StopUser(data, out int sn, out int IsEnable);
                logActionModel.SourceSN = sn;
                SetLogActionModel(webPath: CommonUtility.Breadcrumb(8), Action2: data.SysUserSN == 0 ? Utility.Model.LoginModel.Action2.insert : Utility.Model.LoginModel.Action2.update, SourceTable: "SysUser");

                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, IsEnable == 1 ? "已啟用此帳號" : "已停用此帳號");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入帳號測試";
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入帳號測試");
            }
        }

        /// <summary>
        /// 刪除未登過入的帳號
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DelUser(string key)
        {
            logActionModel.SourceTable = "SysUser";
            logActionModel.Action2 = Utility.Model.LoginModel.Action2.delete;

            if (CommonUtility.UrlKey(ref key))
            {
                var data = new SysUser();
                data.UserID = key;
                data.ProcessDate = DateTime.UtcNow.AddHours(8);
                data.ProcessIPAddress = ContextModel.ProcessIpaddress;
                data.ProcessUserID = UserData != null ? UserData.sysUser.UserID : "";
                if (UserManagementService.DelUser(data, out int sn))
                {
                    logActionModel.SourceSN = sn;
                    SetLogActionModel(webPath: CommonUtility.Breadcrumb(8), Action2: Utility.Model.LoginModel.Action2.delete, SourceTable: "SysUser");
                    return StatusResult(System.Net.HttpStatusCode.OK,  "已刪除成功" );
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.OK,  "此帳號已有資料無法刪除");
                }
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入帳號測試";
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入帳號測試");
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendResetPwd(string key)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(8), Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "SysUserGroupModel");

            logActionModel.SourceTable = "SysUser";
            logActionModel.Action2 = Utility.Model.LoginModel.Action2.update;

            string _key;
            try
            {
                _key = CommonUtility.GetUrlAesDecrypt(key);
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入帳號測試";

                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入帳號測試");
            }
            var data = UserManagementService.GetUserData(_key);
            if (data == null)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入帳號測試";

                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入帳號測試");
            }
            logActionModel.SourceSN = data.SysUserSN;

            var data2 = UserManagementService.CheckForgetUser(data.UserID, data.Email);
            string msg;
            if (!data2.check)
            {
                msg = data2.message;
                logActionModel.status = LoginModel.Status.Error;
                logActionModel.response = msg;
                Log(logActionModel);

                return StatusResult(System.Net.HttpStatusCode.NotFound, data2.message);
            }
            else
            {
                msg = data2.message;
                logActionModel.status = LoginModel.Status.Scuess;
                logActionModel.response = msg;
                Log(logActionModel);

                MailUtility.Sendpwd(MailType.pwdreset, _hostingEnvironment.WebRootPath, out Exception ex, data2.sysUser);

                return StatusResult(System.Net.HttpStatusCode.OK, "已發送重置密碼信");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeUserPwd(SysUser data)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(8), Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "SysUserGroupModel");
            var aeskey = AppSettingHelper.GetAppsetting("AESKey")?.ToUpper();
            logActionModel.SourceTable = "SysUser";
            logActionModel.Action2 = Utility.Model.LoginModel.Action2.update;
            logActionModel.SourceSN = data.SysUserSN;
            data.ProcessUserID = UserData.sysUser.UserID;
            var chk = UserManagementService.UpdatePwd(data, aeskey);
            if (!chk.check)
            {
                logActionModel.status = LoginModel.Status.Error;
                logActionModel.response = chk.message;
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, chk.message);
            }
            else
            {
                logActionModel.response = "";
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, "已重設密碼");
            }
        }
        /// <summary>
        /// 修改帳號對應的群組
        /// </summary>
        /// <param name="editSysUserGroupModels"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSysUserGroup(List<EditSysUserGroupModel> editSysUserGroupModels)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(8), Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "SysUserGroupModel");

            try
            {
                foreach (var data in editSysUserGroupModels)
                {
                    data.UserID = CommonUtility.GetUrlAesDecrypt(data.UserID);
                    data.CreatedUserID = UserData != null ? UserData.sysUser.UserID : "";
                    data.CreatedIPAddress = ContextModel.ProcessIpaddress;

                }
                UserManagementService.EditUserToGroup(editSysUserGroupModels);

                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.Message;
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMode(SysUser data)
        {
            try
            {
                SetLogActionModel(
                    webPath: CommonUtility.Breadcrumb(8),
                    Action2: Utility.Model.LoginModel.Action2.update,
                    SourceTable: "SysUser",
                    SourceSN: data.SysUserSN);
                var key = data.UserID;

                if (CommonUtility.UrlKey(ref key))
                {
                    var aeskey = AppSettingHelper.GetAppsetting("AESKey")?.ToUpper();
                    data.UserID = key;
                    data.ProcessDate = DateTime.UtcNow.AddHours(8);
                    data.ProcessIPAddress = ContextModel.ProcessIpaddress;
                    data.ProcessUserID = UserData != null ? UserData.sysUser.UserID : "";
                    var soure = UserManagementService.EditModeUser(data, aeskey);
                    if (!soure.check)
                    {
                        logActionModel.status = Utility.Model.LoginModel.Status.Error;
                        logActionModel.response = soure.message;

                        return StatusResult(System.Net.HttpStatusCode.BadRequest, soure.message);
                    }
                    logActionModel.response = soure.message;
                    Log(logActionModel);

                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = "更新失敗";
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "錯誤");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "錯誤");
            }

        }

        #endregion
        public FileContentResult ExcelReport(string key = "", string dep = "", string states = "")
        {
            var fileName = $@"帳號管理{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xlsx";
            try
            {
                var path = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                DefaultPager pager = new DefaultPager();

                var list = UserManagementService.GetUserList("", "", key, dep, states, ref pager, true);
                var filePath = $@"{path}/Temp/UserReport.xlsx";
                var Info = new List<string>() {
                $@"製表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"製表人: {UserData?.sysUser.UserName}",
                $@"資料筆數: {list.Count()}"
            };
                var DetailTitle = new List<string>() {
                "帳號",
                "使用者",
                "單位",
                "E-mail",
                "最後修改密碼日期",
                "建立時間/最後上線時間",
                "狀態"
            };
                var EModel = new ExcelModel();
                EModel.Title = "帳號管理";
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;
                EModel.ExcelDetails = list.Select(x => new ExcelDetailModel()
                {
                    a = x.UserID,
                    b = x.UserName,
                    c = x.DepartmentName,
                    d = x.Email,
                    e = x.PwdLastUpdate.HasValue ? x.PwdLastUpdate.Value.ToString("yyyy-MM-dd") : "",
                    f = x.DateCreated.Value.ToString("yyyy-MM-dd") + "/" + (x.LastLoginDate.HasValue ? x.LastLoginDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""),
                    g = x.UserSatus == "1" ? "啟用" : "停用"
                }
                ).ToList();
                excelDatas = Utility.Output.ExampleReport(EModel, filePath);

                return File(excelDatas.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
            }
            catch (Exception ex)
            {
                IList<string> errmsgs = new List<string>();
                string error = "";
                error = ex.Message;
                errmsgs.Add(error);
                return File(System.Text.Encoding.UTF8.GetBytes(error), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
            }
        }


    }
}

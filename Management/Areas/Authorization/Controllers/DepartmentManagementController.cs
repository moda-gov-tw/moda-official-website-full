using DBModel;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Management.Areas.Authorization.Controllers
{
    [Area("Authorization")]
    public class DepartmentManagementController : BaseController
    {
        #region Page 
        /// <summary>
        /// 部門管理
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IActionResult Index(string key = "")
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(10);
            var chkUM = CheckUserMenu(10);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area=""});
            }

            Models.DepartmentManagement.IndexModel indexModel = new Models.DepartmentManagement.IndexModel();
            indexModel.SysDepartments = DepartmentManagementService.GetDepartmentList().Where(x => x.IsEnable == "1" && x.WebSiteId == UserData.WebSiteID).ToList();
            if (!string.IsNullOrEmpty(key))
            {
                indexModel.ParentID = key;
            }
            return View(indexModel);
        }
        /// <summary>
        /// 列表資料
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <param name="states"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult List(string key,string websiteid, string lang = "", string states = "", int p = 1, int DisplayCount = 10)
        {
            int ParentID = 0;
            string sKey = key;
            if (!string.IsNullOrEmpty(key))
            {
                if (CommonUtility.UrlKey(ref sKey))
                {
                    ParentID = int.Parse(sKey);
                }
            }
            else
            {
                key = CommonUtility.GetUrlAesEncrypt(ParentID.ToString());
            }

            var ListModels = new Models.DepartmentManagement.ListModel();
            var titles = new List<SysDepartment>();

            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            var lists = DepartmentManagementService.GetDepartmentByKeys(ParentID, websiteid, states, ref pager);
            DepartmentManagementService.GetParentTitle(ParentID, ref titles);
            ListModels.defaultPager = pager;
            ListModels.SysDepartments = lists;
            ListModels.Titles = titles;
            ListModels.ParentID = key;
            ListModels.SortList = DepartmentManagementService.GetDepartment(ParentID, websiteid);
            return View(ListModels);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public IActionResult Edit(string key, string key2)
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(10);
            int SysDepartmentSN = 0;
            var chkUM = CheckUserMenu(10);
            if (!chkUM.chk)
            {
                return RedirectToAction("ErrorCome", "Home", new { area = "" });
            }
            Models.DepartmentManagement.EditModel editModel = new Models.DepartmentManagement.EditModel();
            editModel.sysLangs = CommonService.GetSysWebSiteLang(UserData.WebSiteID);
            var titles = new List<SysDepartment>();

            if (!string.IsNullOrEmpty(key))
            {
                if (CommonUtility.UrlKey(ref key))
                {
                    SysDepartmentSN = int.Parse(key);
                    editModel.SysDepartments = DepartmentManagementService.GetDepartmentBySysDepartmentSN(SysDepartmentSN);
                }
            }
            if (!string.IsNullOrEmpty(key2))
            {
                var sfs = CommonUtility.UrlKey(ref key2);

                editModel.ParentID = key2;
            }

            DepartmentManagementService.GetParentTitle(int.Parse(editModel.ParentID), ref titles);
            editModel.Titles = titles;

            return View(editModel);
        }
        /// <summary>
        /// 使用者列表
        /// </summary>
        /// <param name="dep"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult UserList(string dep, int p = 1, int DisplayCount = 10)
        {
            var model = new Models.DepartmentManagement.UserListModel();
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            model.Users = DepartmentManagementService.GetDeptUsers(dep, ref pager);
            model.defaultPager = pager;
            return View(model);
        }
        #endregion
        #region HttpPost event 動作
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Save(SysDepartment sysDepartment)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(10), Action2: sysDepartment.SysDepartmentSN == 0 ? Utility.Model.LoginModel.Action2.insert : Utility.Model.LoginModel.Action2.update, SourceTable: "SysDepartment");

            try
            {
                var soure = new sysDepartmentModel();
                sysDepartment.WebSiteId = UserData.WebSiteID;
                sysDepartment.ProcessDate = DateTime.UtcNow.AddHours(8);
                sysDepartment.ProcessUserID = UserData.sysUser.UserID;
                sysDepartment.ProcessIPAddress = UserData.sysUser.ProcessIPAddress;
                if (sysDepartment.SysDepartmentSN == 0)
                {
                    sysDepartment.CreatedDate = DateTime.UtcNow.AddHours(8);
                    sysDepartment.CreatedUserID = UserData.sysUser.UserID;

                    if (sysDepartment.MainSN == 0) 
                    {
                        soure = DepartmentManagementService.Create(sysDepartment, UserData.WebSiteID);
                        logActionModel.SourceSN = sysDepartment.SysDepartmentSN;
                    }
                    else
                    {
                        soure = DepartmentManagementService.CreateSubLang(sysDepartment);
                        logActionModel.SourceSN = sysDepartment.SysDepartmentSN;
                    }
                }
                else
                {
                    logActionModel.SourceSN = sysDepartment.SysDepartmentSN;
                    soure = DepartmentManagementService.Edit(sysDepartment);
                }

                if (soure.check)
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Scuess;
                    logActionModel.response = soure.message;
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = soure.message;
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, soure.message);
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }

        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(string key)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(10), Action2: Utility.Model.LoginModel.Action2.delete, SourceTable: "SysDepartment");
            int SysDepartmentSN = 0;
            try
            {
                var soure = new sysDepartmentModel();
                if (CommonUtility.UrlKey(ref key))
                {
                    SysDepartmentSN = int.Parse(key);
                    logActionModel.SourceSN = SysDepartmentSN;
                    soure = DepartmentManagementService.Delete(SysDepartmentSN);
                }
                if (soure.check)
                {
                    logActionModel.response = soure.message;
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = soure.message;
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, soure.message);
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
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult StopDept(string key)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(10), Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "SysDepartment");

            try
            {
                if (CommonUtility.UrlKey(ref key))
                {
                    logActionModel.SourceSN = int.Parse(key);
                    //bool r = DepartmentManagementService.s
                    var data = new SysDepartment();
                    data.SysDepartmentSN = int.Parse(key);
                    data.ProcessDate = DateTime.UtcNow.AddHours(8);
                    data.ProcessIPAddress = ContextModel.ProcessIpaddress;
                    data.ProcessUserID = UserData != null ? UserData.sysUser.UserID : "";
                    bool r = DepartmentManagementService.StopDept(data);
                    if (r)
                    {
                        logActionModel.status = Utility.Model.LoginModel.Status.Scuess;
                        logActionModel.response = "已停用此部門";

                        return StatusResult(System.Net.HttpStatusCode.OK, "已停用此部門");
                    }
                    else
                    {
                        logActionModel.status = Utility.Model.LoginModel.Status.Error;
                        logActionModel.response = "停用部門失敗";
                        return StatusResult(System.Net.HttpStatusCode.BadRequest, "停用部門失敗");
                    }
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    logActionModel.response = "停用部門失敗";
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "停用部門失敗");
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
        /// BankNote重新排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeptReArrange(string key, string sort)
        {
            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key) && int.TryParse(sort, out int _sort))
            {
                DepartmentManagementService.DeptReArrangeByChild(_key, _sort, UserData.sysUser.UserID, UserData.sysUser.ProcessIPAddress);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
            }
        }
        #endregion
    }
}

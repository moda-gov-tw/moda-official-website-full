using DBModel;
using Management.Areas.SystemManageMent.Models.CodeManagement;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services.SystemManageMent;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Management.Areas.SystemManageMent.Controllers
{
    [Area("SystemManageMent")]
    public class CodeManagementController : BaseController
    {
        public IActionResult Index(string key = "", string key2 = "")
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(13);
            if (!CheckUserMenu(13).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            IndexModel indexModel = new IndexModel();

            if (!string.IsNullOrEmpty(key))
            {
                indexModel.ParentKey = key;
            }
            if (!string.IsNullOrEmpty(key2))
            {
                indexModel.WebSiteID = key2;
            }
            indexModel.sysCategories = CodeManagementService.GetCategoryList(UserData.WebSiteID, UserData.GodMode).Where(x=>x.Lang =="zh-tw").ToList();
            return View(indexModel);
        }
        public IActionResult List(string key, string key2 = "",string key3 = "", int p = 1, int DisplayCount = 10)
        {
            Models.CodeManagement.ListModel listModel = new Models.CodeManagement.ListModel();
            var language = CodeManagementService.GetSysWebSiteLangs(UserData);

            List<DefaultPager> pagers = new List<DefaultPager>();

            if (!string.IsNullOrEmpty(key))
            {
                CommonUtility.UrlKey(ref key);
            }
            else
            {
                key = "";
            }
            if (!string.IsNullOrEmpty(key2))
            {
                CommonUtility.UrlKey(ref key2);
            }
            else
            {
                key2 = "";
            }

            foreach (var lan in language)
            {
                var Page = CodeManagementService.GetCategoryPage(key, key2, key3, lan.Lang,p,DisplayCount);
                pagers.Add(Page);
            }
           
            var titles = new List<SysCategory>();
            var lists = CodeManagementService.GetCategoryByParentKeys(key, key2,key3, p , DisplayCount, language);
            CodeManagementService.GetParentTitle(key, ref titles);
            listModel.CanDelete = CodeManagementService.CheckCanDelete(key, UserData.WebSiteID);
            listModel.Titles = titles;
            listModel.defaultPager = pagers;
            listModel.sysCategories = lists;
            listModel.ParentKey = key;
            listModel.WebSiteID = key2;
            listModel.sysWebSiteLangs = language;
            listModel.SortList = CodeManagementService.GetCategory(key, key2);
            return View(listModel);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="key">SysCategoryKey</param>
        /// <param name="key2">WebSiteID</param>
        /// <param name="key3">ParentKey</param>
        /// <returns>讀取單一資料</returns>
        public IActionResult Edit(string sn, string key, string key2, string key3)
        {
            Models.CodeManagement.EditModel editModel = new Models.CodeManagement.EditModel();

            CommonUtility.UrlKey(ref key2);
            CommonUtility.UrlKey(ref key3);
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(key2) && !string.IsNullOrEmpty(sn))
            {
                CommonUtility.UrlKey(ref sn);
                CommonUtility.UrlKey(ref key);

                editModel.SysCategory = CodeManagementService.GetCategoryList(UserData.WebSiteID, UserData.GodMode)
                   .First(x => x.SysCategoryKey == key && x.WebSiteID == key2 && x.SysCategorySN == int.Parse(sn));
            }
            else if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(key2))
            {
                CommonUtility.UrlKey(ref key);

                editModel.SysCategory = CodeManagementService.GetCategoryList(UserData.WebSiteID, UserData.GodMode).OrderBy(x => x.SysCategorySN)
                    .First(x => x.SysCategoryKey == key && x.WebSiteID == key2);
            }
            if (!string.IsNullOrEmpty(sn))
            {
                editModel.SysCategory.SysCategorySN = int.Parse(sn);
            }
            editModel.ParentKey = key3;
            editModel.WebSiteID = key2;
            return View(editModel);
        }
        /// <summary>
        /// 儲存
        /// </summary>
        /// <param name="sysCategory"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(SysCategory sysCategory, string isUpdate)
        {
            SetLogActionModel(webPath: CommonUtility.Breadcrumb(13), Action2: Utility.Model.LoginModel.Action2.update, SourceTable: "SysCategory");

            try
            {
                sysCategory.ProcessDate = DateTime.UtcNow.AddHours(8);
                sysCategory.ProcessUserID = UserData.sysUser.UserID;
                sysCategory.ProcessIPAddress = UserData.sysUser.ProcessIPAddress;

                var checkMissData = Services.CheckModel.CheckedData.CheckedCategory(ref sysCategory);
                if (!checkMissData.chk)
                {
                    return StatusResult(System.Net.HttpStatusCode.InternalServerError, checkMissData.error);
                }
                bool soure = true;
                if (isUpdate == "0")//Insert
                {
                    sysCategory.CreatedDate = DateTime.UtcNow.AddHours(8);
                    sysCategory.CreatedUserID = UserData.sysUser.UserID;
                    logActionModel.Action2 = Utility.Model.LoginModel.Action2.insert;
                    soure = CodeManagementService.Create(sysCategory);
                }
                else
                {
                    logActionModel.Action2 = Utility.Model.LoginModel.Action2.update;
                    soure = CodeManagementService.Edit(sysCategory);
                }

                if (soure)
                {
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    logActionModel.status = Utility.Model.LoginModel.Status.Error;
                    Log(logActionModel);
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗");
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗");
            }

        }

        /// <summary>
        /// 重新排序
        /// </summary>
        /// <param name="key">SysCategoryKey</param>
        /// <param name="key2">WebSiteID</param>
        /// <param name="sort">欲調整至序號</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CategoryReArrange(string key, string key2, string sort)
        {
            if (CommonUtility.UrlKey(ref key) && CommonUtility.UrlKey(ref key2) && int.TryParse(sort, out int _sort))
            {
                CodeManagementService.CategoryReArrangeByChild(key, key2, _sort, UserData.sysUser.UserID, UserData.sysUser.ProcessIPAddress);
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請別亂輸入測試");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(string ck)
        {
            try
            {
                SetLogActionModel(webPath: CommonUtility.Breadcrumb(13), Action2: Utility.Model.LoginModel.Action2.delete, SourceTable: "SysCategory");
                CodeManagementService.Delate(ck, UserData.sysUser.UserID);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception)
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BindingNoLang()
        {
            try
            {
                CodeManagementService.BindingNoLang();
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception)
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "更新失敗");
            }
        }
    }
}

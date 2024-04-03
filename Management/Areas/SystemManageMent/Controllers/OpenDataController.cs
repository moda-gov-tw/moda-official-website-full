using DBModel;
using Management.Areas.SystemManageMent.Models.OpenData;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services.SystemManageMent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Utility;
using static Utility.Files;

namespace Management.Areas.SystemManageMent.Controllers
{
    [Area("SystemManageMent")]
    public class OpenDataController : BaseController
    {

        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(23);
            if (!CheckUserMenu(23).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            return View();
        }

        public IActionResult List(string keyword = "", int p = 1, int DisplayCount = 10)
        {
           string WEBSiteUrl = "https://" + new Uri(AppSettingHelper.GetAppsetting("WEBAPI")).Host + "/OpenData";

            WEBOpenDataModel opneDataModel = new();
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            opneDataModel.wEBOpenDataMains = OpenDataService.GetOpenData(keyword,UserData.WebSiteID, ref pager);
            opneDataModel.FileUrl = OpenDataService.FileUrl(opneDataModel.wEBOpenDataMains, WEBSiteUrl);
            opneDataModel.defaultPager = pager;
            opneDataModel.SortList = OpenDataService.GetOpenDataSort(UserData.WebSiteID);
            return View(opneDataModel);
        }

        public IActionResult Mode(string key)
        {
            WEBOpenDataEditModel wEBOpenDataEdit = new();
            var fileData = new List<CommonFileModel>();
            var Auth = Services.Authorization.GroupManagementService.GetSysUserGroup(UserData.sysUser.UserID, 1);//權限
            if (!string.IsNullOrWhiteSpace(key))
            {
                SetSession("WEBFile", new List<CommonFileModel>());

                var Main = OpenDataService.GetOpendataMain(key);
                var Detail = OpenDataService.GetOpendataDetail(key);
                var Schema = OpenDataService.GetOpendataSchema(key);
                var objects = OpenDataService.GetSchemasObject(key);
                var Extend = Services.Authorization.WebLevelManagementService.GetWEBNewsExtends(0).Where(x => x.Column_1 == key).FirstOrDefault();

                var data = (from d in objects.AsEnumerable()
                            select new ExpandoObject
                            {
                                OpenDataMainSN = d.Field<int>("WEBOpenDataMainSN"),
                                OpenDataDetailSN = d.Field<int>("OpenDataDetailSN"),
                                Code = d.Field<string>("Code"),
                                TableName = d.Field<string>("TABLE_NAME"),
                                Column = d.Field<string>("COLUMN_NAME"),
                                SortOrder = d.Field<int>("SortOrder"),
                            }).OrderBy(x => x.SortOrder).ToList();

                wEBOpenDataEdit.wEBOpenDataMain = Main;
                wEBOpenDataEdit.wEBOpenDataDetails = Detail;
                wEBOpenDataEdit.wEBOpenDataSchemas = Schema;
                wEBOpenDataEdit.SchemasObject = data;
                wEBOpenDataEdit.wEBNewsExtend = Extend;

                var files = CommonUtility.GetFileByDB(key, "WEBOpenDataMain");

                if (files != null)
                {
                    fileData.AddRange(files);
                    wEBOpenDataEdit.commonFileModels.AddRange(files);
                }
            }
            wEBOpenDataEdit.relSysUserGroup = Auth;
            wEBOpenDataEdit.sysUserSysDepartmentID = UserData.sysUser.DepartmentID;
            return View(wEBOpenDataEdit);
        }

        public IActionResult Save(WEBOpenDataMain wEBOpenDataMain, List<WEBOpenDataDetail> wEBOpenDataDetail, List<WEBOpenDataSchema> wEBOpenDataSchema,WEBNewsExtend wEBNewsExtend, List<CommonFileModel> commonFileModels = null)
        {
            try
            {
                string state = "";
                var files = GetSession<List<CommonFileModel>>("WEBFile");
                wEBOpenDataMain.ProcessUserID = UserData.sysUser.UserID;
                wEBOpenDataMain.DateCreated = DateTime.UtcNow.AddHours(8);
                wEBOpenDataMain.ProcessDate = DateTime.UtcNow.AddHours(8);
                wEBOpenDataMain.WebSiteID = UserData.WebSiteID;

                if (commonFileModels.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var fileModel = commonFileModels.FirstOrDefault(x => x.fileNewName == file.fileNewName);
                        if (fileModel != null)
                        {
                            file.fileTitle = fileModel.fileTitle;
                            file.FileSort = fileModel.FileSort;
                        }
                    }
                }

                if (OpenDataService.SaveOpenData(ref wEBOpenDataMain, wEBOpenDataDetail, wEBOpenDataSchema,wEBNewsExtend, files, ref state))
                {
                    SetSession("WEBFile", new List<CommonFileModel>());
                    return StatusResult(System.Net.HttpStatusCode.OK, state);
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, state);
                }
            }
            catch (Exception ex)
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                return StatusResult(System.Net.HttpStatusCode.BadRequest, Utility.Model.LoginModel.ServieError);
            }
        }

        public IActionResult Delete(string key)
        {

            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key))
            {
                var sorce = OpenDataService.DeleteOpenData(key, UserData.sysUser.UserID);
                if (sorce == true)
                {
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "刪除失敗");
                }
            }
            else
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "刪除失敗");
            }
        }

        /// <summary>
        /// OPENDATA
        /// </summary>
        /// <param name="key">OPENDATA</param>
        /// <param name="sort">欲調整至序號</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewsReArrange(string key, string sort,string websiteid)
        {
            if (CommonUtility.UrlKey(ref key) && int.TryParse(key, out int _key) && int.TryParse(sort, out int _sort))
            {
                OpenDataService.NewsReArrangeByChild(_key, _sort, websiteid, UserData.sysUser.UserID);
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                logActionModel.status = Utility.Model.LoginModel.Status.Error;
                logActionModel.response = "請別亂輸入測試";
                Log(logActionModel);
                return RedirectToAction("Index", "UserManagement", new { area = "Authorization", msg = "請別亂輸入測試" });
            }
        }
    }
}

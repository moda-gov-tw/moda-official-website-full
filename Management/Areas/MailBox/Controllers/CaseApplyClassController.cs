using DBModel;
using Management.ManagementUtility;
using Management.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Services.ModaMailBox;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Management.Areas.MailBox.Controllers
{
    [Area("MailBox")]
    public class CaseApplyClassController : BaseController
    {
        /// <summary>
        /// 分類管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(27);
            if (!CheckUserMenu(27).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            var websiteId = UserData.WebSiteID;
            var IndexModels = new Models.CaseApplyClass.IndexModel();
            IndexModels.casesModels = MailBoxService.GetCases("0").Where(x => x.WebSiteID == websiteId).ToList();
            return View(IndexModels);
        }
        /// <summary>
        /// 列表資料
        /// </summary>
        /// <param name="CaseApplyClassSN"></param>
        /// <param name="dep"></param>
        /// <param name="keyword"></param>
        /// <param name="IsEnable"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult List(string CaseApplyClassSN = "", string dep = "", string keyword = "", string IsEnable = "", int p = 1, int DisplayCount = 10)
        {
            var _dep = 0;
            int.TryParse(dep, out _dep);
            var ListModels = new Management.Areas.MailBox.Models.CaseApplyClass.ListModel();
            var websiteId = UserData.WebSiteID;
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            var lists = MailBoxService.GetGroupList(websiteId, "", ref pager, _dep, keyword, CaseApplyClassSN, IsEnable);
            ListModels.defaultPager = pager;
            ListModels.caseApplyClassModel = lists;
            ListModels.sysCategories = MailBoxService.GetSysCategory();
            ListModels.ParentClass = MailBoxService.GetParentClass().FirstOrDefault(x => x.WebSiteID == websiteId && x.Value == "1");
            return View(ListModels);
        }
        /// <summary>
        /// 詳細資料
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public IActionResult Detail(string SN = "")
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(27);
            if (!CheckUserMenu(27).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            var viewData = new Models.CaseApplyClass.DetailModel();
            try
            {
                var websiteId = UserData.WebSiteID;
                viewData.ParentClass = MailBoxService.GetParentClass();
                if (string.IsNullOrWhiteSpace(SN))
                {
                    return View(viewData);
                }
                else
                {
                    var _sn = 0;
                    if (int.TryParse(SN, out _sn))
                    {
                        viewData.caseApplyClass = MailBoxService.GetCaseApplyClass(websiteId, _sn);
                        viewData.caseApplyClassTos = MailBoxService.GetCaseApplyClassTos(_sn);
                    }
                    return View(viewData);
                }
            }
            catch (Exception)
            {
                return View(viewData);
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="caseApplyClass"></param>
        /// <param name="CCTo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveCaseApplyClass(CaseApplyClass caseApplyClass, List<SelectTxt> CCTo = null)
        {
            var Action2 = caseApplyClass.CaseApplyClassSN == 0 ? Utility.Model.LoginModel.Action2.insert : Utility.Model.LoginModel.Action2.update;
            var missMsg = new List<string>();
            var ParentClass = MailBoxService.GetParentClass().FirstOrDefault(x => x.WebSiteID == UserData.WebSiteID && x.Value == "1");
            if (string.IsNullOrWhiteSpace(caseApplyClass.CaseNo))
            {
                missMsg.Add("意見分類代號");
            }
            if (string.IsNullOrWhiteSpace(caseApplyClass.CaseName))
            {
                missMsg.Add("意見分類");
            }
            if (caseApplyClass.SysDepartmentSN == null)
            {
                missMsg.Add("請選擇承辦單位");
            }
            if (ParentClass != null && caseApplyClass.SysCategoryKey == null)
            {
                missMsg.Add("請選擇民意信箱分類");
            }
            if (missMsg.Count() > 0)
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest,  string.Join("/n", missMsg) );
            }
            SetLogActionModel(Action2: Action2, SourceTable: "CaseApplyClass");
            var CaseApplyClassToData = GetCaseApplyClassTo(CCTo);
            caseApplyClass.CreatedUserID = UserData.sysUser.UserID;
            caseApplyClass.CreatedDate = DateTime.UtcNow.AddHours(8);
            caseApplyClass.WebSiteID = UserData.WebSiteID;
            caseApplyClass.ProcessUserID = UserData.sysUser.UserID;
            caseApplyClass.ProcessDate = DateTime.UtcNow.AddHours(8);
            caseApplyClass.ProcessIPAddress = ContextModel.ProcessIpaddress;
            if (MailBoxService.Save(caseApplyClass, CaseApplyClassToData))
            {
                logActionModel.webPath = "民意信箱/意見分類";
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "資料異常");
            }
        }

        static List<CaseApplyClassTo> GetCaseApplyClassTo(List<SelectTxt> lis)
        {
            List<CaseApplyClassTo> result = new();

            foreach (var item in lis) 
            {
                if (item.txt != null && item.val != null) 
                {
                    result.Add(new CaseApplyClassTo()
                    {
                        Name = item.txt,
                        Email = item.val,
                    });
                }
            }
            return result;
        }
    }
}

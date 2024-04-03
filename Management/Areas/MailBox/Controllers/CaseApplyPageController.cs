using DBModel;
using Management.Areas.MailBox.Models.CaseApplyPage;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services.ModaMailBox;
using System.Collections.Generic;
using static Utility.Files;

namespace Management.Areas.MailBox.Controllers
{
    [Area("MailBox")]
    public class CaseApplyPageController : BaseController
    {
        /// <summary>
        /// 民意信箱頁面列表頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(38);
            if (!CheckUserMenu(38).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }

            IndexModel model = new() 
            {
                CaseApplyPages = MailBoxService.GetCaseApplyPages()
            };
            return View(model);
        }
        /// <summary>
        /// 民意信箱頁面內容頁
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        public IActionResult Detail(int sn)
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(38);
            if (!CheckUserMenu(38).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            SetSession("WEBFile", null);
            var Imgs = CommonUtility.GetFileByDB(sn.ToString(), "CaseApplyPage");
            SetSession("WEBFile", Imgs);

            DetailModel model = new()
            {
                Page = MailBoxService.GetCaseApplyPage(sn),
                PageExtends = MailBoxService.GetCaseApplyPageExtends(sn),
                PageImgs = Imgs
            };
            return View(model);
        }
        /// <summary>
        /// 儲存民意信箱頁面資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(CaseApplyPage page, List<CaseApplyPageExtend> extends) 
        {
            page.ProcessUser = UserData.sysUser.UserID;
            extends.ForEach(x => x.ProcessUser = UserData.sysUser.UserID);
            var Imgs = GetSession<List<CommonFileModel>>("WEBFile");

            if (MailBoxService.EditCaseApplyPage(page, extends, Imgs, out string errorMsg))
            {
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else 
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, errorMsg);
            }
        }
    }
}

using DBModel;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Mvc;
using Services.ModaMailBox;
using System.Linq;
using Utility;
using static Utility.Files;

namespace Management.Areas.MailBox.Controllers
{
    [Area("MailBox")]
    public class CaseReconfirmController : BaseController
    {
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(37);
            if (!CheckUserMenu(28).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            var websiteId = UserData.WebSiteID;
            var viewModel = new Management.Areas.MailBox.Models.CaseReconfirm.IndexModel();
            viewModel.CaseClassList = MailBoxService.GetCases("0").Where(x => x.WebSiteID == websiteId).ToList();
            return View(viewModel);
        }

        public IActionResult List(string strDate = "", string endDate = "", string CaseApplyClassSN = "", string dep = "", string keyword = "", int p = 1, int DisplayCount = 10)
        {
            var _dep = 0;
            int.TryParse(dep, out _dep);
            var ListModels = new Management.Areas.MailBox.Models.CaseReconfirm.ListModel();
            var websiteId = UserData.WebSiteID;
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            var lists = MailBoxService.GetCaseReconfirm(websiteId, strDate, endDate, ref pager, _dep, keyword, CaseApplyClassSN);
            ListModels.defaultPager = pager;
            ListModels.CaseApplyList = lists;
            return View(ListModels);
        }

        public IActionResult Detail(string CaseNo)
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(37);
            if (!CheckUserMenu(28).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            var detail = MailBoxService.GetCaseApply(CaseNo);
            var files = MailBoxService.GetCaseApplyFiles("CaseApply", detail.CaseApplySN, "MailBox");

            var viewData = new Management.Areas.MailBox.Models.CaseReconfirm.DetailModel();
            viewData.caseApply = detail;
            viewData.WebFileList = files;

            viewData.CaseClass = MailBoxService.GetCase(detail.CaseApplyClassSN);
            return View(viewData);
        }

        public IActionResult Resendconfirm(string CaseNo)
        {
            var Case = MailBoxService.GetCaseApply(CaseNo);
            if (Case == null)
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "查無資料");
            }
            else
            {
                var EncodeKey = AppSettingHelper.GetAppsetting("DataProtectionKey:confirmKey");

                if (Services.ModaMailBox.MailBox.SendConfirmMail(Case, EncodeKey))
                {
                    return StatusResult(System.Net.HttpStatusCode.OK, "請民眾查收案件確認信");
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.OK, "請民眾查收案件確認信");
                }    
            }
        }

        public IActionResult ValidateList(string strDate = "", string endDate = "", string keyword = "", int p = 1, int DisplayCount = 10)
        {
            var ListModels = new Management.Areas.MailBox.Models.CaseReconfirm.ValidateListModel();
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            pager.key = "b";
            var lists = MailBoxService.GetCaseValidate(strDate, endDate, ref pager, keyword);
            ListModels.defaultPager = pager;
            ListModels.CaseValidateList = lists;
            return View(ListModels);
        }
    }
}

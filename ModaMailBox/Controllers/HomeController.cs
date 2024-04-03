using DBModel;
using Microsoft.AspNetCore.Mvc;
using ModaMailBox.Models;
using Services.ModaMailBox;
using Utility;
using static Utility.Files;

namespace ModaMailBox.Controllers
{
    [Route("/[action]")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        #region View
        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        [Route("/")]
        [Route("/[action]")]
        [Route("/[controller]/[action]")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 處理流程
        /// </summary>
        /// <returns></returns>
        public IActionResult ProcessingFlow() 
        {
            return View();
        }
        /// <summary>
        /// 寫信給我們
        /// </summary>
        /// <returns></returns>
        public IActionResult MailToUs()
        {
            return View();
        }
        /// <summary>
        /// 認證電子信箱
        /// </summary>
        /// <returns></returns>
        public IActionResult VerifyEmail()
        {
            var EffectiveHours = AppSettingHelper.GetAppsetting("EffectiveHours");
            int.TryParse(EffectiveHours, out int addHours);
            ViewBag.addHours = addHours;
            return View();
        }
        /// <summary>
        /// 認證信已寄送
        /// </summary>
        /// <returns></returns>
        public IActionResult Certification()
        {
            return View();
        }
        /// <summary>
        /// 填寫意見內容
        /// </summary>
        /// <returns></returns>
        public IActionResult WriteMail(string token)
        {
            var writemailModel = new WriteMailModel();

            if (string.IsNullOrWhiteSpace(token))
            {
                writemailModel.Msg = "頁面已失效，請重新操作";
            }
            else
            {
                if (MailBox.MailValidate(token, out CaseApplyValidate ValidateData, out string errorMsg))
                {
                    writemailModel.CaseApplyValidate = ValidateData;
                    writemailModel.CasesModels = MailBoxService.GetCases();
                    writemailModel.CasesModelViewItems = writemailModel.CasesModels.Select(x => new CasesModelViewItem() { cn = x.CaseName, sck = x.SysCategoryKey, sn = x.CaseApplyClassSN, wid = x.WebSiteID }).ToList();
                    writemailModel.SysCategory = MailBoxService.GetSysCategory();
                    writemailModel.ParentClass = MailBoxService.GetParentClass();

                    var tempData = GetSession<CaseApply>("CaseApply");
                    if (tempData == null)
                    {
                        CaseApply caseApply = new CaseApply();
                        caseApply.ContactEmail = ValidateData.Email;
                        SetSession("CaseApply", caseApply);
                    }
                    writemailModel.CaseApply = tempData;
                    writemailModel.CaseApplyClassSN = tempData != null ? tempData.CaseApplyClassSN : 0;
                }
                else
                {
                    writemailModel.Msg = errorMsg;
                }
            }
            return View(writemailModel);
        }
        /// <summary>
        /// 確認意見內容
        /// </summary>
        /// <returns></returns>
        public IActionResult ConfirmMail()
        {
            ConfirmMailModel confirmmailModel = new ConfirmMailModel();
            var tempData = GetSession<CaseApply>("CaseApply");
            var tempFile = GetSession<List<SaveFileModel>>("TempFile");
            confirmmailModel.casesModels = MailBoxService.GetCases();
            confirmmailModel.SysCategory = MailBoxService.GetSysCategory();
            confirmmailModel.ParentClass = MailBoxService.GetParentClass();
            if (tempData == null)
            {
                return Redirect("/");
            }
            else
            {
                confirmmailModel.CaseApply = tempData;
                confirmmailModel.CaseFiles = tempFile;
                confirmmailModel.CaseApplyValidate = MailBoxService.GetCaseApplyValidate(tempData.CaseValidateSN);
            }
            return View(confirmmailModel);
        }
        /// <summary>
        /// Sitemap
        /// </summary>
        /// <returns></returns>
        public IActionResult Sitemap()
        {
            return View();
        }

        #region 取消確認信流程
        /// <summary>
        /// 發送確認信件 step 6 A-6-1 & A-6-2
        /// </summary>
        /// <returns></returns>
        //public IActionResult SubmitCase()
        //{
        //    caseApplyWeb.Title = "發送確認信件｜" + caseApplyWeb.Title;
        //    SubmitCaseModel submitcaseModel = new SubmitCaseModel();
        //    var tempCase = GetSession<CaseApply>("CaseApply");
        //    var tempFile = GetSession<List<SaveFileModel>>("TempFile");
        //    var addDays = AppSettingHelper.GetAppsetting("EffectiveDays");
        //    var EncodeKey = AppSettingHelper.GetAppsetting("DataProtectionKey:confirmKey");
        //    var FileApiUrl = AppSettingHelper.GetAppsetting("FileServiceApi");
        //    var WebApiUrl = AppSettingHelper.GetAppsetting("WEBAPI");
        //    if (tempCase == null)
        //    {
        //        submitcaseModel.Msg = "頁面已失效，請重新操作";
        //    }
        //    else if (MailBox.CreateCase(tempCase, tempFile, addDays, EncodeKey, FileApiUrl, WebApiUrl))
        //    {

        //        submitcaseModel.Msg = "您的意見已送出申請，請至E-mail確認申請";
        //    }

        //    SetSession("CaseApply", null);
        //    SetSession("TempFile", null);
        //    submitcaseModel.AddDays = addDays == null ? "3" : addDays;
        //    return View(submitcaseModel);
        //}
        /// <summary>
        /// 確認信已寄送 step 7 - finish A-7-1 & A-7-2
        /// </summary>
        /// <returns></returns>
        //public IActionResult ConfirMation(string token)
        //{
        //    caseApplyWeb.Title = "確認信已寄送｜" + caseApplyWeb.Title;
        //    ConfirmationModel confirmationModel = new ConfirmationModel();
        //    try
        //    {
        //        var actionKey = AppSettingHelper.GetAppsetting("DataProtectionKey:confirmKey");

        //        if (MailBox.ConfirmCase(token, actionKey, out string errorMsg))
        //        {

        //        }
        //        else 
        //        {
        //            confirmationModel.msg = errorMsg;
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        confirmationModel.msg = "請重新申請";
        //    }
        //    return View(confirmationModel);
        //}
        /// <summary>
        /// 重寄確認信 B-1
        /// </summary>
        /// <returns></returns>
        //public IActionResult ResendConfirm()
        //{
        //    caseApplyWeb.Title = "重寄確認信｜" + caseApplyWeb.Title;
        //    return View();
        //}

        /// <summary>
        /// 重新發送確認信件 
        /// </summary>
        /// <returns></returns>
        //public IActionResult ResendConfirmAtionletter()
        //{
        //    caseApplyWeb.Title = "重新發送確認信件｜" + caseApplyWeb.Title;
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> ResendConfirmAsync(string email, string cftoken)
        //{
        //    if (!await GetTurnstileAsync(cftoken))
        //    {
        //        return StatusResult(System.Net.HttpStatusCode.BadRequest, "驗證碼已失效，請重新送出");
        //    }

        //    var EncodeKey = AppSettingHelper.GetAppsetting("DataProtectionKey:confirmKey");

        //    if (MailBox.ResendLastConfirmMail(email, EncodeKey, out string errorMsg))
        //    {
        //        return StatusResult(System.Net.HttpStatusCode.OK, "請查收案件確認信");
        //    }
        //    else
        //    {
        //        return StatusResult(System.Net.HttpStatusCode.BadRequest, errorMsg);
        //    }
        //}
        #endregion

        /// <summary>
        /// 案件已成立
        /// </summary>
        public IActionResult Confirmation()
        {
            ConfirmationModel confirmationModel = new ConfirmationModel();

            var tempCase = GetSession<CaseApply>("CaseApply");
            var tempFile = GetSession<List<SaveFileModel>>("TempFile");
            var addDays = AppSettingHelper.GetAppsetting("EffectiveDays");
            var EncodeKey = AppSettingHelper.GetAppsetting("DataProtectionKey:confirmKey");
            var FileApiUrl = AppSettingHelper.GetAppsetting("FileServiceApi");
            var WebApiUrl = AppSettingHelper.GetAppsetting("WEBAPI");
            if (tempCase == null)
            {
                confirmationModel.msg = "頁面已失效，請重新操作";
                return View(confirmationModel);
            }
            MailBox.CreateCase(tempCase, tempFile, addDays, EncodeKey, FileApiUrl, WebApiUrl);

            SetSession("CaseApply", null);
            SetSession("TempFile", null);

            return View(confirmationModel);
        }

        /// <summary>
        /// 查詢案件進度
        /// </summary>
        public IActionResult QueryCase()
        {
            SetSession("querycase", null);
            return View();
        }
        /// <summary> 
        /// 查詢進度結果頁
        /// </summary>
        public IActionResult CaseStatus()
        {
            CaseStatusModel casestatusModel = new CaseStatusModel();
            var caseData = GetSession<CaseApply>("querycase");
            if (caseData == null)
            {
                return Redirect("/");
            }
            casestatusModel.caseApply = caseData;
            casestatusModel.presetReply = MailBoxService.GetPresetReply();
            SetSession("MBN", caseData.CaseApplySN);
            return View(casestatusModel);
        }

        /// <summary>
        /// 重寄案件受理通知信頁面
        /// </summary>
        public IActionResult ResendLetter()
        {
            return View();
        }
        #endregion

        #region 滿意度調查
        /// <summary>
        /// 滿意度調查查詢頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Survey()
        {
            var web = MailBoxService.GetCaseApplyWeb();
            var surveymodel = new SurveyModel();
            if (web != null && !web.Satisfaction)
            {
                surveymodel.Msg = "滿意度調查未開放";
            }

            return View(surveymodel);
        }
        /// <summary>
        /// 依案件編號及檢查碼查詢滿意度調查
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SurveySearch(string caseNo, string casePwd, string cfToken)
        {
            MailBox.checkmodel checkModel = new() { focusOn = "" };
            var error = new List<string>();
            var caseApply = new CaseApply();

            if (!GetTurnstileAsync(cfToken).GetAwaiter().GetResult())
            {
                error.Add("驗證碼已失效，請重新送出");
                checkModel.errormsg = String.Join("<br/>", error);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, checkModel);
            }
            if (caseNo == null)
            {
                checkModel.focusOn = checkModel.focusOn ?? "CaseNo";
                error.Add("請填寫「案件編號」");
            }
            if (casePwd == null)
            {
                checkModel.focusOn = checkModel.focusOn ?? "CasePwd";
                error.Add("請填寫「案件檢查碼」");
            }

            if (error.Count() == 0)
            {
                caseApply = MailBoxService.GetCaseApply(caseNo, casePwd);
                if (caseApply == null)
                {
                    checkModel.errormsg = "查無案件編號";
                    checkModel.focusOn = "CaseNo";

                }
                else if (!(caseApply.Status == EnumTpye.GetEnumNumberToSting(Utility.MailBox.EnumCassApplyStatus.step12) ||
                           caseApply.Status == EnumTpye.GetEnumNumberToSting(Utility.MailBox.EnumCassApplyStatus.step13) ||
                           caseApply.Status == EnumTpye.GetEnumNumberToSting(Utility.MailBox.EnumCassApplyStatus.step14) ||
                           caseApply.Status == EnumTpye.GetEnumNumberToSting(Utility.MailBox.EnumCassApplyStatus.step15)))
                {
                    checkModel.errormsg = "案件處理中，完成後方會開放滿意度調查";
                }
                else if (MailBoxService.CheckSurveyExists(caseApply.CaseApplySN))
                {
                    checkModel.errormsg = "本案已填寫過滿意度調查問卷";
                }
                else if ((caseApply.ReplySource == EnumTpye.GetEnumNumberToSting(Utility.MailBox.EnumReplySource.Speed) ? caseApply.ReplyDate : caseApply.ReplySource2Date).Value.AddDays(20) < DateTime.UtcNow.AddHours(8))
                {
                    checkModel.errormsg = "已超過本案滿意度調查有效時間";
                }
            }
            else
            {
                checkModel.errormsg = String.Join("<br/>", error);
            }

            if (!string.IsNullOrEmpty(checkModel.errormsg))
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, checkModel);
            }
            else
            {
                SetSession("surveyCase", caseApply);
                return StatusResult(System.Net.HttpStatusCode.OK, checkModel);
            }
        }
        /// <summary>
        /// 滿意度調查表單
        /// </summary>
        public IActionResult SurveyForm()
        {
            caseApplyWeb.Title = "滿意度調查｜" + caseApplyWeb.Title;

            try
            {
                var data = GetSession<CaseApply>("surveyCase");
                var surveymodel = new SurveyModel();
                surveymodel.CaseApply = MailBoxService.GetCaseApply(data.CaseNo, data.CasePwd);
                return View(surveymodel);
            }
            catch (Exception)
            {
                return new EmptyResult();
            }
        }
        /// <summary>
        /// 儲存滿意度調查
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSurvey(CaseApplySurvey survey, string cftoken)
        {
            var cmodel = new MailBox.checkmodel();
            if (!GetTurnstileAsync(cftoken).GetAwaiter().GetResult())
            {
                cmodel = new MailBox.checkmodel { errormsg = "驗證碼已失效，請重新送出", focusOn = "" };
                return StatusResult(System.Net.HttpStatusCode.BadRequest, cmodel);
            }

            var data = GetSession<CaseApply>("surveyCase");
            if (data != null)
            {
                survey.CaseApplySn = data.CaseApplySN;
            }
            else
            {
                cmodel = new MailBox.checkmodel { errormsg = "請重新查詢案件後填寫", focusOn = "" };
                return StatusResult(System.Net.HttpStatusCode.BadRequest, cmodel);
            }

            cmodel = MailBox.CheckSurvey(survey);
            if (!string.IsNullOrWhiteSpace(cmodel.errormsg))
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, cmodel);
            }

            caseApplyWeb = MailBoxService.GetCaseApplyWeb();
            if (!caseApplyWeb.Satisfaction)
            {
                cmodel = new MailBox.checkmodel { errormsg = "滿意度調查未開放", focusOn = "" };
                return StatusResult(System.Net.HttpStatusCode.BadRequest, cmodel);
            }

            MailBoxService.SaveSurvey(survey);
            SetSession("surveyCase", "");
            return StatusResult(System.Net.HttpStatusCode.OK, "");
        }
        #endregion

        /// <summary>
        /// 驗證電子郵件信箱
        /// </summary>
        /// <param name="data"></param>
        /// <param name="captcha"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyEmail(CaseApplyValidate data, string cftoken)
        {
            if (!GetTurnstileAsync(cftoken).GetAwaiter().GetResult())
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "驗證碼已失效，請重新送出");
            }
            else
            {
                var _AddHours = AppSettingHelper.GetAppsetting("EffectiveHours");
                int.TryParse(_AddHours, out int AddHours);
                data.CreateDate = DateTime.UtcNow.AddHours(8);
                data.ProcessDate = DateTime.UtcNow.AddHours(8);
                data.EffectiveDate = DateTime.UtcNow.AddHours(8).AddHours(AddHours);
                data.ProcessIPAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                data.Token = Guid.NewGuid().ToString();

                if (MailBox.SendValidateMail(data))
                {
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "資料出現問題，請聯絡管理人員");
                }
            }
        }
        /// <summary>
        /// 暫存並檢核意見內容
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TempWriteMail(CaseApply data, string cftoken, string syscategory)
        {
            var cmodel = new MailBox.checkmodel();

            if (!GetTurnstileAsync(cftoken).GetAwaiter().GetResult())
            {
                cmodel = new MailBox.checkmodel { errormsg = "驗證碼已失效，請重新送出", focusOn = "" };
                return StatusResult(System.Net.HttpStatusCode.BadRequest, cmodel);
            }

            if (MailBox.Checkwritemail(syscategory, ref data, out cmodel))
            {
                var sessionData = GetSession<CaseApply>("CaseApply");
                if (sessionData != null)
                {
                    data.ContactEmail = sessionData.ContactEmail;
                    SetSession("CaseApply", data);
                }
                else
                {
                    cmodel = new MailBox.checkmodel { errormsg = "驗證碼已失效，請重新送出", focusOn = "" };
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, cmodel);
                }
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, cmodel);
            }
        }
        /// <summary>
        /// 查詢案件
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QueryCase(CaseApply data, string cftoken)
        {
            var checkmodel = new MailBox.checkmodel();
            var error = new List<string>();
            if (!GetTurnstileAsync(cftoken).GetAwaiter().GetResult())
            {
                error.Add("驗證碼已失效，請重新送出");
            }
            if (data?.CaseNo == null)
            {
                checkmodel.focusOn = checkmodel.focusOn ?? "CaseNo";
                error.Add("請填寫「案件編號」");
            }
            if (data?.CasePwd == null)
            {
                checkmodel.focusOn = checkmodel.focusOn ?? "CasePwd";
                error.Add("請填寫「案件檢查碼」");
            }
            if (checkmodel.focusOn != null)
            {
                checkmodel.errormsg = String.Join("<br/>", error);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, checkmodel);
            }

            var caseApply = MailBoxService.GetCaseApply(data.CaseNo.Trim(), data.CasePwd.Trim());

            if (caseApply == null)
            {
                checkmodel = new MailBox.checkmodel { errormsg = "查無資料，請輸入正確的「案件編號」及「案件檢查碼」查詢", focusOn = "CaseNo" };
                return StatusResult(System.Net.HttpStatusCode.BadRequest, checkmodel);
            }
            else if (caseApply.AcceptDate == null)
            {
                if (caseApply.EffectiveDate < DateTime.UtcNow.AddHours(8))
                {
                    checkmodel = new MailBox.checkmodel { errormsg = "申請案件未成立且已逾期，請重新申請", focusOn = "" };
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, checkmodel);

                }
                else
                {
                    checkmodel = new MailBox.checkmodel { errormsg = "申請案件未成立，請於電子郵件「申請確認信」點選「確認送出申請」連結", focusOn = "" };
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, checkmodel);
                }
            }
            else
            {
                SetSession("querycase", caseApply);
            }

            return StatusResult(System.Net.HttpStatusCode.OK, "");
        }
        /// <summary>
        /// 重寄「案件受理通知信」 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResendLetter(string email, string cftoken)
        {
            if (!GetTurnstileAsync(cftoken).GetAwaiter().GetResult())
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "驗證碼已失效，請重新送出");
            }
            var cassApply = new CaseApply();

            if (MailBox.ResendLastInProgressMail(email, out string errorMsg))
            {
                return StatusResult(System.Net.HttpStatusCode.OK, "已重寄最近一筆「案件受理通知信」，請至電子信箱查看");
            }
            else
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, errorMsg);
            }
        }
        /// <summary>
        /// 使用CloudFlare驗證
        /// </summary>
        static async Task<bool> GetTurnstileAsync(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)) return false;
                var CFTurnstileSecretkey = new StringContent(AppSettingHelper.GetAppsetting("CloudFlareTurnstileSecretkey"));
                var _token = new StringContent(token);
                if (_token == null) return false;
                using var client = new HttpClient();
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(CFTurnstileSecretkey, "\"secret\"");
                    content.Add(_token, "\"response\"");

                    var response = await client.PostAsync("https://challenges.cloudflare.com/turnstile/v0/siteverify", content);
                    var contents = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(contents))
                    {
                        return false;
                    }
                    var a = Utility.ApiContent.JsonDeserializeObject<Models.ApiModel.TurnstileResponsemodel>(contents); ;
                    bool success = a.success;
                    return success;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
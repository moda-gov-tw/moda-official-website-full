using DBModel;
using Management.ManagementUtility;
using Management.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Authorization;
using Services.Models;
using Services.SystemManageMent;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Utility.Model;

namespace Management.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }
        [Route("/PWS")]
        public IActionResult Index(string token = "")
        {
         
            ViewBag.token = token;
            return View();
        }
        /// <summary>
        /// 首次登入修改密碼
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IActionResult First(string u, string key)
        {
            logActionModel.controller = "Home";
            logActionModel.action = "First";
            logActionModel.NeedAECrequest = true;
            try
            {
                var userid = CommonUtility.AesDecrypt(u);
                if (!UserManagementService.CheckUser(userid, key.Replace(" ", "+")))
                {
                    logActionModel.status = LoginModel.Status.Error;
                    Log(logActionModel);
                    return RedirectToAction("Index", "Home", new { area = "", msg = "資訊錯誤，請聯絡管理人員" });
                }
                SysUser sysUser = new SysUser();
                sysUser.p_w_d = key;
                sysUser.UserID = userid;
                Log(logActionModel);
                return View(sysUser);
            }
            catch (System.Exception ex)
            {
                logActionModel.status = LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return RedirectToAction("Index", "Home", new { area = "", msg = "資訊錯誤，請聯絡管理人員" });
            }
        }
        /// <summary>
        /// 重設設定密碼
        /// </summary>
        /// <param name="u"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public IActionResult Reset(string u, string key)
        {
            logActionModel.controller = "Home";
            logActionModel.action = "Reset";
            logActionModel.NeedAECrequest = true;
            try
            {
                var userid = CommonUtility.AesDecrypt(u);
                if (!UserManagementService.CheckUser(userid, key, 1))
                {
                    logActionModel.status = LoginModel.Status.Error;
                    logActionModel.response = "資訊錯誤，請聯絡管理人員";
                    Log(logActionModel);
                    return RedirectToAction("Index", "Home", new { area = "", msg = "資訊錯誤，請聯絡管理人員" });
                }
                SysUser sysUser = new SysUser();
                sysUser.p_w_d = key;
                sysUser.UserID = userid;
                Log(logActionModel);
                return View(sysUser);
            }
            catch (System.Exception ex)
            {
                logActionModel.status = LoginModel.Status.Error;
                logActionModel.response = ex.ToString();
                Log(logActionModel);
                return RedirectToAction("Index", "Home", new { area = "", msg = "資訊錯誤，請聯絡管理人員" });
            }
        }
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="psd"></param>
        /// <param name="captcha"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OldLogin(string ac, string psd, string captcha)
        {
            logActionModel.NeedAECrequest = true;
            logActionModel.SourceTable = "SysUser";
            var SUL = new SysUserLogin
            {
                UserID = ac.Trim(),
                ProcessIPAddress = ContextModel.ProcessIpaddress,
                CreatedDate = DateTime.UtcNow.AddHours(8)
            };
            var user = new SysUser()
            {
                UserID = ac.Trim(),
                p_w_d = psd.Trim(),
                ProcessIPAddress = ContextModel.ProcessIpaddress
            };
            var aesKey = "";
            var IsOpenASEKey = CodeManagementService.GetCategoryByCategoryKey("Management-5-1", "zh-tw");
            if(IsOpenASEKey?.Value =="1") aesKey = AppSettingHelper.GetAppsetting("AESKey")?.ToUpper();

            var login = UserManagementService.Login(user, aesKey);
            SUL.Message = login.message ??= "";
            SUL.Status = login.check ? "1" : "0";
            SUL.WebSiteID = login.WebSiteID;
            LogService.SysUserLogin(SUL);
            if (!login.check)
            {
                var errorCount = GetSession<int>("errorCount");
                SetSession("errorCount", login.errorCount);
                logActionModel.response = login.message;
                logActionModel.status = LoginModel.Status.Error;
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.NotFound, login);
            }
            SetSession("SYSUser", login);
            ViewData["SYSUser"] = login;
            logActionModel.response = login.message;
            return StatusResult(System.Net.HttpStatusCode.OK, "");
        }


        #region AAD 登入
        [Route("/Home/Index")]
        [Route("/Index")]
        [Route("/")]
        public IActionResult Login()
        {
            var ex = new Exception();
            Utility.Mail.sysAdmin = LogService.GetErroEmailAccount();
            ResetSession();
            var openAzureAD = AppSettingHelper.GetAppsetting("OpenAzureAD");
            if (openAzureAD != "1") return RedirectToAction("Index", "Home");
            var tenantid = AppSettingHelper.GetAppsetting("Azure_tanentid");
            var clientid = AppSettingHelper.GetAppsetting("Azure_clientid");
            var callback_url = AppSettingHelper.GetAppsetting("Azure_callback_url");
            var loginUrl = $"https://login.microsoftonline.com/{tenantid}/oauth2/v2.0/authorize?response_type=code&client_id={clientid}&redirect_uri={callback_url}&scope=user.read";
            ViewBag.loginUrl = loginUrl;
            return View();
        }
        [Route("/AzureADcallback")]
        public IActionResult ADLogin(string code = "")
        {
            var ViewModel = new Management.Models.Home.ADLoginModel();
            var tenantid = AppSettingHelper.GetAppsetting("Azure_tanentid");
            var client_secret = AppSettingHelper.GetAppsetting("Azure_secret");
            var client_id = AppSettingHelper.GetAppsetting("Azure_clientid");
            var callback_url = AppSettingHelper.GetAppsetting("Azure_callback_url");
            var posturl = $"https://login.microsoftonline.com/{tenantid}/oauth2/v2.0/token";
            var token = "";
            try
            {
                using (var client = new HttpClient())
                {
                    var str = $"client_id={client_id}&client_secret={client_secret}&code={code}&grant_type=authorization_code&redirect_uri={callback_url}";
                    HttpContent httpContent = new StringContent(str);
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    var result = client.PostAsync(posturl, httpContent).Result.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        var tokenMD = Utility.ApiContent.JsonDeserializeObject<Management.Models.AzureModel.tokenMdeol>(result);
                        token = tokenMD.access_token;
                    }
                }
                if (string.IsNullOrWhiteSpace(token)) return View(ViewModel);
                var urseID = GetUserInfo(token);
                var AesUrseID = ManagementUtility.CommonUtility.GetUrlAesEncrypt(urseID);
                ViewModel.Uid = AesUrseID;
            }
            catch (Exception)
            {
                return View(ViewModel);
            }
            return View(ViewModel);
        }
        static string GetUserInfo(string token)
        {
            try
            {
                var url = "https://graph.microsoft.com/v1.0/me";
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var result = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        var AAD = Utility.ApiContent.JsonDeserializeObject<Management.Models.AzureModel.AADUserData>(result);
                        var UserID = AAD.userPrincipalName.Split("@")[0];
                        return UserID;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                return "";
            }
            return "";
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AADLogin(string uid = "")
        {
            var sysUserModel = new Services.Models.sysUserModel();
            var UrseID = ManagementUtility.CommonUtility.GetUrlAesDecrypt(uid);
            if (UrseID == "")
            {
                sysUserModel.message = "您未有全球資訊網後台權限，請與資訊處聯繫！";
                return StatusResult(System.Net.HttpStatusCode.NotFound, sysUserModel);
            }
            logActionModel.NeedAECrequest = true;
            logActionModel.SourceTable = "SysUser";
            var SUL = new SysUserLogin
            {
                UserID = UrseID.Trim(),
                ProcessIPAddress = ContextModel.ProcessIpaddress,
                CreatedDate = DateTime.UtcNow.AddHours(8)
            };
            var user = new SysUser() { UserID = UrseID, ProcessIPAddress = ContextModel.ProcessIpaddress };

            var login = UserManagementService.ADDLogin(user);
            SUL.Message = login.message ??= "";
            SUL.Status = login.check ? "1" : "0";
            SUL.WebSiteID = login.WebSiteID;
            LogService.SysUserLogin(SUL);
            if (login.check)
            {
                SetSession("SYSUser", login);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            else
            {
                logActionModel.response = login.message;
                logActionModel.status = LoginModel.Status.Error;
                Log(logActionModel);
                sysUserModel.message = "查無資料";
                return StatusResult(System.Net.HttpStatusCode.NotFound, login);
            }

        }
        #endregion
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            logActionModel.needInsertData = true;
            ResetSession();
            return RedirectToAction("Login");
        }


        public IActionResult ChengeWebSite(string websiteid)
        {
            try
            {
                var sysUser = GetSession<sysUserModel>("SYSUser");
                if (sysUser != null)
                {
                    sysUser.WebSiteID = websiteid;
                    SetSession("SYSUser", sysUser);
                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "");
                }
            }
            catch (Exception ex)
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "");
            }
        }

        public IActionResult TimeOut()
        {
            logActionModel.needInsertData = true;
            return View();
        }
        public IActionResult ErrorCome()
        {
            logActionModel.needInsertData = true;
            return View();
        }

        public IActionResult Forget()
        {
            logActionModel.needInsertData = true;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Forget(string ac, string email, string captcha)
        {
            var msg = "";
            logActionModel.controller = "Home";
            logActionModel.action = "forget";
            logActionModel.NeedAECrequest = true;
            logActionModel.Action2 = LoginModel.Action2.update;
            logActionModel.SourceTable = "SysUser";
            if (string.IsNullOrEmpty(ac) || string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(captcha))
            {
                msg = "請輸入帳號、Email及驗證碼";
                logActionModel.status = LoginModel.Status.Error;
                logActionModel.response = msg;
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.NotFound, msg);
            }
            ac = ac.Trim();
            email = email.Trim();
            captcha = captcha.Trim();
            var chkcaptcha = GetSession<string>("CaptchaCode");
            if (chkcaptcha.ToUpper() != captcha.Trim().ToUpper())
            {
                msg = "驗證碼錯誤";
                logActionModel.status = LoginModel.Status.Error;
                logActionModel.response = msg;
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.NotFound, "驗證碼錯誤");
            }

            var data = UserManagementService.CheckForgetUser(ac, email);
            if (!data.check)
            {
                msg = data.message;
                logActionModel.status = LoginModel.Status.Error;
                logActionModel.response = msg;
                Log(logActionModel);

                return StatusResult(System.Net.HttpStatusCode.NotFound, data.message);
            }
            else
            {
                msg = data.message;
                logActionModel.status = LoginModel.Status.Scuess;
                logActionModel.response = msg;
                Log(logActionModel);

                MailUtility.Sendpwd(MailType.pwdreset, _hostingEnvironment.WebRootPath, out Exception ex, data.sysUser);

                return StatusResult(System.Net.HttpStatusCode.OK, $"重設密碼信件已發送<br/>請點擊信件中的連結以重設密碼");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePwd(string ac, string psd, string captcha, bool Needcaptcha = true)
        {
            logActionModel.NeedAECrequest = true;
            logActionModel.SourceTable = "SysUser";
            var chkcaptcha = GetSession<string>("CaptchaCode");
            if (chkcaptcha.ToUpper() != captcha.Trim().ToUpper())
            {
                logActionModel.response = "驗證碼錯誤";
                logActionModel.status = LoginModel.Status.Error;
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.NotFound, "驗證碼錯誤");
            }
            var data = new SysUser()
            {
                UserID = ac,
                p_w_d = psd,
                ProcessIPAddress = ContextModel.ProcessIpaddress,
                ErrLoginnum = 0,
            };
            var aeskey = AppSettingHelper.GetAppsetting("AESKey")?.ToUpper();
            var requertData = UserManagementService.UpdatePwd(data, aeskey);

            if (!requertData.check)
            {
                logActionModel.response = requertData.message;
                logActionModel.status = LoginModel.Status.Error;
                Log(logActionModel);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, requertData.message);
            }
            Log(logActionModel);
            return StatusResult(System.Net.HttpStatusCode.OK, requertData.message);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}

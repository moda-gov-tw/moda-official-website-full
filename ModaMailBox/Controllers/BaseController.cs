using DBModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Services.ModaMailBox;
using System.Net;

namespace ModaMailBox.Controllers
{
    public class BaseController : Controller, IActionFilter
    {
        public static CaseApplyWeb? caseApplyWeb { get; set; }
        public static CaseApplyPage? CaseApplyPage { get; set; }
        public static List<CaseApplyPageExtend>? CaseApplyPageExtends { get; set; }
        public static WEBFile? Logo { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            caseApplyWeb = GetSession<CaseApplyWeb>("CaseApplyWeb");
            Logo = GetSession<WEBFile> ("Logo");
            if (caseApplyWeb == null) {
                caseApplyWeb = MailBoxService.GetCaseApplyWeb();
                caseApplyWeb.Footer = FooterHtml(caseApplyWeb.Footer);
                Logo = MailBoxService.GetCaseApplyFiles("CaseApplyWeb", 1, "Logo").FirstOrDefault();
                if (Logo != null)
                {
                    Logo.FilePath = $@"{AppSettingHelper.GetAppsetting("WEBSiteUrl")}{Logo?.FilePath}";
                }
                SetSession("Logo", Logo);
                SetSession("CaseApplyWeb", caseApplyWeb);
            }
            string actionName = context.RouteData.Values["action"]?.ToString();
            CaseApplyPage = MailBoxService.GetCaseApplyPage(actionName);
            CaseApplyPageExtends = CaseApplyPage != null ? MailBoxService.GetCaseApplyPageExtends(CaseApplyPage.CaseApplyPageSn) : null;
            if (actionName != "Index") caseApplyWeb.Title = $"{CaseApplyPage?.PageTitle ?? ""}｜{caseApplyWeb.Title}";
        }
        protected ActionResult StatusResult(HttpStatusCode code, object content)
        {
            return Json(new { StatusCode = (int)code, Content = content });
        }
        #region Session
        /// <summary>
        /// 設定Session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetSession(string key, object data)
        {
            HttpContext.Session.SetObjectAsJson(key, data);
        }
        /// <summary>
        /// 讀取Session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetSession<T>(string key)
        {
            try
            {
                return HttpContext.Session.GetObjectFromJson<T>(key);

            }
            catch (Exception)
            {
                return default(T);

            }

        }

        public void ResetSession()
        {
            HttpContext.Session.Clear();
        }

        #endregion
        /// <summary>
        /// 拿取主站的圖片
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        static string FooterHtml(string html)
        {
            var webSiteUrl = AppSettingHelper.GetAppsetting("WEBSiteUrl");
            //src="/assets/
            if (html.IndexOf("/copyright/MODA/") > -1)
            {
                html = html.Replace("/copyright/MODA/", $"{webSiteUrl}/copyright/MODA/");
            }
            return html;
        }
    }
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
        /// <summary>
        /// 清除 session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        public static void RemoveObjectFromJson(this ISession session, string key)
        {
            session.Remove(key);
        }
    }
}


using DBModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Authorization;
using Services.Models.WebSite;
using Services.WebSite;
namespace WebSite.Controllers
{
    public class BaseController : Controller, IActionFilter
    {

        public static string MainWebSite { get; set; } = AppSettingHelper.GetAppsetting("MainWebSite");
        public static string MainLang { get; set; } = AppSettingHelper.GetAppsetting("MainLang");
        
     
        #region Session 相關資料
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
        public static string? WebSiteUrl { get;set; }

        /// <summary>
        /// WebSiteID 站台 預設 MainWebSite
        /// </summary>
       public  string WEBSITEID { get; set; } 
     //   /// <summary>
        /// Lang 語系 預設中文
        /// </summary>
        public  string LANG { get; set; } 

        public  string? Host { get; set; } 

        /// <summary>
        ///WebSiteMasterModel 
        /// </summary>
        public static WebSiteMasterModel? WebSiteMasterModel { get; set; } 

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Host = $"{Request.Scheme}://{Request.Host}/";
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                WEBSITEID = string.IsNullOrWhiteSpace(WEBSITEID) ? MainWebSite : WEBSITEID;
                LANG = string.IsNullOrWhiteSpace(LANG) ? MainLang : LANG;
                if (ViewData["WebSiteMaster"] == null)
                {
                    var data = HomeService.getMasterModel(WEBSITEID, LANG);
                    ViewData["WebSiteMaster"] = data;
                }
                else { 
                    var data = ViewData["WebSiteMaster"]  as WebSiteMasterModel;
                    if (data.SysWebSiteLang.WebSiteID != WEBSITEID || data.SysWebSiteLang.Lang != LANG)
                    {
                        data = HomeService.getMasterModel(WEBSITEID, LANG);
                        ViewData["WebSiteMaster"] = data;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserID = "",
                    WebSiteID = "",
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
        }
    }
}

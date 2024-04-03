using DBModel;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services;
using Services.Authorization;
using Services.Models;
using System;
using System.Linq;
using System.Net;
using Utility.Model;

namespace Management
{
    public class BaseController : Controller, IActionFilter
    {
        public static string MessageInput = "";
        /// <summary>
        /// 可以直接服用 登入者資訊 
        /// </summary>
        public static sysUserModel UserData = null;

        #region override OnActionExecuting & OnActionExecuted
        /// <summary>
        /// 執行 Action 之前執行
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                logActionModel = new LogActionModel 
                {
                    controller = context.RouteData.Values["controller"]?.ToString(),
                    action = context.RouteData.Values["action"]?.ToString()
                };

                if (context.HttpContext.Request.Method.ToUpper() == "POST")
                {
                    logActionModel.needInsertData = true;
                }
                UserData = GetSession<sysUserModel>("SYSUser");
                if (ViewData["SYSUser"] == null)
                {
                    ViewData["SYSUser"] = UserData;
                }
                ContextModel = HttpContextData();
                if (logActionModel.controller != "Home" &&
                    ContextModel.PathName != "" &&
                    logActionModel.controller != "Common" &&
                    logActionModel.controller != "Demo" &&
                    UserData == null
                    )
                {
                    context.Result = new RedirectResult("/Home/TimeOut");
                    return;
                }
                MessageInput = Utility.CommFun.JsonToString(context.ActionArguments);
            }
            catch (Exception ex)
            {
                context.Result = new RedirectResult("/Home/TimeOut");
                return;
            }
        }
        /// <summary>
        /// 執行 Action 之後執行
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Log(logActionModel);
        }
        public static CheckUserMenuModel CheckUserMenu(int SysSectionSN)
        {
            CheckUserMenuModel checkUserMenuModel = new CheckUserMenuModel();
            if (UserData.menu.FirstOrDefault(x => x.SysSectionSN == SysSectionSN) == null)
            {
                checkUserMenuModel.chk = false;
                
                return checkUserMenuModel;
            }
            return checkUserMenuModel;
        }
        #endregion
        #region ActionResult 擴充
        /// <summary>
        /// ActionResult 擴充
        /// </summary>
        /// <param name="code"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected ActionResult StatusResult(HttpStatusCode code, object content)
        {
            return Json(new { StatusCode = (int)code, Content = content });
        }
        #endregion
        #region Session 相關資料
        /// <summary>
        /// 設定Session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetSession(string key, object data)
        {
            key = Utility.Files.PathTraversal(key);
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
                key = Utility.Files.PathTraversal(key);
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
        #region Log
        public static LogActionModel logActionModel = new LogActionModel();
        public void Log(LogActionModel logModel)
        {
            try
            {
                if (logModel.needInsertData)
                {
                    var logAction = new LogAction()
                    {
                        ProcessIPAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                        UserID = UserData?.sysUser.UserID,
                        WebSiteID = UserData?.WebSiteID,
                        Controller = logModel.controller,
                        Action = logModel.action,
                        ActionType = logModel.actionType.GetHashCode().ToString(),
                        DepartmentID = logModel.departmentID,
                        MessageInput = logModel.NeedAECrequest ? CommonUtility.AesEncrypt(MessageInput) : MessageInput,
                        MessageResult = logModel.response,
                        Status = logModel.status.GetHashCode().ToString(),
                        WebPath = logModel.webPath, 
                        Action2 = logModel.Action2.ToString(),
                        SourceSN = logModel.SourceSN,
                        SourceTable = logModel.SourceTable,
                        CreatedDate = DateTime.UtcNow.AddHours(8)
                    };
                    LogService.CreateLogAction(logAction);
                }

            }
            catch (Exception ex)
            {
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserID = UserData?.sysUser.UserID,
                    WebSiteID = UserData?.WebSiteID,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
        }
        /// <summary>
        /// 塞入logActionModel資料
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Action2"></param>
        /// <param name="SourceTable"></param>
        /// <param name="SourceSN"></param>
        public void SetLogActionModel(string webPath = "", LoginModel.Action2 Action2 = LoginModel.Action2.insert, string SourceTable = "",int SourceSN = 0 , string MessageResult ="")
        {
            try
            {
                logActionModel.webPath = webPath;
                logActionModel.Action2 = Action2;
                logActionModel.SourceTable = SourceTable;
                logActionModel.SourceSN = SourceSN;
                logActionModel.response = MessageResult;
            }
            catch (Exception ex)
            {
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserID = UserData?.sysUser.UserID,
                    WebSiteID = UserData?.WebSiteID,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
            }
        }
        #endregion

        #region HttpContextData 可以抓取相關 Http相關資料
        public static HttpContextModel ContextModel = new HttpContextModel();
        /// <summary>
        /// HttpContextData 可以抓取相關 Http相關資料
        /// </summary>
        public HttpContextModel HttpContextData()
        {
            var path = HttpContext.Request.Path.Value;
            var pathLen = path.Split('/').Count();
            HttpContextModel httpContextModel = new HttpContextModel();
            httpContextModel.ProcessIpaddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            return httpContextModel;

        }
        #endregion
    }
    public class HttpContextModel
    {
        public string ProcessIpaddress { get; set; }
        public string PathName { get; set; }
        public string FunctionName { get; set; }
    }
    public class LogActionModel
    {
        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string userID { get; set; }

        /// <summary>
        /// 網站代號
        /// </summary>
        public string webSiteID { get; set; }

        /// <summary>
        /// controller
        /// </summary>
        public string controller { get; set; }

        /// <summary>
        /// action
        /// </summary>
        public string action { get; set; }


        /// <summary>
        /// 傳遞的參數資料
        /// </summary>
        public string request { get; set; }


        /// <summary>
        /// 回傳的資料
        /// </summary>
        public string response { get; set; }

        /// <summary>
        /// 是否需要加密，預設不用
        /// </summary>
        public bool NeedAECrequest { get; set; } = false;

        /// <summary>
        /// 哪一階段
        /// </summary>
        public LoginModel.ActionType actionType { get; set; } = LoginModel.ActionType.Web;
        /// <summary>
        /// 成功與否
        /// </summary>
        public LoginModel.Status status { get; set; } = LoginModel.Status.Scuess;
        /// <summary>
        /// 麵包層 
        /// </summary>
        public string webPath { get; set; }
        /// <summary>
        /// 部門代號
        /// </summary>
        public string departmentID { get; set; }

        public LoginModel.Action2 Action2 { get; set; }

        public int? SourceSN { get; set; }

        public string SourceTable { get; set; }
        /// <summary>
        /// 是否需要寫入db紀錄 BASE 地方判斷 HTTP POST 地方都記錄DB
        /// </summary>
        public bool needInsertData { get; set; } = false;

    }

    public class CheckUserMenuModel
    {
        public bool chk { get; set; } = true;
 
    }
}


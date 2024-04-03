using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace FileServices
{
    public class BaseController : Controller, IActionFilter
    {
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
       
    }
}


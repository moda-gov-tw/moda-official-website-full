using System.Net;
using System.Web;

namespace WebSite.WebSiteUtility
{
    public class CommonUtility
    {
        /// <summary>
        /// 頁碼
        /// </summary>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public static string PageDisplayCount(int DisplayCount)
        {
            var str = "";
            var Display = DisplayCount % 15;
            if (Display == 0)
            {
                for (int i = 1; i <= 20; i++)
                {
                    var j = 15 * i;
                    if (DisplayCount == j)
                    {
                        str += $@"<option value={j} selected>{j}</option>";
                    }
                    else
                    {
                        str += $@"<option value={j} >{j}</option>";
                    }
                }
            }
            else
            {
                for (int i = 1; i <= 20; i++)
                {
                    var j = 12 * i;
                    if (DisplayCount == j)
                    {
                        str += $@"<option value={j} selected>{j}</option>";
                    }
                    else
                    {
                        str += $@"<option value={j} >{j}</option>";
                    }
                }
            }
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// HtmlEnCode
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string HtmlEnCode(string txt)
        {
            var _txt = txt.Trim();
            if (string.IsNullOrWhiteSpace(_txt)) return _txt;
            _txt = WebUtility.UrlEncode(_txt);
            _txt = _txt.Replace("-", "").Replace("'", "");
            return _txt;
        }
    }
}

using Nancy.Json;
using System;
using System.Web;
using Utility.Model;

namespace Utility
{
    public class CommFun
    {
        /// <summary>
        /// json字串把特殊字串替換htmlencode
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static string JsonTransfer(string jsonStr)
        { 
            return jsonStr.Replace(@"""", "%22");
        }
        public static string JsonToString(object data)
        {
            try
            {
                var jsonString = new JavaScriptSerializer();
                var jsonStringResult = jsonString.Serialize(data);
                return jsonStringResult;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            try
            {
                string dateDiff = null;
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                dateDiff = ts.Hours.ToString() + "小時" + ts.Minutes.ToString() + "分鐘" + ts.Seconds.ToString() + "秒";
                return dateDiff;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 民國轉西元
        /// </summary>
        /// <param name="roc"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime DateROCToAD(string roc, string time ="")
        { 
            var year = int.Parse( roc.Substring(0, 3)) +1911 ;
            var morth = roc.Substring(3, 2);
            var days = roc.Substring(5, 2);
            if (string.IsNullOrWhiteSpace(time)) {
                return DateTime.Parse($"{year}-{morth}-{days} 23:59:59");
            } else {
                return DateTime.Parse($"{year}-{morth}-{days} {time}");
            }
        }

      //  public static DateTime Get


        /// <summary>
        /// 頁碼
        /// </summary>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public static string PageDisplayCount(int DisplayCount)
        {
            try
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
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 撈取前端a tag 另開視窗attr
        /// </summary>
        /// <param name="target"></param>
        /// <param name="title"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        public static string getattr(string target, string title, string open, string fileType = "", string fileName = "")
        {
            var attr = "";
            try
            {
              
                fileType = fileType ?? "";
                fileName = fileName ?? "";
                if (target == "_blank")
                {
                    attr = $@"target=""{target}"" rel=""noreferrer noopener"" title=""{title}{open}""";
                }
                else if (!string.IsNullOrEmpty(fileName))
                {
                    if (fileType.ToUpper().Contains("PDF"))
                    {
                        attr = $@"title=""{fileName.Replace(fileType.ToLower(), "")}.{fileType?.ToLower()}{open}""";
                    }
                    else
                    {
                        attr = $@"title=""{fileName.Replace(fileType.ToLower(), "")}.{fileType?.ToLower()}""";
                    }
                }
                else
                {
                    attr = $@"target=""{target}"" title=""{title}""";
                }
            }
            catch (Exception)
            {
            }
            return attr;
        }

        public static tagModel taggetattr(string target, string title, string open, string fileType = "", string fileName = "")
        {
            tagModel _babModel = new tagModel();
            var attr = "";
            try
            {
                fileType = fileType ?? "";
                fileName = fileName ?? "";

                if (target == "_blank")
                {
                    _babModel.target = target;
                    _babModel.rel = "noreferrer noopener";
                    _babModel.title = $"{title}{open}";
                    attr = $@"target=""{target}"" rel=""noreferrer noopener"" title=""{title}{open}""";
                }
                else if (!string.IsNullOrEmpty(fileName))
                {
                    _babModel.target = "_self";
                    //_self
                    if (fileType.ToUpper() == ".PDF")
                    {
                        _babModel.title = $"{fileName.Replace(fileType.ToLower(), "")}{fileType?.ToLower()}{open}";
                        attr = $"title='{fileName.Replace(fileType.ToLower(), "")}{fileType?.ToLower()}{open}'";
                    }
                    else
                    {
                        _babModel.title = $"{fileName.Replace(fileType.ToLower(), "")}{fileType?.ToLower()}";
                        attr = $"title='{fileName.Replace(fileType.ToLower(), "")}{fileType?.ToLower()}'";
                    }
                }
                else
                {
                    _babModel.target = target;
                    _babModel.title = title;
                    attr = $@"target=""{target}"" title=""{title}""";
                }
            }
            catch (Exception)
            {
            }
            return _babModel;
        }



        public class tagModel
        {
            public string target { get; set; }
            public string title { get; set; }
            public string rel { get; set; }
        }

        /// <summary>
        /// 是否為站內連結
        /// </summary>
        /// <param name="url"></param>
        /// <param name="www"></param>
        /// <param name="api"></param>
        /// <returns></returns>
        public static bool CheckLocalUrl(string url, string www, string api)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.Substring(0, 1) == "/" || 
                    url.Substring(0, 1) == "#" || 
                    url.Substring(0, 1) == @"\" || 
                    url.StartsWith(www) || 
                    url.StartsWith(api))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}

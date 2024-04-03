using DBModel;
using Services;
using Services.Authorization;
using Services.SystemManageMent;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using static Utility.Files;

namespace Management.ManagementUtility
{
    public class CommonUtility
    {
        /// <summary>
        /// 參數使用Url get傳遞傳遞使用 AES加密
        /// </summary>
        /// <param name="txt">加密內容</param>
        /// <returns></returns>
        public static string GetUrlAesEncrypt(string txt)
        {
            var datekey = DateTime.UtcNow.AddHours(8).DayOfWeek.ToString();
            var strTxt = "";
            var endTxt = "";
            if (string.IsNullOrWhiteSpace(txt)) return "";
            if (txt.Trim().Length > 0)
            {
               strTxt = txt.Substring(0, txt.Length - 1);
               endTxt = txt.Substring(txt.Length - 1, 1);
            }
            return datekey.Substring(0,1) +
                strTxt + 
                datekey.Substring(3, 2)+ 
                datekey.Substring(4, 1) +
                endTxt;
        }
        /// <summary>
        /// 參數使用Url get傳遞傳遞使用 AES解密
        /// </summary>
        /// <param name="txt">加密內容</param>
        /// <returns></returns>
        public static string GetUrlAesDecrypt(string txt)
        {
            try
            {
                var datekey = DateTime.UtcNow.AddHours(8).DayOfWeek.ToString();
                if (datekey.Substring(0, 1) == txt.Substring(0, 1) &&
                   datekey.Substring(4, 1) == txt.Substring(txt.Length - 2, 1) &&
                   datekey.Substring(3, 2) == txt.Substring(txt.Length - 4, 2)
                    )
                {
                    var end = txt.Substring(txt.Length - 1, 1);
                    txt = txt.Substring(1, txt.Length - 1);
                    txt = txt.Substring(0, txt.Length - 4);
                    txt = txt + end;
                    return txt;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {

                return "";
            }
        }

        /// <summary>
        /// 解密 並確認成功與否
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool UrlKey(ref string key)
        {
            var datekey = DateTime.UtcNow.AddHours(8).DayOfWeek.ToString();
            try
            {
                if (key.Trim().Length < 3)return false;
                key = GetUrlAesDecrypt(key);
                if(string.IsNullOrWhiteSpace(key)) return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="txt">加密內容</param>
        /// <returns></returns>
        public static string AesEncrypt(string txt)
        {
            var IsOpenASEKey = CodeManagementService.GetCategoryByCategoryKey("Management-5-1", "zh-tw");
            if (IsOpenASEKey?.Value == "1") {
                return AES.AesEncrypt(txt, AppSettingHelper.GetAppsetting("AESKey").ToUpper());
            } else {
                return txt;
            } 
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string AesDecrypt(string txt)
        {

            var IsOpenASEKey = CodeManagementService.GetCategoryByCategoryKey("Management-5-1", "zh-tw");
            if (IsOpenASEKey?.Value == "1")
            {
                return AES.AesDecrypt(txt.Replace(" ", "+"), AppSettingHelper.GetAppsetting("AESKey").ToUpper());
            }
            else {
                return txt;
            }
           
        }
        /// <summary>
        /// 麵包屑
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Breadcrumb(int key)
        {
            var data = UserManagementService.Breadcrumb(key);
            var bread = "";
            if (data != null)
            {
                bread = $@"{data.TitleP}/{data.Title}";
            }
            return bread;
        }
        /// <summary>
        /// 後台麵包屑<<改版>>
        /// </summary>
        /// <param name="LevelSN"></param>
        /// <param name="NewsSN"></param>
        /// <returns></returns>
        public static List<string> LevelBreadcrumb(int LevelSN, int NewsSN = 0)
        {
            return CommonService.LevelBreadcrumb(LevelSN, NewsSN);
        }

        public static string TreeHtml { get; set; }
        /// <summary>
        /// 樹
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ParentID"></param>
        /// <param name="HierarchyID"></param>
        /// <param name="BigHierarchy"></param>
        /// <returns></returns>
        public static string TreefindTree(List<WebLevel> data, int ParentID, int HierarchyID, int BigHierarchy)
        {
            foreach (var d in data.Where(x => x.ParentSN == ParentID).OrderBy(x => x.SortOrder))
            {
                var toggle = data.FirstOrDefault(x => x.ParentSN == d.WebLevelSN);
                var togglediv = toggle != null ? "<div class='simpleTree-toggle'></div>" : "";
                TreeHtml += $@"
                                        <div class='simpleTree-nodeContainer hasChild on'>
                                            <div class='simpleTree-indent'></div>
                                            {togglediv }
                                            <div class='simpleTree-label'>{d.Title}</div>
                                        </div>
                                    ";
                HierarchyID++;
                //if (d.HierarchyID <= BigHierarchy)
                //{
                //    TreeHtml += "<div class='simpleTree-childrenContainer hasChild on'>  ";
                //    TreefindTree(data, d.WebLevelSN, HierarchyID, BigHierarchy);
                //    TreeHtml += "</div> ";
                //}
            }
            return TreeHtml;
        }

        /// <summary>
        /// 共用從資料庫撈取檔案資料
        /// </summary>
        /// <param name="key2">編號</param>
        /// <param name="SourceTable">主表</param>
        /// <returns></returns>
        public static List<CommonFileModel> GetFileByDB(string key, string SourceTable)
        {
            var files = WebLevelManagementService.GetWEBFiles(key, SourceTable);
            if (files?.Count() > 0)
            {
                var sessionFile = files.Select(x => new CommonFileModel()
                {
                    fileExt = x.FileType,
                    fileNewName = x.FileName,
                    fileOriginName = x.OriginalFileName,
                    FileSort = x.SortOrder,
                    filePath = x.FilePath,
                    fileTitle = x.FileTitle,
                    fileSize = x.FileSize.Value,
                    GroupID = x.GroupID,
                    webFileID = x.WEBFileID,
                    
                }).ToList();

                return sessionFile;
            }
            return null;

        }

        public static int DiffDate(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts1 = new TimeSpan(dt1.Ticks);
            TimeSpan ts2 = new TimeSpan(dt2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2);
            int day = ts.Days;
            return day;
        }

        /// <summary>
        /// 監控排程是否正常
        /// </summary>
        public static void CheckSchedule()
        {
            try
            {
                Utility.Mail.sysAdmin = LogService.GetErroEmailAccount();
                var data = Services.WEBScheduleService.ScheduleData();
                if (data.Count() > 0)
                {
                    var body = "排程出現異常，請確認<br>";
                    foreach (var d in data)
                    {
                        body += $"{d.Name}  : 上一次執行的時間 : {d.ProcessDate} <br>";
                    }
                    var subject = "排程出現異常，請確認";
                    Utility.Mail.Error(body, subject);
                }
            }
            catch (Exception)
            {
                var body = "排程出現異常，請確認<br>";
                var subject = "排程出現異常，請確認";
                Utility.Mail.Error(body, subject);
            }
        }
    }
}

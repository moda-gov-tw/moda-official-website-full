using DBModel;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;
using System.Reflection;
using System.Security.Policy;
using System.Web;
using Utility;
using Utility.MailBox;
using Utility.sysConstTable.field;
using Utility.sysConstTable.field.banner;
using static Utility.DownloadFile;

namespace ConsoleApp
{
    public class StaticHelper
    {
        public static string? gitPush { get; set; }// location

        public static string? gitExecute { get; set; }
        public static string? DemoDNS { get; set; }
        public static string? ResetHours { get; set; }
        public static string? WebSiteUrl { get; set; }

        public static string? IsOfficial { get; set; }

        public static string start = "static";
        public static void Start(string logFile , ref List<string> msg)
        {

            msg.Add("開始執行");
            //
            StaticCssJsImg(logFile , ref msg);
            //
            try
            {
                var StaticLink = Services.Static.StaticLinkService.GetStaticData();
                if (StaticLink != null)
                {
                    var startSiteMap = true;
                    var chkHref = new List<StaticLink>();
                    var AllTrueData = StaticLink.Where(x => x.IsEnable == "1").ToList();
                    msg.Add("上線的檔案數：" + AllTrueData.Count);
                    var NewTrueData = StaticLink.Where(x => x.IsEnable == "1").ToList();
                    int _ResetHours = 4; // 網站會全部重新刷新
                    int.TryParse(ResetHours, out _ResetHours);
                    if (DateTime.UtcNow.AddHours(8).Hour != _ResetHours)
                    {
                        NewTrueData = NewTrueData.Where(x => x.IsLive == "0").ToList();
                        startSiteMap = false;
                    }

                    #region 下架
                    var FalseData = StaticLink.Where(x => x.IsEnable == "0").ToList();
                    OffData(FalseData, out List<OutMsg> OutMsg);
                    var deleteList = OutMsg.Select(x => x.StaticUrl).ToList();
                    var chkFalseData = FalseData.Where(X => deleteList.Contains(X.StaticUrl)).ToList();
                    msg.Add("需要下架檔案數：" + chkFalseData.Count);
                    foreach (var item in chkFalseData)
                    {
                        msg.Add("需要下架檔案數：" + "需要下架檔案清單(成功)：" + item.StaticUrl);
                    } 
                    #endregion

                    msg.Add("需要更新檔案數：" + NewTrueData.Count);

                    OnData(NewTrueData, AllTrueData, ref chkHref);
                    var doubleCheckData = Services.Static.StaticLinkService.DoubleCheckData(AllTrueData);
                    OnData(doubleCheckData, AllTrueData, ref chkHref);
                    for (var i = 0; i < NewTrueData.Count(); i++)
                    {
                        msg.Add("需要更新檔案清單(成功)：" + NewTrueData[i].StaticUrl);
                    }
                    Services.Static.StaticLinkService.IsLiveSaveStatic(NewTrueData, true);
                    Services.Static.StaticLinkService.IsLiveSaveStatic(FalseData, false);
                    msg.Add("更新資料表(StaticLink)");

                    //sitemap
                    if (startSiteMap)
                    {
                        Sitemap.start(WebSiteUrl, IsOfficial, gitPush, chkHref);
                        msg.Add("產生SITEMAP");
                    }
                }
            }
            catch (Exception ex)
            {
                msg.Add($@"錯誤資訊：{ex.Message.ToString()}");
            }
            msg.Add($@"執行結束");
        }

        //上線資料
        static void OnData(List<StaticLink> data, List<StaticLink> FsModel, ref List<StaticLink> chkHref)
        {
            if (data?.Count() > 0)
            {
                var oldDomain = data.OrderBy(x => x.Link).FirstOrDefault(x => x.MainSN == 0)?.Link;
                if (oldDomain != null)
                {
                    var faData = FsModel.Where(x => !string.IsNullOrWhiteSpace(x.Link)).Select(x => new FsModel()
                    {
                        Link = x.Link != null ? x.Link.Replace(oldDomain, "").Replace(DemoDNS, "") : "",
                        StaticUrl = x.StaticUrl,
                    }).ToList();
                    foreach (var hrefData in data.Where(x => !string.IsNullOrWhiteSpace(x.Link)).OrderByDescending(x => x.WebSiteID).ThenByDescending(x => x.SourseTable))
                    {
                        if (Utility.DownloadFile.GetStaticData(gitPush, hrefData.Link, hrefData.StaticUrl, faData, DemoDNS, oldDomain))
                        {
                            chkHref.Add(hrefData);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 下線資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static void OffData(List<StaticLink> data , out List<OutMsg> ourMsgs )
        {
            var AllCount = data.Count();
            var InsertCount = 0;
            ourMsgs = new List<OutMsg>();
            foreach (var hrefData in data.OrderBy(x => x.WebSiteID))
            {
                try
                {
                    InsertCount++;
                    if (Utility.DownloadFile.DeleteData(gitPush, hrefData.StaticUrl))
                    {
                        ourMsgs.Add(new OutMsg() { StaticUrl = hrefData.StaticUrl, Msg = "" });
                    } 
                }
                catch (Exception ex)
                {
                   
                }
            }
        }

        public class OutMsg
        { 
            public string StaticUrl { get; set; }

            public string Msg { get; set; }
        }

        /// <summary>
        /// 讀取css/js/img
        /// </summary>
        static void StaticCssJsImg(string logFile , ref List<string> msg )
        {
            try
            {
                var lineList = new List<string>() {
                    "/assets/js/common.js",
                    "/ACS/assets/js/common.js",
                    "/ADI/assets/js/common.js",
                };
                if (DateTime.UtcNow.AddHours(8) < DateTime.Parse("2024-03-27 00:00:00"))
                {
                    foreach (string line in lineList)
                    {
                        var getUrl = $@"{DemoDNS.Trim()}{line}";

                        var getCssJsImg = Utility.DownloadFile.DownloadHtml(getUrl, DemoDNS.Trim(), DemoDNS.Trim(), true, logFile);
                        if (!string.IsNullOrWhiteSpace(getCssJsImg))
                        {
                            Utility.DownloadFile.SaveHtml(gitPush, getCssJsImg, line);
                        }
                        else
                        {
                           // msg.Add($@"靜態檔移置(失敗)：{gitPush}{line}");
                        }
                    }
                }
                else {
                }
            }
            catch (Exception ex)
            {
                msg.Add($@"靜態檔移置(失敗)：{ex.ToString()}");
            }
        }

    }
}

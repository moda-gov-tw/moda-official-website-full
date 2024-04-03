using DBModel;
using Microsoft.AspNetCore.Mvc;
using Services.Authorization;
using System.Text;
using Utility.Model;

namespace WebAPI.Controllers
{
    [ApiController]
    public class RSSController : Controller
    {
        #region MODA

        /// <summary>
        /// 新聞稿
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/RSSNews")]
        public IActionResult RSSNews()
        {
            string Lang = "zh-tw";
            var Key = "press-releases";

            RssNodeModel rssNodeModel = new RssNodeModel()
            {
                Title = $"數位發展部全球資訊網-新聞發布",
                Description = $"數位發展部全球資訊網 RSS",
                PubDate = DateTime.UtcNow.AddHours(8),
                Link = Common.GetAppsetting("RSS:Link:MODA"),
                Language = Lang,
            };
            //Level
            var webLevelDATA = new WebLevel()
            {
                WebSiteID = "MODA",
                Lang = Lang,
                WebLevelKey = Key
            };
            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA, false);

            //RssFeed
            var feed = Common.CreateRSSFeed2(rssNodeModel, level, Common.GetAppsetting("RSS:Author:MODA"));
            return Content(feed, "application/rss+xml", Encoding.UTF8);
        }

        /// <summary>
        /// 新聞稿en
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/RSSNews/en")]
        public IActionResult RSSNewsen()
        {
            string Lang = "en";
            var Key = "press-releases";

            RssNodeModel rssNodeModel = new RssNodeModel()
            {
                Title = $"Press Releases – Ministry of Digital Affairs",
                Description = $"Ministry of Digital Affairs RSS channel",
                PubDate = DateTime.UtcNow.AddHours(8),
                Link = Common.GetAppsetting("RSS:Link:MODA"),
                Language = Lang,
            };
            //Level
            var webLevelDATA = new WebLevel()
            {
                WebSiteID = "MODA",
                Lang = Lang,
                WebLevelKey = Key
            };
            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA, false);

            //RssFeed
            var feed = Common.CreateRSSFeed2(rssNodeModel, level, Common.GetAppsetting("RSS:Author:MODA"));
            return Content(feed, "application/rss+xml", Encoding.UTF8);
        }
        /// <summary>
        /// 即時新聞澄清
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/RSSClarification")]
        public IActionResult RSSClarification()
        {
            string Lang = "zh-tw";
            var Key = "clarification";
            RssNodeModel rssNodeModel = new RssNodeModel()
            {
                Title = $"數位發展部全球資訊網-即時新聞澄清",
                Description = $"數位發展部全球資訊網 RSS",
                PubDate = DateTime.UtcNow.AddHours(8),
                Link = Common.GetAppsetting("RSS:Link:MODA"),
            };
            //Level
            var webLevelDATA = new WebLevel()
            {
                WebSiteID = "MODA",
                Lang = Lang,
                WebLevelKey = Key
            };
            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA, false);

            //RssFeed
            var feed = Common.CreateRSSFeed2(rssNodeModel, level, Common.GetAppsetting("RSS:Author:MODA"));
            return Content(feed, "application/rss+xml", Encoding.UTF8);
        }
        /// <summary>
        /// 即時新聞澄清en
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/RSSClarification/en")]
        public IActionResult RSSClarificationen()
        {
            string Lang = "en";
            var Key = "clarification";
            RssNodeModel rssNodeModel = new RssNodeModel()
            {
                Title = $"Clarification – Ministry of Digital Affairs",
                Description = $"Ministry of Digital Affairs RSS channel",
                PubDate = DateTime.UtcNow.AddHours(8),
                Link = Common.GetAppsetting("RSS:Link:MODA"),
            };
            //Level
            var webLevelDATA = new WebLevel()
            {
                WebSiteID = "MODA",
                Lang = Lang,
                WebLevelKey = Key
            };
            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA, false);

            //RssFeed
            var feed = Common.CreateRSSFeed2(rssNodeModel, level, Common.GetAppsetting("RSS:Author:MODA"));
            return Content(feed, "application/rss+xml", Encoding.UTF8);
        }

        #endregion

        #region ADI

        /// <summary>
        /// 新聞稿
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/ADINews")]
        public IActionResult RSSADINews()
        {
            string Lang = "zh-tw";
            var Key = Common.GetAppsetting("RSS:NewsKey:ADI"); ;

            RssNodeModel rssNodeModel = new RssNodeModel()
            {
                Title = $"數位發展部數位產業署-新聞發布",
                Description = $"數位發展部數位產業署全球資訊網 RSS",
                PubDate = DateTime.UtcNow.AddHours(8),
                Link = Common.GetAppsetting("RSS:Link:ADI"),
                Language = Lang,
            };
            //Level
            var webLevelDATA = new WebLevel()
            {
                WebSiteID = "ADI",
                Lang = Lang,
                WebLevelKey = Key
            };
            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA, false);

            //RssFeed
            var feed = Common.CreateRSSFeed2(rssNodeModel, level, Common.GetAppsetting("RSS:Author:ADI"));
            return Content(feed, "application/rss+xml", Encoding.UTF8);
        }

        #endregion

        #region ACS

        /// <summary>
        /// 新聞稿
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/ACSNews")]
        public IActionResult RSSACSNews()
        {
            string Lang = "zh-tw";
            var Key = Common.GetAppsetting("RSS:NewsKey:ACS");

            RssNodeModel rssNodeModel = new RssNodeModel()
            {
                Title = $"數位發展部資通安全署-新聞發布",
                Description = $"數位發展部資通安全署全球資訊網 RSS",
                PubDate = DateTime.UtcNow.AddHours(8),
                Link = Common.GetAppsetting("RSS:Link:ACS"),
                Language = Lang,
            };
            //Level
            var webLevelDATA = new WebLevel()
            {
                WebSiteID = "ACS",
                Lang = Lang,
                WebLevelKey = Key
            };
            var level = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA, false);

            //RssFeed
            var feed = Common.CreateRSSFeed2(rssNodeModel, level, Common.GetAppsetting("RSS:Author:ACS"));
            return Content(feed, "application/rss+xml", Encoding.UTF8);
        }

        #endregion


        /// <summary>
        /// 查詢特定RSS頻道
        /// </summary>
        /// <param name="MainSN"></param>
        /// <param name="Lang"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/[action]/{Lang}/{MainSN}")]
        public IActionResult RSSChannel(int MainSN, string Lang)
        {
            try
            {
                var level = Services.WebSite.HomeService.GetWebLevel(MainSN, Lang);
                if (level?.RSSShow == "1")
                {
                    RssNodeModel rssNodeModel = new RssNodeModel()
                    {
                        Title = level.Title,
                        Description = level.Description,
                        PubDate = DateTime.UtcNow.AddHours(8),
                        Language = level.Lang,
                        Link = Common.GetAppsetting($"RSS:Link:{level.WebSiteID}"),
                    };
                    var feed = Common.CreateRSSFeed2(rssNodeModel, level, Common.GetAppsetting($"RSS:Author:{level.WebSiteID}"));
                    return Content(feed, "application/rss+xml",Encoding.UTF8);
                }
                else
                {
                    return NotFound("查無資料");
                }
            }
            catch (Exception ex)
            {
                var error = ex;
                return NotFound("查無資料");
            }
        }
    }
}

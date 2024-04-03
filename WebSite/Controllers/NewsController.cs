using DBModel;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using Services.Models.WebSite;
using Services.WebSite;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class NewsController : BaseController
    {
        [Route("{WebSiteID}/{Lang?}/News/{Key?}")]
        public IActionResult Index(string WebSiteID, int Key, string Lang)
        {
            var modelData = GetDetail(0, Key, Lang);
            var webLevel = WebLevelManagementService.GetWebLevel(modelData.Detail.BasicData.WebLevelSN);
            WebSiteID = webLevel.WebSiteID;
            WEBSITEID = WebSiteID;
            LANG = Lang;
            if (modelData.Detail.BasicData.ArticleType == "10")
            {
                //逐字稿
                modelData.wEBNewsTranscripts = NewsService.GetWEBNewsTranscript(modelData.Detail.BasicData.WEBNewsSN);
            }
            return View(modelData);
        }

        public IActionResult CP(int WebLevelMainSN, string Lang)
        {
            var modelData = GetDetail(WebLevelMainSN, 0, Lang);
            var webLevel = WebLevelManagementService.GetWebLevel(modelData.Detail.BasicData.WebLevelSN);
            LANG = Lang;
            WEBSITEID = webLevel.WebSiteID;
            return View(modelData);
        }

        NewsModel GetDetail(int WebLevelMainSN, int WebNewsMainSN, string Lang)
        {
            var detail = new NewsDetailModel();
            if (WebLevelMainSN != 0)
            {
                var webLevel = WebLevelManagementService.GetWebLevel(WebLevelMainSN);
                var LevelData = new WebLevel
                {
                    MainSN = WebLevelMainSN,
                    Lang = Lang
                };
                detail = NewsService.GetWebSiteNewDetailData(LevelData, null);
            }
            else
            {
                var newData = new WEBNews
                {
                    MainSN = WebNewsMainSN,
                    Lang = Lang
                };
                detail = NewsService.GetWebSiteNewDetailData(null, newData);
            }
            var webSiteBreadcrumbs = CommonService.GetWebSiteBreadcrumb(Lang, detail.BasicData.WebLevelSN, detail.BasicData.MainSN.Value);
            var ogData = CommonService.GetOgData(Lang, detail.BasicData.WebLevelSN, detail.BasicData.MainSN.Value);
            var newTitle = webSiteBreadcrumbs.FirstOrDefault().Title;
            var ogDataTitle = ogData.title.Substring(newTitle.Length + 3  );


            var newOgDatatTitle = $@"{newTitle}｜{ogDataTitle}";
            ogData.title = newOgDatatTitle;
            var newsModel = new NewsModel()
            {
                WebLevel = WebLevelManagementService.GetWebLevel(detail.BasicData.WebLevelSN, Lang),
                Detail = detail,
                webSiteBreadcrumbs = webSiteBreadcrumbs,
                ogData = ogData,
                titleBarMdel = new Services.Models.WebSite.TitleBarModel() { Title = detail.BasicData.Title },
                langCategory = CommonService.GetWebSiteCategory(detail.BasicData.WebSiteID, detail.BasicData.Lang),
                SysWebSiteLang = CommonService.GetSysWebSiteLang(detail.BasicData.WebSiteID, detail.BasicData.Lang)
            };
            //imagesize
            WebSiteUtility.OpenGragh.getImageSize(newsModel.ogData.image_path, out int Height, out int width);
            newsModel.ogData.image_height = Height.ToString();
            newsModel.ogData.image_width = width.ToString();
            return newsModel;
        }
    }
}

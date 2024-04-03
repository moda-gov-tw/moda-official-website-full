using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Services;
using Services.Authorization;
using Services.Models.WebContent.WebLevelManagement;
using Services.Models.WebSite;
using Services.WebSite;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Utility;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class LevelController : BaseController
    {
        

        /// <summary>
        /// 列表資料
        /// </summary>
        /// <param name="Key">MainSN</param>
        /// <param name="Lang">語系</param>
        /// <returns></returns>
        [Route("{WebSiteID}/{Lang?}/Level/{Key?}")]
        public IActionResult Index(string WebSiteID, int Key, string Lang)
        {
            WebSiteID = WebSiteID ?? MainWebSite;
            Lang = Lang ?? MainLang;
            WEBSITEID = WebSiteID;
            LANG = Lang;
            var WebLevelData = WebLevelManagementService.GetWebLevelByMainSN(Key);
            var MainData = WebLevelData.FirstOrDefault(x => x.Lang == "zh-tw");
            var LangData = WebLevelData.FirstOrDefault(x => x.Lang == Lang && x.IsEnable == "1");
            if (MainData != null && LangData != null)
            {
                EnumWebLevelModuleLevel2 module = EnumTpye.GetEnum<EnumWebLevelModuleLevel2>(LangData.Module);
                switch (module)
                {
                    case EnumWebLevelModuleLevel2.PAGELIST:
                    case EnumWebLevelModuleLevel2.RSS:
                        return RedirectToAction("PageList", "Level", new { Key = LangData.WebLevelSN });
                    case EnumWebLevelModuleLevel2.NEWS:
                    case EnumWebLevelModuleLevel2.Schedule:
                    case EnumWebLevelModuleLevel2.Bilingual:
                        return RedirectToAction("NewsList", "Level", new { Key = LangData.WebLevelSN });
                    case EnumWebLevelModuleLevel2.CP:
                        return RedirectToAction("CP", "News", new { WebLevelMainSN = LangData.MainSN, Lang = Lang });
                }
            }
            return RedirectToAction("Error", "Hoom");
        }

        /// <summary>
        /// 類型 - 列表頁
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public IActionResult PageList(int Key)
        {
            var webLevel = WebLevelManagementService.GetWebLevel(Key);
            LANG = webLevel.Lang;
            WEBSITEID = webLevel.WebSiteID;

            var ChiledData = new List<WebSiteWebLevelPageListModel>();
            if (webLevel.Module == EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.RSS))
            {
                ChiledData = WebSiteListService.GetRSSListData(webLevel.WebSiteID, webLevel.Lang);
            }
            else
            {
                ChiledData = WebLevelManagementService.GetWebSiteLevleListData(webLevel.MainSN.Value, webLevel.Lang);
            }

            PageListModel PageListModel = new PageListModel()
            {
                webSiteBreadcrumbs = CommonService.GetWebSiteBreadcrumb(webLevel.Lang, webLevel.MainSN.Value),
                ogData = CommonService.GetOgData(webLevel.Lang, webLevel.MainSN.Value),
                webLevel = webLevel,
                webSiteWebLevelPageListModels = ChiledData,
                titleBarMdel = new Services.Models.WebSite.TitleBarModel() { needSearch = false, Title = webLevel.Title },
                langCategory = CommonService.GetWebSiteCategory(webLevel.WebSiteID, webLevel.Lang),
                SysWebSiteLang = CommonService.GetSysWebSiteLang(WEBSITEID, LANG)
            };
            //imagesize
            WebSiteUtility.OpenGragh.getImageSize(PageListModel.ogData.image_path, out int Height, out int width);
            PageListModel.ogData.image_height = Height.ToString();
            PageListModel.ogData.image_width = width.ToString();
            return View(PageListModel);
        }

        /// <summary>
        /// News 列表
        /// </summary>
        /// <param name="Key">WebLevelSN</param>
        /// <param name="str_date"></param>
        /// <param name="end_date"></param>
        /// <param name="txt"></param>
        /// <param name="p"></param>
        /// <param name="DisplayCount"></param>
        /// <returns></returns>
        public IActionResult NewsList(int Key, string str_date = "", string end_date = "", string txt = "", string Condition4 = "", string Condition5 = "", string Condition6 = "", string CustomizeTag = "", string SysZipCode = "", int p = 1, int DisplayCount = 15)
        {
            var webLevel = WebLevelManagementService.GetWebLevel(Key);
            LANG = webLevel.Lang;
            WEBSITEID = webLevel.WebSiteID;
            DefaultPager pager = new DefaultPager();
            pager.Lang = webLevel.Lang;
            pager.DisplayCount = DisplayCount;
            pager.p = p;
            var list = new List<WEBNewsListModel>();
            var bigJsonData = new List<WEBNewsListModel2>();

            switch (webLevel.Module)
            {
                case "Bilingual":
                    list = WebSiteListService.GetBilingualListData(webLevel.MainSN.Value, webLevel.Lang, txt, ref pager, out List<WEBNewsListModel2> bigData);
                    bigJsonData = bigData;
                    break;
                default: 
                    list = WebSiteListService.GetNewsListData(webLevel.MainSN.Value, webLevel.Lang, str_date, end_date, txt, Condition4, Condition5, Condition6, CustomizeTag, SysZipCode, out List<WEBNewsListModel2> JSONModel, ref pager, "");
                    bigJsonData = JSONModel;
                    foreach (var item in list)
                    {
                        var link = item.webNews.StatesUrl;
                        switch (item.webNews.ArticleType)
                        {
                            case "1":
                                link = item.webFile != null ? item.webFile.FilePath : "";
                                break;
                            case "2":
                                link = item.webNews.URL;
                                break;
                        }
                        item.webUrl = link;
                    }

                    break;

            }

            

            NewsListModel NewsListModel = new NewsListModel()
            {
                webSiteBreadcrumbs = CommonService.GetWebSiteBreadcrumb(webLevel.Lang, webLevel.MainSN.Value),
                ogData = CommonService.GetOgData(webLevel.Lang, webLevel.MainSN.Value),
                WebLevel = webLevel,
                list = list,
                pager = pager,
                levelSN = Key,
                str_Date = str_date,
                end_Date = end_date,
                txt = txt,
                TitleBarModel = new Services.Models.WebSite.TitleBarModel { Title = webLevel.Title, needSearch = true },
                langCategory = CommonService.GetWebSiteCategory(webLevel.WebSiteID, webLevel.Lang),
                SysWebSiteLang = CommonService.GetSysWebSiteLang(WEBSITEID, LANG),
                Conditions = webLevel.Condition != null ? webLevel.Condition.Split(',').ToList() : new List<string>(),
                CustomizeTags = CommonService.GetWebLevelCustomizeTags(webLevel.WebLevelSN),
                sysZipCodes = CommonService.GetZipCodes(),
            };
            //imagesize
            WebSiteUtility.OpenGragh.getImageSize(NewsListModel.ogData.image_path, out int Height, out int width);
            NewsListModel.ogData.image_height = Height.ToString();
            NewsListModel.ogData.image_width = width.ToString();
            NewsListModel.BigjsonData = bigJsonData;
            var sort = 1;
            foreach (var item in NewsListModel.BigjsonData)
            {
                item.no = sort++;
                item.title = item.title?.Replace(@"""", @"\""");
                item.newstitle = item.newstitle?.Replace(@"""", @"\""");
                item.newssubtitle = item.newssubtitle?.Replace(@"""", @"\""");
                item.crosslinkdisplay = item.filetype != "" ? "none" : CommonService.CheckLocalUrl(item.href) ? "none" : "inline";
                item.contenttext = String.IsNullOrWhiteSpace(item.contenttext) ? "" : item.contenttext.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\\", "\\\\").Replace("	", "").Replace("	", "").Replace(@"""", "'");
            }

            NewsListModel.StrBigjsonData = JsonSerializer.Serialize(
                NewsListModel.BigjsonData,
                new JsonSerializerOptions() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) }
                );

            if (webLevel.Module == EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.Schedule) || webLevel.ListType == "AccordionList")
            {
                NewsListModel.foreverApi = true;
                if (webLevel.Module == EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.Schedule))
                {
                    NewsListModel.list = WebSiteListService.GetGetNewsListDataSchedule(NewsListModel.list, NewsListModel.langCategory);
                }
            }
            return View(NewsListModel);
        }


    }
}

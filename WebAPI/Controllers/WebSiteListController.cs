using DBModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using Services.Models.WebSite;
using Services.WebSite;
using System.Text.Json;
using System.Web;
using Utility;

namespace WebAPI.Controllers
{
	/// <summary>
	/// 列表頁
	/// </summary>
	[ApiController]
	[Route("[controller]/[action]")]
	[EnableCors("CorsPolicy")]
	public class WebSiteListController : Controller
	{
		/// <summary>
		/// 查詢新聞列表
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		[EnableCors("CorsPolicy")]
		public ViewResult NewsList([FromBody] NewsListQuery query)
		{
			try
			{
				var WebSiteHost = Common.GetAppsetting("WebSiteHost");
				var webLevel = WebLevelManagementService.GetWebLevel(query.MainSN, query.Lang);
				DefaultPager pager = new DefaultPager();
				pager.Lang = query.Lang;
				pager.DisplayCount = query.DisplayCount;
				pager.p = query.P;

				var list = new List<WEBNewsListModel>();
				var bigJsonData = new List<WEBNewsListModel2>();
				var langCategory = CommonService.GetWebSiteCategory(webLevel.WebSiteID, webLevel.Lang);
				switch (webLevel.Module)
				{
					case "Bilingual":
						var onlyRegulations = query.Regulations == "1" ? true : false; 
                        list = WebSiteListService.GetBilingualListData(webLevel.MainSN.Value, webLevel.Lang, query.SearchString, ref pager, out List<WEBNewsListModel2> bigData , onlyRegulations);
						bigJsonData = bigData;
						break;
					default:
						list = WebSiteListService.GetNewsListData(query.MainSN, query.Lang, query.StartDate, query.EndDate, query.SearchString, query.Condition4, query.Condition5, query.Condition6, query.CustomizeTagSN, query.SysZipCode, out List<WEBNewsListModel2> JSONModel, ref pager, query.Condition7);
						foreach (var item in list)
						{
							var tagModel = CommFun.taggetattr(item.webNews.target, item.webNews.Title, langCategory.FirstOrDefault(x => x.SysCategoryKey == item.webNews.WebSiteID + "-7-33")?.Value, item.webFile?.FileType, item.webFile?.FileTitle);
							var link = item.webNews.StatesUrl;
							item.tagModel = tagModel;
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
						bigJsonData = JSONModel;

						break;

				}


				var contentHeader = HttpUtility.HtmlEncode(webLevel.ContentHeader);
				contentHeader = HttpUtility.HtmlDecode(contentHeader);
				var viewLevel = new WebLevel()
				{
					Module = webLevel.Module,
					ContentHeader = contentHeader,
					ListType = webLevel.ListType,
					WebSiteID = webLevel.WebSiteID,
					ContentFooter = webLevel.ContentFooter,
					Lang = query.Lang
                };

				NewsListModel newsListModel = new NewsListModel()
				{
					webSiteBreadcrumbs = CommonService.GetWebSiteBreadcrumb(webLevel.Lang, webLevel.MainSN.Value),
					ogData = CommonService.GetOgData(webLevel.Lang, webLevel.MainSN.Value),
					WebLevel = viewLevel,
					list = list,
					pager = pager,
					levelSN = query.MainSN,
					str_Date = query.StartDate,
					end_Date = query.EndDate,
					txt = query.SearchString,
					TitleBarModel = new TitleBarModel { Title = webLevel.Title, needSearch = true },
					langCategory = langCategory,
					SysWebSiteLang = CommonService.GetSysWebSiteLang(webLevel.WebSiteID, webLevel.Lang),
					Conditions = webLevel.Condition != null ? webLevel.Condition.Split(',').ToList() : new List<string>(),
					CustomizeTags = CommonService.GetWebLevelCustomizeTags(webLevel.WebLevelSN),
					sysZipCodes = CommonService.GetZipCodes(),
				};

				var BigjsonData = bigJsonData;
				var sort = 1;
				foreach (var item in BigjsonData)
				{
					item.no = sort++;
					item.title = item.title?.Replace(@"""", @"\""");
					item.newstitle = item.newstitle?.Replace(@"""", @"\""");
					item.newssubtitle = item.newssubtitle?.Replace(@"""", @"\""");
					item.crosslinkdisplay = item.filetype != "" ? "none" : CommonService.CheckLocalUrl(item.href) ? "none" : "inline";
					item.contenttext = String.IsNullOrWhiteSpace(item.contenttext) ? "" : item.contenttext.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("\\", "\\\\").Replace("	", "").Replace("	", "").Replace(@"""", "'");
				}
				newsListModel.StrBigjsonData = JsonSerializer.Serialize(BigjsonData);
				if (webLevel.Module == EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.Schedule))
				{
					newsListModel.list = WebSiteListService.GetGetNewsListDataSchedule(newsListModel.list, newsListModel.langCategory);
				}


				return View(newsListModel);
			}
			catch (Exception ex)
			{
				Common.WriteLog($"NewsList - Error {ex.ToString()}");
				NewsListModel NewsListModel = new NewsListModel()
				{
					list = new List<WEBNewsListModel>(),
					langCategory = CommonService.GetWebSiteCategory("MODA", "zh-tw"),
				};
				return View(NewsListModel);
			}
		}
		/// <summary>
		/// 左側選單
		/// </summary>
		/// <param name="sn"></param>
		/// <returns></returns>
		[EnableCors("CorsPolicy")]
		public ViewResult LeftMenu(string sn)
		{
			//sn是weblevelSN - 可以抓取語系 & 站台  ，非MAIN sn
			int _sn = 0;
			if (int.TryParse(sn, out _sn))
			{
				var weblevelData = Services.WebSite.NewsService.GetWebLevelMbyWebLevelSN(_sn);
				if (weblevelData != null)
				{
					var SysWebSiteLangData = HomeService.getSysWebSiteLang(weblevelData.WebSiteID, weblevelData.Lang);

					var breadcrumb = CommonService.GetWebSiteBreadcrumb(weblevelData.Lang, weblevelData.MainSN.Value);
					var allLeftMenu = HomeService.getLeftMenu(weblevelData.WebSiteID, true);
					var baseSN = breadcrumb.FirstOrDefault(x => x.mainSN == breadcrumb.FirstOrDefault(x => x.IsActive)?.ParentSN) ?? breadcrumb.FirstOrDefault(x => x.IsActive);
					var leftMenuBigTitle = allLeftMenu.FirstOrDefault(x => x.MainSN == baseSN?.ParentSN && x.Lang == weblevelData.Lang)?.Title ?? SysWebSiteLangData.Title;
					var model = new WebAPI.Models.LeftMenuModel()
					{
						leftMenus = allLeftMenu.Where(x => x.Lang == weblevelData.Lang).ToList(),
						webSiteBreadcrumbModels = breadcrumb,
						BaseSN = baseSN,
						leftMenuBigTitle = leftMenuBigTitle,
						Lang = weblevelData.Lang,
						sysCategories = CommonService.GetWebSiteCategory(weblevelData.WebSiteID, weblevelData.Lang),
						sysWebSiteId = weblevelData.WebSiteID
					};
					return View(model);
				}
				else
				{
					Common.WriteLog($"LeftMenu - 查無Level {sn}");
					return View(null);
				}
			}
			else
			{
				Common.WriteLog($"LeftMenu - 參數有誤 {sn}");
				return View(null);
			}
		}
	}
}

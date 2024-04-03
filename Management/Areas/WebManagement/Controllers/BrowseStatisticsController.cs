using Google.Analytics.Data.V1Beta;
using Management.Areas.WebManagement.Models.OperationStatistics;
using Management.ManagementUtility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.WebManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utility;

namespace Management.Areas.WebManagement.Controllers
{
    [Area("WebManagement")]
    public class BrowseStatisticsController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public BrowseStatisticsController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(15);
            if (!CheckUserMenu(21).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            return View();
        }

        public IActionResult IndexF()
        {
            ViewData["Breadcrumb"] = CommonUtility.Breadcrumb(21);
            if (!CheckUserMenu(21).chk) { return RedirectToAction("ErrorCome", "Home", new { area = "" }); }
            return View();
        }

        public IActionResult List(string sd, string ed, string websiteid, string key, int p = 1, int DisplayCount = 15)
        {
            string str = Convert.ToDateTime(sd).ToString("yyyy-MM-dd 00:00:00").ToString();
            string end = Convert.ToDateTime(ed).ToString("yyyy-MM-dd 23:59:59").ToString();

            DefaultPager pager = new DefaultPager
            {
                DisplayCount = DisplayCount,
                p = p
            };

            var data = BrowseStatisticsService.GetFilesCount(websiteid, str, end, key, ref pager);

            Models.OperationStatistics.ListModel listModel = new Models.OperationStatistics.ListModel
            {
                defaultPager = pager,
                BrowseStatisticsModels = data
            };

            return View(listModel);
        }

        public IActionResult Export(string sd, string ed, string websiteid, string key)
        {
            try
            {
                string str = Convert.ToDateTime(sd).ToString("yyyy-MM-dd 00:00:00").ToString();
                string end = Convert.ToDateTime(ed).ToString("yyyy-MM-dd 23:59:59").ToString();
                var fpath = _hostingEnvironment.WebRootPath;
                var excelDatas = new MemoryStream();
                var data = BrowseStatisticsService.GetFileData(websiteid, str, end, key);
                var filePath = $@"{fpath}/Temp/ExampleReport.xlsx";
                var Info = new List<string>() {
                $@"建表日期: {DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-dd HH:mm")}",
                $@"建表人: {UserData?.sysUser.UserName}",
                $@"月份：{sd}至{ed}"
                };

                var DetailTitle = new List<string>() {
                "編號",
                "檔案標題",
                "檔案路徑",
                "檔案下載數"
                };

                var EModel = new ExcelModel();
                EModel.Info = Info;
                EModel.DetailTitle = DetailTitle;

                var Lang = new List<string>()
                {
                    "ZH-TW",
                    "EN"
                };

                var WEBSite = string.IsNullOrEmpty(websiteid) ? Services.WebSite.HomeService.GetSysWebSite().Select(x => new[] { x.WebSiteID, x.Title }).ToList() : Services.WebSite.HomeService.GetSysWebSite().Where(x => x.WebSiteID == websiteid).Select(x => new[] { x.WebSiteID, x.Title }).ToList();


                EModel.ExcelDetails = data.Select(x => new ExcelDetailModel()
                {
                    a = x.OriginalFileName,
                    b = x.Path,
                    c = x.DownLoadCount.ToString(),
                    d = x.Lang,
                    e = x.WEBSiteID
                }).ToList();

                excelDatas = Utility.Output.SheetReportByFile(filePath, EModel, Lang, WEBSite);

                SetSession("file", Convert.ToBase64String(excelDatas.ToArray()));

                excelDatas.Dispose();

                return StatusResult(System.Net.HttpStatusCode.OK, "");


            }
            catch (Exception ex)
            {
                IList<string> errmsgs = new List<string>();
                string error = "";
                error = ex.Message;
                errmsgs.Add(error);

                SetSession("file", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(error)));

                return StatusResult(System.Net.HttpStatusCode.BadRequest, "");

            }
        }

        [HttpGet]
        public IActionResult Download()
        {
            byte[] data = Convert.FromBase64String(GetSession<string>("file"));
            var fileName = $@"檔案下載統計數{DateTime.UtcNow.AddHours(8):yyyyMMdd}.xlsx";
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }

        public IActionResult ListG(string SD, string ED, string website, string lan)
        {
            SetSession("ga4Data", null);
            var fpath = _hostingEnvironment.WebRootPath;
            var filePath = $@"{fpath}/Oauth/brave-attic.json";
            var ga4 = $@"{fpath}/Oauth/GA4.json";
            DateTime str = Convert.ToDateTime(SD);
            DateTime end = Convert.ToDateTime(ED);
            GAViewModel gA = new GAViewModel();
            GA4 items = new GA4();
            try
            {
                using (StreamReader r = new StreamReader(ga4))
                {
                    string json = r.ReadToEnd();
                    items = JsonConvert.DeserializeObject<GA4>(json);
                }
                if (items.data.Count > 0)
                {
                    var GAid = items.data.Where(x => x.WebSiteID == website && x.Language == lan.ToUpper()).FirstOrDefault();
                    if (GAid != null)
                    {
                        BetaAnalyticsDataClient client = new BetaAnalyticsDataClientBuilder
                        {
                            CredentialsPath = filePath
                        }.Build();

                        gA.Flow2Data = GetFlow2(str, end, client, GAid.GA4);
                        gA.Flow2Data.chartName = "訪客流量";
                        gA.Flow2Data.chartId = "flowChart";
                        gA.CountryModels = GetMap2(str, end, client, GAid.GA4);
                        gA.Country2Data = GetCountry2(str, end, client, GAid.GA4);
                        gA.Country2Data.chartName = "訪客來源國家";
                        gA.Country2Data.chartId = "countryChart";
                        gA.City2Data = GetCity2(str, end, client, GAid.GA4);
                        gA.City2Data.chartName = "臺灣地區";
                        gA.City2Data.chartId = "cityChart";
                        gA.HotData = GetPagePath2(str, end, client, GAid.GA4, "hot");
                        gA.HotData.chartName = "熱門單元排行";
                        gA.HotData.chartId = "hotChart";
                        gA.ColdData = GetPagePath2(str, end, client, GAid.GA4, "cold");
                        gA.ColdData.chartName = "冷門單元排行";
                        gA.ColdData.chartId = "coldChart";
                    }
                    else
                    {
                        gA.Name = "目前還未設定GA4相關資料，請洽管理員處理";
                    }
                }
                else
                {
                    gA.Name = "目前還未設定GA4相關資料，請洽管理員處理";
                }
                return View(gA);
            }
            catch (Exception ex)
            {
                gA.Name = "目前還未與GA4連結，請洽管理員處理";
                return View(gA);
            }
        }
        #region GA4報表資料
        /// <summary>
        /// 顏色
        /// </summary>
        static List<string> backgroundColor = new List<string>() {
 "#ff6384"
,"#ff9f40"
,"#ffcd56"
,"#7ac142"
,"#4bc0c0"
,"#36a2eb"
,"#8a8acb"
,"#a37df0"
,"#a98c66"
,"#bdbdbd"
        };
        /// <summary>
        /// 新舊使用者
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="client"></param>
        /// <param name="GAid"></param>
        /// <returns></returns>
        public ChartModel GetFlow2(DateTime SD, DateTime ED, BetaAnalyticsDataClient client, string GAid)
        {
            #region GA4資料來源
            var date = new List<string>();
            for (var d = SD; d <= ED; d = d.AddDays(1))
            {
                date.Add(d.ToString("yyyy-MM-dd"));
            }
            var request = new RunReportRequest
            {
                Property = "properties/" + GAid,
                Dimensions = { new Dimension { Name = "date" } },
                Metrics = { new Metric { Name = "totalUsers" }, new Metric { Name = "newUsers" } },
                DateRanges = { new DateRange { StartDate = SD.ToString("yyyy-MM-dd"), EndDate = ED.ToString("yyyy-MM-dd") }, },
                OrderBys = { new OrderBy { Dimension = new OrderBy.Types.DimensionOrderBy() { DimensionName = "date" }, Desc = false }, }
            };
            var response = client.RunReport(request);
            #endregion
            ChartModel flow2 = new ChartModel();
            flow2.chartype = EnumChart.mixed;
            flow2.labels = date;
            flow2.datasets = new List<ChartDatasets>();
            if (response.Rows.Count > 0)
            {

                for (var i = 0; i < response.MetricHeaders.Count; i++)
                {
                    ChartDatasets charDatasets = new ChartDatasets();
                    charDatasets.type = "line";
                    charDatasets.label = $@"{ (i > 0 ? "新" : "")}使用者";
                    charDatasets.data = new List<int>();
                    charDatasets.backgroundColor = backgroundColor.Skip(i).Take(1).ToList();
                    foreach (var d in date)
                    {
                        var row = response.Rows.Where(x => x.DimensionValues[0].Value == d.Replace("-", "")).FirstOrDefault();
                        if (row != null)
                        {
                            charDatasets.data.Add(int.Parse(row.MetricValues[i].Value));
                        }
                        else
                        {
                            charDatasets.data.Add(0);
                        }
                    }
                    flow2.datasets.Add(charDatasets);
                }
            }
            return flow2;
        }
        /// <summary>
        /// 呈現地圖
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="client"></param>
        /// <param name="GAid"></param>
        /// <returns></returns>
        public List<CountryModel> GetMap2(DateTime SD, DateTime ED, BetaAnalyticsDataClient client, string GAid)
        {
            #region MyRegion
            var request = new RunReportRequest
            {
                Property = "properties/" + GAid,
                Dimensions = { new Dimension { Name = "countryId" } },
                Metrics = { new Metric { Name = "screenPageViews" } },
                DateRanges = { new DateRange { StartDate = SD.ToString("yyyy-MM-dd"), EndDate = ED.ToString("yyyy-MM-dd") }, },
                OrderBys = { new OrderBy { Metric = new OrderBy.Types.MetricOrderBy() { MetricName = "screenPageViews" }, Desc = true }, },
                Limit = 200
            };

            var response = client.RunReport(request);
            #endregion
            ChartModel chartModel = new ChartModel();
            var countryid = response.Rows.Where(x => !x.DimensionValues[0].Value.Contains("not set")).ToList();
            if (countryid.Count > 0)
            {
                var countryData = GetCountryCode();
                var popData = (from a in countryid
                               join b in countryData on a.DimensionValues[0].Value equals b.CountryCode2
                               select new CountryModel()
                               {
                                   CountryName = b.CountryName,
                                   CountryCode2 = b.CountryCode2,
                                   code = b.code,
                                   pop = int.Parse(a.MetricValues[0].Value)
                               }).ToList();
                return popData;
            }
            return null;
        }
        /// <summary>
        /// 國家資料
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="client"></param>
        /// <param name="GAid"></param>
        /// <returns></returns>
        public ChartModel GetCountry2(DateTime SD, DateTime ED, BetaAnalyticsDataClient client, string GAid)
        {
            ChartModel chartModel = new ChartModel();
            #region MyRegion
            var request = new RunReportRequest
            {
                Property = "properties/" + GAid,
                Dimensions = { new Dimension { Name = "country" } },
                Metrics = { new Metric { Name = "screenPageViews" } },
                DateRanges = { new DateRange { StartDate = SD.ToString("yyyy-MM-dd"), EndDate = ED.ToString("yyyy-MM-dd") }, },
                OrderBys = { new OrderBy { Metric = new OrderBy.Types.MetricOrderBy() { MetricName = "screenPageViews" }, Desc = true }, },
                Limit = 20
            };
            var response = client.RunReport(request);

            #endregion
            var country = response.Rows.Where(x => !x.DimensionValues[0].Value.Contains("not set")).Take(10).ToList();
            if (country.Count > 0)
            {
                chartModel.chartype = EnumChart.bar;
                chartModel.labels = country.Select(x => x.DimensionValues[0].Value).ToList();
                chartModel.datasets = new List<ChartDatasets>() {
                 new ChartDatasets() {
                       type="bar",
                       axis = "y",
                       data = country.Select(x => int.Parse( x.MetricValues[0].Value)).ToList(),
                       label ="瀏覽人次",
                       backgroundColor = backgroundColor.Take(10).ToList(),
                }};
            }
            return chartModel;
        }
        /// <summary>
        /// 台灣資料
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="client"></param>
        /// <param name="GAid"></param>
        /// <returns></returns>
        public ChartModel GetCity2(DateTime SD, DateTime ED, BetaAnalyticsDataClient client, string GAid)
        {
            ChartModel chartModel = new ChartModel();
            chartModel.chartype = EnumChart.bar;
            var request = new RunReportRequest
            {
                Property = "properties/" + GAid,
                Dimensions = { new Dimension { Name = "city" }, new Dimension { Name = "countryId" } },
                Metrics = { new Metric { Name = "screenPageViews" } },
                DateRanges = { new DateRange { StartDate = SD.ToString("yyyy-MM-dd"), EndDate = ED.ToString("yyyy-MM-dd") }, },
                DimensionFilter = new FilterExpression() { Filter = new Filter() { FieldName = "countryId", StringFilter = new Filter.Types.StringFilter() { Value = "TW" } } },
                OrderBys = { new OrderBy { Metric = new OrderBy.Types.MetricOrderBy() { MetricName = "screenPageViews" }, Desc = true }, },
                Limit = 10
            };

            var response = client.RunReport(request);
            var city = response.Rows.ToList();
            if (city.Count > 0)
            {
                chartModel.labels = city.Select(x => x.DimensionValues[0].Value).ToList();
                chartModel.datasets = new List<ChartDatasets>() {
                 new ChartDatasets() {
                       type="bar",
                       axis = "y",
                       data = city.Select(x => int.Parse( x.MetricValues[0].Value)).ToList(),
                       label ="瀏覽人次",
                       backgroundColor = backgroundColor.Take(10).ToList(),
                }};
            }
            return chartModel;
        }

        /// <summary>
        /// 熱門或冷門資料
        /// </summary>
        /// <param name="SD"></param>
        /// <param name="ED"></param>
        /// <param name="client"></param>
        /// <param name="GAid"></param>
        /// <param name="Type">預設熱門</param>
        /// <returns></returns>
        public ChartModel GetPagePath2(DateTime SD, DateTime ED, BetaAnalyticsDataClient client, string GAid, string Type = "hot")
        {
            ChartModel chartModel = new ChartModel();
            chartModel.chartype = EnumChart.bar;
            var desc = true;
            switch (Type)
            {
                case "cold": desc = false; break;
                default: desc = true; break;
            }
            var request = new RunReportRequest
            {
                Property = "properties/" + GAid,
                Dimensions = { new Dimension { Name = "pageTitle" }, },
                Metrics = { new Metric { Name = "screenPageViews" } },
                DateRanges = { new DateRange { StartDate = SD.ToString("yyyy-MM-dd"), EndDate = ED.ToString("yyyy-MM-dd") }, },
                OrderBys = { new OrderBy { Metric = new OrderBy.Types.MetricOrderBy() { MetricName = "screenPageViews" }, Desc = desc }, },
                Limit = 10
            };
            var esponse = client.RunReport(request);
            var data = esponse.Rows.ToList();
            if (data.Count > 0)
            {
                chartModel.labels = data.Select(x => x.DimensionValues[0].Value.Split('｜')[0]).ToList();
                chartModel.datasets = new List<ChartDatasets>() {
                 new ChartDatasets() {
                       type="bar",
                       axis = "y",
                       data = data.Select(x => int.Parse( x.MetricValues[0].Value)).ToList(),
                       label ="瀏覽人次",
                       backgroundColor = backgroundColor.Take(10).ToList(),
                }};
            }
            return chartModel;
        }
        public IActionResult ChartTemplate()
        {
            return View();
        }
        #endregion

        static List<CountryModel> GetCountryCode()
        {
            List<CountryModel> countryModels = new List<CountryModel>();
            #region 國家Code
            countryModels.Add(new CountryModel() { CountryName = "阿富汗", CountryCode2 = "AF", code = "AFG" });
            countryModels.Add(new CountryModel() { CountryName = "奧蘭(阿赫韋南馬)", CountryCode2 = "AX", code = "ALA" });
            countryModels.Add(new CountryModel() { CountryName = "阿爾巴尼亞", CountryCode2 = "AL", code = "ALB" });
            countryModels.Add(new CountryModel() { CountryName = "阿爾及利亞", CountryCode2 = "DZ", code = "DZA" });
            countryModels.Add(new CountryModel() { CountryName = "美屬薩摩亞(東薩摩亞)", CountryCode2 = "AS", code = "ASM" });
            countryModels.Add(new CountryModel() { CountryName = "安道爾(安道拉)", CountryCode2 = "AD", code = "AND" });
            countryModels.Add(new CountryModel() { CountryName = "安哥拉", CountryCode2 = "AO", code = "AGO" });
            countryModels.Add(new CountryModel() { CountryName = "安圭拉 / 安吉拉", CountryCode2 = "AI", code = "AIA" });
            countryModels.Add(new CountryModel() { CountryName = "南極洲", CountryCode2 = "AQ", code = "ATA" });
            countryModels.Add(new CountryModel() { CountryName = "安地卡及巴布達 / 安提瓜和巴布達", CountryCode2 = "AG", code = "ATG" });
            countryModels.Add(new CountryModel() { CountryName = "阿根廷", CountryCode2 = "AR", code = "ARG" });
            countryModels.Add(new CountryModel() { CountryName = "亞美尼亞 / 阿美尼亞", CountryCode2 = "AM", code = "ARM" });
            countryModels.Add(new CountryModel() { CountryName = "阿魯巴", CountryCode2 = "AW", code = "ABW" });
            countryModels.Add(new CountryModel() { CountryName = "澳大利亞(澳洲)", CountryCode2 = "AU", code = "AUS" });
            countryModels.Add(new CountryModel() { CountryName = "奧地利", CountryCode2 = "AT", code = "AUT" });
            countryModels.Add(new CountryModel() { CountryName = "亞塞拜然 / 阿塞拜疆", CountryCode2 = "AZ", code = "AZE" });
            countryModels.Add(new CountryModel() { CountryName = "巴哈馬", CountryCode2 = "BS", code = "BHS" });
            countryModels.Add(new CountryModel() { CountryName = "巴林", CountryCode2 = "BH", code = "BHR" });
            countryModels.Add(new CountryModel() { CountryName = "孟加拉", CountryCode2 = "BD", code = "BGD" });
            countryModels.Add(new CountryModel() { CountryName = "巴貝多 / 巴巴多斯", CountryCode2 = "BB", code = "BRB" });
            countryModels.Add(new CountryModel() { CountryName = "白俄羅斯(白羅斯)", CountryCode2 = "BY", code = "BLR" });
            countryModels.Add(new CountryModel() { CountryName = "比利時", CountryCode2 = "BE", code = "BEL" });
            countryModels.Add(new CountryModel() { CountryName = "貝里斯 / 伯利茲", CountryCode2 = "BZ", code = "BLZ" });
            countryModels.Add(new CountryModel() { CountryName = "貝南 / 貝寧", CountryCode2 = "BJ", code = "BEN" });
            countryModels.Add(new CountryModel() { CountryName = "百慕達(薩默斯)", CountryCode2 = "BM", code = "BMU" });
            countryModels.Add(new CountryModel() { CountryName = "不丹", CountryCode2 = "BT", code = "BTN" });
            countryModels.Add(new CountryModel() { CountryName = "玻利維亞", CountryCode2 = "BO", code = "BOL" });
            countryModels.Add(new CountryModel() { CountryName = "波士尼亞與赫塞哥維納(波赫) / 波斯尼亞和黑塞哥維那(波黑)", CountryCode2 = "BA", code = "BIH" });
            countryModels.Add(new CountryModel() { CountryName = "波札那 / 博茨瓦納", CountryCode2 = "BW", code = "BWA" });
            countryModels.Add(new CountryModel() { CountryName = "布威島 / 鮑威特島", CountryCode2 = "BV", code = "BVT" });
            countryModels.Add(new CountryModel() { CountryName = "巴西", CountryCode2 = "BR", code = "BRA" });
            countryModels.Add(new CountryModel() { CountryName = "英屬印度洋領地", CountryCode2 = "IO", code = "IOT" });
            countryModels.Add(new CountryModel() { CountryName = "汶萊(文萊)", CountryCode2 = "BN", code = "BRN" });
            countryModels.Add(new CountryModel() { CountryName = "保加利亞", CountryCode2 = "BG", code = "BGR" });
            countryModels.Add(new CountryModel() { CountryName = "布吉納法索 / 布基納法索", CountryCode2 = "BF", code = "BFA" });
            countryModels.Add(new CountryModel() { CountryName = "蒲隆地 / 布隆迪", CountryCode2 = "BI", code = "BDI" });
            countryModels.Add(new CountryModel() { CountryName = "柬埔寨", CountryCode2 = "KH", code = "KHM" });
            countryModels.Add(new CountryModel() { CountryName = "喀麥隆", CountryCode2 = "CM", code = "CMR" });
            countryModels.Add(new CountryModel() { CountryName = "加拿大", CountryCode2 = "CA", code = "CAN" });
            countryModels.Add(new CountryModel() { CountryName = "維德角 / 佛得角", CountryCode2 = "CV", code = "CPV" });
            countryModels.Add(new CountryModel() { CountryName = "荷蘭加勒比區", CountryCode2 = "BQ", code = "BES" });
            countryModels.Add(new CountryModel() { CountryName = "開曼群島", CountryCode2 = "KY", code = "CYM" });
            countryModels.Add(new CountryModel() { CountryName = "中非共和國", CountryCode2 = "CF", code = "CAF" });
            countryModels.Add(new CountryModel() { CountryName = "查德 / 乍得", CountryCode2 = "TD", code = "TCD" });
            countryModels.Add(new CountryModel() { CountryName = "智利", CountryCode2 = "CL", code = "CHL" });
            countryModels.Add(new CountryModel() { CountryName = "中國", CountryCode2 = "CN", code = "CHN" });
            countryModels.Add(new CountryModel() { CountryName = "聖誕島", CountryCode2 = "CX", code = "CXR" });
            countryModels.Add(new CountryModel() { CountryName = "科科斯（基林）群島", CountryCode2 = "CC", code = "CCK" });
            countryModels.Add(new CountryModel() { CountryName = "哥倫比亞", CountryCode2 = "CO", code = "COL" });
            countryModels.Add(new CountryModel() { CountryName = "葛摩 / 科摩羅", CountryCode2 = "KM", code = "COM" });
            countryModels.Add(new CountryModel() { CountryName = "民主剛果", CountryCode2 = "CD", code = "COD" });
            countryModels.Add(new CountryModel() { CountryName = "剛果共和國(剛果)", CountryCode2 = "CG", code = "COG" });
            countryModels.Add(new CountryModel() { CountryName = "庫克群島", CountryCode2 = "CK", code = "COK" });
            countryModels.Add(new CountryModel() { CountryName = "哥斯大黎加 / 哥斯達黎加", CountryCode2 = "CR", code = "CRI" });
            countryModels.Add(new CountryModel() { CountryName = "克羅埃西亞 / 克羅地亞", CountryCode2 = "HR", code = "HRV" });
            countryModels.Add(new CountryModel() { CountryName = "古巴", CountryCode2 = "CU", code = "CUB" });
            countryModels.Add(new CountryModel() { CountryName = "古拉索 / 庫拉索", CountryCode2 = "CW", code = "CUW" });
            countryModels.Add(new CountryModel() { CountryName = "賽普勒斯 / 塞浦路斯", CountryCode2 = "CY", code = "CYP" });
            countryModels.Add(new CountryModel() { CountryName = "捷克", CountryCode2 = "CZ", code = "CZE" });
            countryModels.Add(new CountryModel() { CountryName = "丹麥", CountryCode2 = "DK", code = "DNK" });
            countryModels.Add(new CountryModel() { CountryName = "吉布提", CountryCode2 = "DJ", code = "DJI" });
            countryModels.Add(new CountryModel() { CountryName = "多米尼克", CountryCode2 = "DM", code = "DMA" });
            countryModels.Add(new CountryModel() { CountryName = "多明尼加 / 多米尼加", CountryCode2 = "DO", code = "DOM" });
            countryModels.Add(new CountryModel() { CountryName = "厄瓜多爾(厄瓜多)", CountryCode2 = "EC", code = "ECU" });
            countryModels.Add(new CountryModel() { CountryName = "埃及", CountryCode2 = "EG", code = "EGY" });
            countryModels.Add(new CountryModel() { CountryName = "薩爾瓦多", CountryCode2 = "SV", code = "SLV" });
            countryModels.Add(new CountryModel() { CountryName = "赤道幾內亞", CountryCode2 = "GQ", code = "GNQ" });
            countryModels.Add(new CountryModel() { CountryName = "厄利垂亞 / 厄立特里亞", CountryCode2 = "ER", code = "ERI" });
            countryModels.Add(new CountryModel() { CountryName = "愛沙尼亞", CountryCode2 = "EE", code = "EST" });
            countryModels.Add(new CountryModel() { CountryName = "衣索比亞 / 埃塞俄比亞", CountryCode2 = "ET", code = "ETH" });
            countryModels.Add(new CountryModel() { CountryName = "福克蘭群島", CountryCode2 = "FK", code = "FLK" });
            countryModels.Add(new CountryModel() { CountryName = "法羅群島", CountryCode2 = "FO", code = "FRO" });
            countryModels.Add(new CountryModel() { CountryName = "斐濟", CountryCode2 = "FJ", code = "FJI" });
            countryModels.Add(new CountryModel() { CountryName = "芬蘭", CountryCode2 = "FI", code = "FIN" });
            countryModels.Add(new CountryModel() { CountryName = "法國(法蘭西)", CountryCode2 = "FR", code = "FRA" });
            countryModels.Add(new CountryModel() { CountryName = "法屬圭亞那", CountryCode2 = "GF", code = "GUF" });
            countryModels.Add(new CountryModel() { CountryName = "法屬玻里尼西亞 / 法屬波利尼西亞", CountryCode2 = "PF", code = "PYF" });
            countryModels.Add(new CountryModel() { CountryName = "法屬南部和南極領地(法屬南部領地)", CountryCode2 = "TF", code = "ATF" });
            countryModels.Add(new CountryModel() { CountryName = "加彭 / 加蓬", CountryCode2 = "GA", code = "GAB" });
            countryModels.Add(new CountryModel() { CountryName = "甘比亞 / 岡比亞", CountryCode2 = "GM", code = "GMB" });
            countryModels.Add(new CountryModel() { CountryName = "喬治亞", CountryCode2 = "GE", code = "GEO" });
            countryModels.Add(new CountryModel() { CountryName = "德國", CountryCode2 = "DE", code = "DEU" });
            countryModels.Add(new CountryModel() { CountryName = "迦納(加納)", CountryCode2 = "GH", code = "GHA" });
            countryModels.Add(new CountryModel() { CountryName = "直布羅陀", CountryCode2 = "GI", code = "GIB" });
            countryModels.Add(new CountryModel() { CountryName = "希臘", CountryCode2 = "GR", code = "GRC" });
            countryModels.Add(new CountryModel() { CountryName = "格陵蘭", CountryCode2 = "GL", code = "GRL" });
            countryModels.Add(new CountryModel() { CountryName = "格瑞那達 / 格林納達", CountryCode2 = "GD", code = "GRD" });
            countryModels.Add(new CountryModel() { CountryName = "瓜地洛普 / 瓜德羅普", CountryCode2 = "GP", code = "GLP" });
            countryModels.Add(new CountryModel() { CountryName = "關島", CountryCode2 = "GU", code = "GUM" });
            countryModels.Add(new CountryModel() { CountryName = "瓜地馬拉 / 危地馬拉", CountryCode2 = "GT", code = "GTM" });
            countryModels.Add(new CountryModel() { CountryName = "耿西 / 根西", CountryCode2 = "GG", code = "GGY" });
            countryModels.Add(new CountryModel() { CountryName = "幾內亞", CountryCode2 = "GN", code = "GIN" });
            countryModels.Add(new CountryModel() { CountryName = "幾內亞比索 / 幾內亞比紹", CountryCode2 = "GW", code = "GNB" });
            countryModels.Add(new CountryModel() { CountryName = "蓋亞那 / 圭亞那", CountryCode2 = "GY", code = "GUY" });
            countryModels.Add(new CountryModel() { CountryName = "海地", CountryCode2 = "HT", code = "HTI" });
            countryModels.Add(new CountryModel() { CountryName = "赫德島和麥克唐納群島", CountryCode2 = "HM", code = "HMD" });
            countryModels.Add(new CountryModel() { CountryName = "洪都拉斯 / 宏都拉斯", CountryCode2 = "HN", code = "HND" });
            countryModels.Add(new CountryModel() { CountryName = "香港", CountryCode2 = "HK", code = "HKG" });
            countryModels.Add(new CountryModel() { CountryName = "匈牙利", CountryCode2 = "HU", code = "HUN" });
            countryModels.Add(new CountryModel() { CountryName = "冰島", CountryCode2 = "IS", code = "ISL" });
            countryModels.Add(new CountryModel() { CountryName = "印度", CountryCode2 = "IN", code = "IND" });
            countryModels.Add(new CountryModel() { CountryName = "印度尼西亞(印尼)", CountryCode2 = "ID", code = "IDN" });
            countryModels.Add(new CountryModel() { CountryName = "伊朗", CountryCode2 = "IR", code = "IRN" });
            countryModels.Add(new CountryModel() { CountryName = "伊拉克", CountryCode2 = "IQ", code = "IRQ" });
            countryModels.Add(new CountryModel() { CountryName = "愛爾蘭", CountryCode2 = "IE", code = "IRL" });
            countryModels.Add(new CountryModel() { CountryName = "以色列", CountryCode2 = "IL", code = "ISR" });
            countryModels.Add(new CountryModel() { CountryName = "曼島 / 萌島", CountryCode2 = "IM", code = "IMN" });
            countryModels.Add(new CountryModel() { CountryName = "義大利 / 意大利", CountryCode2 = "IT", code = "ITA" });
            countryModels.Add(new CountryModel() { CountryName = "象牙海岸 / 科特迪瓦", CountryCode2 = "CI", code = "CIV" });
            countryModels.Add(new CountryModel() { CountryName = "牙買加", CountryCode2 = "JM", code = "JAM" });
            countryModels.Add(new CountryModel() { CountryName = "日本", CountryCode2 = "JP", code = "JPN" });
            countryModels.Add(new CountryModel() { CountryName = "澤西", CountryCode2 = "JE", code = "JEY" });
            countryModels.Add(new CountryModel() { CountryName = "約旦", CountryCode2 = "JO", code = "JOR" });
            countryModels.Add(new CountryModel() { CountryName = "哈薩克", CountryCode2 = "KZ", code = "KAZ" });
            countryModels.Add(new CountryModel() { CountryName = "肯亞 / 肯雅", CountryCode2 = "KE", code = "KEN" });
            countryModels.Add(new CountryModel() { CountryName = "吉里巴斯 / 基里巴斯", CountryCode2 = "KI", code = "KIR" });
            countryModels.Add(new CountryModel() { CountryName = "朝鮮(北韓)", CountryCode2 = "KP", code = "PRK" });
            countryModels.Add(new CountryModel() { CountryName = "大韓民國(韓國/南韓)", CountryCode2 = "KR", code = "KOR" });
            countryModels.Add(new CountryModel() { CountryName = "科索沃", CountryCode2 = "XK", code = "XKS" });
            countryModels.Add(new CountryModel() { CountryName = "科威特", CountryCode2 = "KW", code = "KWT" });
            countryModels.Add(new CountryModel() { CountryName = "吉爾吉斯", CountryCode2 = "KG", code = "KGZ" });
            countryModels.Add(new CountryModel() { CountryName = "寮國 / 老撾", CountryCode2 = "LA", code = "LAO" });
            countryModels.Add(new CountryModel() { CountryName = "拉脫維亞", CountryCode2 = "LV", code = "LVA" });
            countryModels.Add(new CountryModel() { CountryName = "黎巴嫩", CountryCode2 = "LB", code = "LBN" });
            countryModels.Add(new CountryModel() { CountryName = "賴索托 / 萊索托", CountryCode2 = "LS", code = "LSO" });
            countryModels.Add(new CountryModel() { CountryName = "賴比瑞亞 / 利比里亞", CountryCode2 = "LR", code = "LBR" });
            countryModels.Add(new CountryModel() { CountryName = "利比亞", CountryCode2 = "LY", code = "LBY" });
            countryModels.Add(new CountryModel() { CountryName = "列支敦斯登 / 列支敦士登", CountryCode2 = "LI", code = "LIE" });
            countryModels.Add(new CountryModel() { CountryName = "立陶宛", CountryCode2 = "LT", code = "LTU" });
            countryModels.Add(new CountryModel() { CountryName = "盧森堡", CountryCode2 = "LU", code = "LUX" });
            countryModels.Add(new CountryModel() { CountryName = "澳門", CountryCode2 = "MO", code = "MAC" });
            countryModels.Add(new CountryModel() { CountryName = "北馬其頓", CountryCode2 = "MK", code = "MKD" });
            countryModels.Add(new CountryModel() { CountryName = "馬達加斯加", CountryCode2 = "MG", code = "MDG" });
            countryModels.Add(new CountryModel() { CountryName = "馬拉威 / 馬拉維", CountryCode2 = "MW", code = "MWI" });
            countryModels.Add(new CountryModel() { CountryName = "馬來西亞(大馬)", CountryCode2 = "MY", code = "MYS" });
            countryModels.Add(new CountryModel() { CountryName = "馬爾地夫 / 馬爾代夫", CountryCode2 = "MV", code = "MDV" });
            countryModels.Add(new CountryModel() { CountryName = "馬利 / 馬里", CountryCode2 = "ML", code = "MLI" });
            countryModels.Add(new CountryModel() { CountryName = "馬爾他", CountryCode2 = "MT", code = "MLT" });
            countryModels.Add(new CountryModel() { CountryName = "馬紹爾群島", CountryCode2 = "MH", code = "MHL" });
            countryModels.Add(new CountryModel() { CountryName = "馬丁尼克 / 馬提尼克", CountryCode2 = "MQ", code = "MTQ" });
            countryModels.Add(new CountryModel() { CountryName = "茅利塔尼亞 / 毛里塔尼亞", CountryCode2 = "MR", code = "MRT" });
            countryModels.Add(new CountryModel() { CountryName = "模里西斯 / 毛里裘斯", CountryCode2 = "MU", code = "MUS" });
            countryModels.Add(new CountryModel() { CountryName = "馬約特", CountryCode2 = "YT", code = "MYT" });
            countryModels.Add(new CountryModel() { CountryName = "墨西哥", CountryCode2 = "MX", code = "MEX" });
            countryModels.Add(new CountryModel() { CountryName = "密克羅尼西亞聯邦", CountryCode2 = "FM", code = "FSM" });
            countryModels.Add(new CountryModel() { CountryName = "摩爾多瓦", CountryCode2 = "MD", code = "MDA" });
            countryModels.Add(new CountryModel() { CountryName = "摩納哥", CountryCode2 = "MC", code = "MCO" });
            countryModels.Add(new CountryModel() { CountryName = "蒙古", CountryCode2 = "MN", code = "MNG" });
            countryModels.Add(new CountryModel() { CountryName = "蒙特內哥羅 / 蒙特尼哥羅(黑山)", CountryCode2 = "ME", code = "MNE" });
            countryModels.Add(new CountryModel() { CountryName = "蒙哲臘 / 蒙塞拉特", CountryCode2 = "MS", code = "MSR" });
            countryModels.Add(new CountryModel() { CountryName = "摩洛哥", CountryCode2 = "MA", code = "MAR" });
            countryModels.Add(new CountryModel() { CountryName = "莫三比克 / 莫桑比克", CountryCode2 = "MZ", code = "MOZ" });
            countryModels.Add(new CountryModel() { CountryName = "緬甸", CountryCode2 = "MM", code = "MMR" });
            countryModels.Add(new CountryModel() { CountryName = "納米比亞", CountryCode2 = "NA", code = "NAM" });
            countryModels.Add(new CountryModel() { CountryName = "諾魯 / 瑙魯", CountryCode2 = "NR", code = "NRU" });
            countryModels.Add(new CountryModel() { CountryName = "尼泊爾", CountryCode2 = "NP", code = "NPL" });
            countryModels.Add(new CountryModel() { CountryName = "荷蘭", CountryCode2 = "NL", code = "NLD" });
            countryModels.Add(new CountryModel() { CountryName = "新喀里多尼亞", CountryCode2 = "NC", code = "NCL" });
            countryModels.Add(new CountryModel() { CountryName = "紐西蘭", CountryCode2 = "NZ", code = "NZL" });
            countryModels.Add(new CountryModel() { CountryName = "尼加拉瓜", CountryCode2 = "NI", code = "NIC" });
            countryModels.Add(new CountryModel() { CountryName = "尼日爾(尼日)", CountryCode2 = "NE", code = "NER" });
            countryModels.Add(new CountryModel() { CountryName = "奈及利亞 / 尼日利亞", CountryCode2 = "NG", code = "NGA" });
            countryModels.Add(new CountryModel() { CountryName = "紐埃", CountryCode2 = "NU", code = "NIU" });
            countryModels.Add(new CountryModel() { CountryName = "諾福克島", CountryCode2 = "NF", code = "NFK" });
            countryModels.Add(new CountryModel() { CountryName = "北馬里亞納群島", CountryCode2 = "MP", code = "MNP" });
            countryModels.Add(new CountryModel() { CountryName = "挪威", CountryCode2 = "NO", code = "NOR" });
            countryModels.Add(new CountryModel() { CountryName = "阿曼", CountryCode2 = "OM", code = "OMN" });
            countryModels.Add(new CountryModel() { CountryName = "巴基斯坦", CountryCode2 = "PK", code = "PAK" });
            countryModels.Add(new CountryModel() { CountryName = "帛琉", CountryCode2 = "PW", code = "PLW" });
            countryModels.Add(new CountryModel() { CountryName = "巴勒斯坦", CountryCode2 = "PS", code = "PSE" });
            countryModels.Add(new CountryModel() { CountryName = "巴拿馬", CountryCode2 = "PA", code = "PAN" });
            countryModels.Add(new CountryModel() { CountryName = "巴布亞紐幾內亞 / 巴布亞新幾內亞", CountryCode2 = "PG", code = "PNG" });
            countryModels.Add(new CountryModel() { CountryName = "巴拉圭", CountryCode2 = "PY", code = "PRY" });
            countryModels.Add(new CountryModel() { CountryName = "秘魯", CountryCode2 = "PE", code = "PER" });
            countryModels.Add(new CountryModel() { CountryName = "菲律賓", CountryCode2 = "PH", code = "PHL" });
            countryModels.Add(new CountryModel() { CountryName = "皮特肯群島", CountryCode2 = "PN", code = "PCN" });
            countryModels.Add(new CountryModel() { CountryName = "波蘭", CountryCode2 = "PL", code = "POL" });
            countryModels.Add(new CountryModel() { CountryName = "葡萄牙", CountryCode2 = "PT", code = "PRT" });
            countryModels.Add(new CountryModel() { CountryName = "波多黎各", CountryCode2 = "PR", code = "PRI" });
            countryModels.Add(new CountryModel() { CountryName = "卡達 / 卡塔爾", CountryCode2 = "QA", code = "QAT" });
            countryModels.Add(new CountryModel() { CountryName = "留尼旺", CountryCode2 = "RE", code = "REU" });
            countryModels.Add(new CountryModel() { CountryName = "羅馬尼亞", CountryCode2 = "RO", code = "ROU" });
            countryModels.Add(new CountryModel() { CountryName = "俄羅斯", CountryCode2 = "RU", code = "RUS" });
            countryModels.Add(new CountryModel() { CountryName = "盧安達 / 盧旺達", CountryCode2 = "RW", code = "RWA" });
            countryModels.Add(new CountryModel() { CountryName = "聖巴泰勒米", CountryCode2 = "BL", code = "BLM" });
            countryModels.Add(new CountryModel() { CountryName = "聖赫勒拿、亞森欣與垂斯坦昆哈 / 聖海倫娜、阿森松和崔斯坦達庫尼亞", CountryCode2 = "SH", code = "SHN" });
            countryModels.Add(new CountryModel() { CountryName = "聖克里斯多福及尼維斯 / 聖基茨和尼維斯", CountryCode2 = "KN", code = "KNA" });
            countryModels.Add(new CountryModel() { CountryName = "聖露西亞 / 聖盧西亞", CountryCode2 = "LC", code = "LCA" });
            countryModels.Add(new CountryModel() { CountryName = "法屬聖馬丁", CountryCode2 = "MF", code = "MAF" });
            countryModels.Add(new CountryModel() { CountryName = "聖皮耶與密克隆 / 聖皮埃爾和密克隆", CountryCode2 = "PM", code = "SPM" });
            countryModels.Add(new CountryModel() { CountryName = "聖文森及格瑞那丁 / 聖文森特和格林納丁斯", CountryCode2 = "VC", code = "VCT" });
            countryModels.Add(new CountryModel() { CountryName = "薩摩亞", CountryCode2 = "WS", code = "WSM" });
            countryModels.Add(new CountryModel() { CountryName = "聖馬利諾", CountryCode2 = "SM", code = "SMR" });
            countryModels.Add(new CountryModel() { CountryName = "聖多美普林西比 / 聖多美和普林西比", CountryCode2 = "ST", code = "STP" });
            countryModels.Add(new CountryModel() { CountryName = "沙烏地阿拉伯 / 沙地阿拉伯", CountryCode2 = "SA", code = "SAU" });
            countryModels.Add(new CountryModel() { CountryName = "塞內加爾", CountryCode2 = "SN", code = "SEN" });
            countryModels.Add(new CountryModel() { CountryName = "塞爾維亞", CountryCode2 = "RS", code = "SRB" });
            countryModels.Add(new CountryModel() { CountryName = "塞席爾 / 塞舌爾", CountryCode2 = "SC", code = "SYC" });
            countryModels.Add(new CountryModel() { CountryName = "獅子山 / 塞拉利昂", CountryCode2 = "SL", code = "SLE" });
            countryModels.Add(new CountryModel() { CountryName = "新加坡", CountryCode2 = "SG", code = "SGP" });
            countryModels.Add(new CountryModel() { CountryName = "荷屬聖馬丁", CountryCode2 = "SX", code = "SXM" });
            countryModels.Add(new CountryModel() { CountryName = "斯洛伐克", CountryCode2 = "SK", code = "SVK" });
            countryModels.Add(new CountryModel() { CountryName = "斯洛維尼亞 / 斯洛文尼亞", CountryCode2 = "SI", code = "SVN" });
            countryModels.Add(new CountryModel() { CountryName = "索羅門群島 / 所羅門群島", CountryCode2 = "SB", code = "SLB" });
            countryModels.Add(new CountryModel() { CountryName = "索馬里 / 索馬利亞", CountryCode2 = "SO", code = "SOM" });
            countryModels.Add(new CountryModel() { CountryName = "南非", CountryCode2 = "ZA", code = "ZAF" });
            countryModels.Add(new CountryModel() { CountryName = "南喬治亞與南桑威奇 / 南佐治亞及南三文治", CountryCode2 = "GS", code = "SGS" });
            countryModels.Add(new CountryModel() { CountryName = "南蘇丹", CountryCode2 = "SS", code = "SSD" });
            countryModels.Add(new CountryModel() { CountryName = "西班牙", CountryCode2 = "ES", code = "ESP" });
            countryModels.Add(new CountryModel() { CountryName = "斯里蘭卡", CountryCode2 = "LK", code = "LKA" });
            countryModels.Add(new CountryModel() { CountryName = "蘇丹", CountryCode2 = "SD", code = "SDN" });
            countryModels.Add(new CountryModel() { CountryName = "蘇利南 / 蘇里南", CountryCode2 = "SR", code = "SUR" });
            countryModels.Add(new CountryModel() { CountryName = "斯瓦巴和揚馬延", CountryCode2 = "SJ", code = "SJM" });
            countryModels.Add(new CountryModel() { CountryName = "史瓦帝尼 / 斯威士蘭", CountryCode2 = "SZ", code = "SWZ" });
            countryModels.Add(new CountryModel() { CountryName = "瑞典", CountryCode2 = "SE", code = "SWE" });
            countryModels.Add(new CountryModel() { CountryName = "瑞士", CountryCode2 = "CH", code = "CHE" });
            countryModels.Add(new CountryModel() { CountryName = "敘利亞", CountryCode2 = "SY", code = "SYR" });
            countryModels.Add(new CountryModel() { CountryName = "臺灣(台灣)", CountryCode2 = "TW", code = "TWN" });
            countryModels.Add(new CountryModel() { CountryName = "塔吉克", CountryCode2 = "TJ", code = "TJK" });
            countryModels.Add(new CountryModel() { CountryName = "坦尚尼亞 / 坦桑尼亞", CountryCode2 = "TZ", code = "TZA" });
            countryModels.Add(new CountryModel() { CountryName = "泰國", CountryCode2 = "TH", code = "THA" });
            countryModels.Add(new CountryModel() { CountryName = "東帝汶", CountryCode2 = "TL", code = "TLS" });
            countryModels.Add(new CountryModel() { CountryName = "多哥", CountryCode2 = "TG", code = "TGO" });
            countryModels.Add(new CountryModel() { CountryName = "托克勞", CountryCode2 = "TK", code = "TKL" });
            countryModels.Add(new CountryModel() { CountryName = "東加 / 湯加", CountryCode2 = "TO", code = "TON" });
            countryModels.Add(new CountryModel() { CountryName = "千里達及托巴哥 / 千里達及多巴哥", CountryCode2 = "TT", code = "TTO" });
            countryModels.Add(new CountryModel() { CountryName = "突尼西亞", CountryCode2 = "TN", code = "TUN" });
            countryModels.Add(new CountryModel() { CountryName = "土耳其", CountryCode2 = "TR", code = "TUR" });
            countryModels.Add(new CountryModel() { CountryName = "土庫曼斯坦(土庫曼)", CountryCode2 = "TM", code = "TKM" });
            countryModels.Add(new CountryModel() { CountryName = "土克凱可群島 / 特克斯和凱科斯群島", CountryCode2 = "TC", code = "TCA" });
            countryModels.Add(new CountryModel() { CountryName = "吐瓦魯 / 圖瓦盧", CountryCode2 = "TV", code = "TUV" });
            countryModels.Add(new CountryModel() { CountryName = "烏干達", CountryCode2 = "UG", code = "UGA" });
            countryModels.Add(new CountryModel() { CountryName = "烏克蘭", CountryCode2 = "UA", code = "UKR" });
            countryModels.Add(new CountryModel() { CountryName = "阿拉伯聯合大公國(阿聯) / 阿拉伯聯合酋長國(阿聯酋)", CountryCode2 = "AE", code = "ARE" });
            countryModels.Add(new CountryModel() { CountryName = "英國", CountryCode2 = "GB", code = "GBR" });
            countryModels.Add(new CountryModel() { CountryName = "美國", CountryCode2 = "US", code = "USA" });
            countryModels.Add(new CountryModel() { CountryName = "美國本土外小島嶼", CountryCode2 = "UM", code = "UMI" });
            countryModels.Add(new CountryModel() { CountryName = "烏拉圭", CountryCode2 = "UY", code = "URY" });
            countryModels.Add(new CountryModel() { CountryName = "烏茲別克", CountryCode2 = "UZ", code = "UZB" });
            countryModels.Add(new CountryModel() { CountryName = "萬那杜 / 瓦努阿圖", CountryCode2 = "VU", code = "VUT" });
            countryModels.Add(new CountryModel() { CountryName = "梵蒂岡", CountryCode2 = "VA", code = "VAT" });
            countryModels.Add(new CountryModel() { CountryName = "委內瑞拉", CountryCode2 = "VE", code = "VEN" });
            countryModels.Add(new CountryModel() { CountryName = "越南", CountryCode2 = "VN", code = "VNM" });
            countryModels.Add(new CountryModel() { CountryName = "英屬維京群島 / 英屬處女群島", CountryCode2 = "VG", code = "VGB" });
            countryModels.Add(new CountryModel() { CountryName = "美屬維京群島 / 美屬處女群島", CountryCode2 = "VI", code = "VIR" });
            countryModels.Add(new CountryModel() { CountryName = "瓦利斯和富圖納", CountryCode2 = "WF", code = "WLF" });
            countryModels.Add(new CountryModel() { CountryName = "西撒哈拉", CountryCode2 = "EH", code = "ESH" });
            countryModels.Add(new CountryModel() { CountryName = "葉門 / 也門", CountryCode2 = "YE", code = "YEM" });
            countryModels.Add(new CountryModel() { CountryName = "尚比亞 / 贊比亞", CountryCode2 = "ZM", code = "ZMB" });
            countryModels.Add(new CountryModel() { CountryName = "辛巴威 / 津巴布韋", CountryCode2 = "ZW", code = "ZWE" });
            #endregion
            return countryModels;
        }

        public IActionResult GetGeoJson()
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "js", "world.geojson");
            var geoJson = System.IO.File.ReadAllText(filePath);
            byte[] UTF8bytes = Encoding.UTF8.GetBytes(geoJson);
            return File(UTF8bytes, "application/geo+json");
        }
    }
}

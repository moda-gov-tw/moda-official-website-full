using DBModel;
using Services.Static;
using System.Linq;
using Utility;

namespace ConsoleApp
{
    public class Sitemap
    {
        public static void start(string WebSiteUrl, string IsOfficial, string gitPush, List<StaticLink> staticLinks)
        {
            var header = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            var start = "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">";
            var info = "";
            var filterEndList = new List<string>() { "Header.html", "Footer.html", "1.html" };
            var filterStrList = new List<string>() { "file", "datasets" };
            var CpNoNeedShowData = StaticLinkService.NoNeedSettingSiteMapData("cp","");
            var CpNoNeedShowDataStaticLinkSN = CpNoNeedShowData.Select(x => x.StaticLinkSN).ToList();
            var VideoNoNeedShowData = StaticLinkService.NoNeedSettingSiteMapData("", "ImageTextList");
            var VideoNoNeedShowDataStaticLinkSN = VideoNoNeedShowData.Select(x => x.StaticLinkSN).ToList();
            var LinkNoNeedShowData = StaticLinkService.NoNeedSettingSiteMapData("", "","2");
            var LinkNoNeedShowDataStaticLinkSN = LinkNoNeedShowData.Select(x => x.StaticLinkSN).ToList();
            var noNeedData = CpNoNeedShowDataStaticLinkSN.Union(VideoNoNeedShowDataStaticLinkSN).Union(LinkNoNeedShowDataStaticLinkSN);
            var StaticUrl = "0";
            foreach (var u in staticLinks.Where(x => x.StaticUrl.Contains("/index.html")))
            {
                u.StaticUrl = u.StaticUrl.Replace("/index.html", "");
            }
            foreach (var data in staticLinks.Where(x => !noNeedData.Contains(x.StaticLinkSN)).OrderByDescending(x=>x.WebSiteID).OrderBy(x => x.StaticUrl))
            {
                if (StaticUrl != data.StaticUrl)
                {
                    var needStiemap = true;
                    var dataArray = data.StaticUrl.Split('/');
                    if (dataArray.Length > 1)
                    {
                        if (filterStrList.Contains(dataArray[1]))
                        {
                            needStiemap = false;
                        }
                        if (filterEndList.Contains(dataArray[dataArray.Length - 1]))
                        {
                            needStiemap = false;
                        }
                    }
                    if (needStiemap)
                    {
                        var Url = $"{WebSiteUrl}{data.StaticUrl}";
                        var date = data.staticDate == null ? DateTime.UtcNow.AddHours(8) : data.staticDate;
                        info += @$"<url>
    <loc>{Url.Replace("&", "%26")}</loc>
    <lastmod>{date.Value.ToString("yyyy-MM-dd")}</lastmod>
</url>
";
                    }
                }
                StaticUrl = data.StaticUrl;
            }
            var end = "</urlset>";
            var txt = $@"{header}
{start}
{info}
{end}";
            DownloadFile.SaveOther($@"{gitPush}\sitemap.xml", txt);
        }
    }
}

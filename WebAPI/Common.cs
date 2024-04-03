using DBModel;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Utility.Model;

namespace WebAPI
{
    public class Common
    {
        /// <summary>
        /// 填寫log
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLog(string text)
        {
            try
            {
                var txt = text.Replace("/", "").Replace("..", "");
                var logFolder = GetAppsetting("LogFolder");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }
                File.AppendAllText($"{logFolder}/{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.txt", $"{txt}〔{DateTime.UtcNow.AddHours(8).ToString("MM/dd HH:mm:ss")}〕" + Environment.NewLine);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 讀取appsetting
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppsetting(string key)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                             .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("appsettings.json");
                var config = builder.Build();
                foreach (var provider in config.Providers)
                {
                    provider.TryGet(key, out var value);
                    return value;
                }
            }
            catch (Exception)
            {
                return "";
            }
            return "";
        }
        #region MyRegion
        public static string CreateRSSFeed2(RssNodeModel channel, WebLevel level, string Author)
        {
            var host = Common.GetAppsetting("WebSiteHost");
            channel.Items = new List<RssItemModel>();
            if (level != null)
            {
                var news = Services.WebSite.HomeService.getRSS(level.WebSiteID, level.Lang, level.MainSN.Value, host);
                var RssItems = news.Select(x => new RssItemModel()
                {
                    Title = x.BasicNews.Title,
                    Description = x.BasicNews.ContentText ?? "",
                    Link = x.DynamicURL,
                    PubDate = x.BasicNews.StartDate.Value,
                    Author = Author
                }).ToList();
                channel.Items = RssItems;
            }
            var data = RssDocumentGenerate(channel);
            data = HttpUtility.HtmlDecode(data);
            return data;
        }
        static string RssDocumentGenerate(RssNodeModel rssNode)
        {
            const string SpecialChars = @"<>&";
            Encoding enc = Encoding.UTF8;
            XmlWriterSettings settings = new();
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;
            settings.NewLineOnAttributes = true;
            settings.NewLineChars = Environment.NewLine;
            settings.Encoding = new UTF8Encoding(false); ;
            MemoryStream ms = new MemoryStream();
            using (XmlWriter xmlTw = XmlWriter.Create(ms, settings))
            {
                xmlTw.WriteStartDocument(false);
                xmlTw.WriteStartElement("rss");
                xmlTw.WriteAttributeString("xmlns", "dc", null, "http://purl.org/dc/elements/1.1/");
                xmlTw.WriteAttributeString("xmlns", "d1p1", null, "schemaLocation");
                xmlTw.WriteAttributeString("xmlns", "xsi", null, "http://www.gov.tw/schema/RSS20.xsd");
                xmlTw.WriteAttributeString("version", "2.0");
                xmlTw.WriteStartElement("channel");
                xmlTw.WriteStartElement("title");
                if (rssNode.Title.IndexOfAny(SpecialChars.ToCharArray()) != -1)
                {
                    xmlTw.WriteCData(rssNode.Title);
                }
                else
                {
                    xmlTw.WriteString($@"{rssNode.Title}");
                }
                xmlTw.WriteEndElement();
                xmlTw.WriteElementString("link", rssNode.Link);
                if (!string.IsNullOrWhiteSpace(rssNode.Description))
                {
                    xmlTw.WriteStartElement("description");
                    if (rssNode.Description.IndexOfAny(SpecialChars.ToCharArray()) != -1)
                    {
                        xmlTw.WriteCData($@"{rssNode.Description.Replace("\n", "")}");
                    }
                    else
                    {
                        xmlTw.WriteString($@"{rssNode.Description.Replace("\n", "")}");
                    }
                    xmlTw.WriteEndElement();
                }
                xmlTw.WriteElementString("lastBuildDate", ToGMTString(rssNode.PubDate.Value));
                xmlTw.WriteElementString("language", rssNode.Language);
                foreach (var item in rssNode.Items)
                {
                    xmlTw.WriteStartElement("item");

                    xmlTw.WriteStartElement("title");
                    if (item.Title.IndexOfAny(SpecialChars.ToCharArray()) != -1)
                    {
                        xmlTw.WriteCData(item.Title);
                    }
                    else
                    {
                        xmlTw.WriteString($@"{item.Title}");
                    }
                    xmlTw.WriteEndElement();
                    xmlTw.WriteElementString("link", item.Link);

                    xmlTw.WriteStartElement("description");
                    if (item.Description.IndexOfAny(SpecialChars.ToCharArray()) != -1)
                    {
                        xmlTw.WriteCData(item.Description.Replace("\n", ""));
                    }
                    else
                    {
                        xmlTw.WriteString($@"{item.Description.Replace("\n", "")}");
                    }
                    xmlTw.WriteEndElement();
                    xmlTw.WriteElementString("author", item.Author);
                    xmlTw.WriteElementString("pubDate", ToGMTString( item.PubDate));
                    xmlTw.WriteEndElement();
                }
                xmlTw.WriteEndElement();
                xmlTw.WriteEndElement();
                xmlTw.WriteEndDocument();
                xmlTw.Flush();
                xmlTw.Close();
            }
            string result = HttpUtility.HtmlEncode( enc.GetString(ms.ToArray()));

            return result;
        }
        #endregion
        public static string ToGMTString(DateTime dt)
        {
            return dt.ToUniversalTime().ToString("r");
        }

    }
}

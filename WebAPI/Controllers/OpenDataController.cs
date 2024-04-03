using Microsoft.AspNetCore.Mvc;
using Services.Files;
using Services.SystemManageMent;
using SixLabors.ImageSharp.ColorSpaces;
using System.Text;
using static Utility.Files;

namespace WebAPI.Controllers
{
    /// <summary>
    /// OpenData
    /// </summary>
    public class OpenDataController : Controller
    {
        /// <summary>
        /// 取得開放資料集
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ODKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/{type}/{ODKey}")]
        public IActionResult GetOpenData(string type, string ODKey)
        {
            try
            {
                var Main = OpenDataService.GetOpendataMain(ODKey);
                var Detail = OpenDataService.GetOpendataDetail(ODKey);
                var Schema = OpenDataService.GetOpendataSchema(ODKey);
                var url = Common.GetAppsetting("WebSiteHost");

                if (Main?.IsEnable == "1") 
                {
                    switch (type)
                    {
                        case "csv":
                            if (Main.IsCSV == null) break;
                            var csv = OpenDataService.CreateCsv(Main, Detail, Schema, url);
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                                {
                                    writer.Write(csv);
                                    writer.Flush();

                                    return File(memoryStream.ToArray(), "text/csv; charset=utf-8");
                                }
                            }
                        case "json":
                            if (Main.IsJSON == null) break;
                            var json = OpenDataService.CreateJson(Main, Detail, Schema, url);
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                                {
                                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                                    serializer.Serialize(writer, json);
                                    writer.Flush();

                                    return File(memoryStream.ToArray(), "application/json; charset=utf-8");
                                }
                            }
                        case "xml":
                            if (Main.IsXML == null) break;
                            var xml = OpenDataService.CreateXml(Main, Detail, Schema, url);
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                                {
                                    writer.WriteLine(xml.Declaration.ToString());
                                    writer.Write(xml);
                                    writer.Flush();
                                    return File(memoryStream.ToArray(), "text/xml; charset=utf-8");
                                }
                            }
                    }
                }
                return Redirect("~/404.html");
            }
            catch (Exception ex)
            {
                Common.WriteLog($"OpenDataController/GetOpenData - Error {ex.ToString()}");
                return Redirect("~/404.html");
            }
        }

        /// <summary>
        /// 取得開放資料集檔案
        /// From OpenData
        /// </summary>
        /// <param name="OpenDataMainSN"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/File/{OpenDataMainSN}")]
        public async Task<ActionResult> GetNewsOpenDataFile(int OpenDataMainSN)
        {
            try
            {
                var WebFile = OpenDataService.GetOpneDataFile(OpenDataMainSN);

                if (WebFile != null) 
                {
                    var filesActionModel = FilesService.Get(WebFile.WEBFileID);
                    var SavefileMdeol = new SaveFileModel()
                    {
                        path = filesActionModel.webfile.FileApiPath
                    };
                    var httpClientHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
                    };
                    using var client = new HttpClient(httpClientHandler);
                    var apiUrl = Common.GetAppsetting("FileServiceApi");
                    string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(SavefileMdeol);
                    HttpContent httpContent = new StringContent(requestJson);
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync($"{apiUrl}/File/Get", httpContent);
                    var contents = await response.Content.ReadAsStringAsync();

                    var a = System.Text.Json.JsonSerializer.Deserialize<Models.ApiModel.ApiResultModel>(contents);
                    if (a.statusCode == 200)
                    {
                        var fr = System.Text.Json.JsonSerializer.Deserialize<FileResponse>(a.content);
                        byte[] myByteArray = Convert.FromBase64String(fr.FileStream);
                        Stream stream = new System.IO.MemoryStream(myByteArray);

                        var filename = "";
                        if (filesActionModel.webfile.FileTitle != null)
                        {
                            filename = filesActionModel.webfile.FileTitle.Replace(filesActionModel.webfile.FileType, "") + filesActionModel.webfile.FileType;
                        }
                        else
                        {
                            filename = filesActionModel.webfile.OriginalFileName;
                        }
                        if (fr.ContentType.IndexOf("pdf") > 0)
                        {
                            byte[] pbytes = new byte[0];
                            using (BinaryReader r = new BinaryReader(stream))
                            {
                                r.BaseStream.Seek(0, SeekOrigin.Begin);
                                pbytes = r.ReadBytes((int)r.BaseStream.Length);
                            }
                            Response.Headers.Add("Content-Disposition", $@"inline; filename={Uri.EscapeUriString(filename)}");
                            return File(pbytes, fr.ContentType);
                        }
                        return File(stream, fr.ContentType, filename);
                    }
                }

                return Redirect("~/404.html");
            }
            catch (Exception ex)
            {
                Common.WriteLog($"OpenDataController/GetOpenData - Error {ex.ToString()}");
                return Redirect("~/404.html");
            }
        }

        /// <summary>
        /// 取得開放資料集檔案
        /// From News
        /// </summary>
        /// <param name="nsn">NewsSN</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/Files/{nsn}")]
        public async Task<ActionResult> GetOpenDataFile(int nsn)
        {
            try
            {
                var news = Services.WebSite.NewsService.GetNews(nsn);
                if (news != null && news.BasicData.Module == "OpendataNews" && news.Files.Count() > 0)
                {
                    if(!Services.CommonService.CheckLevelByTree(news.BasicData.WebLevelSN)) return NotFound("查無資料");

                    var thefile = news.Files.FirstOrDefault();
                    var SavefileMdeol = new SaveFileModel()
                    {
                        path = thefile.FileApiPath
                    };
                    var apiUrl = Common.GetAppsetting("FileServiceApi");
                    string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(SavefileMdeol);
                    var a = Utility.ApiContent.postApi<Models.ApiModel.ApiResultModel>($"{apiUrl}/File/Get" , requestJson , "application/json");
                    if (a?.statusCode == 200)
                    {
                        var fileCoutent = a.content;
                        var fr = Utility.ApiContent.JsonDeserializeObject<FileResponse>(fileCoutent);
                        byte[] myByteArray = Convert.FromBase64String(fr.FileStream);
                        Stream stream = new System.IO.MemoryStream(myByteArray);

                        var filename = "";
                        if (thefile.FileTitle != null)
                        {
                            filename = thefile.FileTitle.Replace(thefile.FileType, "") + thefile.FileType;
                        }
                        else
                        {
                            filename = thefile.OriginalFileName;
                        }
                        if (fr.ContentType.IndexOf("pdf") > 0)
                        {
                            byte[] pbytes = new byte[0];
                            using (BinaryReader r = new BinaryReader(stream))
                            {
                                r.BaseStream.Seek(0, SeekOrigin.Begin);
                                pbytes = r.ReadBytes((int)r.BaseStream.Length);
                            }
                            Response.Headers.Add("Content-Disposition", $@"inline; filename={Uri.EscapeUriString(filename)}");
                            Response.Headers.Add("X-Content-Type-Options", "nosniff");
                            return File(pbytes, fr.ContentType);
                        }
                        return File(stream, fr.ContentType, filename);
                    }
                }

                return Redirect("~/404.html");
            }
            catch (Exception)
            {
                return Redirect("~/404.html");
            }
        }

        [HttpGet]
        [Route("Bilingual/{type}")]
        public IActionResult GetBilingualData(string type )
        {
            var data = OpenDataService.GetBilingualIOpenData();
            var url = Common.GetAppsetting("WebSiteHost");
            switch (type.ToLower()) {
                case "csv":
                    var csv = OpenDataService.CreateBilingualCsv(data, url);
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                        {
                            writer.Write(csv);
                            writer.Flush();
                            return File(memoryStream.ToArray(), "text/csv; charset=utf-8", $@"Bilingual_csv_{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.csv");
                        }
                    }
                case "json":
                    var json = OpenDataService.CreateBilingualJson(data, url);
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                        {
                            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                            serializer.Serialize(writer, json);
                            writer.Flush();

                            return File(memoryStream.ToArray(), "application/json; charset=utf-8" , $@"Bilingual_json_{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.json");
                        }
                    }
                case "xml":
                    var xml = OpenDataService.CreateBilingualXml(data, url);
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                        {
                            writer.WriteLine(xml.Declaration.ToString());
                            writer.Write(xml);
                            writer.Flush();
                            return File(memoryStream.ToArray(), "text/xml; charset=utf-8", $@"Bilingual_xml_{DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")}.xml");
                        }
                    }

            }
            return Redirect("~/404.html");
        }
    }
}

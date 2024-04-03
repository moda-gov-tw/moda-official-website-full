using DBModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services.Authorization;
using Services.Files;
using System.Text.Json;
using static Utility.Files;
using static Utility.MailBox.Api;

namespace WebAPI.Controllers
{
    /// <summary>
    /// FileApi
    /// </summary>
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class FileController : Controller
    {
        /// <summary>
        /// 取得File
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("[controller]/[action]/{fileID?}")]
        public async Task<ActionResult> GetAsync(string fileID)
        {
            try
            {
                var filesActionModel = FilesService.Get(fileID);

                if (filesActionModel.IsActionSuccess)
                {
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

                    var a = JsonSerializer.Deserialize<Models.ApiModel.ApiResultModel>(contents);
                    if (a?.statusCode == 200)
                    {
                        var fr = JsonSerializer.Deserialize<FileResponse>(a.content);
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
                    else
                    {
                        Common.WriteLog($"File/get - File services 錯誤 {fileID} {a.statusCode} {a.content}");
                        return Redirect("~/404.html");
                    }
                }
                else
                {
                    Common.WriteLog($"File/get - 資料庫查無資料 {fileID}");
                    Common.WriteLog($"filesActionModel : {filesActionModel.ActionMessage}");
                    return Redirect("~/404.html");
                }
            }
            catch (Exception ex)
            {
                Common.WriteLog($"File/get - Error {ex.ToString()}");
                return Redirect("~/404.html");
            }
        }

        /// <summary>
        /// 取得File 添加webSiteId&Lang
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="webSiteID"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[controller]/Get/{WebSiteID}/{Lang}/{fileID?}")]
        public async Task<ActionResult> Get2Async(string fileID, string webSiteID, string lang)
        {
            try
            {
                var filesActionModel = FilesService.Get(fileID);

                if (filesActionModel.IsActionSuccess)
                {
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

                    var a = JsonSerializer.Deserialize<Models.ApiModel.ApiResultModel>(contents);
                    if (a.statusCode == 200)
                    {
                        var fr = JsonSerializer.Deserialize<FileResponse>(a.content);
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
                    else
                    {


                        return Redirect("~/404.html");
                    }
                }
                else
                {


                    return Redirect("~/404.html");
                }
            }
            catch (Exception ex)
            {
                return Redirect("~/404.html");
            }
        }
    }
}

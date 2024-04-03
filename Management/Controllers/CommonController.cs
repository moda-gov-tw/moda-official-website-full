using DBModel;
using Management.ManagementUtility;
using Management.Models;
using Management.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using Services.Files;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Utility;
using Utility.sysConstTable.field;
using static Utility.Files;

namespace Management.Controllers
{
    public class CommonController : BaseController
    {


        public IActionResult Upload()
        {
            return View();
        }



        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="files">檔案</param>
        /// <param name="fth">檔案路徑</param>
        /// <param name="gid">群組代號</param>
        /// <param name="maxFilecount">最大檔案設定 沒有設定不限</param>
        /// <returns></returns>
        public async Task<IActionResult> UploadFileAsync(List<IFormFile> files, string fth, string gid = "1", int maxFilecount = 0, bool isFileShare = false, string lan = "")
        {
            try
            {
                var WEBFileID = Regular.GetRandomString(15, RegularType.notspecial);
                var chk = false; //確認狀態
                var item = new ResultFileApiModel();
                //檢核是否為偽造副檔名
                if (!Files.CheckFileCentType(files))
                {
                    return BadRequest("副檔名檢核有誤");
                }
                string fileType = files.FirstOrDefault()?.FileName.Split('.').Last().ToLower();
                if (fileType.Contains("md"))
                {
                    var _str = Files.ReadMD(files);
                    var MD_Data = RelaxMD(_str, lan);
                    if (MD_Data == null) return BadRequest("上傳的格式有問題請再檢查。");
                    SetSession($"MD_Data{lan}", MD_Data);
                }
                var FileSession = GetSession<List<CommonFileModel>>("WEBFile");
                if (FileSession == null)
                {
                    FileSession = new List<CommonFileModel>();
                    SetSession("WEBFile", FileSession);
                }
                var stream = files[0].OpenReadStream();
                var fileMdeol = new SaveFileModel()
                {
                    bytes = getByteByStream(stream),
                    FileName = files[0].FileName,
                    isFileShare = isFileShare,
                    path = @$"{UserData.WebSiteID}/{fth}",
                };

                if (files[0].ContentType.Contains("image"))
                {
                    fileMdeol.isImg = true;
                    fileMdeol.localPath = AppSettingHelper.GetAppsetting("StaticPath");
                    fileMdeol.path = @$"copyright/{UserData.WebSiteID}/";
                    var file = Files.Upload(fileMdeol);

                    if (file.check == true)
                    {
                        chk = true;
                        item.content = file;
                    }
                    else
                    {
                        Utility.Mail.Error(file.msg);
                    }
                }
                else
                {
                    var httpClientHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
                    };

                    using (var client = new HttpClient(httpClientHandler))
                    {
                        var apiUrl = AppSettingHelper.GetAppsetting("FileServiceApi");
                        string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(fileMdeol);
                        HttpContent httpContent = new StringContent(requestJson);
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        var task = await client.PostAsync($"{apiUrl}/File/Save", httpContent);
                        var result = task.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(result))
                        {
                            item = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultFileApiModel>(result);
                            if (item.statusCode == System.Net.HttpStatusCode.OK)
                            {
                                chk = true;
                            }
                            else
                            {
                                return BadRequest("副檔名檢核有誤");
                            }
                        }
                        else
                        {
                            return BadRequest("API有問題");
                        }
                    }
                }
                if (chk)
                {
                    var fileFileApiPath = "";
                    var file = item.content;
                    file.CommonFileModel.GroupID = gid;
                    file.CommonFileModel.fileTitle = file.CommonFileModel.fileOriginName;
                    file.CommonFileModel.webFileID = WEBFileID;
                    file.CommonFileModel.IsImage = files[0].ContentType.Contains("image");
                    file.CommonFileModel.lan = lan;
                    if (!file.CommonFileModel.IsImage)
                    {
                        fileFileApiPath = file.CommonFileModel.filePath;
                        file.CommonFileModel.filePath = $"{AppSettingHelper.GetAppsetting("WEBAPI")}{UserData.WebSiteID.ToLower()}/{lan}/{file.CommonFileModel.webFileID}";
                        //  file.CommonFileModel.filePath = AppSettingHelper.GetAppsetting("WEBAPI")  + file.CommonFileModel.webFileID;
                    }
                    if (maxFilecount != 0)
                    {
                        var sFileSession = FileSession.Where(x => x.GroupID == gid);
                        if (sFileSession.Count() >= maxFilecount)
                        {
                            var removeFile = FileSession.Where(x => x.GroupID == gid).First();
                            FileSession.Remove(removeFile);
                        }
                        FileSession.Add(file.CommonFileModel);
                    }
                    else
                    {
                        FileSession.Add(file.CommonFileModel);
                    }
                    SetSession("WEBFile", FileSession);

                    WEBFile tt = new WEBFile()
                    {
                        WEBFileID = WEBFileID,
                        CreatedDate = DateTime.UtcNow.AddHours(8),
                        FileType = file.CommonFileModel.fileExt,
                        FileName = file.CommonFileModel.fileNewName,
                        OriginalFileName = file.CommonFileModel.fileOriginName,
                        FilePath = file.CommonFileModel.filePath,
                        FileSize = (int)file.CommonFileModel.fileSize,
                        FileApiPath = fileFileApiPath,
                        IsEnable = isFileShare ? "1" : "0",
                    };
                    FilesService.Create(tt);

                    return StatusResult(System.Net.HttpStatusCode.OK, "");
                }
                else
                {                   
                    return BadRequest("上傳出現問題，請稍後試試");
                }
            }
            catch (Exception ex)
            {
                //  Utility.Mail.Error(ex.ToString());
                return BadRequest("上傳出現問題，請稍後試試");
            }
        }

        static List<WEBNewsTranscript> RelaxMD(string str, string lang)
        {
            try
            {
                var _str = str.Split("###");
                var WEBNewsTranscripts = new List<WEBNewsTranscript>();
                var item = 0;
                for (int i = 0; i < _str.Length; i++)
                {
                    if (i == 0)
                    {
                        var title = _str[i].Split('\n');
                        for (int j = 0; j < title.Length; j++)
                        {
                            if (j == 0)
                            {
                                WEBNewsTranscripts.Add(new WEBNewsTranscript()
                                {
                                    TranscriptForm = "",
                                    Item = item++,
                                    TranscriptContent = title[j],
                                });
                            }
                            else
                            {
                                if (title[j].Length > 0)
                                {
                                    if (title[j].Substring(0, 1) == ">")
                                    {
                                        WEBNewsTranscripts.Add(new WEBNewsTranscript()
                                        {
                                            TranscriptForm = ">",
                                            Item = item++,
                                            TranscriptContent = title[j].Substring(1, title[j].Length - 1),
                                        });
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        var from = "";
                        var content = "";
                        var checkInfo = _str[i].IndexOf('：');
                        if (checkInfo > -1)
                        {
                            #region 中文版
                            var info = _str[i].Split('：');
                            var info_Count = info.Count();
                            from = info[0];
                            content = info[1];
                            if (info_Count > 2)
                            {
                                content = "";
                                for (int g = 1; g < info_Count; g++)
                                {
                                    content += info[g] + "：";
                                }
                                content = content.Substring(0, content.Length - 1);
                            }
                            #endregion
                        }
                        else
                        {
                            var info = _str[i].Split(':');
                            from = info[0];
                            var info_Count = info.Count();
                            content = info[1];
                            if (info_Count > 2)
                            {
                                content = "";
                                for (int g = 1; g < info_Count; g++)
                                {
                                    content += info[g] + ":";
                                }
                                content = content.Substring(0, content.Length - 1);
                            }
                        }
                        var RelaxLeng = content.Split('[');
                        foreach (var R in RelaxLeng)
                        {
                            content = RealxHref(content, lang);
                        }
                        if (content.Substring(0, 1) == ">")
                        {
                            WEBNewsTranscripts.Add(new WEBNewsTranscript()
                            {
                                TranscriptForm = ">",
                                Item = item++,
                                TranscriptContent = content.Substring(1, content.Length - 1),
                            });
                        }
                        else
                        {
                            var contents = content.Split("\n\n");
                            foreach (var c in contents)
                            {
                                if (!string.IsNullOrWhiteSpace(c))
                                {
                                    if (c.Substring(0, 1) == ">")
                                    {
                                        WEBNewsTranscripts.Add(new WEBNewsTranscript()
                                        {
                                            TranscriptForm = ">",
                                            Item = item++,
                                            TranscriptContent = c.Substring(1, c.Length - 1),
                                        });
                                    }
                                    else
                                    {
                                        WEBNewsTranscripts.Add(new WEBNewsTranscript()
                                        {
                                            TranscriptForm = from,
                                            Item = item++,
                                            TranscriptContent = c,
                                        });
                                    }

                                }
                            }
                        }
                    }
                }
                return WEBNewsTranscripts;
            }
            catch (Exception)
            {

                return null;
            }
        }


        static string RealxHref(string content, string lang)
        {

            var _content = content;
            var _newopen = "另開新視窗";
            var filedownload = "檔案下載";
            if (lang != "zh-tw") { _newopen = "open in new window"; filedownload = "download"; }
            try
            {
                var chk = content.IndexOf("[");
                if (chk > -1)
                {
                    var str_txt = content.Split('[')[1];
                    var href_title = str_txt.Split(']')[0];
                    var _fileType = "";

                    if (href_title.IndexOf("(") > -1)
                    {
                        var str_fileType = href_title.Split('(')[1];
                        _fileType = $"{str_fileType.Split(')')[0]} {filedownload}";
                    }
                    var _title = $"{href_title}";
                    var str_href = str_txt.Split(']')[1].Split('(')[1];
                    var _href = str_href.Split(')')[0];
                    var _txt = $"[{_title}]({_href})";
                    return _content = content.Replace(_txt, @$" <a href='{_href}'  target='_blank' rel='noreferrer noopener' title='{_fileType}({_newopen})' >{href_title}</a>");
                }
                else
                {
                    return content;
                }
            }
            catch (Exception)
            {
                return content;
            }
        }
        /// <summary>
        /// 取得記錄在session資料
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetSessionFiles()
        {
            var files = GetSession<List<CommonFileModel>>("WEBFile");
            return StatusResult(System.Net.HttpStatusCode.OK, files);
        }


        public IActionResult ReMoreSessionFile(string filenewname)
        {
            var files = GetSession<List<CommonFileModel>>("WEBFile");
            if (!string.IsNullOrWhiteSpace(filenewname))
            {
                var remoreData = files.FirstOrDefault(x => x.fileNewName == filenewname);
                files.Remove(remoreData);
                SetSession("WEBFile", files);
            }
            return StatusResult(System.Net.HttpStatusCode.OK, files);
        }

        public async Task<ActionResult> GetFileAsync(string fileID, string ft = "0")
        {
            var filesActionModel = FilesService.Get(fileID, ft);
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
                var apiUrl = AppSettingHelper.GetAppsetting("FileServiceApi");
                string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(SavefileMdeol);
                HttpContent httpContent = new StringContent(requestJson);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync($"{apiUrl}/File/Get", httpContent);
                var contents = await response.Content.ReadAsStringAsync();

                var a = JsonSerializer.Deserialize<Models.ApiResultModel>(contents);
                if (a.statusCode == 200)
                {
                    var fr = JsonSerializer.Deserialize<FileResponse>(a.content);
                    byte[] myByteArray = Convert.FromBase64String(fr.FileStream);
                    Stream stream = new System.IO.MemoryStream(myByteArray);
                    return File(stream, fr.ContentType, fr.FileName + fr.FileType);
                }
                else
                {
                    return StatusResult(System.Net.HttpStatusCode.BadRequest, a.content);
                }
            }
            else
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, filesActionModel.ActionMessage);
            }
        }

        /// <summary>
        /// 共用帳號選擇元件
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectorUser()
        {
            return View();
        }
        public ActionResult SelectorUserList(string key, string dep, string group, int p = 1, int DisplayCount = 10)
        {
            var data = new SelectorUserListModel();
            if (CommonUtility.UrlKey(ref group) && int.TryParse(group, out int _group))
            {

                DefaultPager pager = new DefaultPager()
                {
                    DisplayCount = DisplayCount,
                    p = p
                };
                var list = CommonService.UserSelectorGetUserList(_group, key, dep, ref pager);
                data.defaultPager = pager;
                data.sysUsers = list;
            }
            return View(data);
        }

        public ActionResult TreeDepartment()
        {
            var list = WebLevelManagementService.sysDepartments();
            return View(list);
        }
        /// <summary>
        /// 回傳所有User的Json字串
        /// </summary>
        /// <returns></returns>
      
        public ContentResult SelectorAllUser(string key, string dep, string group)
        {

            string _dep = WebUtility.HtmlEncode(dep);
            string _key = WebUtility.HtmlEncode(key);
            string result = "";
            if (CommonUtility.UrlKey(ref group) && int.TryParse(group, out int _group))
            {
                var list = CommonService.UserSelectorGetUserList(_group, _key, _dep);

                foreach (var item in list)
                {
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        result += ",";
                    }
                    result += "{\"userID\":\"" + CommonUtility.GetUrlAesEncrypt(item.UserID) + "\"}";
                }
            }
            return Content("[" + result + "]");
        }


        /// <summary>
        /// 分頁
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public ActionResult Pagination(DefaultPager pager)
        {
            logActionModel.needInsertData = false;
            return View(pager);
        }
        
        public ActionResult CommonWebNews(Utility.EnumWeblevelType webLevelManagment, string websiteid, string lan = "", bool disabled = false)
        {
            List<DBModel.WebLevel> data = new List<DBModel.WebLevel>();
            if (!string.IsNullOrWhiteSpace(lan))
            {
                data = WebLevelManagementService.GetWebLevelList(webLevelManagment, websiteid, lan);
            }
            else
            {
                data = WebLevelManagementService.GetWebLevelList(webLevelManagment, websiteid);
            }

            var list = data.Select(x => new CommonWebNewsModel()
            {
                Lang = x.Lang,
                Module = x.Module,
                ParentSN = x.ParentSN.ToString(),
                SortOrder = x.SortOrder.ToString(),
                Title = x.Title,
                WebLevelSN = x.WebLevelSN.ToString(),
                WeblevelType = x.WeblevelType,
                WebSiteID = x.WebSiteID,
                MainSN = x.MainSN.ToString(),
                Disabled = disabled
            }).ToList();

            return View(list);
        }

        public ActionResult CommonLogAction(string Module, string SourceSN, string websiteid)
        {
            DataTable data = new DataTable();

            switch (Module)
            {
                case "WEBLevel":
                    if (!string.IsNullOrWhiteSpace(SourceSN))
                    {
                        data = LogService.GetLogActionByWEBLevel(SourceSN, websiteid);
                    }
                    break;
                default:
                    if (!string.IsNullOrWhiteSpace(SourceSN))
                    {
                        data = LogService.GetLogActionByWEBNews(SourceSN, websiteid);
                    }
                    break;
            }

            var result = (from d in data.AsEnumerable()
                          select new CommonLogActionModel
                          {
                              UserID = d.Field<string>("UserID"),
                              UserName = d.Field<string>("UserName"),
                              Lang = d.Field<string>("Lang"),
                              Action2 = d.Field<string>("Action2"),
                              CreatedDate = d.Field<DateTime>("CreatedDate"),
                              ProcessIPAddress = d.Field<string>("ProcessIPAddress"),
                              SourceSN = d.Field<int>("SourceSN"),
                              MessageInput = d.Field<string>("MessageInput"),
                              SourceTable = d.Field<string>("SourceTable")

                          }).OrderByDescending(x => x.CreatedDate).ToList();

            return View(result);
        }

        public ActionResult CommonNews(string websiteid, string lan, string str, string end, int p = 1)
        {
            var Key = "press-releases";

            var webLevelDATA = new DBModel.WebLevel()
            {
                WebSiteID = websiteid,
                Lang = "zh-tw",
                WebLevelKey = Key
            };

            CommonScheduleModel schedule = new CommonScheduleModel();
            DefaultPager pager = new DefaultPager();
            pager.DisplayCount = 10;
            pager.p = p;

            var data = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA);
            var list = WebLevelManagementService.GetWebNews("SortOrder", "asc", "", str, end, data, "", "", lan, ref pager);

            schedule.SchedulewEBNews = list;
            schedule.defaultPager = pager;

            return View(schedule);
        }

        public ActionResult CommonSchedule(string websiteid, string lan)
        {
            var Key = "press-releases";

            var webLevelDATA = new DBModel.WebLevel()
            {
                WebSiteID = websiteid,
                Lang = "zh-tw",
                WebLevelKey = Key
            };

            CommonScheduleModel schedule = new CommonScheduleModel();
            var data = WebLevelManagementService.GetWebLevelByWebLevelData(webLevelDATA);
            var list = WebLevelManagementService.GetWEBNewByMainSN(data.WebLevelSN);
            schedule.wEBNews = list.Where(x => x.Lang == lan).First();

            return View(schedule);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static Utility.Files;

namespace Utility.MailBox
{
    public class Api
    {
        /// <summary>
        /// Speedapi位置
        /// </summary>
        public static string Url { get; set; }

        /// <summary>
        /// Toeken 資料
        /// </summary>
        public static ToekenModel toekenClass { get; set; }

        /// <summary>
        /// FileService位址
        /// </summary>
        public static string FileServiceUrl { get; set; }

        /// <summary>
        /// MailBox位址
        /// </summary>
        public static string MailBoxUrl { get; set; }
        /// <summary>
        /// 發送 New Case
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="toekenClass"></param>
        /// <param name="addNewCaseModel"></param>
        /// <returns></returns>
        //public static returnAddNewCaseModel SendAddNewCase(AddNewCaseModel addNewCaseModel, List<FileModel> fileModels, string CompanyId, out List<returnFileModel> _returnFileModel)
        //{
        //    //step 1 get token 
        //    //var token = GetServerToken(@$"{Url}/api/GssAuth/GetServerToken", toekenClass);
        //    //step 2 file 
        //    _returnFileModel = new List<returnFileModel>();
        //    if (fileModels != null)
        //    {
        //        foreach (var file in fileModels)
        //        {
        //            var reFile = AddCaseFile(@$"{Url}/api/Uploader/UploadAttachment/{CompanyId}", token, file);
        //            if (reFile.ApiStatus != null)
        //            {
        //                _returnFileModel.Add(reFile);
        //            }
        //        }
        //    }

        //    //step 3 addNEW
        //    //api/Case/AddNewCase
        //    var _case = AddNewCase(@$"{Url}/api/Case/AddNewCase", token, addNewCaseModel, _returnFileModel);

        //    return _case;
        //}
        //public static returnSearchCaseModel SendSearchCase(SearchCaseModel searchCaseModel , ref List<FileMessage> files, string ModaFileService="" )
        //{
            
        //    //step 1 get token 
        //    //api/GssAuth/GetServerToken
        //    var token = GetServerToken(@$"{Url}/api/GssAuth/GetServerToken", toekenClass);
        //    //step 2 SearchCase 
        //    //Case/SearchCase
        //    var SC = SearchCase(@$"{Url}/api/Case/SearchCase", token, searchCaseModel);
        //    //step 3 AddReplyFile
        //    //api/Case/DownloadAttachment
        //    if (SC.Data?.CaseProcessStatus != null && SC.Data?.CaseProcessStatus.Status == "發文完成")
        //    {
        //        files=  DownloadAttachment(@$"{Url}/api/Case/DownloadAttachment", token, SC.Data.CaseReplyStatus[SC.Data.CaseReplyStatus.Length - 1].ReplySaveFileName.ToList() , searchCaseModel.CaseNo , searchCaseModel.CompanyId, ModaFileService);
        //    }
        //    return SC;

        //}
        /// <summary>
        /// 取的Token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="toekenClass"></param>
        /// <returns></returns>
        public static bool GetServerToken(out returnToekenModel returnToken )
        {
            
            try
            {
                using (var client = new HttpClient(setHttpNotSafeSSl()))
                {
                    string url = @$"{Url}/api/GssAuth/GetServerToken";
                    var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(toekenClass);
                    HttpContent httpContent = new StringContent(requestJson);
                    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    var result = client.PostAsync(url, httpContent).Result.Content.ReadAsStringAsync().Result;
                    if (!string.IsNullOrEmpty(result))
                    {
                        returnToken = Newtonsoft.Json.JsonConvert.DeserializeObject<returnToekenModel>(result);
                        return true;
                    }
                }
                returnToken = null;
                return false;
            }
            catch (Exception ex)
            {
             
                returnToken = null;
                return false;
            }
        }

        /// <summary>
        /// 新增 File
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileUrl"></param>
        /// <param name="OriginalFileName"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool AddCaseFile(returnToekenModel returnToekenModel, FileModel fileModel, string CompanyId, out returnFileModel returnFileModel)
        {
            var url = @$"{Url}/api/Uploader/UploadAttachment/{CompanyId}";
            var s = GetApiFileAsync(fileModel.fileUrl);
            var streamContent = new StreamContent(s);
            var fileContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);
            var formdata = new MultipartFormDataContent();
            formdata.Add(fileContent, fileModel.OriginalFileName, fileModel.OriginalFileName);
            using (HttpClient client = new HttpClient(setHttpNotSafeSSl()))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(returnToekenModel.token_type, returnToekenModel.access_token);
                var response = client.PostAsync(url, formdata).Result;
                var statuscode = response.StatusCode.ToString();
                var result = response.Content.ReadAsStringAsync().Result;
                returnFileModel = new returnFileModel();
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        returnFileModel = Newtonsoft.Json.JsonConvert.DeserializeObject<returnFileModel>(result);
                        returnFileModel.StatusCode = statuscode;
                        returnFileModel.Context = result;
                        returnFileModel.SendDateTime = DateTime.UtcNow.AddHours(8);
                        return true;
                    }
                    catch 
                    {
                        returnFileModel = new returnFileModel() 
                        {
                            StatusCode = statuscode,
                            Context = result,
                            SendDateTime = DateTime.UtcNow.AddHours(8)
                        };

                        return false;
                    }      
                }
                else 
                {
                    returnFileModel.StatusCode = statuscode;
                    returnFileModel.Context = result ?? "";
                    returnFileModel.SendDateTime = DateTime.UtcNow.AddHours(8);

                    return false;
                }
            }
        }

        public static Stream GetApiFileAsync(string FileApiPath)
        {
            var SavefileMdeol = new SaveFileModel()
            {
                path = FileApiPath
            };
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };

            using var client = new HttpClient(httpClientHandler);
            string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(SavefileMdeol);
            HttpContent httpContent = new StringContent(requestJson);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync($"{FileServiceUrl}/File/Get", httpContent).GetAwaiter().GetResult();
            var contents = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var a = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResultModel>(contents);
            if (a.statusCode == 200)
            {
                var fr = Newtonsoft.Json.JsonConvert.DeserializeObject<FileResponse>(a.content);
                byte[] myByteArray = Convert.FromBase64String(fr.FileStream);
                Stream stream = new System.IO.MemoryStream(myByteArray);
                return stream;
            }
            else
            {

            }
            return null;
        }
        /// <summary>
        /// 新增case
        /// </summary>
        /// <param name="url"></param>
        /// <param name="returnToekenModel"></param>
        /// <param name="addNewCaseModel"></param>
        /// <returns></returns>
        public static bool AddNewCase(returnToekenModel returnToekenModel, AddNewCaseModel addNewCaseModel, out returnAddNewCaseModel returnAddNewCaseModel)
        {
            var url = @$"{Url}/api/Case/AddNewCase";

            using (var client = new HttpClient(setHttpNotSafeSSl()))
            {
                var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(addNewCaseModel);
                HttpContent httpContent = new StringContent(requestJson);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(returnToekenModel.token_type, returnToekenModel.access_token);
                // httpContent.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token"); ;
                var response = client.PostAsync(url, httpContent).Result;
                var statuscode = response.StatusCode.ToString();
                var result = response.Content.ReadAsStringAsync().Result;
                returnAddNewCaseModel = new returnAddNewCaseModel(); ;
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        returnAddNewCaseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<returnAddNewCaseModel>(result);
                        returnAddNewCaseModel.StatusCode = statuscode;
                        returnAddNewCaseModel.Context = result;
                        returnAddNewCaseModel.SendDateTime = DateTime.UtcNow.AddHours(8);

                        return true;
                    }
                    catch
                    {
                        returnAddNewCaseModel = new returnAddNewCaseModel()
                        {
                            StatusCode = statuscode,
                            Context = result,
                            SendDateTime = DateTime.UtcNow.AddHours(8)
                        };

                        return false;
                    }
                }
                else
                {
                    returnAddNewCaseModel.StatusCode = statuscode;
                    returnAddNewCaseModel.Context = result ?? "";
                    returnAddNewCaseModel.SendDateTime = DateTime.UtcNow.AddHours(8);

                    return false;
                }
            }
        }
        /// <summary>
        /// 查詢案件資料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="returnToekenModel"></param>
        /// <param name="searchCaseModel"></param>
        /// <returns></returns>
        public static bool SearchCase(returnToekenModel returnToekenModel, SearchCaseModel searchCaseModel, out returnSearchCaseModel returnSearchCaseModel)
        {
            var url = @$"{Url}/api/Case/SearchCase";
            using (var client = new HttpClient(setHttpNotSafeSSl()))
            {
                var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(searchCaseModel);
                HttpContent httpContent = new StringContent(requestJson);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(returnToekenModel.token_type, returnToekenModel.access_token);
                var response = client.PostAsync(url, httpContent).Result;
                var statuscode = response.StatusCode.ToString();
                var result = response.Content.ReadAsStringAsync().Result;
                returnSearchCaseModel = new returnSearchCaseModel(); ;
                if (!string.IsNullOrEmpty(result))
                {
                    try
                    {
                        returnSearchCaseModel = Newtonsoft.Json.JsonConvert.DeserializeObject<returnSearchCaseModel>(result);
                        returnSearchCaseModel.StatusCode = statuscode;
                        returnSearchCaseModel.Context = result;
                        returnSearchCaseModel.SendDateTime = DateTime.UtcNow.AddHours(8);

                        return true;
                    }
                    catch
                    {
                        returnSearchCaseModel = new returnSearchCaseModel()
                        {
                            StatusCode = statuscode,
                            Context = result,
                            SendDateTime = DateTime.UtcNow.AddHours(8)
                        };

                        return false;
                    }
                }
                else
                {
                    returnSearchCaseModel.StatusCode = statuscode;
                    returnSearchCaseModel.Context = result ?? "";
                    returnSearchCaseModel.SendDateTime = DateTime.UtcNow.AddHours(8);

                    return false;
                }
            }
        }

        public static List<FileMessage> DownloadAttachment(returnToekenModel returnToekenModel, List<ReplySaveFileName> replySaveFileNames, string CaseNo, string CompanyId)
        {
            try
            {
                var url = @$"{Url}/api/Case/DownloadAttachment";
                List<FileMessage> FileMessages = new List<FileMessage>();
                //SaveFileModel
                foreach (var f in replySaveFileNames)
                {
                    var SaveFile = new SaveFileModel()
                    {
                        FileName = f.FileName,
                        isFileShare = false,
                        path = @$"MailBox/SpeedApi/{CaseNo}",
                    };
                    #region 下載
                    using (var client = new HttpClient(setHttpNotSafeSSl()))
                    {
                        var DownloadAttachmentModel = new DownloadAttachmentModel()
                        {
                            CompanyId = CompanyId,
                            FileID = f.FileID,
                        };
                        var requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(DownloadAttachmentModel);
                        HttpContent httpContent = new StringContent(requestJson);
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(returnToekenModel.token_type, returnToekenModel.access_token);
                        var result = client.PostAsync(url, httpContent).Result;
                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            var file = result.Content.ReadAsByteArrayAsync().Result;
                            SaveFile.bytes = file;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    #endregion
                    #region 上傳
                    using (var client = new HttpClient(setHttpNotSafeSSl()))
                    {
                        string requestJson = Newtonsoft.Json.JsonConvert.SerializeObject(SaveFile);
                        HttpContent httpContent = new StringContent(requestJson);
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        var task = client.PostAsync($"{FileServiceUrl}/File/Save", httpContent).Result;
                        var result = task.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrEmpty(result))
                        {
                            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultFileApiModel>(result);
                            if (item.statusCode == System.Net.HttpStatusCode.OK)
                            {
                                FileMessages.Add(item.content);
                            }
                        }
                    }
                    #endregion

                }
                //FileMessage
                return FileMessages;
            }
            catch (Exception)
            {
                return null;
            }
        }

        static HttpClientHandler setHttpNotSafeSSl()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
               (httpRequestMessage, cert, cetChain, policyErrors) =>
               {
                   return true;
               };
            return handler;
        }

        #region Toeken
        public class ToekenModel
        {
            public string ClientId { get; set; }

            public string ClientSecret { get; set; }
        }
        public class returnToekenModel
        {
            public string access_token { get; set; }

            public string token_type { get; set; }

            public int expires_in { get; set; } = 0;
        }
        #endregion
        public class BigFileData
        {
            public int CaseApplySN { get; set; }
            public AddNewCaseModel addNewCaseModel1 { get; set; }
            public List<FileModel> fileModels { get; set; }
        }
        #region File
        public class FileModel
        {
            public string fileUrl { get; set; }
            public string OriginalFileName { get; set; }
            public string FileName { get; set; }

            public string WEBFileID { get; set; }
        }
        public class returnFileModel: ResponseModel
        {
            public string[] FileNo { get; set; }
            public string ApiStatus { get; set; }
            public string Message { get; set; }
        }
        #endregion
        #region AddNew
        public class AddNewCaseModel
        {
            public string Subject { get; set; } = "";
            public string Content { get; set; } = "";
            public string ItemTypeUid { get; set; } = "001";
            public string ApplyUser { get; set; } = "";
            public string ApplyUserGenderCode { get; set; } = "";
            public string ContactEmail { get; set; } = "";
            public string TelAreacode { get; set; } = "";
            public string Tel { get; set; } = "";
            public string TelExtensioncode { get; set; } = "";
            public string ContactMphone { get; set; } = "";
            public string ContactFax { get; set; } = "";
            public string ContactFaxArea { get; set; } = "";
            public string IsProtectUserProfile { get; set; } = "";
            public string LocationLongitude { get; set; } = "";
            public string LocationLatitude { get; set; } = "";
            public string Location { get; set; } = "";
            public string AddressRegionCode1 { get; set; } = "";
            public string AddressRegionCode2 { get; set; } = "";
            public string Address { get; set; } = "";
            public string CompanyId { get; set; } = "";
            public string CompanyCaseNo { get; set; } = "";

            public string StartDate { get; set; } = "";
            public string[] Attachments { get; set; }
        }
        public class returnAddNewCaseModel: ResponseModel
        {
            public string ApiStatus { get; set; }

            public string Message { get; set; }

            public returnAddNewCaseModelData Data { get; set; }

        }
        public class returnAddNewCaseModelData
        {
            public string CompanyId { get; set; }
            public string CompanyCaseNo { get; set; }
            public string CaseNo { get; set; }
            public string CasePwd { get; set; }
            public string AcceptDate { get; set; }
        }
        #endregion
        #region SearchCase
        public class SearchCaseModel
        {
            public string CompanyId { get; set; }
            public string CaseNo { get; set; }
            public string CasePwd { get; set; }
        }

        public class SearchModel
        {
            public int CaseApplySN { get; set; }

            public SearchCaseModel SearchCaseModel { get; set; }
        }

        public class returnSearchCaseModel: ResponseModel
        {
            // 1:OK 2:Fail 3:Error
            public string ApiStatus { get; set; }
            public string Message { get; set; }
            public Datum Data { get; set; }
        }
        public class ResponseModel 
        {
            public string StatusCode { get; set; }
            public string Context { get; set; }
            public DateTime SendDateTime { get; set; } 
        }
        public class Datum
        {
            public Caseprocessstatus CaseProcessStatus { get; set; }
            public Casereplystatu[] CaseReplyStatus { get; set; }
            public string Subject { get; set; }
            public string Content { get; set; }
            public string ParentItemTypeName { get; set; }
            public string ItemTypeName { get; set; }
            public string ApplyUser { get; set; }
            public string ApplyUserGenderCode { get; set; }
            public string ApplyUserGenderCodeDescription { get; set; }
            public string ContactEmail { get; set; }
            public string CphoneCodeArea { get; set; }
            public string ContactCphone { get; set; }
            public string CphoneExtension { get; set; }
            public string ContacMphone { get; set; }
            public string FaxCodeArea { get; set; }
            public string ContactFax { get; set; }
            public string FaxExtension { get; set; }
            public bool IsProtectUserProfile { get; set; }
            public string LocationLongitude { get; set; }
            public string LocationLatitude { get; set; }
            public string Location { get; set; }
            public string AddressRegionCode1 { get; set; }
            public object AddressRegionName1 { get; set; }
            public string AddressRegionCode2 { get; set; }
            public object AddressRegionName2 { get; set; }
            public string Address { get; set; }
            public string AddressFullDescription { get; set; }
            public object Attchments { get; set; }
            public object[] OrginalFileName { get; set; }
            public string OrginalFileNames { get; set; }
            public string FilePath { get; set; }
            public string ApplyDate { get; set; }
            public string CaseNo { get; set; }
        }

        public class Caseprocessstatus
        {
            public string Content { get; set; }
            public string Deadline { get; set; }
            public string Closedat { get; set; }
            public string Status { get; set; }
            public string ExeOrgUnit { get; set; }
            public string DealContent { get; set; }
            public string IsDrawbacked { get; set; }
            public string DrawbackReason { get; set; }
            public string Address { get; set; }
            public string Location { get; set; }
            public string ResultInfo { get; set; }
        }

        public class Casereplystatu
        {
            public string CaseNo { get; set; }
            public string HandleSerilNo { get; set; }
            public string ReplyWay { get; set; }
            public string ReplyWayDesc { get; set; }
            public string ReplyWayInfo { get; set; }
            public string ReplyContent { get; set; }
            public string ReplyDate { get; set; }
            public string ExtraSms { get; set; }
            public string ExtraSmsNo { get; set; }
            public string IsExtraSms { get; set; }
            public string DocNo { get; set; }
            public string DocDept { get; set; }
            public object[] ReplyOrginalFileName { get; set; }
            public string ReplyOrginalFileNames { get; set; }
            public ReplySaveFileName[] ReplySaveFileName { get; set; }
            public string ReplySaveFileNames { get; set; }
            public string FilePath { get; set; }
        }

        public class DownloadAttachmentModel
        {
            public string CompanyId { get; set; }
            public string FileID { get; set; }
        }
        public class ReplySaveFileName
        {
            public string FileName { get; set; }
            public string FileID { get; set; }
        }
        #endregion
        public class ResultFileApiModel
        {
            public System.Net.HttpStatusCode statusCode { get; set; }

            public FileMessage content { get; set; }
        }


    }
}

using Microsoft.AspNetCore.Mvc;
using Services.Authorization;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Utility;
using static Utility.Files;

namespace ModaMailBox.Controllers
{
    public class CommonController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        /// <summary> 圖形驗證 </summary>
        /// <returns></returns>
        public ActionResult GetCaptcha()
        {
            var captcha = new Captcha();

            captcha.ShowRandomLine = false;
            string code;
            var bm = captcha.GetCaptcha(out code, 5, "0123456789", 5);
            byte[] result = null;

            using (MemoryStream ms = new MemoryStream())
            {
                bm.Save(ms, ImageFormat.Gif);
                SetSession("CaptchaCode", code);
                result = ms.GetBuffer();
            }
            return File(result, "image/gif");
        }
        
        [HttpPost]
        public ActionResult SendAgain()
        {
            var no = GetSession<string>("MBN");
            var _no = 0;
            if (int.TryParse(no, out _no)) {
                Services.ModaMailBox.MailBox.SendReplyMail(_no, out string erroe, true);
                return StatusResult(System.Net.HttpStatusCode.OK, "已重寄「案件回覆說明信」，請至電子信箱查看");
            } else {
                return StatusResult(System.Net.HttpStatusCode.OK, "請重新查詢資料");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ScanFile()
        {
            try
            {
                var antivirus = AppSettingHelper.GetAppsetting("antivirus"); 

                Utility.Mail.sysAdmin = LogService.GetErroEmailAccount();
                var date = Request;
                var files = Request.Form.Files;
                long size = files.Sum(f => f.Length);
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        List<IFormFile> iFormFile = new List<IFormFile>();
                        iFormFile.Add(formFile);
                        //step 1驗證准許的副檔名
                        //檔案格式須為txt、csv、gif、jpg、png、tif、tiff
                        var FileExtList = new List<string>() { "txt", "csv", "gif", "jpg", "jpeg", "png", "tif", "tiff" };
                        string fileExt = formFile.FileName.Split('.').Last().ToLower();
                        string filename = formFile.FileName.Replace("." + formFile.FileName.Split('.').Last(),"");
                        if (!FileExtList.Any(x => x == fileExt))
                        {
                            return StatusResult(System.Net.HttpStatusCode.BadRequest, "檔案格式須為 txt、csv、gif、jpg、jpeg、png、tif、tiff");
                        }
                        //step 2 驗證檔案大小 & 單檔限制5MB
                        var Maxlength = 5 * 1024 * 1024;
                        var fileLength = formFile.Length;
                        if (fileLength > Maxlength)
                        {
                            return StatusResult(System.Net.HttpStatusCode.BadRequest, "檔案容量超過單檔 5MB 上限");
                        }
                        //step 3 驗證副檔名正確性
                        if (!Files.CheckFileCentType(iFormFile))
                        {
                            return StatusResult(System.Net.HttpStatusCode.BadRequest, "副檔名與實際檔案不相符");
                        }
                        if (!Utility.Regular.FileNameRule(filename))
                        {
                            return StatusResult(System.Net.HttpStatusCode.BadRequest, "檔名為「文字」和「數字」組合，請勿含 「\\」、 「/」、 「:」、 「*」、 「?」、 「\"」、 「<」、 「>」、 「|」、 「#」、 「{」、 「}」、 「%」、 「~」、 「&」等特殊符號。");
                        }
                        //step 4 掃描
                        var IsEsetScan = AppSettingHelper.GetAppsetting("IsEsetScan");
                       
                        if (IsEsetScan == "1")
                        {
                            var tempFile = AppSettingHelper.GetAppsetting("tempFile");
                            tempFile =Utility.Files.PathTraversal(tempFile);
                            if (string.IsNullOrWhiteSpace(tempFile))
                            {
                                Utility.Mail.Error("民意信箱tempFile沒有設定");
                                ///不掃毒
                                return StatusResult(System.Net.HttpStatusCode.OK, formFile.Length);
                            }
                            switch (antivirus.ToLower()) {
                                case "clamav":
                                    var clam = Utility.MailBox.Scan.ClamdScan(iFormFile, tempFile, out string log);
                                    if (!string.IsNullOrWhiteSpace(log))
                                    {
                                        Utility.Mail.Error(log);
                                    }
                                    if (clam.statusCode.ToString() != "OK")
                                    {
                                        return StatusResult(System.Net.HttpStatusCode.BadRequest, string.Join(",", clam.Msg));
                                    }
                                    else
                                    {
                                        return StatusResult(System.Net.HttpStatusCode.OK, formFile.Length);
                                    }
                                    break;
                                case "eset":
                                    var scan = Utility.MailBox.Scan.EsetScan(iFormFile, tempFile);
                                    if (scan.statusCode.ToString() != "OK")
                                    {
                                        return StatusResult(System.Net.HttpStatusCode.BadRequest, string.Join(",", scan.Msg));
                                    }
                                    else
                                    {
                                        return StatusResult(System.Net.HttpStatusCode.OK, formFile.Length);
                                    }
                                    break;
                                default:
                                    Utility.Mail.Error("民意信箱 appsetting antivirus參數 設定錯誤，請速處理，目前上傳檔案功能停止"); //寄給系統管理人員
                                    return StatusResult(System.Net.HttpStatusCode.BadRequest, "目前上傳檔案功能發生異常，暫停上傳功能，如需上傳檔案請稍後再嘗試"); //民眾顯示的訊息
                                    break;
                            }
                        }
                        else
                        {
                            ///不掃毒
                            return StatusResult(System.Net.HttpStatusCode.OK, formFile.Length);
                        }

                    }
                }
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "請選擇檔案");
            }
            catch (Exception)
            {
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "檔案格式須為 txt、csv、gif、jpg、jpeg、png、tif、tiff");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TempFile()
        {
            SetSession("TempFile", null);
            try
            {
                var date = Request;
                var files = Request.Form.Files;
                long size = files.Sum(f => f.Length);
                List<SaveFileModel> iFormFile = new List<SaveFileModel>();
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var stream = formFile.OpenReadStream();
                        var fileMdeol = new SaveFileModel()
                        {
                            bytes = getByteByStream(stream),
                            FileName = formFile.FileName,
                            isFileShare = false,
                            path = @$"MailBox",
                        };

                        iFormFile.Add(fileMdeol);
                    }
                }
                SetSession("TempFile", iFormFile);
                return StatusResult(System.Net.HttpStatusCode.OK, "");
            }
            catch (Exception)
            {
                SetSession("TempFile", null);
                return StatusResult(System.Net.HttpStatusCode.BadRequest, "檔案上傳失敗");
            }
        }
    }
}

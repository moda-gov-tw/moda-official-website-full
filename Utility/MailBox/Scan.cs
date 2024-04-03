using Microsoft.AspNetCore.Http;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using static Utility.CommFun2.Status;
using static Utility.Files;

namespace Utility.MailBox
{
    public class Scan
    {
        //  static string path = $"D:\\WEB\\MODA\\UploadFile";
        /// <summary>
        /// 防毒執行檔
        /// </summary>
        public static string AntiVirusPath { get; set; }


        public static Rlt ClamdScan(List<IFormFile> files, string tempFile, out string log)
        {
            log = string.Empty;
            List<string> msg = new List<string>();
            Rlt rlt = SaveFile(files, tempFile);
            //Scan File
            foreach (UploadFileRlt o in rlt.uploadFileRlt)
            {
                try
                {
                    string scriptPath = $@"{AntiVirusPath} {o.NewFileName}";
                    ProcessStartInfo psi = new ProcessStartInfo()
                    {
                        FileName = "sh", // 使用sh執行Shell指令
                        Arguments = scriptPath, // 傳遞.sh檔案路徑作為參數
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    using (Process process = new Process { StartInfo = psi })
                    {
                        // 開始執行Shell指令
                        process.Start();

                        // 等待執行完成
                        process.WaitForExit();
                        // 讀取輸出
                        string output = process.StandardOutput.ReadToEnd();
                        ScanRlt oScanRlt = new ScanRlt();
                        oScanRlt.CodeID = output.Trim();
                        oScanRlt.Msg = GetClamdScanMsg(output);
                        o.ScanRlt = oScanRlt;
                        if (output.Trim().Length > 0)
                        {
                            switch (output.Trim().Substring(0, 1))
                            {
                                case "0":
                                    //成功不做任何阻擋
                                    break;
                                case "1":
                                    rlt.statusCode = HttpStatusCode.BadRequest;
                                    if (rlt.Msg != null) msg = rlt.Msg;
                                    msg.Add(o.OldFileName + " 檔案掃毒出現問題，請重新上傳其他檔案");
                                    rlt.Msg = msg;
                                    break;
                                case "2":
                                    log = $@"民意信箱-> sh: {scriptPath}  , output : {output.Trim()}";
                                    rlt.statusCode = HttpStatusCode.BadRequest;
                                    if (rlt.Msg != null) msg = rlt.Msg;
                                    msg.Add("目前上傳檔案功能發生異常，暫停上傳功能，如需上傳檔案請稍後再嘗試");
                                    rlt.Msg = msg;
                                    break;
                            }
                        }
                        else
                        {
                            log = $@"民意信箱-> sh: {scriptPath}  , output : {output.Trim()}";
                            rlt.statusCode = HttpStatusCode.BadRequest;
                            if (rlt.Msg != null) msg = rlt.Msg;
                            msg.Add("目前上傳檔案功能發生異常，暫停上傳功能，如需上傳檔案請稍後再嘗試");
                            rlt.Msg = msg;
                        }
                        return rlt;
                    }

                }
                catch (Exception ex)
                {
                    log = $@"民意信箱-> scan error: {ex.Message}";
                    msg.Add("目前上傳檔案功能發生異常，暫停上傳功能，如需上傳檔案請稍後再嘗試");
                    return rlt;
                }
            }
            return rlt;
        }


        public static Rlt EsetScan(List<IFormFile> files, string tempFile)
        {
            Rlt rlt = SaveFile(files, tempFile);
            //Scan File
            foreach (UploadFileRlt o in rlt.uploadFileRlt)
            {
                Process proc = null;
                proc = new System.Diagnostics.Process();
                string args = "\"{0}\"  /log-file={1} /clean-mode=Delete"; //結束後都刪除

                proc.StartInfo.FileName = AntiVirusPath;
                proc.StartInfo.Arguments = string.Format(args, tempFile + "\\" + o.NewFileName, tempFile + "\\ESET_scanlog.txt");

                proc.Start();
                proc.WaitForExit();//等待

                int code = proc.ExitCode;

                ScanRlt oScanRlt = new ScanRlt();
                oScanRlt.CodeID = code.ToString();
                oScanRlt.Msg = GetScanMsg(code);
                o.ScanRlt = oScanRlt;

                if (code > 0)
                {
                    rlt.statusCode = HttpStatusCode.BadRequest;
                    List<string> msg = new List<string>();
                    if (rlt.Msg != null) msg = rlt.Msg;
                    msg.Add(o.OldFileName + "檔案掃毒出現問題，請重新上傳檔案");
                    rlt.Msg = msg;
                }
            }
            return rlt;
        }
        private static Rlt SaveFile(List<IFormFile> files, string tempFile)
        {
            List<UploadFileRlt> NewFileNames = new List<UploadFileRlt>();
            Rlt rlt = new Rlt();
            List<string> Msg = new List<string>();
            bool SaveFileSuccessFlg = true;
            string newfilename = "";
            //取得目前 HTTP 要求的 HttpRequestBase 物件
            int idx = 0;
            foreach (IFormFile file in files)
            {
                try
                {
                    if (file != null && file.Length > 0)
                    {
                        FileSave(tempFile, file, ref newfilename);
                        if (newfilename.Length > 0)
                        {
                            UploadFileRlt o = new UploadFileRlt();
                            o.NewFileName = newfilename;
                            o.OldFileName = file.FileName;
                            NewFileNames.Add(o);
                        }
                    }
                    else
                    {
                        Msg.Add($"SaveFile>>File Name{idx.ToString()} is Empty");
                        SaveFileSuccessFlg = false;
                    }
                }
                catch (Exception ex)
                {
                    Msg.Add($"SaveFile>>{file.FileName}>>{ex.Message}");
                    SaveFileSuccessFlg = false;
                }
            }

            rlt.statusCode = (SaveFileSuccessFlg) ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            rlt.Msg = Msg;
            rlt.uploadFileRlt = NewFileNames;
            return rlt;
        }
        private static void FileSave(string path, IFormFile postedFile, ref string fileName)
        {
            string extension = Path.GetExtension(postedFile.FileName);
            fileName = $"{Guid.NewGuid()}{extension}";
            FileExists(path);
            var filePath = $@"{path}/{fileName}";
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                // postedFile.CopyTo(stream);
                var s0 = postedFile.OpenReadStream();
                var file = getStreamByByte(getByteByStream(s0));
                file.CopyTo(stream);
            }
        }
        private static String GetScanMsg(int code)
        {
            switch (code)
            {
                case 0:
                    return "找不到威脅";
                case 1:
                    return " 找到威脅且已清除";
                case 10:
                    return " 無法掃描某些檔案(可能是威脅)";
                case 50:
                    return "找到威脅";
                case 100:
                    return " 掃描錯誤";
                default:
                    return code.ToString();
            }

        }

        private static string GetClamdScanMsg(string code)
        {
            switch (code)
            {
                case "0":
                    return "沒問題";
                case "1":
                    return "有發現感染的檔案";
                default:
                    return "其他問題，可能是程式問題或socket沒有建立連線，詳見錯誤訊息";
            }
        }

        public class Rlt
        {
            public HttpStatusCode statusCode { get; set; }
            public List<string> Msg { get; set; }
            public List<UploadFileRlt> uploadFileRlt { get; set; }
        }
        public class UploadFileRlt
        {
            public string NewFileName { get; set; }
            public string OldFileName { get; set; }
            public string ErrMsg { get; set; }
            public ScanRlt ScanRlt { get; set; }
        }

        public class ScanRlt
        {
            public string CodeID { get; set; }
            public string Msg { get; set; }
        }

    }
}

using Nancy;
using System.Diagnostics;

namespace ModaMailBox.MailBoxUtility
{
    public class EsetScan
    {
      //  static string path = $"D:\\WEB\\MODA\\UploadFile";
        static string ESETPath = AppSettingHelper.GetAppsetting("ESETPath");
        public static Rlt Scan(List<IFormFile> files , string tempFile)
        {
            Rlt rlt = SaveFile(files, tempFile);
            //Scan File
            foreach (UploadFileRlt o in rlt.uploadFileRlt)
            {
                Process proc = null;
                proc = new System.Diagnostics.Process();
                string args = "\"{0}\"  /log-file={1} /clean-mode=Delete";//發現病毒直接刪除

                proc.StartInfo.FileName = ESETPath;
                proc.StartInfo.Arguments = string.Format(args, tempFile + "\\" + o.NewFileName, tempFile + "\\ESET_scanlog.txt");
                //proc.StartInfo.Arguments = string.Format(args, path + "\\" + o.NewFileName);

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
                    msg.Add(o.OldFileName + "掃毒失敗");
                    rlt.Msg = msg;
                }
            }
            return rlt;
        }
        private static Rlt SaveFile(List<IFormFile> files ,string tempFile)
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
            //if (Request.Form.Files.Count == 0)
            //{
            //    SaveFileSuccessFlg = false;
            //    Msg.Add($"SaveFile>>No uploaded files");
            //}
            rlt.statusCode = (SaveFileSuccessFlg) ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
            rlt.Msg = Msg;
            rlt.uploadFileRlt = NewFileNames;
            return rlt;
        }
        private static void FileSave(string path, IFormFile postedFile, ref string fileName)
        {
            string extension = Path.GetExtension(postedFile.FileName);
            fileName = $"{Guid.NewGuid()}{extension}";
            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                postedFile.CopyTo(stream);

            }
        }
        private static String GetScanMsg(int code)
        {
            switch (code)
            {
                case 0:
                    return "找不到威脅";
                    break;
                case 1:
                    return " 找到威脅且已清除";
                    break;
                case 10:
                    return " 無法掃描某些檔案(可能是威脅)";
                    break;
                case 50:
                    return "找到威脅";
                    break;
                case 100:
                    return " 掃描錯誤";
                    break;
            }
            return "";

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

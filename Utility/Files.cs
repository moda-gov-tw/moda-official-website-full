using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utility
{
    //上傳模組
    public class Files
    {
        public static FileMessage Upload(SaveFileModel saveFileModel )
        {
          
            var errorPath = "";
            var _localPaths = saveFileModel.localPath.Replace("..", "").Replace( @"\", "/").Split(@"/");
            var _paths0 = saveFileModel.path.Replace("..", "").Replace(@"\", "/").Split(@"/");
            var _pathss = _localPaths.Concat(_paths0).ToArray();
            string _path = FilePath(_pathss);
            var fileMessage = new FileMessage();
            try
            {
                FileExists(_path);
                var commonFileModel = new CommonFileModel();
                if (saveFileModel.bytes != null)
                {
                    //取原始檔名中的副檔名
                    var fileExt = Path.GetExtension(saveFileModel.FileName).Replace("/", "").Replace("..", "");
                    //為避免使用者上傳的檔案名稱發生重複，重新給一個亂數名稱
                    var fileNewName = "";
                    if (!saveFileModel.isFileShare)
                    {
                        fileNewName = Path.GetRandomFileName();
                    }
                    else
                    {
                        fileNewName = Path.GetFileNameWithoutExtension(saveFileModel.FileName).Replace("/", "").Replace("..", "");
                    }
                    var UploadPath = $@"{_path}/{fileNewName}{fileExt}";
                    errorPath = UploadPath;
                    using (var stream = new FileStream(UploadPath, FileMode.Create))
                    {
						
						//Utility.LogExpansion.Write(DateTime.UtcNow.AddHours(8).ToString("yyyyMMdd")+ "File", UploadPath);
                        var webfilepath = new List<string>();
                        if (!saveFileModel.isImg)
                        {
                            webfilepath.Add("Uploads");
                        }
                        webfilepath.Add(saveFileModel.path);
                        webfilepath.Add($@"{fileNewName}{fileExt}");
                        string _webfilepath = FilePath(webfilepath.ToArray());
                        var file = getStreamByByte(saveFileModel.bytes);
                        file.CopyTo(stream);
                        commonFileModel.fileExt = fileExt;
                        commonFileModel.fileNewName = $"{fileNewName}{fileExt}";
                        commonFileModel.fileOriginName = saveFileModel.FileName;
                        commonFileModel.filePath = $@"/{_webfilepath.Replace(@"\", @"/")}";
                        commonFileModel.fileSize = file.Length / 1024;
                        fileMessage.CommonFileModel = commonFileModel;
                    }
                }
            }
            catch (Exception ex)
            {
              string  error = $@"FilePath:{errorPath} , error Msg {ex.ToString()}";
                fileMessage.msg = $@"{error}"+"  error: " + ex.Message;
                fileMessage.check = false;
                return fileMessage;
            }
            return fileMessage;
        }

        public static bool GetFile(string path, out FileResponse response)
        {
            response = new FileResponse();
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    response.msg = "指定的資料不存在";
                    return false;
                }

                byte[] buffer = null;
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                }
                response.FileStream = Convert.ToBase64String(buffer);
                response.ContentType = getcontenttype(path);
                response.FileName = Path.GetFileNameWithoutExtension(path);
                response.FileType = Path.GetExtension(path).ToLowerInvariant();
                return true;
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
                return false;
            }
        }

        public static bool DelFile(string path, string FileName)
        {
            try
            {
                var _Paths = path.Replace("..", "").Replace("/", @"\").Split(@"\");
                var _FileName = FileName.Replace("..", "").Replace("/", @"\").Split(@"\");
                var _pathss = _Paths.Concat(_FileName).ToArray();
                string _path = FilePath(_pathss);
                System.IO.File.Delete(_path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 檢查副檔名是否為允許上傳的檔案類型；
        /// 讀取前兩個byte判斷是否偽造副檔名
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static bool CheckFileCentType(List<IFormFile> files)
        {
            try
            {
                foreach (var file in files)
                {
                    using (var ms = new MemoryStream())
                    {
                        var fileArray =new List<string>();
                        var bomArray = new List<string>() { "239", "187", "191" };
                        file.CopyTo(ms);
                        ms.Position = 0;
                        var fileclass = "";
                        for (int i = 0; i < 5; i++) { fileArray.Add(ms.ReadByte().ToString());}
                        if (fileArray.Intersect(bomArray).Count() != 3) {fileclass = string.Join("", fileArray.Take(2)); }
                        else { fileclass = string.Join("", fileArray.Skip(3).Take(2));}
                        string fileType = file.FileName.Split('.').Last().ToLower();
                        FileExtension extension = FileExtension.VALIDFILE;
                        switch (fileType)
                        {
                            case "js":
                            case "css":
                            case "json":
                            case "csv":
             
                            case "7z": return true;
                            default:
                                if (Enum.TryParse(fileType, out extension))
                                {
                                    if (!(fileclass == ((int)extension).ToString() || (int)extension == 0))
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }

        public static string ReadMD(List<IFormFile> files)
        {
            try
            {
                foreach (var file in files)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        string str = System.Text.Encoding.Default.GetString(ms.ToArray());
                        return str;
                    }
                }
                return "";
            }
            catch (Exception)
            {

                return "";
            }
        }

        /// <summary>
        /// 防止PathTraversal
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string PathTraversal(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return path.Replace("..", "");
            }
            return "";
        }
        /// <summary>
        /// 可允許上傳之副檔名；
        /// 前兩個byte組出來的字串
        /// </summary>
        private enum FileExtension
        {
            jpg = 255216,
            jpeg = 255216,
            png = 13780,
            gif = 7173,
            bmp = 6677,
            doc = 208207,
            docx = 8075,
            ppt = 208207,
            pptx = 8075,
            xls = 208207,
            xlsx = 8075,
            pdf = 3780,
            rar = 8297,
            zip = 8075,
            md = 3532,
            odt = 8075,
            odp = 8075,
            ods = 8075,
            tif = 7373,
            tiff = 7373,
            json = 9110,
            xml = 6063,
            VALIDFILE = 9999999,
            txt = 0,
            mp3 = 0,
            csv = 0,
            svg = 0,
            mp4 = 0,
            shp = 0,
            avi = 0,
            geojson = 0,
        }

        /// <summary>
        /// 判斷資料夾是否存在不存在的話就新增
        /// </summary>
        /// <param name="path"></param>
        public static void FileExists(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// file 回傳模型
        /// </summary>
        public class FileMessage
        {
            /// <summary>
            /// 上傳是否成功
            /// </summary>
            public bool check = true;
            /// <summary>
            /// 錯誤訊息
            /// </summary>
            public string msg { get; set; }
            public List<IFormFile> errorFile = new List<IFormFile>();
            public CommonFileModel CommonFileModel { get; set; }

        }
        public class CommonFileModel
        {
            /// <summary>
            /// 群組代號
            /// </summary>
            public string GroupID { get; set; } = "1";
            /// <summary>
            /// 原始名稱
            /// </summary>
            public string fileOriginName { get; set; }
            /// <summary>
            /// 檔案大小 
            /// </summary>
            public long fileSize { get; set; }
            /// <summary>
            /// 附檔名
            /// </summary>
            public string fileExt { get; set; }
            /// <summary>
            /// 檔案新名稱
            /// </summary>
            public string fileNewName { get; set; }
            /// <summary>
            /// 檔案路徑
            /// </summary>
            public string filePath { get; set; }

            public string fileTitle { get; set; }

            public int FileSort { get; set; } = 1;

            public string webFileID { get; set; }

            public string IsEnable { get; set; }

            public bool IsImage { get; set; }
            /// <summary>
            /// 語系
            /// </summary>
            public string lan { get; set; }

        }


        /// <summary>
        /// 路徑組合
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FilePath(string[] path)
        {
            try
            {
                return string.Format(Path.Combine(path));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] getByteByStream(Stream stream)
        {
            try
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return bytes;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static Stream getStreamByByte(byte[] bytes)
        {
            try
            {
                Stream stream = new MemoryStream(bytes);

                return stream;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class SaveFileModel
        {
            //因容器調整Img調整路徑
            public string ImgPath { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public byte[] bytes { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string localPath { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string path { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool isFileShare { get; set; } = false;
            /// <summary>
            /// 
            /// </summary>
            public bool isImg { get; set; } = true;
        }

        /// <summary>
        /// 檔案下載回傳
        /// </summary>
        public class FileResponse
        {
            /// <summary>
            /// 資料流-base64
            /// </summary>
            public string FileStream { get; set; }
            /// <summary>
            /// 檔案類別
            /// </summary>
            public string ContentType { get; set; }
            /// <summary>
            /// 檔案名稱
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// 副檔名
            /// </summary>
            public string FileType { get; set; }
            /// <summary>
            /// 回傳訊息
            /// </summary>
            public string msg { get; set; }
        }

        public static string getcontenttype(string filename)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filename, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }

        public class ApiResultModel
        {
            public int statusCode { get; set; }
            public string content { get; set; }
        }
    }
}

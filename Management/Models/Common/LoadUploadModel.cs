using System.Collections.Generic;
using static Utility.Files;

namespace Management.Models.Common
{
    public class LoadUploadModel
    {
        /// <summary>
        /// 標題
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 檔案類型 1-圖 2-檔 3-頁首頁尾logo 0-全部 99-md
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 檔案
        /// </summary>
        public List<CommonFileModel> commonFileModels { get; set; }

        /// <summary>
        /// 系統子資料夾路徑 
        /// </summary>
        public string fth { get; set; }

        /// <summary>
        /// 檔案分類
        /// </summary>
        public string fileGroup { get; set; }
        /// <summary>
        /// 需要複製功能嗎
        /// </summary>
        public bool needCopy { get; set; } = true;
        /// <summary>
        /// 編號
        /// </summary>
        public string file_trNumber { get; set; } = "1";

        public string lan { get; set; }  = string.Empty;

        /// <summary>
        /// 單檔最大上傳限制(單位KB) 沒有特別設定的化預設25MB
        /// </summary>
        public int maxFileSize { get; set; } = 25600;
        /// <summary>
        /// 必填欄位
        /// </summary>
        public int Required { get; set; } = 0 ;
        /// <summary>
        /// 0多檔/1單檔
        /// </summary>
        public int FileCountState { get; set; } = 0;
        /// <summary>
        /// 類型
        /// </summary>
        /// <param name="fileType">1-圖 2-檔 0-全部  99-特殊(md)</param>
        /// <returns></returns>
        public string GetFileType(string fileType)
        {
            return fileType switch
            {
                "1" => "JPG,JPEG,PNG,SVG,GIF,TIFF,ODG",
                "2" => "PDF,ODT,ODS,ODP,CSV,ZIP",
                "3" => "JPG,JPEG,PNG,SVG,GIF,TIFF,ODG",
                "4" => "CSV,XML,JSON,PDF,ODS,ODT,ODP,ZIP,7Z,SHP,GEOJSON",
                "5" => "CSS,JSON,JS",
                "99" => "MD",
                _ => "CSV,XML,JSON,PDF,ODS,ODT,ODP,ZIP,7Z,SHP,GEOJSON,JPG,JPEG,PNG,SVG,GIF,TIFF,ODG",
            };
        }
        /// <summary>
        /// 備註
        /// </summary>
        public string Description { get; set; } = "";
    }
}

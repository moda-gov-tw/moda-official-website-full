using Services.Models.WebContent;
using System;
using System.Collections.Generic;
using System.Text;
using DBModel;

namespace Services.Models.WebSite
{
    /// <summary>
    /// WebNewsDetailModel
    /// </summary>
    public class WebNewsDetailModel
    {
        //基本資訊，EX標題、內容、內容模式、URL.....
        public WEBNews BasicData;
        //相關檔案、圖片
        public Attachment attachment;

        public List<WebLink> webLinks;

        public struct Attachment
        {
            /// <summary>
            /// 網頁模式
            /// </summary>
            public Page page;

            /// <summary>
            /// 檔案下載模式
            /// </summary>
            public Download download;
        }

        public struct Page
        {
            /// <summary>
            /// 相關檔案
            /// </summary>
            public List<WebFileAndGroupIDModel> Files;

            /// <summary>
            /// 相關圖檔
            /// </summary>
            public List<WebFileAndGroupIDModel> Imgs ;

        }

        public struct Download
        {
            /// <summary>
            /// 單檔下載
            /// </summary>
            public WebFileAndGroupIDModel File ;
            //string s = Utility.CommFun.Status.GetIsEnableDesc()
        }
    }

    public class NewsDetailModel
    {
        //基本資訊，EX標題、內容、內容模式、URL.....
        public WEBNews BasicData;

        /// <summary>
        /// 相關連結 關鍵字  相關法規 相關影片
        /// </summary>
        public List<WEBNewsExtend> newsExtends { get; set; }

        public List<SysCategory> sysCategories { get; set; }

        /// <summary>
        /// 相關檔案
        /// </summary>
        public List<WEBFile> Files;

        /// <summary>
        /// 相關圖檔
        /// </summary>
        public List<WEBFile> Imgs;

        public string DepartmentName { get; set; }
    }
}


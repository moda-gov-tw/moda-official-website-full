using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 附件GroupID命名定義
    /// XXOVVVV
    /// XX:對應模組
    /// O:單檔(S)、多檔(M)
    /// VVV:上傳檔案類別簡寫(不限字數)
    /// </summary>
    public class WebFileGroupID
    {
        /// <summary>
        /// 主視覺模組附件對應GroupID
        /// </summary>
        public class Banner
        {
            /// <summary>
            /// 大圖(單張)
            /// </summary>
            public static string BigImg { get; } = "BRSBI";
            /// <summary>
            /// 小圖(單張)
            /// </summary>
            public static string SmallImg { get; } = "BRSSI";
        }
        /// <summary>
        /// 相關連結模組應附對件GroupID
        /// </summary>
        public class Link
        {
            /// <summary>
            /// 相關圖檔(單張)
            /// </summary>
            public static string Img { get; } = "LISI";

            public static string Img1 { get; set; } = "LISI1";
        }


        /// <summary>
        /// NEWS一般模附件組對應GroupID
        /// </summary>
        public class News
        {
            /// <summary>
            /// 內嵌檔案(多筆)
            /// </summary>
            public static string InlineImgs { get; } = "NWMII";
            /// <summary>
            /// 相關圖片(多筆)
            /// </summary>
            public static string Imgs { get; } = "NWMI";
            /// <summary>
            /// 相關檔案(多筆)
            /// </summary>
            public static string Files { get; } = "NWMF";
            /// <summary>
            /// 相關檔案(單筆)(檔案下載式)
            /// </summary>
            public static string File { get; } = "NWSF";
            /// <summary>
            /// NEWS頁使用的
            /// </summary>
            public static string Logo { get; } = "LOGO";

        }
        ///// <summary>
        ///// PageList模組附件對應GroupID
        ///// </summary>
        //public class PageList
        //{
        //    /// <summary>
        //    /// 內嵌檔案(多筆)
        //    /// </summary>
        //    public static string InlineImgs { get; } = "PLMII";
           
        //    public static string LogoImg { get; } = "PLSLI";
        //}
        /// <summary>
        /// CP模組附件對應GroupID
        /// </summary>
        public class CP
        {
            /// <summary>
            /// 內嵌檔案(多筆)
            /// </summary>
            public static string InlineImgs { get; } = "CPMII";
            /// <summary>
            /// 相關圖片(多筆)
            /// </summary>
            public static string Imgs { get; } = "CPMI";
            /// <summary>
            /// 相關檔案(多筆)
            /// </summary>
            public static string Files { get; } = "CPMF";
            /// <summary>
            /// 相關檔案(單筆)(檔案下載式)
            /// </summary>
            public static string File { get; } = "CPSF";

            /// <summary>
            /// CP頁使用的
            /// </summary>
            public static string Logo { get; } = "LOGO";
        }

        public class OpenData
        {
            public static string Files { get; } = "OPMF";
            /// <summary>
            /// 相關檔案(單筆)(檔案下載式)
            /// </summary>
            public static string File { get; } = "OPSF";
        }

        /// <summary>
        /// ImgText模組附件對應GroupID
        /// </summary>
        public class ImgText
        {
            /// <summary>
            /// 圖片(單筆)
            /// </summary>
            public static string Img { get; } = "IMSI";


        }
        /// <summary>
        /// Module模組附件對應GroupID
        /// </summary>
        public class Module
        {
            /// <summary>
            /// 內嵌檔案(多筆)
            /// </summary>
            public static string InlineImgs { get; } = "MMII";
            /// <summary>
            /// 單元LOGO(多筆)
            /// </summary>
            public static string LogoImg { get; } = "MSLI";

            public static string OtherFiles { get; } = "Other";

        }

        public class Transcript
        {
            public static string MD { get; } = "MD";
        }

        /// <summary>
        /// MEDIA模組附件對應GroupID
        /// </summary>
        public class Media
        {
            /// <summary>
            /// 圖片(單筆)
            /// </summary>
            public static string Img { get; } = "MESI";

        }

        /// <summary>
        /// Extend模組附件對應GroupID
        /// </summary>
        public class Extend
        {
            /// <summary>
            /// 圖片(單筆)
            /// </summary>
            public static string Img { get; } = "EXSI";

        }

        public class SysWebSite
        {
            public static string Img { get; } = "SWSI";
        }

        public class FileShare 
        {
            public static string file { get; } = "FSSF";
        }

        public class MailBox
        {
            public static string Logo { get; } = "Logo";
            public static string Imgs { get; } = "Imgs";
            /// <summary>
            /// 管理系統-回覆檔案
            /// </summary>
            public static string File { get; } = "MBMF";
            /// <summary>
            /// 民眾上傳
            /// </summary>
            public static string MailBoxFile { get; } = "MailBox";
            /// <summary>
            /// 公文系統-回覆檔案
            /// </summary>
            public static string SpeedApiFile { get; } = "SpeedApi";
            /// <summary>
            /// 民意信箱頁面內嵌圖片
            /// </summary>
            public static string PageImg { get; } = "PageImgs";
        }

    }
}
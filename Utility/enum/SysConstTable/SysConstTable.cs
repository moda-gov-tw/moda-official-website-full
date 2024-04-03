using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utility
{
    /// <summary>系統變數值對照表</summary>
    public  class SysConstTable
    {

        /// <summary>
        /// 資料列查無資有效資料中英文描述
        /// </summary>
        public class ListNotFound 
        {
            /// <summary>
            /// 中文版錯誤訊息
            /// </summary>
            public static string TW = "查無有效資料";
            /// <summary>
            /// 英文版錯誤訊息
            /// </summary>
            public static string EN = "Not found";

        };


        /// <summary>
        /// 內容狀態描述
        /// </summary>
        public class CntStatus
        {
            public static string Publish = "發布";
            public static string NoPublish = "停用";
            public static string OffShelf = "下架";   //發布但時間已過
            public static string Draft = "草稿";
            public static string Del = "刪除";
            public static string Reviewer = "審稿中";
            public static string Returned = "審稿退回";

        };
        /// <summary>
        /// 欄位描述
        /// </summary>
        //public class Field
        //{
        //    public class commField
        //    {
        //        public static string StartDate = "發布日期";
        //        public static string EndDate = "下架日期";
        //        public static string OpenOtherWindow = "另開視窗";
        //        public static string PageView = "瀏覽/點閱數";
        //    }

        //    public class WebLevel:Field {
        //        public static string Title = "前台標題";
        //        public static string Status = "是否啟用";
        //    }



        //    public class WebNews :Field{
        //        public static string Title = "標題";
        //        public static string Status = "資料狀態";
        //        public class CP : News
        //        {
        //        }
        //        public class News : WebNews
        //        {

        //            public class Page:News
        //            {
        //                public static string Content = "內容";
        //            }
        //            public class LINK : News
        //            {
        //                public static string URL = "連結";
        //            }
        //            public class DOWNLOAD : News
        //            {
        //                public static string RelFile = "相關檔案";
        //            }
        //        }

        //    }
        //    public class WebJournal { }
        //    public class WebBanknote { }

        //    /// <summary>
        //    /// 發布日期不能小於發布日期
        //    /// </summary>
        //    /// <returns></returns>
        //    public static string SDMoreED()
        //    {
        //        return $"[{commField.EndDate}]不能小於[{commField.StartDate}]";
        //    }
        //    /// <summary>
        //    /// 必填欄位回傳值
        //    /// </summary>
        //    /// <param name="FldName"></param>
        //    /// <returns></returns>
        //    public static string Required(string FldName)
        //    {
        //        return $"[{FldName}]必填";
        //    }
        //}


    }
}

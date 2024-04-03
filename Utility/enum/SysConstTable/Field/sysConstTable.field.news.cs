using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utility.sysConstTable.field
{
    /// <summary>模組NEWS欄位描述值</summary>
    public class News: Utility.sysConstTable.Field
    {

    }
}

namespace Utility.sysConstTable.field.news
{

    /// <summary>模組NEWS下[類型]為[網頁(一般資料格式)]欄位定義值</summary>
    public class Page : Utility.sysConstTable.field.News
    {
        ///// <summary>
        ///// 內容；複寫Utility.sysConstTable.Field.Content
        ///// </summary>
        //public static new string Content = "內容2";

    }


    /// <summary>模組NEWS下[類型]為[檔案(檔案下載式)]欄位定義值</summary>
    public class DownLoad : Utility.sysConstTable.field.News
    {
        public static string File = "相關檔案";

    }


    /// <summary>模組NEWS下[類型]為[連結(URL 連接式)]欄位定義值</summary>
    public class LINK : Utility.sysConstTable.field.News
    {
        public static string URL = "連結";

    }
}


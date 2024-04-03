using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utility.sysConstTable.field
{
    /// <summary>模組Banner欄位描述值</summary>
    public class Banner: Utility.sysConstTable.Field
    {
        public static new string Title = "主標題";

    }

    
}
namespace Utility.sysConstTable.field.banner
{
    /// <summary>模組Banner下[類型]為[附件、(即圖片的意思)]欄位定義值</summary>
    public class Img : Utility.sysConstTable.field.Banner
    {
        public static string URL = "連結";
        public static string Big = "相關圖片(大圖)";
        public static string Small = "相關圖片(小圖)";
    }

    /// <summary>模組Banner下[類型]為[iframe]欄位定義值</summary>
    public class Media : Utility.sysConstTable.field.Banner
    {
        public static string iURL = "iframe連結";
        public static string URL = "連結";
        public static string FldDesc = "相關圖片";
    }
}

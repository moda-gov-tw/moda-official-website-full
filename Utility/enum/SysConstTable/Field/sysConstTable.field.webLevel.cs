using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utility.sysConstTable.field
{
    /// <summary>系統變數值對照表</summary>
    public class WebLevel: Utility.sysConstTable.Field
    {
        /// <summary>
        /// 標題；複寫Utility.sysConstTable.Field.WebLevelKey
        /// </summary>
        public static  string WebLevelKey = "節點代號";
        /// <summary>
        /// 標題；複寫Utility.sysConstTable.Field.Title
        /// </summary>
        public static new string Title = "節點名稱";
        /// <summary>
        /// 資料狀態；複寫Utility.sysConstTable.Field.Status
        /// </summary>
        public static new string Status = "是否啟用";

        /// <summary>
        /// 模組類型
        /// </summary>
        public static string ModuleType = "節點類型";
                /// <summary>
        /// 模組
        /// </summary>
        public static string Module = "模組";
        /// <summary>
        /// 列表樣式
        /// </summary>
        public static string ListType = "列表樣式";
        /// <summary>
        /// 顯示於FatFooter
        /// </summary>
        public static string FatFooterShow = "顯示於FatFooter";
        /// <summary>
        /// 顯示於主選單
        /// </summary>
        public static string MainMenuShow = "顯示於主選單";
        /// <summary>
        /// 顯示左側選單
        /// </summary>
        public static string LeftMenuShow = "顯示於左側選單";

        /// <summary>
        /// RSS
        /// </summary>
        public static string RSSShow = "RSS";
        /// <summary>
        /// 排序方式
        /// </summary>
        public static string SortMethod = "排序方式";

        /// <summary>
        /// 版型選擇
        /// </summary>
        public static string Template = "版型選擇";

        public static string Description = "摘要簡介";

        public static string Condition = "查詢項目";

        public static string DepartmentID = "發布單位";

    }
}

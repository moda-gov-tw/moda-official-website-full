using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utility.sysConstTable.field
{
    /// <summary>系統變數值對照表</summary>
    public class Journal: Utility.sysConstTable.Field
    {
        /// <summary>
        /// 標題；複寫Utility.sysConstTable.Field.Title
        /// </summary>
        public static new string Title = "期刊標題";
        /// <summary>
        /// 卷/期
        /// </summary>
        public static string VolumeNumber = "卷/期";
        /// <summary>
        /// 卷 e.g. 37
        /// </summary>
        public static string Volume = "卷";
        /// <summary>
        ///卷下的期數e.g. 2 
        /// </summary>
        public static string Number = "期";
        /// <summary>
        /// 期數
        /// </summary>
        public static string Issue = "期數";
        /// <summary>
        /// 發行月份
        /// </summary>
        public static string IssueMonth = "發行月份";
        /// <summary>
        /// 發行日期
        /// </summary>
        public static string IssueDate = "發行日期";

        /// <summary>
        /// (封面)設計者
        /// </summary>
        public static string Designer = "(封面)設計者";

        /// <summary>
        /// (封面)設計理念
        /// </summary>
        public static string Concept = "(封面)設計理念";

        /// <summary>
        /// 封面圖片
        /// </summary>
        public static string CoverImg = "封面圖片";


    }
}

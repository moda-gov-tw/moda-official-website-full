using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utility.sysConstTable.field
{
    /// <summary>系統變數值對照表</summary>
    public class BankNote: Utility.sysConstTable.Field
    {
        /// <summary>
        /// 標題；複寫Utility.sysConstTable.Field.Title
        /// </summary>
        public static new string Title = "標題";
        /// <summary>
        /// 語言
        /// </summary>
        public static string Lang = "語言";
        /// <summary>
        /// 時期
        /// </summary>
        public static string Period = "時期";
        /// <summary>
        /// 印製工廠
        /// </summary>
        public static string Factory = "印製工廠";
        /// <summary>
        /// 券別
        /// </summary>
        public static string Type = "券別";
        /// <summary>
        /// 製版
        /// </summary>
        public static string Platemade = "製版";
        /// <summary>
        /// 尺寸
        /// </summary>
        public static string Size = "尺寸";
        /// <summary>
        /// 面額
        /// </summary>
        public static string Denomination = "面額";
        /// <summary>
        /// 鈔券正面
        /// </summary>
        public static string ObverseFileSN = "鈔券正面";
        /// <summary>
        /// 鈔券反面
        /// </summary>
        public static string ReverseFileSN = "鈔券反面";
    }
}

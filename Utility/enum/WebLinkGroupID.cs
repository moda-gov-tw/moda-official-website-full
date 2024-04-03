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
    public class WebLinkGroupID
    {


        /// <summary>
        /// NEWS一般模附件組對應GroupID
        /// </summary>
        public class News
        {
            /// <summary>
            /// 相關連結(多筆)
            /// </summary>
            public static string Links { get; } = "NWML";
            
        }

        /// <summary>
        /// CP模組附件對應GroupID
        /// </summary>
        public class CP
        {
            /// <summary>
            /// 相關連結(多筆)
            /// </summary>
            public static string Links { get; } = "CPML";
  
        }

        /// <summary>
        /// 鈔券模組組對應GroupID
        /// </summary>
        public class Banknote
        {
            /// <summary>
            /// 相關連結(多筆)
            /// </summary>
            public static string Links { get; } = "BKML";

        }
    }


}
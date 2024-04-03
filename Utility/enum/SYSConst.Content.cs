using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Utility.SYSConst
{
    /// <summary>
    /// 變數檔
    /// </summary>

    public class Content
    {
        #region enum
        /// <summary>
        /// 前台判斷抓取格式
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// PAGE
            /// </summary>
            [Description("PAGE")]
            PAGE = 0,
            /// <summary>
            /// LINK
            /// </summary>
            [Description("LINK")]
            LINK = 2,
            /// <summary>
            /// DOWNLOAD
            /// </summary>
            [Description("DOWNLOAD")]
            DOWNLOAD = 1,
            /// <summary>
            /// IFRAME
            /// </summary>
            [Description("IFRAME")]
            IFRAME = 3,
            /// <summary>
            /// WEBLEVELMSN
            /// </summary>
            [Description("WEBLEVELMSN")]
            WEBLEVELMSN = 4,
            
            /// <summary>
            /// 逐字稿
            /// </summary>
            [Description("逐字稿")]
            Transcript = 10,
            /// <summary>
            /// 雙語詞彙
            /// </summary>
            [Description("雙語詞彙")]
            Bilingual = 11,
        }

        #endregion


    }

}

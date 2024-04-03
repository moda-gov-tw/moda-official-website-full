using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    /// <summary>
    /// FileType
    /// </summary>
    [Flags]
    public enum EnumFileType
    {
        // 1-圖 2-檔 3-頁首頁尾logo 0-全部 99-md

        /// <summary>
        /// 全部
        /// </summary>
        all = 0,
        /// <summary>
        /// 圖片
        /// </summary>
        img = 1,
        /// <summary>
        /// 檔案<<PDF,ODT,ODS,ODP,CSV,ZIP>>
        /// </summary>
        file = 2,
        /// <summary>
        ///頁首頁尾圖檔 
        /// </summary>
        headerfooterLogo = 3,
        /// <summary>
        /// 更多檔案格式<<CSV,XML,JSON,PDF,ODS,ODT,ODP,ZIP,7Z,SHP,GEOJSON>>
        /// </summary>
        morefile = 4,
        /// <summary>
        /// 更多檔案格式<<JS,CSS,JSON >>
        /// </summary>
        ScriptCss = 5,
        /// <summary>
        /// md檔
        /// </summary>
        md = 99
    }
}

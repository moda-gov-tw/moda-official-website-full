using Services.Models.WebContent;
using System;
using System.Collections.Generic;
using System.Text;
using DBModel;

namespace Services.Models.WebSite
{
    /// <summary>
    /// WebLinkModel
    /// </summary>
    public class WebLinkModel 
    {
        /// <summary>
        /// 資訊
        /// </summary>
        public WEBNews BasicData { get; set; }

        /// <summary>
        /// 對應圖檔
        /// </summary>
        public WebFileAndGroupIDModel Img { get; set; }


        public WebFileAndGroupIDModel Img2 { get; set; }

    }
}

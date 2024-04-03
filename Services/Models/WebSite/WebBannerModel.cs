using Services.Models.WebContent;
using System;
using System.Collections.Generic;
using System.Text;
using DBModel;

namespace Services.Models.WebSite
{
    /// <summary>
    /// WebBannerModel
    /// </summary>
    public class WebBannerModel
    {

        /// <summary>
        /// 主視覺資訊
        /// </summary>
        public WEBNews BasicData { get; set; }

        /// <summary>
        /// 主視覺對應圖檔(大圖)
        /// </summary>
        public WebFileAndGroupIDModel BigImg { get; set; }

        /// <summary>
        /// 主視覺對應圖檔(小圖)
        /// </summary>
        public WebFileAndGroupIDModel SmallImg { get; set; }

    }
}

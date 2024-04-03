using DBModel;
using Services.Models.WebContent.WebLevelManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models.WebSite
{
    /// <summary>
    /// WebSiteMasterModel
    /// </summary>
    public class WebSiteMasterModel
    {
        public SysWebSite Master { get; set; }

        public string Lang { get; set; }
        /// <summary>
        /// HeadMenu
        /// </summary>
        public List<WebLevelModel> HeadMenu { get; set; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public List<string> PopularKeys { get; set; }
        /// <summary>
        /// FatFooterMenu
        /// </summary>
        public List<WebLevelModel> FatFooterMenu { get; set; }
        /// <summary>
        /// LeftMenu
        /// </summary>
        public List<WebLevelModel> LeftMenu { get; set; }
        //網站宣告
        public List<WebLevelModel> WebsiteAnnouncementArea { get; set; }
        /// <summary>
        /// 社群分享
        /// </summary>
        public List<WebLinkModel> SocialMediaArea { get; set; }
        //便民服務區
        public List<WebLinkModel> ConvenienceServiceArea { get; set; }
        // 聯絡資訊
        public ContactUsModel ContactUsArea { get; set; }
        //Logo圖
        public WEBFile LogoImg { get; set; }
        //深色版Logo圖
        public WEBFile DarkLogoImg { get; set; }
        //導覽列
        public List<WebSiteExtend> NavBarArea { get; set; }
        //無障礙圖示
        public WEBFile AAimg { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow.AddHours(8);

        /// <summary>
        /// 瀏覽人次
        /// </summary>
        public int InSiteCount { get; set; } = 0;
        //網站語言
        public SysWebSiteLang SysWebSiteLang { get; set; }

        public List <SysCategory>  sysCategories { get; set; }

    }

    public class ogModel
    {
        public string title { get; set; }
        public string image { get; set; }
        public string image_type { get; set; }
        public string image_width { get; set; }
        public string image_height { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string keyword { get; set; }
        public string image_path { get; set; }
    }
}

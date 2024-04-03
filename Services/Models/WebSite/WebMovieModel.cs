using DBModel;

namespace Services.Models.WebSite
{
    public class WebMediaModel
    {
        /// <summary>
        /// 主視覺資訊
        /// </summary>
        public WEBNews BasicData { get; set; }

        /// <summary>
        /// 對應圖檔
        /// </summary>
        public WebFileAndGroupIDModel Img { get; set; }

    }
}

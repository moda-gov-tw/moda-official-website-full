using DBModel;
using Services.Models.WebSite;

namespace WebSite.Models
{
    public class NewsModel:meteModel
    {
        public NewsDetailModel Detail { get; set; }

        public WebLevel WebLevel { get; set; }

        /// <summary>
        /// 逐字稿
        /// </summary>
        public List<WEBNewsTranscript> wEBNewsTranscripts { get; set; } = new List<WEBNewsTranscript>();

    }
}

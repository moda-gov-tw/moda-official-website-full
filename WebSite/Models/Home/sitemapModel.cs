using DBModel;
using Services.Models.WebSite;

namespace WebSite.Models
{
    public class sitemapModel: meteModel
    {
        public WebLevel localLevel { get; set; }

        public List<WebLevelModel> webLevels { get; set; }
    }
}

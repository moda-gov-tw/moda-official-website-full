using DBModel;
using Services.Models.WebSite;

namespace WebSite.Models
{
    public class HomeModel
    {
        public class IndexModel : meteModel {
            public MODAIndexModel? modaIndexModel { get; set; }
            public ACSIndexModel? acsIndexModel { get; set; }
            public ADIIndexModel? adiIndexModel { get; set; }
        }
        /// <summary>
        /// MODA主站首頁
        /// </summary>
        public class MODAIndexModel
        {
            public List<WebSiteChildModel>? Children { get; set; }
        }
        /// <summary>
        /// ACS 首頁
        /// </summary>
        public class ACSIndexModel
        {
            public List<WebSiteChildModel>? Children { get; set; }
        }
        /// <summary>
        /// ADI 首頁
        /// </summary>
        public class ADIIndexModel
        {
            public List<WebSiteChildModel>? Children { get; set; }
        }
    }
}

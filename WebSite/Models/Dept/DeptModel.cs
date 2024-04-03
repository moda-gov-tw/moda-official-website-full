using DBModel;
using Services.Models;
using Services.Models.WebSite;

namespace WebSite.Models
{
    public class DeptModel : meteModel
    {
        /// <summary>
        /// 司資料
        /// </summary>
        public WebLevel? Dept { get; set; }


        /// <summary>
        /// Logo
        /// </summary>
        public WEBFile? LogoImg { get; set; }
        /// <summary>
        /// 頁面資料
        /// </summary>
        public List<WebSiteChildModel>? ChildNodes { get; set; }
    }
}

using DBModel;
using Services.Models;
using Services.Models.WebSite;

namespace WebSite.Models
{
    public class meteModel
    {
        public ogModel ogData { get; set; } 
        /// <summary>
        /// 麵包屑
        /// </summary>
        public List<WebSiteBreadcrumb>  webSiteBreadcrumbs { get; set; }

        public TitleBarModel titleBarMdel { get; set; }

        public List<SysCategory> langCategory { get; set; }

        public SysWebSiteLang SysWebSiteLang { get; set; }

    }
}
    
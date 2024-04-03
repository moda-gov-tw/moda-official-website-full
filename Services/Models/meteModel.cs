using DBModel;
using Services.Models;
using Services.Models.WebSite;
using System.Collections.Generic;

namespace Services.Models.WebSite
{
    public class meteModel
    {
        public ogModel ogData { get; set; } 
        /// <summary>
        /// 麵包屑
        /// </summary>
        public List<WebSiteBreadcrumb>  webSiteBreadcrumbs { get; set; }

        public TitleBarModel TitleBarModel { get; set; }

        public List<SysCategory> langCategory { get; set; }

        public SysWebSiteLang SysWebSiteLang { get; set; }

    }
}
    
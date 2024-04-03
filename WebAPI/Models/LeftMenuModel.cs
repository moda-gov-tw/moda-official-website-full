using DBModel;
using Services.Models;
using Services.Models.WebSite;

namespace WebAPI.Models
{
    public class LeftMenuModel
    {
        public string leftMenuBigTitle { get; set; } = string.Empty;

        public List<WebLevelModel> leftMenus { get; set; } = new List<WebLevelModel>();

        public List<WebSiteBreadcrumb> webSiteBreadcrumbModels { get; set; } = new List<WebSiteBreadcrumb>();

        public WebSiteBreadcrumb BaseSN { get; set; } = new WebSiteBreadcrumb();

        public string sysWebSiteId { get; set; }
        public string Lang { get; set; }

        public List<SysCategory> sysCategories { get; set; }

    }
}

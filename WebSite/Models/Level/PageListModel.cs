using DBModel;
using Services.Models.WebContent.WebLevelManagement;

namespace WebSite.Models
{
    public class PageListModel : meteModel
    {
        public WebLevel webLevel { get; set; }

        public List<WebSiteWebLevelPageListModel> webSiteWebLevelPageListModels { get; set; }

    }
}

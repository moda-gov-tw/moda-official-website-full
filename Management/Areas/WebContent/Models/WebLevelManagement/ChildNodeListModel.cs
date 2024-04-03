using DBModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;


namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class ChildNodeListModel
    {
        public List<string> LevelBreadcrumb { get; set; }
        public WebLevel ParentLevel { get; set; }
        public List<WebLevel> webLevels { get; set; }
        public DefaultPager defaultPager { get; set; }
        public List<WebLevel> SortList { get; set; }
        public  List<int> sortData { get; set; }
        public string sorttitle { get; set; } = "SortOrder";

        public string sorttype { get; set; } = "asc";

        public bool CheckYoutubeApiKey { get; set; } = false;

    }
}

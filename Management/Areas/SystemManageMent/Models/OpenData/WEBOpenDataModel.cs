using DBModel;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.SystemManageMent.Models.OpenData
{
    public class WEBOpenDataModel
    {
        public List<WEBOpenDataMain> wEBOpenDataMains { get; set; }

        public DefaultPager defaultPager { get; set; }

        public List<WEBOpenDataMain> SortList { get; set; }

        public List<KeyValuePair<int,string>> FileUrl { get; set; }
    }
}

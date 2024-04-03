using System.Collections.Generic;
using Utility;

namespace Management.Areas.Authorization.Models.GroupManagement
{
    public class ListModel
    {
        public List<Services.Models.Authorization.GroupModel> SysGroups { get; set; }

        public List<int> SortData { get; set; }

        public DefaultPager defaultPager { get; set; }


    }
}

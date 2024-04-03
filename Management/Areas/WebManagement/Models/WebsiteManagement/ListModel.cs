using DBModel;
using System.Collections.Generic;
using Utility;
namespace Management.Areas.WebManagement.Models.WebsiteManagement
{
    public class ListModel
    {
        public List<SysWebSite> sysWebSites { get; set; }

        public DefaultPager defaultPager { get; set; }
    }
}

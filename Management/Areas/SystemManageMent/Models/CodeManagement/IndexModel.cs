using DBModel;
using System.Collections.Generic;

namespace Management.Areas.SystemManageMent.Models.CodeManagement
{
    public class IndexModel
    {
        public string ParentKey { get; set; }
        public string WebSiteID { get; set; }

        public List<SysCategory> sysCategories { get; set; }

    }
}

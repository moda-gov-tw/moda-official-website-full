using DBModel;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.SystemManageMent.Models.CodeManagement
{
    public class ListModel:WEBNewsExtend
    {
        public string ParentTitle { get; set; }
        public List<SysCategory> sysCategories { get; set; }
        public string ParentKey { get; set; }
        public string WebSiteID { get; set; }

        public List<SysWebSiteLang> sysWebSiteLangs { get; set; }

        public List<DefaultPager> defaultPager { get; set; }

        public List<SysCategory> Titles { get; set; }

        /// <summary>
        /// 排序List
        /// </summary>
        public List<SysCategory> SortList { get; set; }
        /// <summary>
        /// 是否可以刪除 改成-99
        /// </summary>
        public bool CanDelete { get; set; } = false;
    }


}

using DBModel;

namespace Management.Areas.SystemManageMent.Models.CodeManagement
{
    public class EditModel
    {
        public SysCategory SysCategory { get; set; }
        public string SysCategoryKey { get; set; }
        public string ParentKey { get; set; }
        public string WebSiteID { get; set; }
    }
}

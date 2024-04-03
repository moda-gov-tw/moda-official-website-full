using DBModel;
using System.Collections.Generic;

namespace Management.Areas.Authorization.Models.DepartmentManagement
{
    public class EditModel
    {
        public List<SysDepartment> SysDepartments { get; set; }

        public List<SysWebSiteLang> sysLangs { get; set; }

        public List<SysDepartment> Titles { get; set; }

        public string ParentID { get; set; }
    }
}

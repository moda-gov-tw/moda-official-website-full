using DBModel;
using System.Collections.Generic;

namespace Management.Areas.Authorization.Models.DepartmentManagement
{
    public class IndexModel
    {
        public List<SysDepartment> SysDepartments { get; set; }
        public string ParentID { get; set; }
    }
}

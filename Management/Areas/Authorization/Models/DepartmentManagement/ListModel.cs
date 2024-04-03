using DBModel;
using System.Collections.Generic;
using Utility;
namespace Management.Areas.Authorization.Models.DepartmentManagement
{
    public class ListModel
    {
        public List<SysDepartment> SysDepartments { get; set; }
        public string ParentID { get; set; }
        public DefaultPager defaultPager { get; set; }
        public List<SysDepartment> Titles { get; set; }
        /// <summary>
        /// 排序List
        /// </summary>
        public List<SysDepartment> SortList { get; set; }
    }
}

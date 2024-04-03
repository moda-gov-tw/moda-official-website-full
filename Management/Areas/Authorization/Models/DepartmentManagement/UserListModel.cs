using DBModel;
using System.Collections.Generic;
using Utility;
namespace Management.Areas.Authorization.Models.DepartmentManagement
{
    public class UserListModel
    {
        public List<SysUser> Users { get; set; }

        public DefaultPager defaultPager { get; set; }
    }
}

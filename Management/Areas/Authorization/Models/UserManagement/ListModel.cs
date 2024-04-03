using DBModel;
using System.Collections.Generic;
using Utility;
namespace Management.Areas.Authorization.Models.UserManagement
{
    public class ListModel
    {
        public List<vw_UserLeftDep> UserLeftDeps { get; set; }

        public DefaultPager defaultPager { get; set; }
    }
}

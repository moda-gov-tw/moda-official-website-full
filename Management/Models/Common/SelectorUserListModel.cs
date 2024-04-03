using DBModel;
using System.Collections.Generic;
using Utility;
namespace Management.Models.Common
{
    public class SelectorUserListModel
    {

        public List<vw_UserLeftDep> sysUsers { get; set; }
        public DefaultPager defaultPager { get; set; }

    }
}

using DBModel;
using System.Collections.Generic;
using Utility;


namespace Management.Areas.LogManagement.Models.LoginLog
{
    public class ListModel
    {
        public List<SysUserLogin> sysUserLogins { get; set; }
        public List<SysUser> UserData { get; set; }
        public DefaultPager defaultPager { get; set; }
    }
}

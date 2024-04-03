using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModel;
using Utility;

namespace Services.Models.Common
{
    public class CommonUserSelectorModel
    {
        public List<vw_UserLeftDep> sysUsers { get; set; } 
        public DefaultPager defaultPager { get; set; }
    }
}

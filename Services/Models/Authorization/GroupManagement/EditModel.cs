using DBModel;
using Services.Models.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace Services.Models.Authorization
{
    public class EditModel
    {
        public List<SysWebSite> sysWebSites { get; set; }
        public List<GroupSectionByGroupModel> groupSectionByGroupModels { get; set; }
        public SysGroup sysGroup { get; set; }

        public List<SysSection> sysSections { get; set; }

        public List<GroupUser> GroupUsers { get; set; }
        public DefaultPager defaultPager { get; set; }
        public class GroupUser
        {
            public string UserID {get;set;}
            public string UserName { get; set; }
            public string DepartmentName { get; set; }
            public string JobTitle { get; set; }
            public int RelSysGroupUserSN { get; set; }
            public DateTime CreatedDate { get; internal set; }
        }
    }
}

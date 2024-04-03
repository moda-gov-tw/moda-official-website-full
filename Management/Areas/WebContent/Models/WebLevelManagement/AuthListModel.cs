using DBModel;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class AuthListModel
    {
        public string q { get; set; } = "";

        public List<SysGroup> sysGroups { get; set; }

        public List<AuthSysGroupWebLevel> authSysGroupWebLevels { get; set; }

        public string webLevelSN { get; set; }

        /// <summary>
        /// 語系
        /// </summary>
        public string lang { get; set; }

        public DefaultPager defaultPager { get; set; }
    }
}

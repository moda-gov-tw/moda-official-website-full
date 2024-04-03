using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class AuthModel
    {
        public List<string> LevelBreadcrumb { get; set; }
        public List<SysGroup> sysGroups { get; set; }

        public List<AuthSysGroupWebLevel>  authSysGroupWebLevels { get; set; }

        public string webLevelSN { get; set; }

        /// <summary>
        /// 語系
        /// </summary>
        public string lang { get; set; }
    }
}

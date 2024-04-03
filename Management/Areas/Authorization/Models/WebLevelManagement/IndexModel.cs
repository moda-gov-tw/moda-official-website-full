using DBModel;
using System.Collections.Generic;

namespace Management.Areas.Authorization.Models.WebLevelManagement
{
    public class IndexModel
    {
        public List<WebLevel> levelForTrees { get; set; }
        public bool GodMode { get; set; } = false;
        public List<AuthSysGroupWebLevel> authSysGroupWebLevels { get; set; }
        /// <summary>
        /// ONLY CP OR other need sp
        /// </summary>
        public List<WEBNews> WEBCP { get; set; }
    }
}

using DBModel;
using System.Collections.Generic;
namespace Management.Areas.WebContent.Models.WebLevelManagement
{
    public class IndexModel
    {
        public List<string> LevelBreadcrumb { get; set; }
        public List<WebLevel> levelForTrees { get; set; }
        public bool GodMode { get; set; } = false;
        public List<AuthSysGroupWebLevel> authSysGroupWebLevels { get; set; }
        /// <summary>
        /// ONLY CP OR other need sp
        /// </summary>
        public List<WEBNews> WEBCP { get; set; }
        /// <summary>
        /// 有開啟的
        /// </summary>
        public List<int> ParentSNOnList { get; set; }

    }
}

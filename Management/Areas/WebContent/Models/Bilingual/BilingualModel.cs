using DBModel;
using System.Collections.Generic;

namespace Management.Areas.WebContent.Models
{
    /// <summary>
    /// 雙語詞彙
    /// </summary>
    public class BilingualModel : NewsDefaultModel
    {

        public WEBNews twWEBNews { get; set; }

        public WEBNews enWEBNews { get; set; }

        public string WebSiteId { get; set; }
        public List<AuthSysGroupWebLevel> AuthSysGroupWebLevels { get; set; }
    }
}

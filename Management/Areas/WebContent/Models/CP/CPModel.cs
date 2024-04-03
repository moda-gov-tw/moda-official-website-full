using DBModel;
using System.Collections.Generic;
using static Utility.Files;

namespace Management.Areas.WebContent.Models
{
    public class CPModel : NewsDefaultModel
    {

        public string WebSiteId { get; set; }

        public List<AuthSysGroupWebLevel> AuthSysGroupWebLevels { get; set; }
    }
}

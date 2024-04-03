using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Utility.Files;

namespace Management.Areas.WebContent.Models
{
    public class NEWSModel : NewsDefaultModel
    {
        public string WebSiteId { get; set; }
        public List<AuthSysGroupWebLevel> AuthSysGroupWebLevels { get; set; }
    }
}

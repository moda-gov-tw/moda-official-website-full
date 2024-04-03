using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Utility.Files;

namespace Management.Areas.WebContent.Models
{
    public class TABModel : NewsDefaultModel
    {
        public List<WebLevel> relWebLevel { get; set; } = new List<WebLevel>();
    }
}

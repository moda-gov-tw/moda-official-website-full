using DBModel;
using Utility;
using System.Collections.Generic;
using static Utility.Files;

namespace Management.Models
{
    public class CommonScheduleModel
    {
        public WEBNews wEBNews { get; set; }
        public List<WEBNews> SchedulewEBNews { get; set; }

        public DefaultPager defaultPager { get; set; }
    }
}

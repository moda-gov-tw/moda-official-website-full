using Services.Models.ModaMailBox;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.MailBox.Models.SpeedLog
{
    public class ListModel
    {
        public List<SpeedLogModel> SpeedLogs { get; set; }
        public DefaultPager defaultPager { get; set; }
    }
}

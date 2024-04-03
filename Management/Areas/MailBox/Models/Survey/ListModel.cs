using Services.Models.ModaMailBox;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.MailBox.Models.Survey
{
    public class ListModel
    {
        public List<SurveyModel> surveys { get; set; }
        public DefaultPager defaultPager { get; set; }
        public Statistics statistics { get; set; }
        public string DateRange { get; set; }
    }
}

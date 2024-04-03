using Services.Models.MailBox;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.MailBox.Models.Report
{
    public class ListModel
    {
        public List<MailBoxReportModel> ReportModel { get; set; }
        public DefaultPager defaultPager { get; set; }
    }
}

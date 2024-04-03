using Services.Models;
using System.Collections.Generic;
using Utility;

namespace Management.Areas.WebManagement.Models.OperationStatistics
{
    public class ListModel
    {
        public List<BrowseStatisticsModel> BrowseStatisticsModels { get; set; }
        public DefaultPager defaultPager { get; set; }
    }
}

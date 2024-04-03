using Services.Models.WebManagement;
using System.Collections.Generic;
using Utility;
namespace Management.Areas.LogManagement.Models.UserOperationLog
{
    public class ListModel
    {
        public List<OperationStatisticsModel> operationStatisticsModels { get; set; }
        public DefaultPager defaultPager { get; set; }
    }
}

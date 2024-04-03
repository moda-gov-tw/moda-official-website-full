using System.Collections.Generic;
using Utility;

namespace Management.Areas.WebManagement.Models.OperationStatistics
{
    public class GAViewModel
    {
        public string Name { get; set; }



        public ChartModel Flow2Data { get; set; }

        public ChartModel Map2Data { get; set; }

        public ChartModel Country2Data { get; set; }

        public ChartModel City2Data { get; set; }

        public ChartModel HotData { get; set; }

        public ChartModel ColdData { get; set; }

        public List<CountryModel> CountryModels { get; set; }

    }



    public class CountryModel
    {
        public string CountryName { get; set; }
        public string CountryCode2 { get; set; }
        public string code { get; set; }
        public int pop { get; set; } = 0;
    }

    public class ChartModel
    {
        public string chartName { get; set; }
        public string chartId { get; set; }
        public EnumChart chartype { get; set; } 
        public List<ChartDatasets> datasets { get; set; }
        public List<string> labels { get; set; } = new List<string>();

        public int borderWidth { get; set; } = 1;

        public int hoverOffset { get; set; } = 4;
    }
    public class ChartDatasets
    {
        public string type { get; set; }
        public string label { get; set; }

        public bool clip { get; set; }= true;
        
        public List<string> backgroundColor { get; set; }

        public bool fill { get; set; } = false;

        public string axis { get; set; } = "x";

        public List<int> data { get; set; } = new List<int>();
    }

    public class GA4
    {
        public List<GACode> data { get; set; } = new List<GACode>();
    }

    public class GACode
    {
        public string WebSiteID { get; set; }
        public string Language { get; set; }
        public string GA4 { get; set; }
    }
}

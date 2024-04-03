using DBModel;
using Services.Models.WebSite;
using System.Collections.Generic;

namespace ManagementManagement.Areas.WebContent.Models
{
    public class NewsModel:meteModel 
    {
        public NewsDetailModel Detail { get; set; }

        public WebLevel WebLevel { get; set; }

        public List<WEBNewsTranscript> WEBNewsTranscript { get; set; }


    }
}

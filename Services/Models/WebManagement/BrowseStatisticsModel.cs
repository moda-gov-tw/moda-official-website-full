using System;
using System.Collections.Generic;
using System.Text;


namespace Services.Models
{
    public class BrowseStatisticsModel
    {
        public string WEBSiteID { get; set; }

        public string FileTitle { get; set; }

        public string OriginalFileName { get; set; }

        public string Path { get; set; }

        public int DownLoadCount { get; set; }

        public string Lang { get; set; }

    }

}

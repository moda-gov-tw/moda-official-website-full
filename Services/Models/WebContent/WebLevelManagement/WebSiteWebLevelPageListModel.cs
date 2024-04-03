using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.WebContent.WebLevelManagement
{
    public class WebSiteWebLevelPageListModel
    {
        public WebLevel webLevel { get; set; }
        public WEBFile webFile { get; set; } = new WEBFile();

        public WEBFile webLogo { get; set; }
        public string  url {get;set;}
        public string youtubeLink {get;set;}
        public string target { get; set; }
    }
}

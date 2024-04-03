using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models.WebManagement
{
   public class PathModel
    {
        public int WebLevelSN { get; set; }

        public int ParentSN { get; set; }

        public string Title { get; set; }
        public string Path { get; set; }

    }
}

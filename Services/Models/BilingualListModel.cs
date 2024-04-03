using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class BilingualListModel
    {
        public int MainSN { get; set; }

        public string zhtw{ get; set; }

        public string en { get; set; }

        public int ArticleType { get; set; }
        public string Url { get; set; }
        public string target { get; set; }
    }
    public class BilingualListDBModel
    {
        public int MainSN { get; set; }

        public string Lang { get; set; }

        public string Title { get; set; }

        public int ArticleType { get; set; }

        public string Url { get; set; }

        public string target { get; set; }

        public string FileType { get; set; }

        public string RegulationsURL { get; set; }

        public string RegulationsTitle { get; set; }


    }


}

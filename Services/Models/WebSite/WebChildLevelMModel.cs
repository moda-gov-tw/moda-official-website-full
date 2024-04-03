using DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models.WebSite
{
    public class childModel
    {
        public PAGELISTModel localData { get; set; }

        public WEBFile file { get; set; }
    }

    public class newsChildModel
    { 
        public WEBNews localData { get; set; }

        public WEBFile file { get; set; }

        public WEBFile coverfile { get; set; }
        /// <summary>
        /// 前台顯示連結
        /// </summary>
        public string DynamicURL { get; set; }
    }

}

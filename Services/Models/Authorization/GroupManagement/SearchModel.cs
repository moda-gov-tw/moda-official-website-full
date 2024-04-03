using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModel;

namespace Services.Models.Authorization
{
    public class SearchModel
    {
        public class SelectOptions 
        {
            public string Value { get; set; }

            public string Title { get; set; }
        }
        /// <summary>
        /// 選單_單位
        /// </summary>
        public List<SelectOptions> SysDepartments { get; set; }

        /// <summary>
        /// 選單_單位2
        /// </summary>
        public List<SysDepartment> sysDepartments { get; set; }
        /// <summary>
        /// 選單_功能名稱
        /// </summary>
        public List<SysSection> SysSections { get; set; }
        /// <summary>
        /// 選單_網站單元
        /// </summary>
        public List<SelectOptions> WebLevelDs { get; set; }

    }
}

using DBModel;
using System.Collections.Generic;

namespace Management.Areas.Authorization.Models.UserManagement
{
    public class IndexModel
    {
        /// <summary>
        /// 部門資料
        /// </summary>
        public List<SysDepartment> sysDepartments { get; set; }
    }
}

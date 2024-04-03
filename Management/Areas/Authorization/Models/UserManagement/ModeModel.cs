using DBModel;
using Services.Models;
using System.Collections.Generic;
namespace Management.Areas.Authorization.Models.UserManagement
{
    public class ModeModel
    {     
        /// <summary>
        /// 會員基本資料
        /// </summary>
        public SysUser sysUser { get; set; }
        /// <summary>
        /// 會員群組資料
        /// </summary>
        public List<sysGroupToUserModel> sysGroupToUserModels { get; set; }
        /// <summary>
        /// 部門資料
        /// </summary>
        public List<SysDepartment> sysDepartments { get; set; }

        /// <summary>
        /// 登入者ID
        /// </summary>
        public string UserID { get; set; }
    }
}

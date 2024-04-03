using DBModel;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace Services.Authorization
{
    public class SYSUserService
    {


        public static SysUser GetUserData(string UserID)
        {
            using (var db = new MODAContext())
            {
                return db.SysUser.Where(x => x.UserID == UserID).First();
            }
        }

    }
}

using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Services.LogManagement
{
    public class LoginLogService
    {
       
        /// <summary>
        /// 登入紀錄
        /// </summary>
        /// <param name="str"></param>
        /// <param name="end"></param>
        /// <param name="pager"></param>
        /// <param name="isExport">不匯出就要分頁，匯成Excel不分頁</param>
        /// <returns></returns>
        public static List<SysUserLogin> GetUserLogins(string str, string end, ref DefaultPager pager, bool isExport=false , string userid="" , string ip="")
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Data = db.SysUserLogin.Where(x => 1 == 1);
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        var _str = DateTime.Parse(str);
                        Data = Data.Where(x => (x.CreatedDate >= _str));
                    }
                    if (!string.IsNullOrWhiteSpace(end))
                    {
                        var _end = DateTime.Parse(end);
                        Data = Data.Where(x => (x.CreatedDate <= _end));
                    }
                    if (!string.IsNullOrWhiteSpace(userid))
                    {
                        Data = Data.Where(x => x.UserID == userid);
                    }
                    if (!string.IsNullOrWhiteSpace(ip))
                    {
                        Data = Data.Where(x => x.ProcessIPAddress == ip);
                    }
                    if (isExport == false)
                    {
                        var allData = Data.Count();
                        pager.TotalCount = allData;
                        pager.PageIndex = pager.p - 1;
                        //可以下ORDER BY 條件
                        var searchData = Data.OrderByDescending(o => o.CreatedDate).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();
                        return searchData;
                    }
                    else
                    {
                        return Data.OrderByDescending(o => o.CreatedDate).ToList();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static List<SysUser> GetUserName(List<SysUserLogin> sysUserLogin)
        {
            using (var db = new MODAContext())
            {
                var data = (from u in sysUserLogin
                            join s in db.SysUser
                            on u.UserID equals s.UserID
                            select new SysUser
                            {
                                UserID = u.UserID,
                                UserName = s.UserName,
                            }).Distinct(x => x.UserID).ToList();

                return data;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utility.sysConstTable.field
{
    /// <summary>欄位錯誤訊息</summary>
    public class Error
    {

        /// <summary>
        /// 發布日期不能大於結束日期錯誤訊息
        /// </summary>
        /// <returns></returns>
        public static string ErrMsgSDMoreED()
        {
            return $"[{Utility.sysConstTable.Field.EndDate}]不能小於[{Utility.sysConstTable.Field.StartDate}]";
        }
        /// <summary>
        /// 必填欄位回傳值
        /// </summary>
        /// <param name="FldName"></param>
        /// <returns></returns>
        public static string Required(string FldName)
        {
            return $"[{FldName}]必填";
        }
        /// <summary>
        /// 必填欄位-不能為中文
        /// </summary>
        /// <param name="FldName"></param>
        /// <returns></returns>
        public static string RequiredChines(string FldName)
        {
            return $"[{FldName}]不能為中文";
        }

        public static string Required(string[] FldName)
        {
            string rtn = "";
            foreach (var item in FldName)
            {
                rtn += $"[{item}]";
            }
            if (!string.IsNullOrWhiteSpace(rtn)) rtn += "必填";
            return rtn;
        }
    }
}

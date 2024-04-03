using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Utility.Model
{
    public class LoginModel
    {
        /// <summary>
        /// 狀態
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// 失敗
            /// </summary>
            Error = 0,
            /// <summary>
            /// 成功
            /// </summary>
            Scuess = 1,
        }
        /// <summary>
        /// 0:Web, 1:Service, 2:Utility
        /// </summary>
        public enum ActionType
        {
            Web = 0,
            Service = 1,
            Utility = 2
        }
        /// <summary>
        /// 新增修
        /// </summary>
        public enum Action2
        {
            insert = 0,
            update = 1,
            returned = -2,
            delete = 2

        }

        #region AAD Setting
        public string tenant_id { get; set; }
        public string client_id { get; set; }
        public string client_secret {get;set;}
        public string callback_url { get; set; }
        #endregion


        public static string ServieError = "系統發生問題\n請稍後嘗試或聯繫系統工程師";

    }
}

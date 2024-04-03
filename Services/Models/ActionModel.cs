using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class ActionModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsActionSuccess { get; set; } = true;
        /// <summary>
        /// 回傳訊息
        /// </summary>
        public string ActionMessage { get; set; } = "";
        /// <summary>
        /// 動作有誤
        /// </summary>
        /// <param name="msg">錯誤訊息</param>
        public void ActionExption(string msg) 
        {
            IsActionSuccess = false;
            ActionMessage = msg;
        }
    }
}

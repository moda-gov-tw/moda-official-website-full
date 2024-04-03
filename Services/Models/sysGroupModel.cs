using DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models
{
    public class sysGroupModel
    {
        /// <summary>
        /// 確認是否正確
        /// </summary>
        public bool check { get; set; } = true;

        /// <summary>
        /// 登入者訊息
        /// </summary>
        public SysGroup sysGroup { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string message { get; set; }

        ///// <summary>
        ///// Menu 尚未設定
        ///// </summary>
        //public string menu { get; set; }

        //public int errorCount { get; set; } = 0;

        //public string type { get; set; }

    }
}

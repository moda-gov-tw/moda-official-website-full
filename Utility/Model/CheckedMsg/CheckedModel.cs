using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Model
{
    public class CheckedModel
    {
        /// <summary>
        /// 確認
        /// </summary>
        public bool chk { get; set; } = true;
        /// <summary>
        /// error訊息
        /// </summary>
        public string error { get; set; }

    }
}

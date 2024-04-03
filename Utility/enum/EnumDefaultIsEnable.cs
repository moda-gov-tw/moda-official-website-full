using System;
using System.ComponentModel;

namespace Utility.Default
{
    [Flags]
    public  enum EnumDefaultIsEnable
    {
        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        stop = 0,
        /// <summary>
        /// 啟動
        /// </summary>
        [Description("啟動")]
        start = 1,
        /// <summary>
        /// 刪除
        /// </summary>
        [Description("刪除")]
        delete = -99,
    }
}

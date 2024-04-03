using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Utility
{
    [Flags]
    public enum EnumChart
    {
        /// <summary>
        /// 折線圖
        /// </summary>
        [Description("折線圖")]
        line,
        /// <summary>
        /// 線條圖
        /// </summary>
        [Description("線條圖")]
        bar,
        /// <summary>
        /// 混合型
        /// </summary>
        [Description("混合型")]
        mixed,
        /// <summary>
        /// 甜甜圈
        /// </summary>
        [Description("甜甜圈")]
        boughnut,
        /// <summary>
        /// 圓餅圖
        /// </summary>
        [Description("圓餅圖")]
        pie,
        /// <summary>
        /// 極地面積圖
        /// </summary>
        [Description("極地面積圖")]
        polar,
        /// <summary>
        /// 雷達圖
        /// </summary>
        [Description("雷達圖")]
        radar,
        /// <summary>
        /// 散點圖
        /// </summary>
        [Description("散點圖")]
        scatter ,

    }
}

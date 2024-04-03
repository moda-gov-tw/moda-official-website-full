using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Utility.Default
{
    [Flags]
    public enum EnumDeptTemplateValue
    {
        /// <summary>
        /// 標籤列表
        /// </summary>
        [Description("標籤列表")]
        Tab1 = 1,
        /// <summary>
        /// 圖示列表(六角)
        /// </summary>
        [Description("圖示列表(六角)")]
        Tab2 = 2,
        /// <summary>
        /// 多欄文字列表
        /// </summary>
        [Description("多欄文字列表")]
        Tab3 = 3,
        /// <summary>
        /// 單欄文字列表
        /// </summary>
        [Description("單欄文字列表")]
        Tab4 = 4,
        /// <summary>
        /// 色塊列表(橢圓)
        /// </summary>
        [Description("色塊列表(橢圓)")]
        Tab5 = 5,
        /// <summary>
        /// 無日期文字列表")
        /// </summary>
        [Description("無日期文字列表")]
        Tab6 = 6,
        /// <summary>
        /// 手風琴列表
        /// </summary>
        [Description("手風琴列表")]
        Tab7 = 7,
        /// <summary>
        /// 影音列表
        /// </summary>
        [Description("影音列表")]
        Tab8 = 8,
        /// <summary>
        /// 圖片列表
        /// </summary>
        [Description("圖片列表")]
        Tab9 = 9,
        /// <summary>
        /// 雙語詞彙
        /// </summary>
        [Description("雙語詞彙")]
        Tab10 = 10,
    }
}

using System.ComponentModel;

namespace Utility
{
    /// <summary>
    /// 樹的類型
    /// </summary>
    public enum EnumWeblevelType
    {
        /// <summary>
        /// 全部都要有
        /// </summary>
        [Description("全部都要有")]
        All = 0,
        /// <summary>
        /// 網站單元維護
        /// </summary>
        [Description("網站單元維護")]
        WebLevelManagment = 1,
        /// <summary>
        /// 首頁區塊維護
        /// </summary>
        [Description("首頁區塊維護")]
        WebHomeManagment = 2,
        /// <summary>
        ///頁首頁底區塊維護
        /// </summary>
        [Description("頁首頁底區塊維護")]
        WebHeaderFooterManagment = 3
    }



}

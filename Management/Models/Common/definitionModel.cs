using System.Collections.Generic;

namespace Management.Models.Common
{
    /// <summary>
    /// 初始CommonModel
    /// </summary>
    public class definitionModel
    {
        /// <summary>
        /// 選取的Id & Name
        /// </summary>
        public string IdName { get; set; } = string.Empty;
        /// <summary>
        /// 自己的KEY - 可以是多選
        /// </summary>
        public List<string> selectIds { get; set; } = new List<string>();
        /// <summary>
        /// 控制項類型
        /// </summary>
        public htmlType HtmlType { get; set; } = htmlType.selector;
        /// <summary>
        /// 是否需要 <請選擇>
        /// </summary>
        public bool NeedNull { get; set; } = true;
        /// <summary>
        /// 類型
        /// </summary>
        public enum htmlType
        {
            radion = 1,
            selector = 2,
            checkbox = 3,
        }

        /// <summary>
        /// 允許所有子網站
        /// </summary>
        public bool AllWebsite { get; set; } = false;
    }
}

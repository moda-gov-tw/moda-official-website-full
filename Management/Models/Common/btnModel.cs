namespace Management.Models.Common
{
    public class btnModel : definitionModel
    {
        /// <summary>
        /// Btntype類型
        /// </summary>
        public btntype Btntype { get; set; } = btntype.其他;

        /// <summary>
        /// onclickFunction
        /// </summary>
        public string onclickFunction { get; set; } = string.Empty;
        /// <summary>
        /// JS使用的 className
        /// </summary>
        public string jsUseClassName { get; set; } = string.Empty;
        /// <summary>
        /// 按鈕文字-<這邊之後可能會移除>
        /// </summary>
        public string value { get; set; } = string.Empty;
        /// <summary>
        /// 擴充attr
        /// </summary>
        public string attr_data_id { get; set; } = string.Empty;
        /// <summary>
        /// 擴充attr 
        /// </summary>
        public string attr_data_type { get; set; } = string.Empty;
        /// <summary>
        /// 權限
        /// </summary>
        public System.Collections.Generic.List<string> Auth { get; set; }
        /// <summary>
        /// 送審狀態
        /// </summary>
        public string isenable { get; set; } = string.Empty;
        public string module { get; set; } = string.Empty;

        /// <summary>
        /// 停用按紐
        /// </summary>
        public bool disable { get; set; } = false;

        /// <summary>
        /// btntype 樣板
        /// </summary>
        public enum btntype
        {
            登入,
            新增,
            儲存,
            取消,
            回列表,
            查詢,
            匯出,
            確認,
            編修,
            不可編輯,
            不可刪除,
            其他,
            刪除,
            啟用,
            停用,
            預覽,
            選擇,
            回上一頁,
            送審,
            發布,
            關閉,
            退回,
            確認新增,
            複製,
            展開,
            重置靜態,
            送審確認,
            送審退回,
            開始掃描,
            發送API,
            取API,
            AzureAD登入,
            發信,
            暫存,
            改分,
            檢視,
            下載
        }


    }
}

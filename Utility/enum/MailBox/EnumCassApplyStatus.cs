using System;
using System.ComponentModel;

namespace Utility.MailBox
{
    [Flags]
    public enum EnumCassApplyStatus
    {
        /// <summary>
        /// 新增電子信箱認證信
        /// </summary>
        [Description("新增電子信箱認證信")]
        step0 = 0,
        /// <summary>
        /// 電子信箱認證信寄送成功
        /// </summary>
        [Description("電子信箱認證信寄送成功")]
        step1 = 1,
        /// <summary>
        /// 電子信箱認證信寄送失敗
        /// </summary>
        [Description("電子信箱認證信寄送失敗")]
        step2 = 2,
        /// <summary>
        /// 電子信箱已完成認證
        /// </summary>
        [Description("電子信箱已完成認證")]
        step3 = 3,
        /// <summary>
        /// 暫存民眾的意見內容
        /// </summary>
        [Description("暫存民眾的意見內容")]
        step4 = 4,
        /// <summary>
        /// 案件申請確認信寄送成功
        /// </summary>
        [Description("案件申請確認信寄送成功")]
        step5 = 5,
        /// <summary>
        /// 案件申請確認信寄送失敗
        /// </summary>
        [Description("案件申請確認信寄送失敗")]
        step6 = 6,
        /// <summary>
        /// 成案
        /// </summary>
        [Description("已成案，待排程拋送")]
        step7 = 7,
        /// <summary>
        /// 系統傳送成功，公文回應受理結果成功
        /// </summary>
        [Description("公文系統辦理中")]
        step8 = 8,
        /// <summary>
        /// 系統傳送成功，公文回應受理結果失敗
        /// </summary>
        [Description("已成案，公文回應受理結果失敗")]
        step9 = 9,
        /// <summary>
        /// 系統傳送失敗
        /// </summary>
        [Description("已成案，後台系統拋送失敗")]
        step10 = 10,
        /// <summary>
        /// 公文系統：待改分
        /// </summary>
        [Description("公文系統：待改分")]
        step11 = 11,
        /// <summary>
        /// 公文系統：結案
        /// </summary>
        [Description("公文系統結案")]
        step12 = 12,
        /// <summary>
        /// 管理系統結案
        /// </summary>
        [Description("管理系統結案")]
        step13 = 13,
        /// <summary>
        /// 已回覆，案件回覆說明信寄送成功
        /// </summary>
        [Description("已回覆，案件回覆說明信寄送成功")]
        step14 = 14,
        /// <summary>
        /// 已回覆，案件回覆說明信寄送失敗
        /// </summary>
        [Description("已回覆，案件回覆說明信寄送失敗")]
        step15 = 15,

        /// <summary>
        /// 已回覆，系統已自動回覆
        /// </summary>
        [Description("已回覆，系統已自動回覆")]
        step16 = 16 ,

        /// <summary>
        /// 辦理中(併)
        /// </summary>
        [Description("辦理中(併)")]
        step20 = 20,
        /// <summary>
        /// 發文完成(併)
        /// </summary>
        [Description("發文完成(併)")]
        step21 = 21,
        /// <summary>
        /// 存查
        /// </summary>
        [Description("存查")]
        step22 = 22,
        /// <summary>
        /// 存查(併)
        /// </summary>
        [Description("存查(併)")]
        step23 = 23,


    }
    [Flags]

    public enum EnumReplySource
    {
        /// <summary>
        /// 公文系統
        /// </summary>
        [Description("公文系統")]
        Speed = 1,
        /// <summary>
        /// 管理系統
        /// </summary>
        [Description("官網後台")]
        Mgr = 2,

        [Description("未回覆")]
        None = 0,
    }
    //[Flags]
    //public enum EnumCassApplyValidateStatus
    //{
    //    /// <summary>
    //    /// 新增
    //    /// </summary>
    //    step0 = 0,
    //    /// <summary>
    //    /// 使用者驗證
    //    /// </summary>
    //    step1 = 1,
    //    /// <summary>
    //    /// 已成案
    //    /// </summary>
    //    step2 = 2,
    //}

    /// <summary>
    /// 後台查詢條件
    /// </summary>
    public enum MgrStatus
    {
        /// <summary>
        /// 已成案
        /// </summary>
        [Description("已成案")]
        Accepted = 0,
        /// <summary>
        /// 辦理中
        /// </summary>
        [Description("辦理中/辦理中(併)")]
        Process = 1,
        /// <summary>
        /// 管理系統草稿暫存
        /// </summary>
        [Description("管理系統草稿暫存")]
        Temp = 2,
        /// <summary>
        /// 已結案
        /// </summary>
        [Description("已結案")]
        Closed = 3,
        /// <summary>
        /// 公文系統結案
        /// </summary>
        [Description("公文系統結案")]
        SpeedClosed = 4,
        /// <summary>
        /// 官網後台結案
        /// </summary>
        [Description("官網後台結案")]
        MgrClosed = 5,
        ///// <summary>
        ///// 辦理中(併)
        ///// </summary>
        //[Description("辦理中(併)")]
        //ProcessMerge = 6,
        /// <summary>
        /// 發文完成(併)
        /// </summary>
        [Description("發文完成(併)")]
        SpeedClosedMerge = 7,
        /// <summary>
        /// 存查/存查(併)
        /// </summary>
        [Description("存查/存查(併)")]
        CheckClosedMerge = 8,
        /// <summary>
        /// 系統自動結案
        /// </summary>
        [Description("系統自動結案")]
        AutoClosed = 9,
    }
}

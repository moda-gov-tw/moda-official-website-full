using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Utility
{
    /// <summary>
    ///  節點類型第一層 非存入的資料
    /// </summary>
    public enum EnumWebLevelModuleLevel1 
    {
       

        /// <summary>
        /// 子單元列表
        /// </summary>
        [Description("子單元列表")]
        PAGELIST = 1,
        /// <summary>
        /// 靜態網頁
        /// </summary>
        [Description("靜態網頁")]
        CP = 2,
        /// <summary>
        /// 資料上稿模組
        /// </summary>
        [Description("資料上稿模組")]
        NEWS = 3,
        /// <summary>
        /// 司主題頁
        /// </summary>
        [Description("司主題頁")]
        DEPT = 4,
        /// <summary>
        /// 還未設定
        /// </summary>
        [Description("還未設定")]
        error = 99,
    }
    /// <summary>
    /// 節點類型 實際存入的Module
    /// </summary>
    public enum EnumWebLevelModuleLevel2
    {
        /// <summary>
        /// 新聞維護
        /// </summary>
        [Description("新聞維護")]
        NEWS,
        /// <summary>
        /// 主視覺維護
        /// </summary>
        [Description("主視覺維護")]
        BANNER,
        /// <summary>
        /// 輪播圖片
        /// </summary>
        [Description("輪播圖片")]
        BANNER2,
        /// <summary>
        /// 靜態頁維護
        /// </summary>
        [Description("靜態頁維護")]
        CP,
        /// <summary>
        /// 圖文維護
        /// </summary>
        [Description("圖文維護")]
        IMGTEXT,
        /// <summary>
        /// 連結維護(標題、連結、圖檔)
        /// </summary>
        [Description("連結維護(標題、連結、圖檔)")]
        LINK,
        /// <summary>
        /// 影音維護(標題、影音連結)
        /// </summary>
        [Description("影音維護(標題、影音連結)")]
        MEDIA,
        /// <summary>
        /// 列表頁
        /// </summary>
        [Description("列表頁")]
        PAGELIST,
        /// <summary>
        /// 頁籤維護
        /// </summary>
        [Description("頁籤維護")]
        TAB,
        /// <summary>
        /// 文字維護(標題)
        /// </summary>
        [Description("文字維護(標題)")]
        TEXT,
        /// <summary>
        /// RSS頻道
        /// </summary>
        [Description("RSS頻道")]
        RSS,
        /// <summary>
        /// 司維護
        /// </summary>
        [Description("司維護")]
        DEPT,
        /// <summary>
        /// 首長行程
        /// </summary>
        [Description("首長行程")]
        Schedule,
        /// <summary>
        /// 開放資料集
        /// </summary>
        [Description("開放資料集")]
        OpendataNews,
        /// <summary>
        /// 擴充圖文
        /// </summary>
        [Description("擴充圖文")]
        Extend,
        /// <summary>
        /// 雙語詞彙
        /// </summary>
        [Description("雙語詞彙")]
        Bilingual,
        /// <summary>
        /// 還未設定
        /// </summary>
        [Description("還未設定")]
        error = 99,
    }
    public class WebLevelModule
    {
        /// <summary>
        /// 類型
        /// </summary>
        /// <returns></returns>
        public static List<WebLevelModuleModel> GetWebLevelMenu()
        {
            return new List<WebLevelModuleModel>() {
               new WebLevelModuleModel() { title ="子單元列表", value= EnumTpye.GetEnumName(EnumWebLevelModuleLevel1.PAGELIST) , TypeName=EnumTpye.GetEnumNumberToSting(EnumWebLevelModuleLevel1.PAGELIST) },
               new WebLevelModuleModel() { title ="靜態網頁", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel1.CP) , TypeName=EnumTpye.GetEnumNumberToSting(EnumWebLevelModuleLevel1.CP)  },
               new WebLevelModuleModel() { title ="資料上稿模組", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel1.NEWS) , TypeName=EnumTpye.GetEnumNumberToSting(EnumWebLevelModuleLevel1.NEWS)  },
               new WebLevelModuleModel(){  title ="司主題頁" ,value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel1.DEPT),TypeName=EnumTpye.GetEnumNumberToSting(EnumWebLevelModuleLevel1.DEPT) },
            };
        }
        /// <summary>
        /// 取得 WebLevelModule<<模組>>
        /// </summary>
        /// <returns></returns>
        public static List<WebLevelModuleModel> GetList(EnumWeblevelType weblevelType)
        {
            var list = new List<WebLevelModuleModel>() {
               new WebLevelModuleModel() { title ="新聞維護", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.NEWS) , TypeName="" , weblevelType1 = EnumWeblevelType.All},
               new WebLevelModuleModel() { title ="靜態頁維護", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.CP),TypeName="CP" , weblevelType1 = EnumWeblevelType.All},
               new WebLevelModuleModel() { title ="列表頁", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.PAGELIST) , TypeName=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.PAGELIST) , weblevelType1 = EnumWeblevelType.All},
               new WebLevelModuleModel() { title ="司維護" , value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.DEPT), TypeName = "DEPT", weblevelType1 = EnumWeblevelType.All},
               new WebLevelModuleModel() { title ="RSS頻道", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.RSS) , TypeName="RSS" , weblevelType1 = EnumWeblevelType.WebLevelManagment},
               new WebLevelModuleModel() { title ="首長行程", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.Schedule), TypeName = "Schedule", weblevelType1 = EnumWeblevelType.WebLevelManagment},
               new WebLevelModuleModel() { title ="開放資料集", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.OpendataNews), TypeName ="OpendataNews", weblevelType1 = EnumWeblevelType.WebLevelManagment},
               new WebLevelModuleModel() { title ="雙語詞彙", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.Bilingual) , TypeName="Bilingual" , weblevelType1 = EnumWeblevelType.WebLevelManagment},
               new WebLevelModuleModel() { title ="主視覺維護", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.BANNER) , sort = true, TypeName="" , weblevelType1 = EnumWeblevelType.WebHomeManagment },
               new WebLevelModuleModel() { title ="輪播圖片", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.BANNER2) , sort = true, TypeName="" , weblevelType1 = EnumWeblevelType.WebHomeManagment },
               new WebLevelModuleModel() { title ="圖文維護", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.IMGTEXT) , TypeName="" , weblevelType1 = EnumWeblevelType.WebHomeManagment },
               new WebLevelModuleModel() { title ="連結維護(標題、連結、圖檔)", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.LINK) , sort= true , weblevelType1 = EnumWeblevelType.WebHomeManagment},
               new WebLevelModuleModel() { title ="影音維護(標題、影音連結)", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.MEDIA) , TypeName="" , weblevelType1 = EnumWeblevelType.WebHomeManagment},
               new WebLevelModuleModel() { title ="頁籤維護", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.TAB) , TypeName="" , weblevelType1 = EnumWeblevelType.WebHomeManagment},
               new WebLevelModuleModel() { title ="文字維護(標題)", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.TEXT) , TypeName="" , weblevelType1 = EnumWeblevelType.WebHomeManagment},
               new WebLevelModuleModel() { title ="擴充圖文", value=EnumTpye.GetEnumName(EnumWebLevelModuleLevel2.Extend), TypeName = "Extend", weblevelType1 = EnumWeblevelType.WebHomeManagment}
            };
            list = list.Where(x => x.weblevelType1 == EnumWeblevelType.All || x.weblevelType1 == weblevelType).ToList();

            return list;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public static List<WebLevelModuleModel> GetPageListModel()
        {
            var list = new List<WebLevelModuleModel>() {
               new WebLevelModuleModel() { title ="色塊列表", value="ColorBlockList" , TypeName="1,3" },
               new WebLevelModuleModel() { title ="文字列表", value="TextList" , TypeName="1" },
               new WebLevelModuleModel() { title ="標籤列表", value="LabelList" , TypeName="1,3" },
               new WebLevelModuleModel() { title ="圖示列表", value="IcoList" , TypeName="1,3" },
               new WebLevelModuleModel() { title ="單欄文字列表", value="OneTextList" , TypeName="3" },
               new WebLevelModuleModel() { title ="雙欄文字列表", value="TwoTextList" , TypeName="3" },
               new WebLevelModuleModel() { title ="無日期文字列表", value="NoDateTextList" , TypeName="3" },
               new WebLevelModuleModel() { title ="手風琴列表", value="AccordionList" , TypeName="3" },
               new WebLevelModuleModel() { title ="影音列表", value="ImageTextList" , TypeName="3" },
               new WebLevelModuleModel() { title ="圖片列表", value="ImageTextList2" , TypeName="3" },
               new WebLevelModuleModel() { title ="雙語詞彙列表", value="BilingualList" , TypeName="3" },
            };
            return list;
        }

        public static List<WebLevelModuleModel> GetTemplate()
        {
            var list = new List<WebLevelModuleModel>()
            {
                new WebLevelModuleModel(){title="連結維護(標題、連結、圖檔)",value="LINK",TypeName="1",IsEnable = "0"},
                new WebLevelModuleModel(){title="頁籤維護",value="TAB",TypeName = "2",IsEnable = "1"},
            };
            return list;
        }

        /// <summary>
        /// 司版型
        /// </summary>
        /// <returns></returns>
        public static List<WebLevelModuleModel> GetTemplateList()
        {
            var list = new List<WebLevelModuleModel>()
            {
                new WebLevelModuleModel(){title="標籤列表",value="1",TypeName ="2",Image=""},
                new WebLevelModuleModel(){title="圖示列表",value="2",TypeName ="2",Image=""},
                new WebLevelModuleModel(){title="多欄文字列表",value="3",TypeName ="2",Image=""},
                new WebLevelModuleModel(){title="單欄文字列表",value="4",TypeName ="2",Image=""},
                new WebLevelModuleModel(){title="色塊列表",value="5",TypeName="2",Image=""},
                new WebLevelModuleModel(){title="無日期文字列表",value="6",TypeName="2",Image=""},
                new WebLevelModuleModel(){title="手風琴列表",value="7",TypeName="2",Image=""},
                new WebLevelModuleModel(){title="影音列表",value="8",TypeName="2",Image=""},
                new WebLevelModuleModel(){title="圖片列表",value="9",TypeName="2",Image=""}
            };
            return list;
        }

        public static List<WebLevelModuleModel> Condition()
        {
            var list = new List<WebLevelModuleModel>()
            {
                //new WebLevelModuleModel() { title ="政策計畫", value="4" },
                //new WebLevelModuleModel(){ title = "業務分類",value="5"},
                new WebLevelModuleModel(){ title="服務對象",value="6"},
                new WebLevelModuleModel(){ title="分類",value="8"},
            };
            return list;
        }
    }
    public class WebLevelModuleModel
    {
        /// <summary>
        /// 名稱
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }

        public bool sort { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public string TypeName { get; set; }

        public EnumWeblevelType weblevelType1 { get; set; }

        public string Image { get; set; }

        public string IsEnable { get; set; }

    }


}

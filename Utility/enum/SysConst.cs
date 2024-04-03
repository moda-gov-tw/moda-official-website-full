using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 變數檔
    /// </summary>
    public class SysConst
    {
        #region enum
        /// <summary>
        /// 資料庫來源定義檔(數字不重要，重點是文字)
        /// </summary>
        public enum SourceTable
        {
            [Description("WEBNEWS")]
            WEBNEWS = 3,

            [Description("WEBLEVEL")]
            WEBLEVEL = 5,
        }
        /// <summary>
        /// 模組變數定義檔(數字不重要，重點是文字)
        /// </summary>
        public enum Module
        {
            BANNER = 3,
            CP = 4,
            FOOTER = 5,
            IMGTEXT = 6,
            JOURNAL = 7,
            LINK = 8,
            MEDIA = 9,
            NEWS = 10,
            PAGELIST = 11,
            TAB = 12,
            TEXT = 13,
            BANNER2 = 14,
            Schedule = 15,
            OpendataNews = 16,
            Extend = 17,
            Bilingual= 18,
        }
        /// <summary>
        /// 顯示/不顯示
        /// </summary>
        public enum Show
        {
            Display = 1,
            NotDisplay = 0
        }

        /// <summary>
        /// <summary>
        /// 動作：Browse(瀏灠),Add(新增),Edit(修改),Del(刪除),Search(查詢),Print(列印),
        /// Export(匯出),Mail(寄信),Download(下載),Login(登入),Logout(登出)
        /// </summary>
        public enum Action
        {
            Browse = 0,
            Add = 1,
            Edit = 2,
            Del = 3,
            Search = 4,
            Print = 5,
            Export = 6,
            Mail = 7,
            Download = 8,
            Login = 9,
            Logout = 10,
        }


        #endregion

        #region 查字典
        // 查字典01
        public static string FindInDictionary(Dictionary<string, string> MyDic, string FindMe, string rtnVal = "Not Found")
        {
            if (true == (MyDic.ContainsKey(FindMe)))
            {
                return MyDic[FindMe];
            }
            else
            {
                return rtnVal;
            }
        }

        // 查字典02
        public static string FindInDictionaryByVal(Dictionary<string, string> MyDic, string FindMe)
        {
            foreach (var pair in MyDic)
            {

                if (pair.Value == FindMe)
                {
                    return pair.Key;
                }
            }
            return "";
        }
        #endregion

       

        #region class
        /// <summary>
        /// 語系變數
        /// </summary>
        public class Lang
        {
            public static string TW { get; } = "TW";
            public static string EN { get; } = "ENG";
        }
        /// <summary>
        /// 狀態參數
        /// </summary>
        public class IsEnable
        {
            public enum Code
            {
                NoPublish = 0,
                Publish = 2,
                Draft = 1,
                OffShelf = 3,
                Del = -99
            }

            //public static List<string> BackendQryIsEnableItem = new List<string> {  "1", "0", "2", "3" };
            public static List<string> BackendQryIsEnableItem = new List<string> { "1", "0", "3" };

            public static Dictionary<string, string> Items()
            {
                Dictionary<string, string> film = new Dictionary<string, string>();
                film.Add("0", SysConstTable.CntStatus.NoPublish);
                film.Add("1", SysConstTable.CntStatus.Publish);
                film.Add("2", SysConstTable.CntStatus.Draft);
                film.Add("3", SysConstTable.CntStatus.OffShelf);
                film.Add("-99", SysConstTable.CntStatus.Del);

                return film;
            }
            public static Dictionary<string, string> Items(List<string> items)
            {
                Dictionary<string, string> film = Items();

                Dictionary<string, string> rtnfilm = new Dictionary<string, string>();
                string val = "";
                foreach (string code in items)
                {
                    val = FindInDictionary(film, code, ""); //找對應值
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        rtnfilm.Add(code, val);
                    }
                }

                return rtnfilm;
            }
        }

        #endregion

    }


}

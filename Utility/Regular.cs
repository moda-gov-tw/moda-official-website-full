using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Utility
{
    public enum RegularType
    {
        /// <summary>
        /// 全部大小寫數字特殊符號
        /// </summary>
        all,
        number,
        big_en,
        smail_en,
        special,
        notspecial,
        base58
    }
    public class Regular
    {
        /// <summary>
        /// 隨機取字串
        /// </summary>
        /// <param name="length">字串長度</param>
        /// /// <param name="RegularType">隨機類型</param>
        /// <returns></returns>
        public static string GetRandomString(int length, RegularType regularType)
        {
            var str = "";

            try
            {
                switch (regularType)
                {
                    case RegularType.all:
                        str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*";
                        break;
                    case RegularType.number:
                        str = "0123456789";
                        break;
                    case RegularType.big_en:
                        str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                        break;
                    case RegularType.smail_en:
                        str = "abcdefghijklmnopqrstuvwxyz";
                        break;
                    case RegularType.special:
                        str = "!@#$%^&*";
                        break;
                    case RegularType.notspecial:
                        str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                        break;
                    case RegularType.base58:
                        str = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
                        break;
                }
                var next = new Random();
                var builder = new StringBuilder();
                for (var i = 0; i < length; i++)
                {
                    builder.Append(str[next.Next(0, str.Length)]);
                }
                return builder.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }


        /// <summary>
        /// 檢查身分證字號
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public static bool checkId(string user_id)
        {
            try
            {
                int[] uid = new int[10]; //數字陣列存放身分證字號用
                int chkTotal; //計算總和用
                if (user_id.Length == 10) //檢查長度
                {
                    user_id = user_id.ToUpper(); //將身分證字號英文改為大寫

                    if (int.TryParse(user_id.Substring(1, 9), out int _idint))
                    {
                        //將輸入的值存入陣列中
                        for (int i = 1; i < user_id.Length; i++)
                        {
                            uid[i] = Convert.ToInt32(user_id.Substring(i, 1));
                        }
                    }
                    else
                    {
                        return false;
                    }

                    //將開頭字母轉換為對應的數值
                    switch (user_id.Substring(0, 1).ToUpper())
                    {
                        case "A": uid[0] = 10; break;
                        case "B": uid[0] = 11; break;
                        case "C": uid[0] = 12; break;
                        case "D": uid[0] = 13; break;
                        case "E": uid[0] = 14; break;
                        case "F": uid[0] = 15; break;
                        case "G": uid[0] = 16; break;
                        case "H": uid[0] = 17; break;
                        case "I": uid[0] = 34; break;
                        case "J": uid[0] = 18; break;
                        case "K": uid[0] = 19; break;
                        case "L": uid[0] = 20; break;
                        case "M": uid[0] = 21; break;
                        case "N": uid[0] = 22; break;
                        case "O": uid[0] = 35; break;
                        case "P": uid[0] = 23; break;
                        case "Q": uid[0] = 24; break;
                        case "R": uid[0] = 25; break;
                        case "S": uid[0] = 26; break;
                        case "T": uid[0] = 27; break;
                        case "U": uid[0] = 28; break;
                        case "V": uid[0] = 29; break;
                        case "W": uid[0] = 32; break;
                        case "X": uid[0] = 30; break;
                        case "Y": uid[0] = 31; break;
                        case "Z": uid[0] = 33; break;
                    }
                    //檢查第一個數值是否為1.2(判斷性別)
                    if (uid[1] == 1 || uid[1] == 2)
                    {
                        chkTotal = (uid[0] / 10 * 1) + (uid[0] % 10 * 9);

                        int k = 8;
                        for (int j = 1; j < 9; j++)
                        {
                            chkTotal += uid[j] * k;
                            k--;
                        }

                        chkTotal += uid[9];

                        if (chkTotal % 10 != 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 確認是否符合規格
        /// </summary>
        /// <returns></returns>
        public static bool CheckTxt(RegularModel model)
        {
            try
            {
                var regexTxt = "";
                //數字驗證
                if (model.number) regexTxt += @"(?=.*\d)";
                if (!model.bigorsmill)
                {
                    //小寫驗證
                    if (model.smillEngilsh) regexTxt += "(?=.*[a-z])";
                    //大寫驗證
                    if (model.bigEnglish) regexTxt += "(?=.*[A-Z])";
                }
                else
                {
                    regexTxt += "(?=.*[a-zA-Z])";
                }
                //特殊字串
                if (model.special) regexTxt += @"(?=.*\W)";
                //長度驗證
                regexTxt += ".{" + model.minLin + "," + model.maxLine + "}";
                regexTxt = $@"^{regexTxt}$";
                Regex regex = new Regex(regexTxt);
                return regex.IsMatch(model.txt);

            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 驗證模型
        /// </summary>
        public class RegularModel
        {
            /// <summary>
            /// 最小字長 預設1
            /// </summary>
            public int minLin { get; set; } = 1;
            /// <summary>
            /// 最大長度 預設100
            /// </summary>
            public int maxLine { get; set; } = 100;
            /// <summary>
            /// 是否需要數字 預設要
            /// </summary>
            public bool number { get; set; } = true;
            /// <summary>
            /// 大寫或小寫 << false = and  , true = or >> 預設大小寫都需要
            /// </summary>
            public bool bigorsmill { get; set; } = false;

            /// <summary>
            /// 是否需要大寫 預設要
            /// </summary>
            public bool bigEnglish { get; set; } = true;
            /// <summary>
            /// 是否需要小寫 預設要
            /// </summary>
            public bool smillEngilsh { get; set; } = true;
            /// <summary>
            /// 是否需要特殊字元 預設要
            /// </summary>
            public bool special { get; set; } = true;
            /// <summary>
            /// 驗證文字
            /// </summary>
            public string txt { get; set; }

        }

        /// <summary>
        /// 檔名規則
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>是否符合規則</returns>
        public static bool FileNameRule(string filename)
        {
            string[] vs = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|", "#", "{", "}", "%", "~", "&" };
            return !vs.Any(filename.Contains);
        }
    }
}

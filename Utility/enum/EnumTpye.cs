using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utility
{
    public static class EnumTpye
    {

        /// <summary>
        /// 取的Enum  int 轉成 string 
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static string GetEnumNumberToSting(object T)
        {
            try
            {
                return ((int)T).ToString();
            }
            catch 
            {
                return "";
            }
        }

        /// <summary>
        /// 取的Enum  int 
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static int GetEnumNumberToInt(object T)
        {
            try
            {
                return ((int)T);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        ///  取的Enum 屬性名稱
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static string GetEnumName(object T)
        {
            try
            {
                return (T).ToString();
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// 取Description的值
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static string GetEnumDescription(object T)
        {
            FieldInfo fi = T.GetType().GetField(T.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
            typeof(DescriptionAttribute), false);
            if (attributes.Length > 0) return attributes[0].Description;
            else return "未設置Description";
           
        }
        /// <summary>
        /// 用Name找enum 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T GetEnum<T>(string str) 
        {
            try
            {
                var item = (T)Enum.Parse(typeof(T), str);
                return item;
            }
            catch (Exception )
            {
                return (T)Enum.Parse(typeof(T), "99");
            }
        }
        /// <summary>
        /// 用Int找enum 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T GetEnum<T>(int str)
        {
            try
            {
                var item = (T)Enum.Parse(typeof(T), str.ToString());
                return item;
            }
            catch (Exception)
            {
                return (T)Enum.Parse(typeof(T), "99");
            }
        }
    }
}

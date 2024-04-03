using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Management
{
    public static class SessionExtensions
    {
        /// <summary>
        /// 設定儲存session
        /// </summary>
        /// <param name="session">session 本體不用特別設定</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        /// <summary>
        /// 取得Session
        /// </summary>
        /// <typeparam name="T">取得出來的模型</typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
        /// <summary>
        /// 清除 session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        public static void RemoveObjectFromJson(this ISession session, string key)
        {
            session.Remove(key);
        }



    }
}

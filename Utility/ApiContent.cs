using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Utility
{
    /// <summary>
    /// post API公版
    /// </summary>
    public class ApiContent
    {
        public static T postApi<T>(string url, string requestJson, string ContentType)
        {
            using (var client = new HttpClient(setHttpNotSafeSSl()))
            {
                HttpContent httpContent = new StringContent(requestJson);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
                var result = client.PostAsync(url, httpContent).Result.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(result))
                {
                    var item = JsonDeserializeObject<T>(result);
                    return item;
                }
                return default(T);
            }
        }

        public static T JsonDeserializeObject<T>(string str)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        static HttpClientHandler setHttpNotSafeSSl()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
               (httpRequestMessage, cert, cetChain, policyErrors) =>
               {
                   return true;
               };
            return handler;
        }
    }
}

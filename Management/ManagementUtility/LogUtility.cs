using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static Management.Models.LogUtilityModel;

namespace Management.ManagementUtility
{
    public class LogUtility
    {
        /// <summary>
        /// Log分析
        /// </summary>
        /// <param name="before">前一次</param>
        /// <param name="after">這次</param>
        /// <param name="tableName"></param>
        /// <param name="beforeAction2">i/u/d</param>
        public static List<string> LogAnalyze(string before, string after, string tableName, string beforeAction2 = "", string afterAction2 = "")
        {
            switch (tableName.ToLower())
            {
                case "webnews": return NewsAnalyze(before, after);
                case "weblevel": return WebLevelAnalyze(before, after, beforeAction2, afterAction2);
                case "sysuser": return sysUserAnalyze(before, after);
                default: return null;
            }
        }
        #region News
        static List<string> NewsAnalyze(string before, string after)
        {
            List<string> msg = new List<string>();
            try
            {
                var beforeModel = JsonConvert.DeserializeObject<NewsAnalyzeModel>(before, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
                var afterModel = JsonConvert.DeserializeObject<NewsAnalyzeModel>(after, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });

                //異動檔案/圖片
                var strfileinfo = JsonConvert.SerializeObject(beforeModel.fileinfo);
                var strafterModel = JsonConvert.SerializeObject(afterModel.fileinfo);
                if (strfileinfo != strafterModel)
                {
                    msg.Add("異動檔案/圖片");
                }
                //異動發布資訊：單位/發布日期/下架日期
                if (beforeModel?.wEBNews?.departmentID != afterModel?.wEBNews?.departmentID ||
                    beforeModel?.wEBNews?.startDate != afterModel?.wEBNews?.startDate ||
                    beforeModel?.wEBNews?.endDate != afterModel?.wEBNews?.endDate
                    )
                {
                    msg.Add("異動發布資訊");
                }
                //異動狀態為：發布/不公開/送審/退件
                if (beforeModel?.wEBNews?.isEnable != afterModel?.wEBNews?.isEnable)
                {
                    msg.Add($"異動狀態:{NewsIsEnableString(beforeModel?.wEBNews?.isEnable)}->{NewsIsEnableString(afterModel?.wEBNews?.isEnable)}");
                }
                //異動資料內容
                var foxbModel = beforeModel;
                var foxaModel = afterModel;
                foxNewsAnalyze(ref foxbModel);
                foxNewsAnalyze(ref foxaModel);
                var strfoxbModel = JsonConvert.SerializeObject(foxbModel);
                var strfoxaModel = JsonConvert.SerializeObject(foxaModel);

                if (strfoxbModel != strfoxaModel)
                {
                    msg.Add("異動資料內容");
                }

                return msg;
            }
            catch (Exception ex)
            {
                Utility.LogExpansion.Write("D:\\Log",ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 整理 將非異動資料內容的內容統一，避免出錯
        /// </summary>
        /// <param name="newsAnalyzeModel"></param>
        /// <returns></returns>
        static void foxNewsAnalyze(ref NewsAnalyzeModel newsAnalyzeModel)
        {
            newsAnalyzeModel.fileinfo = null;
            newsAnalyzeModel.wEBNews.wEBNewsSN = 0;
            newsAnalyzeModel.wEBNews.departmentID = null;
            newsAnalyzeModel.wEBNews.startDate = "";
            newsAnalyzeModel.wEBNews.endDate = "";
            newsAnalyzeModel.wEBNews.isEnable = "0";
            newsAnalyzeModel.wEBNews.createdDate = "";
            newsAnalyzeModel.wEBNews.createdUserID = "";
            newsAnalyzeModel.wEBNews.processDate = "";
            newsAnalyzeModel.wEBNews.processUserID = "";
        }
        #endregion
        #region WebLevel
        static List<string> WebLevelAnalyze(string before, string after, string beforeAction2, string afterAction2)
        {

            try
            {
                List<string> msg = new List<string>();
                var beforeModel = JsonConvert.DeserializeObject<WebLevelAnalyzeModel>(before, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
                var afterModel = JsonConvert.DeserializeObject<WebLevelAnalyzeModel>(after, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
                //異動檔案/圖片
                if (JsonConvert.SerializeObject(beforeModel.fileinfo) != JsonConvert.SerializeObject(afterModel.fileinfo))
                {
                    msg.Add("異動檔案/圖片");
                }
                //異動發布資訊：發布日期/下架日期
                if (
                     beforeModel?.data?.startDate != afterModel?.data?.startDate ||
                     beforeModel?.data?.endDate != afterModel?.data?.endDate ||
                     beforeModel?.data?.departmentID != afterModel?.data?.departmentID
                    )
                {
                    msg.Add("異動發布資訊");
                }
                //異動狀態為：發布/不公開/送審/退件
                if ((beforeModel?.data?.isEnable != afterModel?.data?.isEnable))
                {
                    msg.Add($"異動狀態:{NewsIsEnableString(beforeModel?.data?.isEnable)}->{NewsIsEnableString(afterModel?.data?.isEnable)}");
                }
                //異動資料內容
                var foxbModel = beforeModel;
                var foxaModel = afterModel;
                foxWebLevelAnalyze(ref foxbModel);
                foxWebLevelAnalyze(ref foxaModel);

                if (JsonConvert.SerializeObject(foxbModel) != JsonConvert.SerializeObject(foxaModel))
                {
                    msg.Add("異動資料內容");
                }

                return msg;
            }
            catch (Exception ex)
            {
                Utility.LogExpansion.Write("D:\\Log", ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 整理 將非異動資料內容的內容統一，避免出錯
        /// </summary>
        /// <param name="WebLevelAnalyze"></param>
        /// <returns></returns>
        static void foxWebLevelAnalyze(ref WebLevelAnalyzeModel webLevelAnalyzeModel)
        {
            if (webLevelAnalyzeModel.fileinfo != null && webLevelAnalyzeModel.data != null)
            {
                webLevelAnalyzeModel.fileinfo = null;
                webLevelAnalyzeModel.data.webLevelSN = 0;
                webLevelAnalyzeModel.data.startDate = "";
                webLevelAnalyzeModel.data.endDate = "";
                webLevelAnalyzeModel.data.isEnable = "0";
                webLevelAnalyzeModel.data.processDate = "";
                webLevelAnalyzeModel.data.processUserID = "";
            }
        }
        #endregion
        #region sysUser
        static List<string> sysUserAnalyze(string before, string after)
        {

            List<string> msg = new List<string>();
            var beforeModel = JsonConvert.DeserializeObject<sysUserAnalyzeModel>(before, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            var afterModel = JsonConvert.DeserializeObject<sysUserAnalyzeModel>(after, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None });
            //異動發布資訊：發布日期/下架日期
            if ((beforeModel.data != null && afterModel.data != null) && beforeModel.data.disableDate != afterModel.data.disableDate)
            {
                msg.Add("異動停用日期");
            }
            //異動狀態為：發布/不公開/送審/退件
            if ((beforeModel.data != null && afterModel.data != null) && beforeModel.data.userSatus != afterModel.data.userSatus)
            {
                msg.Add($"異動狀態:{NewsIsEnableString(beforeModel.data.userSatus)}->{NewsIsEnableString(afterModel.data.userSatus)}");
            }
            //異動資料內容
            var foxbModel = beforeModel;
            var foxaModel = afterModel;
            foxsysUserAnalyze(ref foxbModel);
            foxsysUserAnalyze(ref foxaModel);
            if (!object.Equals(foxbModel, foxaModel))
            {
                msg.Add("異動資料內容");
            }

            return msg;

        }
        /// <summary>
        /// 整理 將非異動資料內容的內容統一，避免出錯
        /// </summary>
        /// <param name="WebLevelAnalyze"></param>
        /// <returns></returns>
        static void foxsysUserAnalyze(ref sysUserAnalyzeModel webLevelAnalyzeModel)
        {
            if (webLevelAnalyzeModel.data != null)
            {
                webLevelAnalyzeModel.data.sysUserSN = 0;
                webLevelAnalyzeModel.data.userSatus = "";
                webLevelAnalyzeModel.data.disableDate = "";
                webLevelAnalyzeModel.data.processDate = "";
                webLevelAnalyzeModel.data.processUserID = "";
                webLevelAnalyzeModel.data.processIPAddress = "";
            }
        }
        #endregion


    }
}

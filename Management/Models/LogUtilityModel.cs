using System;
using System.Collections.Generic;

namespace Management.Models
{
    public class LogUtilityModel
    {
        #region News
        public class NewsAnalyzeModel
        {
            public NewsAnalyzeDetailModel wEBNews { get; set; }
            public List<FileinfoAnalyzeModel> fileinfo { get; set; }
            public List<RelatedModel> linkinfo { get; set; }
            public string[] tab { get; set; }
            public List<RelatedModel> keyword { get; set; }
            public string[] whole { get; set; }
            public string[] policy { get; set; }
            public string[] business { get; set; }
            public string[] serve { get; set; }
            public List<RelatedModel> relatedlink { get; set; }
            public List<RelatedModel> relatedvideo { get; set; }
            public List<RelatedModel> relatedmoj { get; set; }
        }
        public class NewsAnalyzeDetailModel
        {
            public int wEBNewsSN { get; set; }
            public string webSiteID { get; set; }
            public int webLevelSN { get; set; }
            public string lang { get; set; }
            public string module { get; set; }
            public string articleType { get; set; }
            public string title { get; set; }
            public string subTitle { get; set; }
            public string description { get; set; }
            public string publishDate { get; set; }
            public string contentText { get; set; }
            public string uRL { get; set; }
            public string target { get; set; }
            public string youtubeID { get; set; }
            public string isTop { get; set; }
            public string departmentID { get; set; }
            public string pageView { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string isEnable { get; set; }
            public string processUserID { get; set; }
            public string processDate { get; set; }
            public string processIPAddress { get; set; }
            public string createdUserID { get; set; }
            public string createdDate { get; set; }
            public string sortOrder { get; set; }
            public string mainSN { get; set; }
            public string pageViewType { get; set; }
            public string tag { get; set; }
            public int? customizeTagSn { get; set; }
            public int? zipCodeSn { get; set; }
            public string keyWord { get; set; }
            public string performance { get; set; }
            public string eProject { get; set; }
            public string serviceObject { get; set; }
            public string otherLangerUrl { get; set; }
            public string statesUrl { get; set; }
            public string sEODescription { get; set; }
            public string sEOKeywords { get; set; }
        }

        #endregion
        #region WebLevel Update
        public class WebLevelAnalyzeModel
        {
            public WebLevelAnalyzeDetailModel data { get; set; }
            public string module { get; set; }
            public List<RelatedModel> customize { get; set; }
            public List<FileinfoAnalyzeModel> fileinfo { get; set; }
        }

        public class WebLevelAnalyzeDetailModel
        {
            public int webLevelSN { get; set; }
            public string webLevelKey { get; set; }
            public int parentSN { get; set; }
            public object webSiteID { get; set; }
            public string lang { get; set; }
            public string weblevelType { get; set; }
            public string module { get; set; }
            public string parameter { get; set; }
            public string title { get; set; }
            public object contentText { get; set; }
            public string fatFooterShow { get; set; }
            public string mainMenuShow { get; set; }
            public object subMemuShow { get; set; }
            public string leftMenuShow { get; set; }
            public string rSSShow { get; set; }
            public int pageView { get; set; }
            public string startDate { get; set; }
            public string endDate { get; set; }
            public object contentHeader { get; set; }
            public object contentFooter { get; set; }
            public string listType { get; set; }
            public string sortMethod { get; set; }
            public string isEnable { get; set; }
            public object processUserID { get; set; }
            public object processDate { get; set; }
            public object processIPAddress { get; set; }
            public object createdUserID { get; set; }
            public object createdDate { get; set; }
            public object sortOrder { get; set; }
            public object mainSN { get; set; }
            public object statesUrl { get; set; }
            public object templateValue { get; set; }
            public object description { get; set; }
            public object condition { get; set; }
            public object sEODescription { get; set; }
            public object sEOKeywords { get; set; }
            public string departmentID { get; set; }

        }
        #endregion
        #region  sysUser Insert or Update
        public class sysUserAnalyzeModel
        {
            public sysUserAnalyzeDetailModel data { get; set; }
        }
        public class sysUserAnalyzeDetailModel
        {
            public int sysUserSN { get; set; }
            public object aDUserID { get; set; }
            public string userID { get; set; }
            public string userName { get; set; }
            public object engName { get; set; }
            public object nickName { get; set; }
            public object gender { get; set; }
            public object p_w_d { get; set; }
            public string userSatus { get; set; }
            public object tel { get; set; }
            public object mobile { get; set; }
            public string email { get; set; }
            public object disableDate { get; set; }
            public object lastLoginDate { get; set; }
            public object pwdLastUpdate { get; set; }
            public object errLoginnum { get; set; }
            public object empID { get; set; }
            public object jobTitle { get; set; }
            public object officePhone { get; set; }
            public object photo { get; set; }
            public string departmentID { get; set; }
            public object processUserID { get; set; }
            public object processDate { get; set; }
            public object processIPAddress { get; set; }
            public object dateCreated { get; set; }
            public object sortOrder { get; set; }
        }
        #endregion

        #region 通用
        public class RelatedModel
        {
            public string val { get; set; }
            public string txt { get; set; }
        }
        public class FileinfoAnalyzeModel
        {
            public string groupID { get; set; }
            public object fileOriginName { get; set; }
            public int fileSize { get; set; }
            public object fileExt { get; set; }
            public string fileNewName { get; set; }
            public object filePath { get; set; }
            public string fileTitle { get; set; }
            public int fileSort { get; set; }
            public object webFileID { get; set; }
            public object isEnable { get; set; }
            public bool isImage { get; set; }
            public string lan { get; set; }
        }
        public static string NewsIsEnableString(string isEnable)
        {
            switch (isEnable)
            {
                case "0": return "停用";
                case "1": return "啟用";
                case "3": return "送審";
                case "-2": return "退審稿";
                case "-99": return "刪除";
                default: return "";
            }
        } 
        #endregion

    }
}

using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utility.Model;
using static Services.ModaMailBox.MailBox;
using static Utility.Files;

namespace Services.CheckModel
{
    public class CheckedData
    {
        /// <summary>
        /// 節點維護資料正確性檢查
        /// </summary>
        /// <param name="webLevelM"></param>
        /// <param name="webLevelD"></param>
        /// <param name="act">Utility.SysConst.Action</param>
        /// <param name="ckEditorurl">ref List<string></param>
        /// <returns></returns>
        public static CheckedModel CheckWebLevelData(ref WebLevel webLevel, Utility.SysConst.Action act, ref List<string> ckEditorurl)
        {
            CheckedModel checkedModel = new CheckedModel();

            List<string> missData = new List<string>();
            //節點代號檢查
            if (string.IsNullOrWhiteSpace(webLevel.WebLevelKey)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.WebLevel.WebLevelKey));
            if (IsChines(webLevel.WebLevelKey.Trim())) missData.Add(Utility.sysConstTable.field.Error.RequiredChines(Utility.sysConstTable.field.WebLevel.WebLevelKey));
            //節點名稱檢查
            if (string.IsNullOrWhiteSpace(webLevel.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.WebLevel.Title));
            //模組檢查
            string[] s = new string[] { Utility.sysConstTable.field.WebLevel.ModuleType, Utility.sysConstTable.field.WebLevel.Module };
            if (string.IsNullOrWhiteSpace(webLevel.Module)) missData.Add(Utility.sysConstTable.field.Error.Required(s));
            //發布單位檢查
            if (string.IsNullOrWhiteSpace(webLevel.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.WebLevel.DepartmentID));
            //
            if (webLevel.WeblevelType == Utility.EnumTpye.GetEnumNumberToSting(Utility.EnumWeblevelType.WebLevelManagment))
            {
                bool chk = false;
                if (webLevel.Module != Utility.SysConst.Module.CP.ToString())
                {
                    if (webLevel.Module == Utility.SysConst.Module.PAGELIST.ToString() || webLevel.Module == Utility.SysConst.Module.NEWS.ToString()) chk = true;
                }
                if (chk && string.IsNullOrWhiteSpace(webLevel.ListType))
                {
                    missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.WebLevel.ListType));
                }
            }
            //ckeditor 檢查
            if (!string.IsNullOrWhiteSpace(webLevel.ContentHeader))
            {
                missData.AddRange(Chkckeditor(webLevel.ContentHeader, ref ckEditorurl));
            }
            if (!string.IsNullOrWhiteSpace(webLevel.ContentFooter))
            {
                missData.AddRange(Chkckeditor(webLevel.ContentFooter, ref ckEditorurl));
            }


            if (act == Utility.SysConst.Action.Edit)
            {
                
                if (webLevel.WeblevelType == Utility.EnumTpye.GetEnumNumberToSting(Utility.EnumWeblevelType.WebLevelManagment))
                {
                    //顯示於FatFooter
                    if (string.IsNullOrWhiteSpace(webLevel.FatFooterShow)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.WebLevel.FatFooterShow));
                    //顯示於主選單
                    if (string.IsNullOrWhiteSpace(webLevel.MainMenuShow)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.WebLevel.MainMenuShow));
                    //顯示左邊清單
                    if (string.IsNullOrWhiteSpace(webLevel.LeftMenuShow)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.WebLevel.LeftMenuShow));
                    //RSS
                    if (webLevel.Module == Utility.SysConst.Module.NEWS.ToString())
                    {
                        if (string.IsNullOrWhiteSpace(webLevel.RSSShow)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.WebLevel.RSSShow));
                    }
                }
            }

            if (missData.Count == 0)
            {
                webLevel.Title = webLevel.Title.Trim();
            }
            else
            {
                checkedModel.chk = false;
                checkedModel.error = string.Join("<br/>", missData);
            }

            return checkedModel;

        }
        /// <summary>
        /// 確認 WebNews 資料正確性
        /// </summary>
        /// <param name="wEBNews"></param>
        /// <param name="commonFileModels"></param>
        /// <returns></returns>
        public static CheckedModel CheckedWebNews(ref WEBNews wEBNews, ref List<string> ckEditorurl, List<CommonFileModel> commonFileModels = null, List<WebLink> linkinfo = null, List<WEBNewsTranscript> wEBNewsTranscripts = null, List<WEBNewsExtend> webNewsExtend = null  )
        {
            CheckedModel checkedModel = new CheckedModel();

            Utility.SysConst.Module enumModule;
            Enum.TryParse(wEBNews.Module, out enumModule);
            switch (enumModule)
            {
                case Utility.SysConst.Module.CP:
                case Utility.SysConst.Module.NEWS:
                    checkedModel = News.CheckData(ref wEBNews, ref ckEditorurl, commonFileModels, linkinfo, wEBNewsTranscripts, webNewsExtend);
                    break;
                case Utility.SysConst.Module.BANNER:
                    checkedModel = Banner.CheckData(ref wEBNews, commonFileModels);
                    break;
                case Utility.SysConst.Module.LINK:
                    checkedModel = Link.CheckData(ref wEBNews, commonFileModels);
                    break;
                case Utility.SysConst.Module.MEDIA:
                    checkedModel = Media.CheckData(ref wEBNews, commonFileModels);
                    break;
                case Utility.SysConst.Module.TAB:
                    checkedModel = Tab.CheckData(ref wEBNews);
                    break;
                case Utility.SysConst.Module.TEXT:
                    checkedModel = Text.CheckData(ref wEBNews);
                    break;
                case Utility.SysConst.Module.IMGTEXT:
                    checkedModel = ImgText.CheckData(ref wEBNews, commonFileModels);
                    break;
                case Utility.SysConst.Module.BANNER2:
                    checkedModel = Banner2.CheckData(ref wEBNews, commonFileModels);
                    break;
                case Utility.SysConst.Module.Schedule:
                    checkedModel = Schedule.CheckData(ref wEBNews, webNewsExtend);
                    break;
                case Utility.SysConst.Module.Bilingual:
                    checkedModel = Bilingual.CheckData(ref wEBNews);
                    break;
            }
            return checkedModel;
        }
        /// <summary>
        /// 檢查模組NEWS
        /// </summary>
        public class News
        {
            /// <summary>
            /// 模組NEW檢查欄位，ArticleType分流
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews, ref List<string> ckEditorurl, List<CommonFileModel> commonFileModels, List<WebLink> linkinfo, List<WEBNewsTranscript> wEBNewsTranscripts = null, List<WEBNewsExtend> webNewsExtend = null)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();
                switch (Utility.EnumTpye.GetEnum<Utility.SYSConst.Content.Type>(wEBNews.ArticleType))
                {
                    case Utility.SYSConst.Content.Type.PAGE: News.ChkDataByArticleType0(ref missData, ref wEBNews, ref ckEditorurl, commonFileModels, linkinfo); break;
                    case Utility.SYSConst.Content.Type.LINK: News.ChkDataByArticleType2(ref missData, ref wEBNews, commonFileModels); break;
                    case Utility.SYSConst.Content.Type.DOWNLOAD: News.ChkDataByArticleType1(ref missData, ref wEBNews, commonFileModels); break;
                    case Utility.SYSConst.Content.Type.Transcript: News.ChkDataByArticleType10(ref missData, ref wEBNews, commonFileModels, linkinfo, wEBNewsTranscripts); break;
                }
                //判斷擴充套件
                var relatedUrl = new List<string>() { "relatedlink", "relatedmoj" };
                var wEBNewsExtends = webNewsExtend.Where(x => relatedUrl.Contains(x.GroupID)).ToList();
                var chk = true;
                foreach (var url in wEBNewsExtends?.Select(x => x.Column_1)) {
                    if (chk)
                    {
                        chk = CheckUrl(url, ref missData);
                    }
                }
                if (missData.Count > 0)
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;
            }

            /// <summary>
            /// 模組NEWS的ArticleType=0(網頁(一般資料格式))欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            /// <param name="chkFileModels"></param>
            public static void ChkDataByArticleType0(ref List<string> missData, ref WEBNews wEBNews, ref List<string> ckEditorurl, List<CommonFileModel> commonFileModels = null, List<WebLink> linkinfo = null, List<ChkFileModel> chkFileModels = null)
            {

                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.Page.Title));
                //內容檢查
                if (!string.IsNullOrWhiteSpace(wEBNews.ContentText))
                {
                    //ckeditor 檢查
                    missData.AddRange(Chkckeditor(wEBNews.ContentText, ref ckEditorurl));
                }
                //部門檢查
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.Page.SysDepartment));

                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                //相關連結檢查
                foreach (var item in commonFileModels)
                {
                    if (string.IsNullOrWhiteSpace(item.fileTitle))
                    {
                        missData.Add("[相關檔案/相關圖片]標題及Url必填");
                        break;
                    }
                }


                //相關連結檢查
                foreach (var item in linkinfo)
                {
                    if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.URL))
                    {
                        missData.Add("[相關連結]標題及Url必填");
                        break;
                    }
                }

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                }
            }

            /// <summary>
            /// 模組NEWS的ArticleType=2(連結(URL 連接式))欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            /// <param name="chkFileModels"></param>
            public static void ChkDataByArticleType2(ref List<string> missData, ref WEBNews wEBNews, List<CommonFileModel> commonFileModels = null, List<ChkFileModel> chkFileModels = null)
            {
                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.LINK.Title));
                //連結檢查
                if (string.IsNullOrWhiteSpace(wEBNews.URL)) {
                    missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.LINK.URL)); } 
                else {
                    CheckUrl(wEBNews.URL , ref missData);
                }
                //部門檢查
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.Page.SysDepartment));
                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

               

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    wEBNews.URL = wEBNews.URL.Trim();
                }
            }

            /// <summary>
            /// 模組NEWS的ArticleType=1(檔案(檔案下載式))欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            public static void ChkDataByArticleType1(ref List<string> missData, ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {

                #region 標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.DownLoad.Title));
                #endregion

                #region 檔案檢查
                List<ChkFileModel> chkFileModels = new List<ChkFileModel>();

                Utility.SysConst.Module module = Utility.SysConst.Module.NEWS;
                Enum.TryParse(wEBNews.Module, out module);

                if (module == Utility.SysConst.Module.NEWS)
                {
                    chkFileModels.Add(new ChkFileModel() { GroupID = Utility.WebFileGroupID.News.File.ToString(), FldDesc = Utility.sysConstTable.field.news.DownLoad.File });
                }
                else if (module == Utility.SysConst.Module.CP)
                {
                    chkFileModels.Add(new ChkFileModel() { GroupID = Utility.WebFileGroupID.CP.File.ToString(), FldDesc = Utility.sysConstTable.field.news.DownLoad.File });
                }

                CheckFile(ref missData, commonFileModels, chkFileModels);
                #endregion

                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.Page.SysDepartment));

                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                }
            }
            /// <summary>
            /// 模組NEWS的ArticleType=10(檔案(逐字稿))欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            /// <param name="linkinfo"></param>
            /// <param name="wEBNewsTranscripts"></param>
            public static void ChkDataByArticleType10(ref List<string> missData, ref WEBNews wEBNews, List<CommonFileModel> commonFileModels = null, List<WebLink> linkinfo = null, List<WEBNewsTranscript> wEBNewsTranscripts = null)
            {

                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.Page.Title));

                //部門檢查
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.news.Page.SysDepartment));

                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                //相關連結檢查
                foreach (var item in commonFileModels)
                {
                    if (string.IsNullOrWhiteSpace(item.fileTitle))
                    {
                        missData.Add("[相關檔案/相關圖片]標題及Url必填");
                        break;
                    }
                }
                //相關連結檢查
                foreach (var item in linkinfo)
                {
                    if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.URL))
                    {
                        missData.Add("[相關連結]標題及Url必填");
                        break;
                    }
                }
                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                }
                if (wEBNewsTranscripts == null)
                {
                    missData.Add("請上傳MD檔");
                }
                else
                {
                    if (wEBNewsTranscripts.Count() == 0)
                    {
                        missData.Add("請上傳MD檔");
                    }
                }
            }

        }

        /// <summary>
        /// 檢查模組IMGTEXT
        /// </summary>
        public class ImgText
        {
            /// <summary>
            /// 檢查模組IMGTEXT
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();
                //標題
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.ImgText.Title));
                //內容
                if (string.IsNullOrWhiteSpace(wEBNews.ContentText)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.ImgText.Content));

                //圖檔
                List<ChkFileModel> chkFileModels = new List<ChkFileModel>() {
                    new ChkFileModel(){ GroupID=Utility.WebFileGroupID.ImgText.Img,FldDesc=Utility.sysConstTable.field.ImgText.Img}
                 };
                CheckFile(ref missData, commonFileModels, chkFileModels);
                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                }
                else
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }

                return checkedModel;

            }
        }

        /// <summary>
        /// 檢查模組Banner
        /// </summary>
        public class Banner
        {
            /// <summary>
            /// 檢查模組Banner，ArticleType分流
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();

                int iArticleType = 0;
                int.TryParse(wEBNews.ArticleType, out iArticleType);
                switch (iArticleType)
                {
                    case (int)Utility.SYSConst.Content.Type.DOWNLOAD:
                        Banner.ChkDataByArticleType1(ref missData, ref wEBNews, commonFileModels);
                        break;
                    case (int)Utility.SYSConst.Content.Type.IFRAME:
                        Banner.ChkDataByArticleType3(ref missData, ref wEBNews);
                        break;

                }

                if (missData.Count > 0)
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;
            }

            /// <summary>
            /// 模組Banner的ArticleType=1(附件、(即圖片的意思))欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            public static void ChkDataByArticleType1(ref List<string> missData, ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {
                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Img.Title));
                //連結檢查
                if (string.IsNullOrWhiteSpace(wEBNews.URL)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Img.URL));
                //發布單位檢查
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.SysDepartment));

                #region 大、小圖檢查
                List<ChkFileModel> chkFileModels = new List<ChkFileModel>() {
                        new ChkFileModel(){ GroupID=Utility.WebFileGroupID.Banner.BigImg,FldDesc=Utility.sysConstTable.field.banner.Img.Big},
                        new ChkFileModel(){ GroupID=Utility.WebFileGroupID.Banner.SmallImg,FldDesc=Utility.sysConstTable.field.banner.Img.Small}
                    };
                CheckFile(ref missData, commonFileModels, chkFileModels);
                #endregion


                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);


                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    wEBNews.URL = wEBNews.URL.Trim();
                }
            }
            /// <summary>
            /// 模組Banner的ArticleType=3(iframe)欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            public static void ChkDataByArticleType3(ref List<string> missData, ref WEBNews wEBNews)
            {
                //標題
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Media.Title));
                //iframe連結
                if (string.IsNullOrWhiteSpace(wEBNews.URL)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Media.URL));
                //發布單位檢查
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.SysDepartment));
                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    wEBNews.URL = wEBNews.URL.Trim();
                }
            }


        }

        /// <summary>
        /// 檢查模組Banner2 有圖片跟影片代碼
        /// </summary>
        public class Banner2
        {
            /// <summary>
            /// 檢查模組Banner，ArticleType分流
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();

                int iArticleType = 0;
                int.TryParse(wEBNews.ArticleType, out iArticleType);
                switch (iArticleType)
                {
                    case (int)Utility.SYSConst.Content.Type.DOWNLOAD:
                        Banner2.ChkDataByArticleType1(ref missData, ref wEBNews, commonFileModels);
                        break;
                    case (int)Utility.SYSConst.Content.Type.IFRAME:
                        Banner2.ChkDataByArticleType3(ref missData, ref wEBNews);
                        break;

                }

                if (missData.Count > 0)
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;
            }

            /// <summary>
            /// 模組Banner的ArticleType=1(附件、(即圖片的意思))欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            public static void ChkDataByArticleType1(ref List<string> missData, ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {
                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Img.Title));
                //連結檢查
                if (string.IsNullOrWhiteSpace(wEBNews.URL)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Img.URL));

                #region 檢查
                List<ChkFileModel> chkFileModels = new List<ChkFileModel>() {
                        new ChkFileModel(){ GroupID=Utility.WebFileGroupID.Banner.BigImg,FldDesc=Utility.sysConstTable.field.banner.Img.Big}
                    };
                CheckFile(ref missData, commonFileModels, chkFileModels);
                #endregion
                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);
                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    wEBNews.URL = wEBNews.URL.Trim();
                }
            }
            /// <summary>
            /// 模組Banner的ArticleType=3(youtube代碼)欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            public static void ChkDataByArticleType3(ref List<string> missData, ref WEBNews wEBNews)
            {
                //標題
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Media.Title));
                //youtube代碼
                if (string.IsNullOrWhiteSpace(wEBNews.URL)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Media.URL));

                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    wEBNews.URL = wEBNews.URL.Trim();
                }
            }


        }


        /// <summary>
        /// 檢查模組Link
        /// </summary>
        public class Link
        {
            /// <summary>
            /// 模組LINK檢查欄位，ArticleType分流
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();

                int iArticleType = 0;
                int.TryParse(wEBNews.ArticleType, out iArticleType);
                switch (iArticleType)
                {
                    case (int)Utility.SYSConst.Content.Type.LINK:
                        ChkDataByArticleType2(ref missData, ref wEBNews, commonFileModels);
                        break;
                    case (int)Utility.SYSConst.Content.Type.WEBLEVELMSN:
                        ChkDataByArticleType4(ref missData, ref wEBNews);
                        break;

                }

                if (missData.Count > 0)
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;
            }
            /// <summary>
            /// 模組LINK的ArticleType=2(連結(URL 連接式))欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            /// <param name="commonFileModels"></param>
            public static void ChkDataByArticleType2(ref List<string> missData, ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {
                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.link.Link.Title));
                //連結檢查
                if (string.IsNullOrWhiteSpace(wEBNews.URL)) {
                    missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.link.Link.URL)); } 
                else {
                    CheckUrl(wEBNews.URL, ref missData);
                }
                //發布單位檢查
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.SysDepartment));
                #region 圖檢查
                List<ChkFileModel> chkFileModels = new List<ChkFileModel>() {
                        new ChkFileModel(){ GroupID=Utility.WebFileGroupID.Link.Img,FldDesc=Utility.sysConstTable.field.link.Link.Img}
                    };
                CheckFile(ref missData, commonFileModels, chkFileModels);
                #endregion

                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);


                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    wEBNews.URL = wEBNews.URL.Trim();
                }
            }
            /// <summary>
            /// 模組LINK的ArticleType=4(站內連結(URL紀錄WEBLevelMSN))欄位檢查
            /// </summary>
            /// <param name="missData"></param>
            /// <param name="wEBNews"></param>
            public static void ChkDataByArticleType4(ref List<string> missData, ref WEBNews wEBNews)
            {
                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Media.Title));
                //站內連結檢查
                if (string.IsNullOrWhiteSpace(wEBNews.URL)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.banner.Media.URL));
                //發布單位檢查
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.SysDepartment));
                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);


                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    wEBNews.URL = wEBNews.URL.Trim();
                }
            }

        }
        /// <summary>
        /// 檢查模組TAB
        /// </summary>
        public class Tab
        {
            /// <summary>
            /// 檢查模組TAB
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();
                //標題
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Tab.Title));
                //關聯頁
                if (string.IsNullOrWhiteSpace(wEBNews.URL)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Tab.URL));
                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    wEBNews.URL = wEBNews.URL.Trim();
                }
                else
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;


            }
        }

        /// <summary>
        /// 檢查模組MEDIA
        /// </summary>
        public class Media
        {
            /// <summary>
            /// 檢查模組MEDIA
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews, List<CommonFileModel> commonFileModels)
            {
                CheckedModel checkedModel = new CheckedModel();
                int iArticleType = 0;
                int.TryParse(wEBNews.ArticleType, out iArticleType);

                var missData = new List<string>();
                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Media.Title));
                switch (iArticleType)
                {
                    case (int)Utility.SYSConst.Content.Type.DOWNLOAD:
                        List<ChkFileModel> chkFileModels = new List<ChkFileModel>() {
                new ChkFileModel(){ GroupID=Utility.WebFileGroupID.Media.Img,FldDesc=Utility.sysConstTable.field.Media.FldDesc}
            };
                        CheckFile(ref missData, commonFileModels, chkFileModels);
                        break;
                    case (int)Utility.SYSConst.Content.Type.IFRAME:
                        //iframe連結檢查
                        if (string.IsNullOrWhiteSpace(wEBNews.URL)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Media.iURL));
                        break;
                }

                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                    if (!string.IsNullOrWhiteSpace(wEBNews.URL)) wEBNews.URL = wEBNews.URL.Trim();
                }
                else
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;
            }
        }

        /// <summary>
        /// 檢查模組TEXT
        /// </summary>
        public class Text
        {

            /// <summary>
            /// 檢查模組TEXT
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();
                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.Title));
                //發布單位
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.SysDepartment));
                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);

                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                }
                else
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;
            }
        }
        /// <summary>
        /// 檢查模組 Schedule
        /// </summary>
        public class Schedule
        {
            /// <summary>
            /// 檢查模組Schedule
            /// </summary>
            /// <param name="wEBNews"></param>
            /// <returns></returns>
            public static CheckedModel CheckData(ref WEBNews wEBNews, List<WEBNewsExtend> WEBNewsExtend)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();
                //標題檢查
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.Title));
                //行程時間 PublishDate
                if (wEBNews.PublishDate == null) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.PublishDate));
                //發布單位 dep
                if (string.IsNullOrWhiteSpace(wEBNews.DepartmentID)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.SysDepartment));
                //檢查發布日期及下架日期
                missData = CheckSDED(missData, wEBNews);
                
                //相關連結檢查 //檢查首長
                if (WEBNewsExtend?.Count() > 0)
                {
                    if (WEBNewsExtend.Any(x => x.GroupID == "relatedlink" && (string.IsNullOrWhiteSpace(x.Column_1) || (string.IsNullOrWhiteSpace(x.Column_2))))) { missData.Add("[相關連結]標題及Url必填"); }
                    if (!WEBNewsExtend.Any(x => x.GroupID == "chief" )) { missData.Add("首長 必填"); }
                }
                else {
                    missData.Add("首長 必填");
                }
                if (missData.Count == 0)
                {
                    wEBNews.Title = wEBNews.Title.Trim();
                }
                else
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;
            }
        }
        public class Bilingual
        {
            public static CheckedModel CheckData(ref WEBNews wEBNews)
            {
                CheckedModel checkedModel = new CheckedModel();
                var missData = new List<string>();
                if (string.IsNullOrWhiteSpace(wEBNews.Title)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Text.Title));
                if (missData?.Count() > 0)
                {
                    checkedModel.chk = false;
                    checkedModel.error = string.Join("<br/>", missData);
                }
                return checkedModel;
            }
        }


        public static CheckedModel CheckedCategory(ref SysCategory sysCategory)
        {
            CheckedModel checkedModel = new CheckedModel();
            List<string> missData = new List<string>();
            //代號
            if (string.IsNullOrWhiteSpace(sysCategory.Description)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Category.Description));
            //標題
            if (string.IsNullOrWhiteSpace(sysCategory.Value)) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.field.Category.Value));

            if (missData.Count != 0)
            {

                checkedModel.chk = false;
                checkedModel.error = string.Join("<br/>", missData);
            }
            return checkedModel;
        }
        static void CheckFile(ref List<string> missData, List<CommonFileModel> commonFileModels, List<ChkFileModel> chkFileModels)
        {
            foreach (ChkFileModel r in chkFileModels)
            {
                if (!CheckFile(r.GroupID, commonFileModels)) missData.Add(Utility.sysConstTable.field.Error.Required(r.FldDesc));
            }
        }

        static bool CheckFile(string groupID, List<CommonFileModel> commonFileModels)
        {
            if (commonFileModels == null)
            {
                return false;
            }
            else
            {
                if (commonFileModels.Count == 0)
                {
                    return false;
                }
                else
                {
                    if (!commonFileModels.Any(x => x.GroupID == groupID)) return false;
                    return true;
                }
            }
        }
        /// <summary>
        /// 檢查發布日期及下架日期
        /// </summary>
        /// <param name="missData"></param>
        /// <param name="wEBNews"></param>
        /// <returns></returns>
        static List<string> CheckSDED(List<string> missData, WEBNews wEBNews)
        {
            //發布日期檢查
            if (!wEBNews.StartDate.HasValue) missData.Add(Utility.sysConstTable.field.Error.Required(Utility.sysConstTable.Field.StartDate));
            //發布日期下架日期檢查
            if (wEBNews.StartDate.HasValue && wEBNews.EndDate.HasValue)
            {
                if (wEBNews.StartDate > wEBNews.EndDate) { missData.Add(Utility.sysConstTable.field.Error.ErrMsgSDMoreED()); }
            }

            return missData;

        }

        /// <summary>
        /// 檢查file 
        /// </summary>
        public class ChkFileModel
        {
            /// <summary>
            /// WEBGroupID
            /// </summary>
            public string GroupID { get; set; }
            /// <summary>
            /// 顯示欄位值
            /// </summary>
            public string FldDesc { get; set; }


        }
        /// <summary>
        /// 確認ckitor內容
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="ckEditorurl"></param>
        /// <returns></returns>
        static List<string> Chkckeditor(string txt, ref List<string> ckEditorurl)
        {
            var missList = new List<string>();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(txt);
            var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]"); //超連結
            var imgNodes = doc.DocumentNode.SelectNodes("//img[@src]"); //img 
            var tableNodes = doc.DocumentNode.SelectNodes("//table");
            var iframeNodes = doc.DocumentNode.SelectNodes("//iframe");
            //iframe
            //files
            var chkAgain = false;
            if (linkNodes != null)
            {
                foreach (var item in linkNodes)
                {
                    if (item.GetAttributeValue("target", "") == "_blank")
                    {
                        if (!chkAgain)
                        {
                            if (item.GetAttributeValue("title", "").IndexOf("另開新視窗") < 0 && item.GetAttributeValue("title", "").IndexOf("(open in new window)") < 0)
                            {
                                missList.Add($@"超連結{item.GetAttributeValue("href", "")}請title添加(另開新視窗) or (open in new window)");
                                chkAgain = true;
                            }

                        }
                    }
                    if (item.GetAttributeValue("href", "") != "#")
                    {
                        ckEditorurl.Add(item.GetAttributeValue("href", ""));
                    }
                }
            }
            if (imgNodes != null)
            {
                foreach (var item in imgNodes)
                {
                    if (item.GetAttributeValue("alt", "") == null)
                    {
                        if (!chkAgain)
                        {
                            missList.Add($@"圖片{item.GetAttributeValue("src", "")}少添加alt屬性");
                            chkAgain = true;
                        }
                    }
                    ckEditorurl.Add(item.GetAttributeValue("src", ""));
                }
            }
            if (tableNodes != null)
            {
                foreach (var item in tableNodes)
                {
                    var thData = item.SelectNodes("//th");
                    if (thData == null)
                    {
                        if (!chkAgain)
                        {
                            missList.Add($@"Table缺少th");
                            chkAgain = true;
                        }

                    }
                    else
                    {
                        foreach (var itmeth in thData)
                        {
                            if (string.IsNullOrWhiteSpace(itmeth.GetAttributeValue("scope", "")))
                            {
                                if (!chkAgain)
                                {
                                    missList.Add($@"Table th 缺少scope屬性");
                                    chkAgain = true;
                                }
                            }
                        }
                    }
                }
            }
            if (iframeNodes != null)
            {
                if (!chkAgain)
                {
                    foreach (var item in iframeNodes)
                    {
                        if (string.IsNullOrWhiteSpace(item.GetAttributeValue("title", "")))
                        {
                            if (!chkAgain)
                            {
                                missList.Add($@"iframe請添加title");
                                chkAgain = true;
                            }

                        }
                        ckEditorurl.Add(item.GetAttributeValue("src", ""));
                    }
                }
            }

            return missList;
        }

        /// <summary>
        /// 判斷是否有中文字(在 ASCII碼表中，英文的範圍是0-127，而中文是大於127)
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static bool IsChines(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if ((int)text[i] > 127) return true;
            }
            return false;
        }

        /// <summary>
        /// 超連結判斷
        /// </summary>
        /// <param name="url"></param>
        /// <param name="missData"></param>
        static bool CheckUrl(string url, ref List<string> missData)
        {
            if (url.Trim().Substring(0, 1) == "/" || (url.Trim().Length > 3 &&  url.Trim().Substring(0, 4) == "http"))
            {
                return true;
            }
            else {
                missData.Add(@"如有使用超連結，網址開頭部分站內請以/，站外請以http or https  ");
                return false;
            }
        }
    }

    
}

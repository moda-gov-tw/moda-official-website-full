using DBModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Services.Authorization;
using Services.Models;
using Services.Models.WebSite;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using static Utility.Files;

namespace Services.WebManagement
{
    public class WebsiteManagementService
    {
        public static List<SysWebSiteLang> GetSysWebSiteID(string WEBSiteID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(WEBSiteID))
                    {
                        return db.SysWebSiteLang.Where(x => x.WebSiteID == WEBSiteID).ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public static List<SysWebSite> GetSysWebSiteByWebSiteID(string WebSiteID, ref DefaultPager pager)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    var Data = db.SysWebSite.Where(x => 1 == 1);
                    if (!string.IsNullOrWhiteSpace(WebSiteID))
                    {
                        Data = Data.Where(x => x.WebSiteID.Contains(WebSiteID));
                    }
                    Data = Data.Where(x => x.IsEnable != ((int)Utility.SysConst.IsEnable.Code.Del).ToString());

                    var allData = Data.Count();
                    pager.TotalCount = allData;
                    pager.PageIndex = pager.p - 1;
                    var searchData = Data.OrderBy(o => o.SortOrder).Skip((pager.p - 1) * pager.DisplayCount).Take(pager.DisplayCount).ToList();

                    return searchData;
                }
                catch (Exception)
                {
                    return null;
                }
            }

        }

        public static SysWebSite GetSysWebSiteLangByWebSiteID(string WebSiteID)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.SysWebSite.FirstOrDefault(x => x.WebSiteID == WebSiteID);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static bool Create(SysWebSiteLang sysWebSite)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    sysWebSite.WebSiteID= sysWebSite.WebSiteID.Trim().ToUpper();
                    sysWebSite.Title = sysWebSite.Title.Trim();
                    sysWebSite.Description = sysWebSite.Description == null ? "" : sysWebSite.Description.Trim();
                    sysWebSite.GoogleSearchKey = sysWebSite.GoogleSearchKey == null ? "" : sysWebSite.GoogleSearchKey.Trim();
                    if (sysWebSite.SortOrder == 0)
                    {
                        var Data = db.SysWebSite.Where(x => x.IsEnable == "1");
                        var allData = Data.Count();
                        sysWebSite.SortOrder = allData + 1;
                    }

                    db.Add(sysWebSite);
                    db.SaveChanges();
                }
                CreateWeblevel(sysWebSite);
                return true;
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = sysWebSite.ProcessIPAddress,
                    UserID = sysWebSite.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Insert",
                    SourceTable = "SysWebSite",
                    Action = "Create",
                    Controller = "WebsiteManagementService",
                    SourceSN = 0,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }
        /// <summary>
        /// 新增站台:需要建立基本節點
        /// </summary>
        /// <param name="sysWebSite"></param>
        static void CreateWeblevel(SysWebSiteLang sysWebSite)
        {
            var WeblevelType1 = new WebLevel()
            {
                IsEnable = "1",
                ParentSN = 0,
                WebSiteID = sysWebSite.WebSiteID,
                Lang = sysWebSite.Lang,
                WeblevelType = "1",
                Module = "PAGELIST",
                Title = "全球資訊網",
                FatFooterShow = "1",
                MainMenuShow = "1",
                SubMemuShow = "1",
                RSSShow = "0",
                StartDate = DateTime.UtcNow.AddHours(8),
                ProcessUserID = sysWebSite.ProcessUserID,
                CreatedUserID = sysWebSite.CreatedUserID,
                ProcessDate = DateTime.UtcNow.AddHours(8),
                CreatedDate = DateTime.UtcNow.AddHours(8),
                ProcessIPAddress = sysWebSite.ProcessIPAddress,
                SortOrder = 1,
            };
            var WeblevelType2 = new WebLevel()
            {
                IsEnable = "1",
                ParentSN = 0,
                WebSiteID = sysWebSite.WebSiteID,
                Lang = sysWebSite.Lang,
                WeblevelType = "2",
                Module = "PAGELIST",
                Title = "首頁區塊維護",
                FatFooterShow = "1",
                MainMenuShow = "1",
                SubMemuShow = "1",
                RSSShow = "0",
                StartDate = DateTime.UtcNow.AddHours(8),
                ProcessUserID = sysWebSite.ProcessUserID,
                CreatedUserID = sysWebSite.CreatedUserID,
                ProcessDate = DateTime.UtcNow.AddHours(8),
                CreatedDate = DateTime.UtcNow.AddHours(8),
                ProcessIPAddress = sysWebSite.ProcessIPAddress,
                SortOrder = 2,
            };
            var WeblevelType3 = new WebLevel()
            {
                IsEnable = "1",
                ParentSN = 0,
                WebSiteID = sysWebSite.WebSiteID,
                Lang = sysWebSite.Lang,
                WeblevelType = "3",
                Module = "PAGELIST",
                Title = "頁首頁底區塊維護",
                FatFooterShow = "1",
                MainMenuShow = "1",
                SubMemuShow = "1",
                RSSShow = "0",
                StartDate = DateTime.UtcNow.AddHours(8),
                ProcessUserID = sysWebSite.ProcessUserID,
                CreatedUserID = sysWebSite.CreatedUserID,
                ProcessDate = DateTime.UtcNow.AddHours(8),
                CreatedDate = DateTime.UtcNow.AddHours(8),
                ProcessIPAddress = sysWebSite.ProcessIPAddress,
                SortOrder = 3,
            };
            using (var db = new MODAContext())
            {
                try
                {
                    db.WebLevel.Add(WeblevelType1);
                    db.WebLevel.Add(WeblevelType2);
                    db.WebLevel.Add(WeblevelType3);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
        }

        public static List<WebSiteExtend> GetWebSiteExtendsByWebSiteID(string v)
        {
            using (var db = new MODAContext())
            {
                try
                {
                    return db.WebSiteExtend.Where(x => x.WebSiteID == v).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static bool Edit(SysWebSiteLang sysWebSite, List<CommonFileModel> commonFileModels = null, List<WebSiteExtend> webExtend = null)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    #region Update SysWebSite
                    var oldData = db.SysWebSiteLang.FirstOrDefault(x => x.SysWebSiteLangSN == sysWebSite.SysWebSiteLangSN);
                    oldData.WebSiteID = sysWebSite.WebSiteID.Trim().ToUpper();
                    oldData.Title = sysWebSite.Title.Trim();
                    //oldData.DNS = sysWebSite.DNS;
                    oldData.Description = sysWebSite.Description == null ? "" : sysWebSite.Description.Trim();
                    oldData.GoogleSearchKey = sysWebSite.GoogleSearchKey == null ? "" : sysWebSite.GoogleSearchKey.Trim();
                    oldData.GACode = sysWebSite.GACode;
                    oldData.IsEnable = sysWebSite.IsEnable;
                    oldData.ProcessUserID = sysWebSite.ProcessUserID;
                    oldData.ProcessDate = sysWebSite.ProcessDate;
                    oldData.ProcessIPAddress = sysWebSite.ProcessIPAddress;
                    oldData.SortOrder = sysWebSite.SortOrder;
                    //oldData.Lang = sysWebSiteLangs.Lang;
                    //oldData.Title = sysWebSiteLangs.Title;
                    #endregion
                    #region 無障礙圖片上傳
                    var imgData = db.RelWebFileContent.FirstOrDefault(x => x.SourceTable == "SysWebSite" && x.SourceSN == sysWebSite.SysWebSiteLangSN);
                    if (imgData != null)
                    {
                        var file = db.WEBFile.FirstOrDefault(x => x.WEBFileSN == imgData.WEBFileSN && x.IsEnable == "1");
                        if (file != null)
                        {
                            file.IsEnable = "0";
                            file.ProcessDate = DateTime.UtcNow.AddHours(8);
                            file.ProcessUserID = sysWebSite.ProcessUserID;
                            file.ProcessIPAddress = sysWebSite.ProcessIPAddress;
                            db.RelWebFileContent.Remove(imgData);
                        }

                    }
                    if (commonFileModels != null && commonFileModels.Any())
                    {
                        var FileData = db.WEBFile.First(x => x.FileName == commonFileModels.First().fileNewName);
                        var RelWebFileContentData = new RelWebFileContent()
                        {
                            WEBFileSN = FileData.WEBFileSN,
                            SourceTable = "SysWebSite",
                            SourceSN = sysWebSite.SysWebSiteLangSN,
                            GroupID = commonFileModels.First().GroupID,
                            CreatedUserID = sysWebSite.ProcessUserID,
                            CreatedDate = DateTime.UtcNow.AddHours(8),
                            SortOrder = 1,
                        };
                        FileData.IsEnable = "1";
                        FileData.FileTitle = commonFileModels.First().fileTitle;
                        db.WEBFile.Update(FileData);
                        db.RelWebFileContent.Add(RelWebFileContentData);

                    }
                    #endregion

                    db.SysWebSiteLang.Update(oldData);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Utility.Mail.Error(ex.ToString());
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = sysWebSite.ProcessIPAddress,
                    UserID = sysWebSite.ProcessUserID,
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Update",
                    SourceTable = "SysWebSite",
                    Action = "Edit",
                    Controller = "WebsiteManagementService",
                    SourceSN = sysWebSite.SysWebSiteLangSN,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }

        public static bool Delete(int SN)
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var oldData = db.SysWebSiteLang.FirstOrDefault(x => x.SysWebSiteLangSN == SN);
                    oldData.IsEnable = "-99";
                    oldData.ProcessDate = DateTime.UtcNow.AddHours(8);
                    db.SysWebSiteLang.Update(oldData);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogService.CreateLogAction(new LogAction()
                {
                    Status = "0",
                    MessageResult = ex.ToString(),
                    ProcessIPAddress = "",
                    UserID = "",
                    WebSiteID = "",
                    WebPath = "",
                    ActionType = "1",
                    Action2 = "Delete",
                    SourceTable = "SysWebSite",
                    Action = "Delete",
                    Controller = "WebsiteManagementService",
                    SourceSN = SN,
                    CreatedDate = DateTime.UtcNow.AddHours(8)
                });
                return false;
            }
        }
    }
}

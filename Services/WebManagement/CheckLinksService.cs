using Services.Models.WebManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Services.WebManagement
{
    public class CheckLinksService
    {
        public static List<CheckLinkModel.CheckingLink> GetCheckingLinks(string websiteid)
        {
            var weblinks = new List<CheckLinkModel.CheckingLink>();

            try
            {
                using (var db = new MODAContext())
                {
                    var AliveStaticLink = Static.StaticLinkService.GetStaticData().Where(x => x.WebSiteID == websiteid && x.IsEnable == "1").ToList();

                    var CntData = (from s in AliveStaticLink
                                   join l in db.WebCntLink on new { SourceTable = s.SourseTable, SourceSN = s.SourseSN.Value } equals new { SourceTable = l.SourceTable.ToLower(), l.SourceSN }
                                   select new CheckLinkModel.CheckingLink
                                   {
                                       SourceTable = s.SourseTable,
                                       SourceSN = s.SourseSN.Value,
                                       URL = HttpUtility.HtmlDecode(l.URL),
                                       ExtendSN = 0,
                                       LinkType = 0,
                                   }).ToList();

                    weblinks.AddRange(CntData);

                    var ExtendData = (from s in AliveStaticLink.Where(x => x.SourseTable == "webnews")
                                      join l in db.WEBNewsExtend on s.SourseSN.Value equals l.WEBNewsSN.Value
                                      where l.GroupID == "relatedlink"
                                      select new CheckLinkModel.CheckingLink
                                      {
                                          SourceTable = s.SourseTable,
                                          SourceSN = s.SourseSN.Value,
                                          URL = l.Column_1,
                                          ExtendSN = l.WEBNewsExtendSN,
                                          LinkType = 1,
                                      }).ToList();

                    weblinks.AddRange(ExtendData);

                    var NewsURLData = (from s in AliveStaticLink.Where(x => x.SourseTable == "webnews")
                                       join l in db.WEBNews on s.SourseSN.Value equals l.WEBNewsSN
                                       where l.ArticleType == "2"
                                       select new CheckLinkModel.CheckingLink
                                       {
                                           SourceTable = s.SourseTable,
                                           SourceSN = s.SourseSN.Value,
                                           URL = l.URL,
                                           ExtendSN = 0,
                                           LinkType = 2,
                                       }).ToList();

                    weblinks.AddRange(NewsURLData);
                }
                return weblinks.Where(x => !CommonService.CheckLocalUrl(x.URL)).ToList(); ;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<CheckLinkModel.ExportLink> GetExportLinks(List<CheckLinkModel.CheckingLink> links)
        {
            var exportLinks = new List<CheckLinkModel.ExportLink>();

            using (var db = new MODAContext())
            {
                var defaultdept = db.SysDepartment.Where(x => x.ParentID == 0 && x.Lang == "zh-tw" && x.IsEnable == "1").ToList();

                var newsExtendlink = links.Where(x => x.SourceTable == "webnews" && x.LinkType == 1).ToList();

                var newsExtendExportLinks = (from n in newsExtendlink
                                             join s in db.StaticLink on new { SourceTable = n.SourceTable, n.SourceSN } equals new { SourceTable = s.SourseTable, SourceSN = s.SourseSN.Value }
                                             join l in db.WEBNews on n.SourceSN equals l.WEBNewsSN
                                             join d in db.SysDepartment.Where(x => x.IsEnable == "1") on new { WebSiteId = l.WebSiteID, l.DepartmentID, Lang = "zh-tw" } equals new { d.WebSiteId, d.DepartmentID, d.Lang } into cd
                                             from d in cd.DefaultIfEmpty()
                                             join e in db.WEBNewsExtend on n.ExtendSN equals e.WEBNewsExtendSN
                                             select new CheckLinkModel.ExportLink
                                             {
                                                 Breadcrumb = String.Join(">", CommonService.LevelBreadcrumb(l.WebLevelSN, l.WEBNewsSN).ToArray()),
                                                 URL = n.URL,
                                                 PageTitle = l.Title,
                                                 WebURL = CommonService.WebSiteUrl + s.StaticUrl,
                                                 DeptName = d?.DepartmentName ?? defaultdept.FirstOrDefault(x => x.WebSiteId == l.WebSiteID)?.DepartmentName ?? "" ,
                                                 Title = e?.Column_2 ?? "",
                                                 error = n.error
                                             }).ToList();

                exportLinks.AddRange(newsExtendExportLinks);

                var newslink = links.Where(x => x.SourceTable == "webnews" && x.LinkType != 1).ToList();

                var newsExportLinks = (from n in newslink
                                       join s in db.StaticLink on new { SourceTable = n.SourceTable, n.SourceSN } equals new { SourceTable = s.SourseTable, SourceSN = s.SourseSN.Value }
                                       join l in db.WEBNews on n.SourceSN equals l.WEBNewsSN
                                       join d in db.SysDepartment.Where(x => x.IsEnable == "1") on new { WebSiteId = l.WebSiteID, l.DepartmentID, Lang = "zh-tw" } equals new { d.WebSiteId, d.DepartmentID, d.Lang } into cd
                                       from d in cd.DefaultIfEmpty()
                                       select new CheckLinkModel.ExportLink
                                       {
                                           Breadcrumb = String.Join(">", CommonService.LevelBreadcrumb(l.WebLevelSN, l.WEBNewsSN).ToArray()),
                                           URL = n.URL,
                                           PageTitle = l.Title,
                                           WebURL = CommonService.WebSiteUrl + s.StaticUrl,
                                           DeptName = d?.DepartmentName ?? defaultdept.FirstOrDefault(x => x.WebSiteId == l.WebSiteID)?.DepartmentName ?? "",
                                           Title = n.LinkType == 0 ? "[請見網頁內容編輯區塊]" : l.Title,
                                           error = n.error
                                       }).ToList();

                exportLinks.AddRange(newsExportLinks);

                var levellink = links.Where(x => x.SourceTable == "weblevel").ToList();

                var levelExportLinks = (from n in levellink
                                        join s in db.StaticLink on new { SourceTable = n.SourceTable, n.SourceSN } equals new { SourceTable = s.SourseTable, SourceSN = s.SourseSN.Value }
                                        join l in db.WebLevel on n.SourceSN equals l.WebLevelSN
                                        join d in db.SysDepartment.Where(x => x.IsEnable == "1") on new { WebSiteId = l.WebSiteID, ParentID = 0, Lang = "zh-tw" } equals new { d.WebSiteId, ParentID = d.ParentID.Value, d.Lang } into cd
                                        from d in cd.DefaultIfEmpty()
                                        select new CheckLinkModel.ExportLink
                                        {
                                            Breadcrumb = String.Join(">", CommonService.LevelBreadcrumb(l.WebLevelSN).ToArray()),
                                            URL = n.URL,
                                            PageTitle = l.Title,
                                            WebURL = CommonService.WebSiteUrl + s.StaticUrl,
                                            DeptName = d?.DepartmentName ?? defaultdept.FirstOrDefault(x => x.WebSiteId == l.WebSiteID)?.DepartmentName ?? "",
                                            Title = "[請見網頁內容編輯區塊]",
                                            error = n.error
                                        }).ToList();

                exportLinks.AddRange(levelExportLinks);
            }
            return exportLinks;
        }

        public static void UpdateCheckLinksDate(string websiteid) 
        {
            try
            {
                using (var db = new MODAContext())
                {
                    var website = db.SysWebSite.FirstOrDefault(x => x.WebSiteID == websiteid);
                    website.CheckLinksDate = DateTime.UtcNow.AddHours(8);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

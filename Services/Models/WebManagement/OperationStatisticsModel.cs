using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Models.WebManagement
{
    public class OperationStatisticsModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public string ProcessIPAddress { get; set; }

        public string Action2 { get; set; }

        public string MessageInput { get; set; }

        public string SourceTable { get; set; }

        public int SourceSN { get; set; }

        public string Lang { get; set; }

        public DateTime CreatedDate { get; set; }

        public string WebPath { get; set; }

    }

    public class OperationStatisticsModel2
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public string ProcessIPAddress { get; set; }

        public string Action2 { get; set; }

        public string MessageInput { get; set; }

        public string MessageResult { get; set; }
        public string SourceTable { get; set; }

        public int SourceSN { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Webpath { get; set; }

        public string WebSiteTitle { get; set; }

        public string Lang { get; set; }

        public int? ParentSN { get; set; }

        public string ParentTitle { get; set; }

    }

    public class OperationStatisticsModel3
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Title { get; set; }
        public string SourceTable { get; set; }
        public int SourceSN { get; set; }
        public DateTime StrDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Webpath { get; set; }
        public string Lang { get; set; }
        public string WebSiteID { get; set; }
        public int InsertSUM { get; set; }
        public int UpdateSUM { get; set; }
        public int DeleteSUM { get; set; }
    }

    public class OperationStatisticsModel4
    {
        public string WebSiteTitle { get; set; }
        public int SortOrder { get; set; }
        public string CreateDepName { get; set; }
        public string ReleaseDepName { get; set; }
        public int HelpOtherCount { get; set; }
    }
}

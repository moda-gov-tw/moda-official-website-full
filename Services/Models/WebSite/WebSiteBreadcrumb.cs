namespace Services.Models
{
    public  class WebSiteBreadcrumb
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public int sort { get; set; } = 0;
        public int mainSN { get; set; }
        public string WebSiteID { get; set; }
        public string lang { get; set; }
        public int ParentSN { get; set; }
        public bool IsActive { get; set; }
        public string SourseTable { get; set; }

    }
}

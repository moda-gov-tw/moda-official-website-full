namespace WebAPI
{
    public class NewsListQuery
    {
        public int MainSN { get; set; }
        public string? Lang { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? SearchString { get; set; }
        public string? Condition4 { get; set; }
        public string? Condition5 { get; set; }
        public string? Condition6 { get; set; }

        public string? Condition7 { get; set; }

        public string? CustomizeTagSN { get; set; }
        public string? SysZipCode { get; set; }
        public int P { get; set; } = 1;
        public int DisplayCount { get; set; } = 15;

        public string? Regulations { get; set; }


    }
}

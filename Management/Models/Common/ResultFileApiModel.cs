using static Utility.Files;

namespace Management.Models
{
    public class ResultFileApiModel
    {
        public System.Net.HttpStatusCode statusCode { get; set; }

        public FileMessage content { get; set; }
    }

    public class ApiResultModel 
    {
        public int statusCode { get; set; }
        public string content { get; set; }
    }
}

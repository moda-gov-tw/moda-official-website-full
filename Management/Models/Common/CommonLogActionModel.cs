using System;

namespace Management.Models
{
    public class CommonLogActionModel
    {
        public string UserID { get; set; }
        public string Lang { get; set; }
        public string UserName { get; set; }
        public string Action2 { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProcessIPAddress { get; set; }
        public int SourceSN { get; set; }
        public string MessageInput { get; set; }
        public string SourceTable { get; set; }

    }
}

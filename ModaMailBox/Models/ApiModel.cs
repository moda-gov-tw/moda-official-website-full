namespace ModaMailBox.Models
{
    public class ApiModel
    {
        public class TurnstileResponsemodel
        {
            public bool success { get; set; }
            public string challenge_ts { get; set; }
            public string hostname { get; set; }
            public string action { get; set; }
            public string cdata { get; set; }
        }

        public class TurnstileRequestmodel 
        {
            public string secret { get; set; }

            public string response { get; set; }

            public string remoteip { get; set; }
        }
    }
}

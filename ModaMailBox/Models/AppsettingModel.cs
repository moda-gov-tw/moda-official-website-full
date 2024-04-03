using Utility;

namespace ModaMailBox.Models
{
    public class AppsettingModel
    {
        public class MailModel
        {
            public string sysAdmin { get; set; }

            public bool IsOfficialMail { get; set; } = true;
            public DefaultMailSettingModel Default { get; set; }
            public DefaultMailSettingModel MailBox { get; set; }

        }

    }
}

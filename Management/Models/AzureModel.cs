namespace Management.Models
{
    public class AzureModel
    {
        public class tokenMdeol
        {
            public string token_type { get; set; }
            public string scope { get; set; }
            public int expires_in { get; set; }
            public int ext_expires_in { get; set; }
            public string access_token { get; set; }
        }
        public class AADUserData {
            public string odatacontext { get; set; }
            public object[] businessPhones { get; set; }
            public string displayName { get; set; }
            public object givenName { get; set; }
            public object jobTitle { get; set; }
            public object mail { get; set; }
            public object mobilePhone { get; set; }
            public object officeLocation { get; set; }
            public object preferredLanguage { get; set; }
            public object surname { get; set; }
            public string userPrincipalName { get; set; }
            public string id { get; set; }

        }
    }
}


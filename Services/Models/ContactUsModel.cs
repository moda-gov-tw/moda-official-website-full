using DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class ContactUsModel
    {
        public WebLevel BasicLevel { get; set; }

        public WEBFile LogoFile { get; set; }

        public WEBFile DarkLogoFile { get; set; }
    }
}

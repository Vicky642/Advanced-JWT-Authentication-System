using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advanced_JWT_Authentication_System.Models.Authentication
{
    public class GoogleSignInModel
    {
        public string codeModel { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string redirectUri { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.RequestModel
{
    public class AuthenticateRegistrationRequestModel
    {
        public string Email { get; set; }
        public string ConfirmCode { get; set; }
    }
}

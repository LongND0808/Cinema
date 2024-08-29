using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.RequestModel
{
    public class ResetPasswordRequestModel
    {
        public string Email { get; set; }
        public string ResetCode { get; set; }
    }
}

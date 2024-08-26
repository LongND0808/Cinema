using BaseInsightDotNet.Commons.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Core.RequestModel
{
    public class RegisterRequestModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }
        public Enumerate.Gender Gender { get; set; }
        public DateTime DateOfBirth {  get; set; }
    }
}

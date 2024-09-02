using BaseInsightDotNet.Commons.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.RequestModel.ManageUser
{
    public class CreateUserRequestModel
    {
        public int Point { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Enumerate.Gender Gender { get; set; }

        public string AvatarUrl { get; set; }

        public Guid UserStatusId { get; set; }

        public Guid[] RoleIds { get; set; }
    }
}

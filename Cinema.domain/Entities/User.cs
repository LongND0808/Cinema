using BaseInsightDotNet.Commons.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public int Point { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Enumerate.Gender Gender { get; set; } = Enumerate.Gender.Unknown;
        public string AvatarUrl { get; set; }
        public Guid RankCustomerId { get; set; }
        public Guid UserStatusId { get; set; }
        public bool isDeleted { get; set; }

        public virtual UserStatus? UserStatuse { get; set; }

        public virtual RankCustomer? RankCustomer { get; set; }

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }

        public virtual ICollection<Bill>? Bills { get; set; }

        public virtual ICollection<ConfirmEmail>? ConfirmEmails { get; set; }
    }
}

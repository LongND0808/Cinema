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
    public class User : IdentityUser<int>
    {
        public int? Point { get; set; }
        public string FullName { get; set; }
        public int? RankCustomerId { get; set; }
        public int? UserStatusId { get; set; }
        public bool IsActive { get; set; }

        public virtual UserStatus? UserStatuse { get; set; }

        public virtual RankCustomer? RankCustomer { get; set; }

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }

        public virtual ICollection<Bill>? Bills { get; set; }

        public virtual ICollection<ConfirmEmail>? ConfirmEmails { get; set; }

        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}

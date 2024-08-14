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
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int? RankCustomerId { get; set; }
        public int? UserStatusId { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("UserStatusId")]
        public virtual UserStatus? UserStatuse { get; set; }

        [ForeignKey("RankCustomerId")]
        public virtual RankCustomer? RankCustomer { get; set; }

        public virtual ICollection<RefreshToken>? RefreshTokens { get; set; }

        public virtual ICollection<Bill>? Bills { get; set; }
            
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        public virtual ICollection<ConfirmEmail>? ConfirmEmails { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class ConfirmEmail : BaseEntity
    {
        public int? UserId { get; set; }
        public DateTime RequiredDateTime { get; set; }
        public DateTime ExpiredDateTime { get; set; }
        public string? ConfirmCode { get; set; }
        public bool IsConfirm { get; set; }

        public virtual User? User { get; set; }
    }
}

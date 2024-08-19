using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class Bill : BaseEntity
    {
        public double TotalMoney { get; set; }
        public string TradingCode { get; set; }
        public DateTime CreateTime { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public DateTime CreateAt { get; set; }
        public int PromotionId { get; set; }
        public bool IsActive { get; set; }
        public int BillStatusId { get; set; }

        public virtual User? User { get; set; }

        public virtual Promotion? Promotion { get; set; }

        public virtual BillStatus? BillStatuses { get; set; }

        public virtual BillFood? BillFood { get; set; }

        public virtual ICollection<BillTicket>? BillTickets { get; set; }

    }
}

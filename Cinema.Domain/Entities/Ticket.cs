using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public string Code { get; set; }
        public Guid ScheduleId { get; set; }
        public Guid SeatId { get; set; }
        public bool IsActive { get; set; }
        public double PriceTicket { get; set; }

        public virtual ICollection<BillTicket>? BillTickets { get; set; }

        public virtual Schedule? Schedule { get; set; }

        public virtual Seat? Seat { get; set; }

    }
}

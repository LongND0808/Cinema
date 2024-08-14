using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class Seat : BaseEntity
    {
        public int Number { get; set; }
        public int SeatStatusId { get; set; }
        public string Line { get; set; }
        public int RoomId { get; set; }
        public bool IsActive { get; set; }
        public int SeatTypeId { get; set; }

        [ForeignKey("SeatTypeId")]
        public virtual SeatType? SeatType { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        [ForeignKey("SeatStatusId")]
        public virtual SeatStatus? SeatStatus { get; set; }

        public virtual ICollection<Ticket>? Tickets { get; set; }

    }
}

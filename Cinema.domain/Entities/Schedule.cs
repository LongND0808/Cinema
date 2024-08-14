using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class Schedule : BaseEntity
    {
        public double Price { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Code { get; set; }
        public int MovieId { get; set; }
        public string Name { get; set; }
        public int RoomId { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie? Movie { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        public virtual ICollection<Ticket>? Ticket { get; set; }

    }
}

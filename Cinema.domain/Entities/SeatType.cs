using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class SeatType : BaseEntity
    {
        public string NameType { get; set; }

        public virtual ICollection<Seat>? Seats { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class Bonus : BaseEntity
    {
        public double Point { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Bill>? Bills { get; set; }
    }
}

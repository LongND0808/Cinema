using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class RankCustomer : BaseEntity
    {
        public int Point { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public double Discount { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}

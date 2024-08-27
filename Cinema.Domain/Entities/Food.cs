using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class Food : BaseEntity
    {
        public double Price { get; set; }
        public string Description { get; set; }
        public string image { get; set; }
        public string NameOfFood { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<BillFood>? BillFoods { get; set; }

    }
}

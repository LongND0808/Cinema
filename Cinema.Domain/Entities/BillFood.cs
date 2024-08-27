using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class BillFood : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid BillId { get; set; }
        public Guid FoodId { get; set; }

        public virtual Bill? Bill { get; set; }
        public virtual Food? Food { get; set; }

    }
}

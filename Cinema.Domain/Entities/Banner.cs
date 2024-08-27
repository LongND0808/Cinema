using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class Banner : BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
    }
}

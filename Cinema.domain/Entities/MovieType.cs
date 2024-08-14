using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Domain.Entities
{
    public class MovieType : BaseEntity
    {
        public string MovieTypeName { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Movie>? Movies { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.RequestModel
{
    public class GetMovieRequestModel
    {
        public Guid? MovieTypeId { get; set; }

        public Guid? CinemaId { get; set; }

        public Guid? RoomId { get; set; }
    }
}

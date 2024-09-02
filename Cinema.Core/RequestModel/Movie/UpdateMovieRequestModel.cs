using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.RequestModel.Movie
{
    public class UpdateMovieRequestModel
    {
        public Guid Id { get; set; }
        public int MovieDuration { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime PremiereDate { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public string Image { get; set; }
        public string Language { get; set; }
        public Guid MovieTypeId { get; set; }
        public string Name { get; set; }
        public Guid RateId { get; set; }
        public string Trailer { get; set; }
        public bool IsActive { get; set; }
    }
}

using Cinema.Core.DTOs;
using Cinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.IConverters
{
    public interface IMovieConverter
    {
        MovieDTO ConvertToDTO(Movie movie);
        Movie ConvertToEntity(MovieDTO movieDTO);
    }
}

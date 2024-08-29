using Cinema.Core.DTOs;
using Cinema.Core.IConverters;
using Cinema.Domain.Entities;
using System;

namespace Cinema.Core.Converters
{
    public class MovieConverter : IMovieConverter
    {
        public MovieDTO ConvertToDTO(Movie movie)
        {
            if (movie == null)
            {
                throw new ArgumentNullException(nameof(movie), "Movie cannot be null");
            }

            return new MovieDTO
            {
                Id = movie.Id,
                MovieDuration = movie.MovieDuration,
                EndTime = movie.EndTime,
                PremiereDate = movie.PremiereDate,
                Description = movie.Description,
                Director = movie.Director,
                Image = movie.Image,
                Language = movie.Language,
                MovieTypeId = movie.MovieTypeId,
                Name = movie.Name,
                RateId = movie.RateId,
                Trailer = movie.Trailer,
                IsActive = movie.IsActive
            };
        }

        public Movie ConvertToEntity(MovieDTO movieDTO)
        {
            if (movieDTO == null)
            {
                throw new ArgumentNullException(nameof(movieDTO), "MovieDTO cannot be null");
            }

            return new Movie
            {
                Id = movieDTO.Id,
                MovieDuration = movieDTO.MovieDuration,
                EndTime = movieDTO.EndTime,
                PremiereDate = movieDTO.PremiereDate,
                Description = movieDTO.Description,
                Director = movieDTO.Director,
                Image = movieDTO.Image,
                Language = movieDTO.Language,
                MovieTypeId = movieDTO.MovieTypeId,
                Name = movieDTO.Name,
                RateId = movieDTO.RateId,
                Trailer = movieDTO.Trailer,
                IsActive = movieDTO.IsActive
            };
        }
    }
}

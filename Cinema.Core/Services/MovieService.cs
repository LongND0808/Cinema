using Cinema.Core.DTOs;
using Cinema.Core.IConverters;
using Cinema.Core.InterfaceRepository;
using Cinema.Core.IService;
using Cinema.Core.RequestModel;
using Cinema.Core.ResponseModel;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Services
{
    public class MovieService : IMovieService
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Schedule> _scheduleRepository;
        private readonly IRepository<Domain.Entities.Cinema> _cinemaRepository;
        private readonly IMovieConverter _movieConverter;

        public MovieService(
                IRepository<Movie> movieRepository,
                IRepository<Room> roomRepository,
                IRepository<Schedule> scheduleRepository,
                IRepository<Domain.Entities.Cinema> cinemaRepository,
                IMovieConverter movieConverter)
        {
            _movieRepository = movieRepository;
            _roomRepository = roomRepository;
            _scheduleRepository = scheduleRepository;
            _cinemaRepository = cinemaRepository;
            _movieConverter = movieConverter;
        }

        public Task<BaseResponseModel<MovieDTO>> CreateMovie(CreateMovieRequestModel request)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseModel<object>> DeleteMovie(Guid requestId)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponseModel<IQueryable<MovieDTO>>> GetAllMovie(GetMovieRequestModel? request)
        {
            if (request == null)
            {
                var res = await _movieRepository.GetAllAsyncUntracked(selector: s => _movieConverter.ConvertToDTO(s));

                if (res == null || !res.Any())
                {
                    return new BaseResponseModel<IQueryable<MovieDTO>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No movie existed!",
                        Data = null
                    };
                }

                return new BaseResponseModel<IQueryable<MovieDTO>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Get movies successfully",
                    Data = res.AsQueryable()
                };
            }
            else
            {
                var allMovie = await _movieRepository.GetAllAsyncUntracked(selector: s => s);

                if (allMovie == null || !allMovie.Any())
                {
                    return new BaseResponseModel<IQueryable<MovieDTO>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No movie existed!",
                        Data = null
                    };
                }

                if (request.MovieTypeId.HasValue)
                {
                    allMovie = allMovie.Where(record => record.MovieTypeId == request.MovieTypeId.Value);
                }

                if (request.CinemaId.HasValue)
                {
                    var cinema = await _cinemaRepository.GetOneAsyncUntracked(
                        filter: f => f.Id == request.CinemaId.Value,
                        selector: s => s);

                    if (cinema == null)
                    {
                        return new BaseResponseModel<IQueryable<MovieDTO>>
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Cinema is not valid",
                            Data = null
                        };
                    }

                    if (request.RoomId.HasValue)
                    {

                        var rooms = await _roomRepository.GetOneAsyncUntracked(
                            filter: f => f.Id == request.RoomId.Value,
                            selector: s => s);

                        if (rooms == null)
                        {
                            return new BaseResponseModel<IQueryable<MovieDTO>>
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = "Room is not valid",
                                Data = null
                            };
                        }

                        var movieIds = await _scheduleRepository.GetAllAsyncUntracked(
                            filter: f => f.RoomId == request.RoomId.Value,
                            selector: s => s.MovieId);

                        allMovie = allMovie.Where(record => movieIds.Contains(record.Id));
                    }
                    else
                    {
                        var roomIds = await _roomRepository.GetAllAsyncUntracked(
                        filter => filter.CinemaId == request.CinemaId.Value,
                        selector: s => s.Id);

                        if (roomIds == null || !roomIds.Any())
                        {
                            return new BaseResponseModel<IQueryable<MovieDTO>>
                            {
                                Status = StatusCodes.Status400BadRequest,
                                Message = "Cinema do not have any room!",
                                Data = null
                            };
                        }

                        var movieIds = await _scheduleRepository.GetAllAsyncUntracked(
                            filter: f => roomIds.Contains(f.RoomId),
                            selector: s => s.MovieId);

                        allMovie = allMovie.Where(record => movieIds.Contains(record.Id));
                    }
                }

                var res = allMovie.Select(x => _movieConverter.ConvertToDTO(x));

                if (res == null || !res.Any())
                {
                    return new BaseResponseModel<IQueryable<MovieDTO>>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "No movie existed!",
                        Data = null
                    };
                }

                return new BaseResponseModel<IQueryable<MovieDTO>>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Get movies successfully",
                    Data = res.AsQueryable()
                };
            }
        }

        public async Task<BaseResponseModel<MovieDTO>> GetMovieById(Guid requestId)
        {
            var movie = await _movieRepository.GetByIdAsync(requestId);

            if (movie == null)
            {
                return new BaseResponseModel<MovieDTO>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Movie found!",
                    Data = null
                };
            }

            var movieDTO = _movieConverter.ConvertToDTO(movie);

            return new BaseResponseModel<MovieDTO>
            {
                Status = StatusCodes.Status404NotFound,
                Message = "Movie not found!",
                Data = movieDTO
            };

        }

        public Task<BaseResponseModel<MovieDTO>> UpdateMovie(UpdateMovieRequestModel request)
        {
            throw new NotImplementedException();
        }
    }
}

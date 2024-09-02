using Cinema.Core.DTOs;
using Cinema.Core.IConverters;
using Cinema.Core.InterfaceRepository;
using Cinema.Core.IService;
using Cinema.Core.RequestModel.Movie;
using Cinema.Core.ResponseModel;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Services
{
    public class MovieService : IMovieService
    {
        #region Private fields
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<Schedule> _scheduleRepository;
        private readonly IRepository<MovieType> _movieTypeRepository;
        private readonly IRepository<Rate> _rateRepository;
        private readonly IRepository<Domain.Entities.Cinema> _cinemaRepository;
        private readonly IMovieConverter _movieConverter;
        private readonly IAuthService _authService;
        #endregion

        #region Constructor
        public MovieService(
                IRepository<Movie> movieRepository,
                IRepository<Room> roomRepository,
                IRepository<Schedule> scheduleRepository,
                IRepository<Domain.Entities.Cinema> cinemaRepository,
                IMovieConverter movieConverter,
                IAuthService authService,
                IRepository<MovieType> movieTypeRepository,
                IRepository<Rate> rateRepository)
        {
            _movieRepository = movieRepository;
            _roomRepository = roomRepository;
            _scheduleRepository = scheduleRepository;
            _cinemaRepository = cinemaRepository;
            _movieConverter = movieConverter;
            _authService = authService;
            _movieTypeRepository = movieTypeRepository;
            _rateRepository = rateRepository;
        }
        #endregion

        #region CreateMovie method 
        public async Task<BaseResponseModel<MovieDTO>> CreateMovie(CreateMovieRequestModel request)
        {
            try
            {
                if (!_authService.IsUserInRole("Admin"))
                {
                    return new BaseResponseModel<MovieDTO>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You lack the necessary permissions to perform this task.",
                        Data = null
                    };
                }

                var movieType = await _movieTypeRepository.GetByIdAsync(request.MovieTypeId);

                if (movieType == null)
                {
                    return new BaseResponseModel<MovieDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Movie type does not exist.",
                        Data = null
                    };
                }

                var rate = await _rateRepository.GetByIdAsync(request.RateId);

                if(rate == null)
                {
                    return new BaseResponseModel<MovieDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Rate does not exist.",
                        Data = null
                    };
                }    

                var movie = new Movie
                {
                    Name = request.Name,
                    Description = request.Description,
                    Director = request.Director,
                    Language = request.Language,
                    Image = request.Image,
                    Trailer = request.Trailer,
                    PremiereDate = request.PremiereDate,
                    MovieDuration = request.MovieDuration,
                    EndTime = request.EndTime,
                    MovieTypeId = request.MovieTypeId,
                    RateId = request.RateId,
                    IsActive = request.IsActive,
                };

                await _movieRepository.AddAsync(movie);

                return new BaseResponseModel<MovieDTO>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Add new movie successfully",
                    Data = _movieConverter.ConvertToDTO(movie)
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred while processing your request.";

                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }

                return new BaseResponseModel<MovieDTO>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = errorMessage,
                    Data = null
                };
            }
        }
        #endregion

        #region DeleteMovie method
        public async Task<BaseResponseModel<object>> DeleteMovie(Guid requestId)
        {
            try
            {
                if (!_authService.IsUserInRole("Admin"))
                {
                    return new BaseResponseModel<object>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You lack the necessary permissions to perform this task.",
                        Data = null
                    };
                }

                var movie = await _movieRepository.GetByIdAsync(requestId);

                if (movie == null)
                {
                    return new BaseResponseModel<object>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Movie not found.",
                        Data = null
                    };
                }

                await _movieRepository.DeleteAsync(movie);

                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Movie deleted successfully.",
                    Data = null
                };

            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred while processing your request.";

                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }

                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = errorMessage,
                    Data = null
                };
            }
        }
        #endregion

        #region GetAllMovie method
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
        #endregion

        #region GetMovieById method
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
        #endregion

        #region UpdateMovie method
        public async Task<BaseResponseModel<MovieDTO>> UpdateMovie(UpdateMovieRequestModel request)
        {
            try
            {
                if (!_authService.IsUserInRole("Admin"))
                {
                    return new BaseResponseModel<MovieDTO>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You lack the necessary permissions to perform this task.",
                        Data = null
                    };
                }

                var movieType = await _movieTypeRepository.GetByIdAsync(request.MovieTypeId);

                if (movieType == null)
                {
                    return new BaseResponseModel<MovieDTO>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Movie type does not exist.",
                        Data = null
                    };
                }

                var rate = await _rateRepository.GetByIdAsync(request.RateId);

                if (rate == null)
                {
                    return new BaseResponseModel<MovieDTO>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Rate does not exist.",
                        Data = null
                    };
                }

                if(request.PremiereDate > request.EndTime)
                {
                    return new BaseResponseModel<MovieDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "PremiereDate and EndTime must be valid.",
                        Data = null
                    };
                }    

                var existingMovie = await _movieRepository.GetByIdAsync(request.Id);

                if (existingMovie == null)
                {
                    return new BaseResponseModel<MovieDTO>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Movie not found.",
                        Data = null
                    };
                }

                existingMovie.Name = request.Name;
                existingMovie.Description = request.Description;
                existingMovie.Director = request.Director;
                existingMovie.Language = request.Language;
                existingMovie.Image = request.Image;
                existingMovie.Trailer = request.Trailer;
                existingMovie.PremiereDate = request.PremiereDate;
                existingMovie.MovieDuration = request.MovieDuration;
                existingMovie.EndTime = request.EndTime;
                existingMovie.MovieTypeId = request.MovieTypeId;
                existingMovie.RateId = request.RateId;
                existingMovie.IsActive = request.IsActive;

                await _movieRepository.UpdateAsync(existingMovie);

                return new BaseResponseModel<MovieDTO>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Movie updated successfully.",
                    Data = _movieConverter.ConvertToDTO(existingMovie)
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred while processing your request.";

                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }

                return new BaseResponseModel<MovieDTO>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = errorMessage,
                    Data = null
                };
            }
        }
        #endregion
    }
}

using Cinema.Core.DTOs;
using Cinema.Core.RequestModel;
using Cinema.Core.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.IService
{
    public interface IMovieService
    {
        Task<BaseResponseModel<MovieDTO>> CreateMovie(CreateMovieRequestModel request);
        Task<BaseResponseModel<object>> DeleteMovie(Guid requestId);
        Task<BaseResponseModel<IQueryable<MovieDTO>>> GetAllMovie(GetMovieRequestModel? request);
        Task<BaseResponseModel<MovieDTO>> GetMovieById(Guid requestId);
        Task<BaseResponseModel<MovieDTO>> UpdateMovie(UpdateMovieRequestModel request);
    }
}

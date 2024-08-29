using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.ResponseModel
{
    public class BaseResponseModel<T> where T : class
    {
        public int Status {  get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}

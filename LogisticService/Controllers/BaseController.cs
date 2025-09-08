using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LogisticService.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        public BaseController()
        {
        }

        protected HTTPResponseClient<T> Success<T>(T data, string message = "Success", int statusCode = 200)
        {
            return new HTTPResponseClient<T>
            {
                StatusCode = statusCode,
                Message = message,
                DateTime = System.DateTime.Now,
                Data = data
            };
        }

        protected HTTPResponseClient<T> Fail<T>(string message, int statusCode = 400, T? data = default)
        {
            return new HTTPResponseClient<T>
            {
                StatusCode = statusCode,
                Message = message,
                DateTime = System.DateTime.Now,
                Data = data
            };
        }
    }
}
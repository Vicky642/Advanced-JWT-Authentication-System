using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advanced_JWT_Authentication_System.Models.Results
{
    public class ExecutionResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string ErrorDetails { get; set; }
        public string Token { get; set; }

        // Constructor for success without data
        public ExecutionResult(bool success, string message, T data = default, string token = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Token = token;
        }

        // Constructor for success with data
        public ExecutionResult(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        // Constructor for failure
        public ExecutionResult(string message, string errorDetails = null)
        {
            Success = false;
            Message = message;
            ErrorDetails = errorDetails;
        }
    }
}

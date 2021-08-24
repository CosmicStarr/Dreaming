using System;

namespace Dreaming.Errors
{
    public class ApiErrorResponse
    {
        public ApiErrorResponse(int StatusCode, String Message = null)
        {
            statusCode = StatusCode;
            this.Message = Message ?? GetStatusCodes(StatusCode);
        }
        public int statusCode { get; set; }
        public string Message { get; set; }

        private string GetStatusCodes(int statusCodes)
        {
            return statusCodes switch
            {
                400 => "You've made a BadRequest!",
                401 => "You're not Authorized!",
                404 => "What you're looking for does not exist!",
                500 => "The error you made will bring you to a dark place!",
                _   => null
            };
        }
    }
}
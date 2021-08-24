using System.Collections.Generic;

namespace Dreaming.Errors
{
    public class ApiValidation : ApiErrorResponse
    {
        public ApiValidation():base(400)
        {
            
        }
        public IEnumerable<string> Errors { get; set; }
    }
}
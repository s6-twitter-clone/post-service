using System.Net;

namespace post_service.Exceptions
{
    public class InternalServerErrorException : AppException
    {
        public InternalServerErrorException(string message) : base(HttpStatusCode.InternalServerError, message) { }
    }
}

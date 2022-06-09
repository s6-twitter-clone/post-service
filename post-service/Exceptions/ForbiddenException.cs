using System.Net;

namespace post_service.Exceptions;

public class ForbiddenException: AppException
{
    public ForbiddenException(string message) : base (HttpStatusCode.Forbidden, message)
    {

    }
}

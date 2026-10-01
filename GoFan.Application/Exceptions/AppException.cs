using System.Net;

namespace GoFan.Application.Exceptions;

public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public List<string>? Errors { get; }

    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, List<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }
}

public class BadRequestException : AppException
{
    public BadRequestException(string message, List<string>? errors = null)
        : base(message, HttpStatusCode.BadRequest, errors)
    {
    }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}

public class ForbiddenException : AppException
{
    public ForbiddenException(string message = "Forbidden access")
        : base(message, HttpStatusCode.Forbidden)
    {
    }
}

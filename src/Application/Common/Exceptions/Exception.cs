using FluentValidation.Results;

namespace DHAFacilitationAPIs.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base() { }
    public ForbiddenAccessException(string message) : base(message) { }
}

public class UnAuthorizedException : Exception
{
    public UnAuthorizedException(string message) : base(message)
    {

    }
}
public class NoRecordException : Exception
{
    public NoRecordException(string message) : base(message)
    {

    }
}
public class DBOperationException : Exception
{
    public DBOperationException(string message) : base(message)
    {

    }
}
public class RecordAlreadyExistException : Exception
{
    public RecordAlreadyExistException(string message) : base(message)
    {

    }
}
public class ServiceUnavailableException : Exception
{
    public ServiceUnavailableException(string message) : base(message)
    {

    }
}
public class InvalidResponseException : Exception
{
    public InvalidResponseException(string message) : base(message)
    {

    }
}
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {

    }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}




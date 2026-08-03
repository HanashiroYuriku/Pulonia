namespace Pulonia.Domain.Exceptions;

public abstract class PuloniaException : Exception
{
    protected PuloniaException(string message) : base(message) { }

    protected PuloniaException(string message, Exception innerException) : base(message, innerException) { }
}

// Data Conflict exception
public class ConflictException : PuloniaException
{
    public ConflictException(string message) : base(message) { }
    protected ConflictException(string message, Exception innerException) : base(message, innerException) { }
}

// Data Not Found exeption
public class NotFoundException : PuloniaException
{
    public NotFoundException(string message) : base(message) { }
    protected NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

// Unauthorized User exception
public class UnauthorizedException : PuloniaException
{
    public UnauthorizedException(string message) : base(message) { }
    protected UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }
}
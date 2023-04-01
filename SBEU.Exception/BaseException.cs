namespace SBEU.Exceptions
{
    public class BaseException : System.Exception
    {
        public BaseException(string message) : base(message) { }
        public BaseException(string message, Exception? inner) : base(message,inner) { }

    }

    public class EntityNotFoundException : BaseException
    {
        public EntityNotFoundException(string message) : base(message) { }
    }

    public class CriticalRepositoryException : BaseException
    {
        public CriticalRepositoryException(string message) : base(message) { } 
    }
    public class NonDeletableEntityException : BaseException
    {
        public NonDeletableEntityException(string message) : base(message) { }
    }
    public class NoAccessException : BaseException
    {
        public NoAccessException(string message) : base(message) { }
        public NoAccessException(string message,Exception? inner) : base(message,inner) { }

    }
}
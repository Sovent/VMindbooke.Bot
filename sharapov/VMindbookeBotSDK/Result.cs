using System.Xml.Serialization;

namespace VMindbookeBot
{
    public class Result<T, TError>
    {
        
        public Optional<T> OptionalResult { get; }
        public TError Error { get; }
        private Result(Optional<T> some, TError error)
        {
            OptionalResult = some;
            Error = error;
        }
        
        public static Result<T, TError> Some(T result, TError successCode)
        {
            return new Result<T, TError>(Optional<T>.Some(result), successCode);
        }
        
        public static Result<T, TError> None(TError error)
        {
            return new Result<T, TError>(Optional<T>.None(), error);
        }

        public bool HasValue => OptionalResult.HasValue;
    }
}
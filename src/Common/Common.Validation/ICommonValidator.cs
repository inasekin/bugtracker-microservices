namespace Common.Validation;

public interface ICommonValidator<T> where T : class
{
    void ValidateAndThrow(T obj);
}

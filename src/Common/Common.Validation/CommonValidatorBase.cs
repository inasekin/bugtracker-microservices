using FluentValidation;

namespace Common.Validation;
public abstract class CommonValidatorBase<T> : AbstractValidator<T>, ICommonValidator<T> where T : class
{
    public void ValidateAndThrow(T obj)
    {
        var validateResult = base.Validate(obj);
        if (!validateResult.IsValid)
            throw new ValidationException(validateResult.Errors);
    }
}

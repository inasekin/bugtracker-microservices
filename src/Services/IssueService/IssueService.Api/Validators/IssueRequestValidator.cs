using FluentValidation;
using IssueService.Api.Contracts;
using Common.Validation;

namespace IssueService.Api.Validators;

public class IssueRequestValidator : CommonValidatorBase<IssueRequest>
{
    public IssueRequestValidator()
    {
        RuleFor(issue => issue.Title).NotEmpty();
    }
}

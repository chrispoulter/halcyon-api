using FluentValidation;

namespace Halcyon.Api.Features
{
    public class UpdateRequestValidator : AbstractValidator<UpdateRequest>
    {
        public UpdateRequestValidator()
        {
        }
    }
}
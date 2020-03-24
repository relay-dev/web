using Core.Exceptions;
using FluentValidation.Results;
using System.Linq;

namespace Microservices.Validation
{
    public static class ValidatorExtensions
    {
        public static void ThrowIfInvalid(this ValidationResult validationResult)
        {
            if (!validationResult.Errors.Any())
                return;
            
            var validationFailureResult = new Core.Validation.ValidationFailureResult
            {
                Message = "Input validation failed",
                Errors = validationResult.Errors.Select(e => 
                    new Core.Validation.ValidationFailureDetail
                    {
                        Property = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    }).ToArray()
            };

            throw new ValidationException(validationFailureResult);
        }
    }
}

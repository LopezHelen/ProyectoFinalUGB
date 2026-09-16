using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace UGB.MVC.Aplicaciones.Seguras.Helper
{
    public static class FluentHelper
    {
        public static IRuleBuilderOptions<T, string> NotNullOrWhiteSpace<T>
        (this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(x=>!string.IsNullOrWhiteSpace(x));
        }

        public static ErrorResponse ToErrorResponse(this ValidationResult validationResult)
        {
            return new ErrorResponse
            {
                Message = string.Join(" ", validationResult.Errors.Select(x => x.ErrorMessage)),
                StatusCode = 400
            };
        }
    }
}
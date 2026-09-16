using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace UGB.MVC.Validations.UsersValidation
{
    public static class ValidationInjection
    {
        public static IServiceCollection AddValidationInjection(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateUserDTOValidation>();
            return services;
        }
    }
}
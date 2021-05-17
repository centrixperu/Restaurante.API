using Centrix.Encore.Common.Resources;
using Centrix.Encore.Model.Authentication;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Centrix.Encore.ApiGateway.Models.Seguridad
{
    public class AuthUserValidator : AbstractValidator<AuthModel>
    {
        public AuthUserValidator()
        {
            RuleFor(c => c.Username).NotEmpty().WithMessage(UserResource.usuario_cuenta_requerida);
            RuleFor(c => c.Password).NotEmpty().WithMessage(UserResource.usuario_login_contrasena_requerida);
        }
    }
}

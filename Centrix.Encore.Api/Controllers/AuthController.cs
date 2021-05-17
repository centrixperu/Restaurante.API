using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Centrix.Encore.Api.Models.Seguridad;
using Centrix.Encore.Application.Interfaces;
using Centrix.Encore.Common;
using Centrix.Encore.Common.Exceptions;
using Centrix.Encore.Common.Resources;
using Centrix.Encore.Common.Schema;
using Centrix.Encore.Dto.Base;
using Centrix.Encore.IoC;
using Centrix.Encore.Model.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static Centrix.Encore.Common.Constants.Common;

namespace Centrix.Encore.Api.Controllers
{
    [Authorize(AuthenticationSchemes = AuthenticateScheme.Seguridad)]
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Lazy<IAuthApplication> _authApplication;
        private readonly AppSetting _appSettings;

        public AuthController(IOptions<AppSetting> appSettings)
        {
            _authApplication = new Lazy<IAuthApplication>(() => IoCContainer.Current.Resolve<IAuthApplication>());
            _appSettings = appSettings.Value;
        }

        private IAuthApplication AuthApplication
        {
            get { return _authApplication.Value; }
        }

        [AllowAnonymous]
        [HttpPost("AutenticarUsuario")]
        public async Task<JsonResult> AuthUser([FromBody] AuthModel model)
        {
            ResponseDTO response = new ResponseDTO();
            try
            {
                var authUserValidator = new AuthUserValidator().Validate(model);
                if (!authUserValidator.IsValid)
                    throw new FunctionalException(CodigoEstado.Required, CommonResource.campo_requerido, authUserValidator.Errors.Select(c => c.ErrorMessage));
                response = await AuthApplication.AuthUser(model.Username, model.Password);
            }
            catch (FunctionalException ex)
            {
                response = new ResponseDTO { Status = ex.FuntionalCode, Message = ex.Message, Data = ex.Data, TransactionId = ex.TransactionId };
                //Logger.Warning(ex.TransactionId, ex.Message, ex, JsonConvert.SerializeObject(_appSettings));
            }
            catch (TechnicalException ex)
            {
                response = new ResponseDTO { Status = ex.ErrorCode, Message = ex.Message, Data = ex.Data, TransactionId = ex.TransactionId };
                //Logger.Error(ex.TransactionId, ex.Message, ex);
            }
            catch (Exception ex)
            {
                response = new ResponseDTO { Status = CodigoEstado.TechnicalError, Message = ex.Message };
                //Logger.Error(response.TransactionId, ex.Message, ex, JsonConvert.SerializeObject(model));
            }
            return new JsonResult(response);
        }
    }
}

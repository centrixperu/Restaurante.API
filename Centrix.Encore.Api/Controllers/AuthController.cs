using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Centrix.Encore.Common.Exceptions;
using Centrix.Encore.Common.Schema;
using Centrix.Encore.Dto.Base;
using Centrix.Encore.Model.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Centrix.Encore.Common.Constants.Common;

namespace Centrix.Encore.Api.Controllers
{
    [Authorize(AuthenticationSchemes = AuthenticateScheme.Seguridad)]
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("AutenticarUsuario")]
        public async Task<JsonResult> AuthUser([FromBody] AuthModel model)
        {
            ResponseDTO response = new ResponseDTO();
            try
            {

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

using Centrix.Encore.Common;
using Centrix.Encore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Centrix.Encore.Application.Interfaces.ApiGateway;
using Microsoft.Extensions.Options;
using Autofac;
using Centrix.Encore.Dto.Base;
using Centrix.Encore.Model.Aplicacion;
using Centrix.Encore.Common.Exceptions;
using static Centrix.Encore.Common.Constants.Common;

namespace Centrix.Encore.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly Lazy<IAplicacionApplication> _appApplication;
        private readonly AppSetting _appSettings;
        public ApplicationController(IOptions<AppSetting> appSettings, ILifetimeScope lifetimeScope)
        {
            _appApplication = new Lazy<IAplicacionApplication>(() => lifetimeScope.Resolve<IAplicacionApplication>());
            _appSettings = appSettings.Value;
        }

        private IAplicacionApplication AppApplication
        {
            get { return _appApplication.Value; }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("AgregarAplicacion")]
        public async Task<ResponseDTO> AddApp([FromBody] AplicacionModel model)
        {
            var response = new ResponseDTO();
            try
            {
                response = await AppApplication.AddApplication(model);
            }
            catch (FunctionalException ex)
            {
                response = new ResponseDTO { Status = ex.FuntionalCode, Message = ex.Message, Data = ex.Data, TransactionId = ex.TransactionId };
                //Logger.Warning(ex.TransactionId, ex.Message, ex);
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
            return response;
        }
    }
}

using Centrix.Encore.Dto.Base;
using Centrix.Encore.Model.Aplicacion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Centrix.Encore.Application.Interfaces.ApiGateway
{
    public interface IAplicacionApplication
    {
        Task<ResponseDTO> AddApplication(AplicacionModel model);
    }
}

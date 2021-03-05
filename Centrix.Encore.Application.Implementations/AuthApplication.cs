using Centrix.Encore.Application.Interfaces;
using Centrix.Encore.Dto.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Centrix.Encore.Application.Implementations
{
    public class AuthApplication: IAuthApplication
    {
        public async Task<ResponseDTO> AuthUser(string userName, string password)
        {
            var response = new ResponseDTO();
            return response;
        }
    }
}

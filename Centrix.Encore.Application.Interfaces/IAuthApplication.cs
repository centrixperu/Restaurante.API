using Centrix.Encore.Dto.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Centrix.Encore.Application.Interfaces
{
    public interface IAuthApplication
    {
        Task<ResponseDTO> AuthUser(string userName, string password);
    }
}

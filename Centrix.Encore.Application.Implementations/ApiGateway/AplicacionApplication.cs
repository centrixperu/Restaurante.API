using Autofac;
using Centrix.Encore.Application.Interfaces.ApiGateway;
using Centrix.Encore.Common;
using Centrix.Encore.Dto.Base;
using Centrix.Encore.Model.Aplicacion;
using Centrix.Encore.Repository.Interfaces.Data;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Centrix.Encore.Application.Implementations.ApiGateway
{
    public class AplicacionApplication : IAplicacionApplication
    {
        private readonly Lazy<IUnitOfWork> _unitOfWork;
        private readonly AppSetting _settings;


        public AplicacionApplication(IOptions<AppSetting> settings, ILifetimeScope lifetimeScope)
        {
            _settings = settings.Value;
            _unitOfWork = new Lazy<IUnitOfWork>(() => lifetimeScope.Resolve<IUnitOfWork>());
        }
        private IUnitOfWork UnitOfWork
        {
            get
            {
                return _unitOfWork.Value;
            }
        }

        public async Task<ResponseDTO> AddApplication(AplicacionModel model)
        {
            var response = new ResponseDTO();

            //var apiKey = GetRandomNumber();
            //var apiSecret = GetRandomNumber();
            //var salt = GetRandomNumber();

            //await UnitOfWork.Set<ExternalApplicationEntity>().AddAsync(new ExternalApplicationEntity
            //{
            //    ApiKey = apiKey,
            //    ApiSecret = apiSecret,
            //    AppName = model.AppName,
            //    Salt = salt,
            //    Hash = apiKey + apiSecret + salt
            //});
            //UnitOfWork.SaveChanges();
            return response;
        }

        private string GetRandomNumber()
        {
            var key = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);

            return Convert.ToBase64String(key);
        }
    }
}

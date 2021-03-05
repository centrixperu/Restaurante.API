using System;
using System.Collections.Generic;
using System.Text;

namespace Centrix.Encore.Common
{
    public class AppSetting
    {
        public ConnectionString ConnectionStrings { get; set; }
        public JWTConfiguration JWTConfigurations { get; set; }
        public ServiceConfiguration ServicioSAPWS { get; set; }

    }
    public class ConnectionString
    {
        public string DefaultConnection { get; set; }
    }
    public class JWTConfiguration
    {
        public string Secret { get; set; }
        public int ExpirationTimeHours { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }

    public class ServiceConfiguration
    {
        public string URL { get; set; }
        public Credential Credentials { get; set; }
    }

    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

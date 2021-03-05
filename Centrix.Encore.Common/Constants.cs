using System;
using System.Collections.Generic;
using System.Text;

namespace Centrix.Encore.Common
{
    public class Constants
    {
        public struct SystemStatusCode
        {
            public const int Ok = 0;
            public const int TechnicalError = -1;

            public struct Login
            {
                public const int UserFieldsRequired = 1;
                public const int UserCredentialsError = 2;
                public const int UserInformationMissing = 3;
                public const int UsuarioNoSociedad = 4;
                public const int UsuarioNoRolAsignado = 5;
                public const int UsuarioSociedadMultiple = 6;
            }
        }
        public struct Core
        {
            public struct Audit
            {
                public const string CurrentUserId = "CurrentUserId";
                public const string CreationUser = "CreationUser";
                public const string CreationDate = "CreationDate";
                public const string ModificationUser = "ModificationUser";
                public const string ModificationDate = "ModificationDate";
                public const string RowStatus = "RowStatus";
                public const string System = "System";
            }
            public struct UserClaims
            {
                public const string UserName = "UserName";
                public const string Society = "Sociedad";
                public const string ServiceOrganization = "ServiceOrganization";
                public const string Roles = "Roles";
                public const string FullName = "FullName";
            }
        }

        public struct Common
        {
            public struct CodigoEstado
            {
                public const int Required = 1;
                public const int Ok = 0;
                public const int TechnicalError = -1;
                public const int FuncionalError = 2;
            }
        }

        public struct DateTimeFormats
        {
            public const string DD_MM_YYYY = "dd/MM/yyyy";
            public const string DD_MM_YYYY_HH_MM_SS = "dd/MM/yyyy HH:mm:ss";
            public const string DD_MM_YYYY_HH_MM_TT_12 = "dd/MM/yyyy hh:mm tt";
            public const string DD_MM_YYYY_HH_MM_SS_TT_12 = "dd/MM/yyyy hh:mm:ss tt";
            public const string DD_MM_YYYY_HH_MM_24 = "dd/MM/yyyy HH:mm";
            public const string DD_MM_YYYY_HH_MM_SS_FFF = "yyyyMMddHHmmssFFF";
        }
    }
}

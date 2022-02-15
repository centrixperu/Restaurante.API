using System;
using System.Collections.Generic;
using System.Text;

namespace Centrix.Encore.Model.Authentication
{
    public class AuthModel
    {
        //[Required]
        public string Username { get; set; }

        //[Required]
        public string Password { get; set; }

        public string Sucursal { get; set; }
    }
}

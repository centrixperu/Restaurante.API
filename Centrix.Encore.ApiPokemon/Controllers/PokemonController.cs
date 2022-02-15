using Centrix.Encore.Common.Schema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Centrix.Encore.ApiPokemon.Controllers
{
    [Authorize]
    [Authorize(AuthenticationSchemes = AuthenticateScheme.Pokemon)]
    [Route("[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
    }
}

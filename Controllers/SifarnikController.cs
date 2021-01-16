using Fiskal.Model;
using FiskalApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class SifarnikController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        [Route("siframjera/all")]
        public ActionResult GetSifreMjera()
        {
            return Ok(EnumExtensions.GetValues<SifraMjera>());
        }
        [Authorize]
        [HttpGet]
        [Route("vrstaartikla/all")]
        public ActionResult GetVrstaArtikla()
        {
            return Ok(EnumExtensions.GetValues<VrstaArtikla>());
        }
        [Authorize]
        [HttpGet("getNacinPlacanja")]
        public ActionResult GetNacinPlacanja()
        {
            return Ok(EnumExtensions.GetValues<NacinPlacanja>());
        }
    }
}

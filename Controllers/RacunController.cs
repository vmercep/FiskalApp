using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Helpers;
using FiskalApp.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class RacunController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IFiskalizacija _fiskalizacija;
        private IRepositoryWrapper _repoWrapper;
        public RacunController(AppDbContext context, IFiskalizacija fiskalizacija, IRepositoryWrapper repoWrapper)
        {
            _context = context;
            _fiskalizacija = fiskalizacija;
            _repoWrapper = repoWrapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllRacun()
        {
            try
            {
                var appDbContext = _context.Racun.Include(p=>p.User);
                return Ok(await appDbContext.ToListAsync());

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }
        [Authorize]
        [HttpGet("/getRacunByYear/{godina}")]
        public async Task<ActionResult> GetAllRacunByYear(int godina)
        {
            try
            {
                var appDbContext = _context.Racun.Include(p => p.User).Where(x=>x.Godina==godina).OrderByDescending(m=>m.Id);
                return Ok(await appDbContext.ToListAsync());

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }
        [Authorize]
        [HttpGet("/getRacunById/{id}")]
        public async Task<ActionResult<Racun>> GetRacun(int id)
        {
            try
            {
                var appDbContext = _context.Racun.Include(p => p.User).Include(m=>m.StavkeRacuna).Where(x=>x.Id==id);
                if (appDbContext == null) return NotFound();
                return await appDbContext.FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }

        /// <summary>
        /// Metoda za fiskaliziranje računa
        /// </summary>
        /// <param name="racun"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPost("fiskaliziraj")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Racun>> FiskalizirajRacun(Racun racun)
        {
            try
            {
                await _fiskalizacija.FiskalizirajRacun(racun);
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }

        /// <summary>
        /// Metoda za fiskaliziranje računa
        /// </summary>
        /// <param name="racun"></param>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpGet("/fiskalizirajById/{id}")]
        public async Task<ActionResult<Racun>> FiskalizirajRacunbyIdRacuna(int id)
        {
            try
            {
                var racun=await _fiskalizacija.FiskalizirajRacun(id);
                return Ok(racun);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database "+ex.Message);
            }

        }
        [Authorize]
        [HttpGet("getBrojRacuna")]
        public ActionResult<string> GetBrojRacuna()
        {
            try
            {
                
                return Ok(JsonConvert.SerializeObject(_repoWrapper.Racun.GetRacunBroj() ));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fiskal.Model;
using FiskalApp.Model;
using Microsoft.AspNetCore.Http;
using FiskalApp.Contracts;
using FiskalApp.Helpers;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FiskalApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class StavkeRacunaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IRepositoryWrapper _repoWrapper;
        private readonly ILogger<StavkeRacunaController> _logger;
        private readonly IFiskalizacija _fiskalizacija;
        public StavkeRacunaController(AppDbContext context, IRepositoryWrapper repoWrapper, ILogger<StavkeRacunaController> logger, IFiskalizacija fiskalizacija)
        {
            _context = context;
            _repoWrapper = repoWrapper;
            _logger = logger;
            _fiskalizacija = fiskalizacija;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetStavkeRacuna()
        {

            try
            {
                var appDbContext = _context.StavkeRacuna.Include(s => s.Artikl).Include(s => s.Racun);
                return Ok(await appDbContext.ToListAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }
        [Authorize]
        [HttpGet("{racunId}")]
        public async Task<ActionResult> GetStavkeRacuna(int racunId)
        {

            try
            {
                var appDbContext = _context.StavkeRacuna.Include(s => s.Artikl).Where(p=>p.RacunId==racunId);
                return Ok(await appDbContext.ToListAsync());
            }
            catch (Exception e)
            {
                _logger.LogError("Error ocured in StavkeRacunaController " + e);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }

        //[Authorize]
        [HttpPost("insertRacun")]
        public async Task<ActionResult> InsertRacun(Racun racun)
        {

            try
            {
                _logger.LogInformation("Inserting new bill in database "+ JsonSerializer.Serialize(racun));
                _repoWrapper.Racun.PopulateRacun(racun);
                _repoWrapper.Racun.Create(racun);
                foreach(var stavka in racun.StavkeRacuna)
                {
                    _repoWrapper.StavkeRacuna.Create(stavka);
                }
                _repoWrapper.Save();

                return Ok(racun);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database "+e.Message);
            }
        }

        //[Authorize]
        [HttpPost("insertRacunFiskal")]
        public async Task<ActionResult> InsertRacunWithFiskal(Racun racun)
        {

            try
            {
                _logger.LogInformation("Inserting new bill in database " + JsonSerializer.Serialize(racun));
                _repoWrapper.Racun.PopulateRacun(racun);
                _repoWrapper.Racun.Create(racun);
                foreach (var stavka in racun.StavkeRacuna)
                {
                    _repoWrapper.StavkeRacuna.Create(stavka);
                }
                _repoWrapper.Save();
                _logger.LogInformation("Fiskaliziram racun "+racun.BrojRacuna);
                var racunFiskaliziran=await _fiskalizacija.FiskalizirajRacun(racun);

                return Ok(racunFiskaliziran);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error ocured in InsertRacunWithFiskal "+e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database " + e.Message);
            }
        }


    }
}

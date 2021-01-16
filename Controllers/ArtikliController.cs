using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Helpers;
using FiskalApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ArtikliController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IArtikliRepository _artikliRepository;
        public ArtikliController(AppDbContext context, IArtikliRepository artikliRepository)
        {
            _context = context;
            _artikliRepository = artikliRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllArtikli()
        {
            try
            {
                var appDbContext = _context.Artikli;
                return Ok(await appDbContext.ToListAsync());

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetAllArtikliById(int id)
        {
            try
            {
                var appDbContext = _context.Artikli.Where(x=>x.Id==id);
                return Ok(await appDbContext.FirstOrDefaultAsync());

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }
        [Authorize]
        [HttpPut("updateArtikl")]
        public async Task<ActionResult> UpdateArtikl(Artikli artikl)
        {
            try
            {
                await _artikliRepository.UpdateArtikl(artikl);
                return Ok(StatusCodes.Status200OK);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }
        [Authorize]
        [HttpPost("insertArtikl")]
        public async Task<ActionResult> InsertArtikl(Artikli artikl)
        {
            try
            {
                await _artikliRepository.InsertArtikl(artikl);
                return Ok(StatusCodes.Status200OK);

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }
        [Authorize]
        [HttpGet]
        [Route("getSifreArtikla/all")]
        public async Task<ActionResult> GetSifreArtikla()
        {
            try
            {
                var appDbContext = _context.Artikli.Select(u=>u.Sifra);
                return Ok(await appDbContext.ToListAsync());

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }
        [Authorize]
        [HttpGet("getBySifra/{getBySifraArtikla}")]
        public async Task<ActionResult> GetBySifraArtikla(string getBySifraArtikla)
        {
            try
            {
                var appDbContext = _context.Artikli.Where(u=>u.Sifra.Contains(getBySifraArtikla));
                return Ok(await appDbContext.ToListAsync());

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }
    }
}

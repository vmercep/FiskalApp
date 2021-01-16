using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FiskalApp.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsRepository _settingsRepository;
        public SettingsController(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetSettings()
        {
            try
            {
                return Ok(await _settingsRepository.GetSettings());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }

        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateSettings([FromBody]Settings settings)
        {
            try
            {
                if (settings == null) return BadRequest();

                var updatedSetting = await _settingsRepository.UpdateSettings(settings);

                return Ok(updatedSetting);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("addCertificate")]
        public async Task<ActionResult> InsertCertificate(IFormCollection Upload)
        {
            try
            {
                if (Upload.Files.Count > 0)
                {
                    var file = Upload.Files[0];
                    string filename = file.FileName;
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string s = Convert.ToBase64String(fileBytes);
                        await _settingsRepository.SaveCertificateAsync(s, filename);
                    }
                    //If we have at least one file in the IFormCollection, then perform whatever storage actions
                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }

        [Authorize]
        [HttpPut("updateCertificatePassword")]
        public async Task<ActionResult> UpdateCertPassword([FromBody] string password)
        {
            try
            {
                if (password == null) return BadRequest();

                var updatedSetting = await _settingsRepository.UpdateCertificatePassword(password);

                return Ok(updatedSetting);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }


        [Authorize]
        [HttpGet("certificate")]
        public async Task<ActionResult> GetCertificateDetails()
        {
            try
            {
                return Ok(await _settingsRepository.GetCertificateDetails());
            }
            catch (Exception e)
            {
                return new ContentResult() { Content =  e.Message , StatusCode = 211 };
            }

        }


    }
}

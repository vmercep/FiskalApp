using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Helpers;
using FiskalApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FiskalApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController: ControllerBase
    {
        private IUserRepository _userService;
        private AppSettings _appsettings;
        private readonly ILogger _logger;
        public UserController(IUserRepository userService, IOptions<AppSettings> op, ILogger<StavkeRacunaController> logger)
        {
            _userService = userService;
            _appsettings = op.Value;
            _logger = logger;

        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            try
            {
                _logger.LogInformation("Trying to authenticate User" + model.Username);
                var response = _userService.Authenticate(model, _appsettings);

                if (response == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                return Ok(response);

            }
            catch(Exception e)
            {
                _logger.LogInformation(String.Format("Error in user {0} auth {1}"), model.Username, e.Message);
                return BadRequest(new { message = "Username or password is incorrect" });
            }

        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var users = _userService.GetById(id);
            return Ok(users);
        }

        [Authorize]
        [HttpPost("insertuser")]
        public IActionResult InsertUser(Users newUser)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Username or password is incorrect" });
                }


                var user = _userService.InsertUser(newUser);
                if(user == null) return StatusCode(StatusCodes.Status409Conflict);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }

        [Authorize]
        [HttpPut("updateUser")]
        public async Task<ActionResult> UpdateUser([FromBody] Users user)
        {
            try
            {
                if (user == null) return BadRequest();

                var updateduser = await _userService.UpdateUser(user);

                return Ok(updateduser);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error "+e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            } 
        }
        [Authorize]
        [HttpPut("changePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] Users user)
        {
            try
            {
                if (user == null) return BadRequest();

                var updateduser = await _userService.UpdateUserWithPassword(user);

                return Ok(updateduser);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Error " + e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retriving data from database");
            }
        }


    }
}

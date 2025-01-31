using Advanced_JWT_Authentication_System.Interfaces;
using Advanced_JWT_Authentication_System.Models.Authentication;
using Advanced_JWT_Authentication_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Advanced_JWT_Authentication_System.Controllers.Authentication
{
    [Route("Authentication/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            var result = await _userRepository.RegisterAsync(model);

            if (result.Success)
            {
                return Ok(result); // Return the success result with user data
            }

            return BadRequest(result); // Return the error result with message
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _userRepository.LoginAsync(model);

            if (result.Success)
            {
                return Ok(new { Token = result.Token, Message = result.Message });
            }

            return Unauthorized(new { Message = result.Message });
        }
    }
}

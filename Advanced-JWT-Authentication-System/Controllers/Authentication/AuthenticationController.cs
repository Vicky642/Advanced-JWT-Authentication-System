using Advanced_JWT_Authentication_System.Interfaces;
using Advanced_JWT_Authentication_System.Models.Authentication;
using Advanced_JWT_Authentication_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        public AuthenticationController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
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

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state)
        {

            var googleSign = new GoogleSignInModel
            {
                clientId = _configuration["GoogleAuth:ClientId"],
                clientSecret = _configuration["GoogleAuth:ClientSecret"],
                redirectUri = _configuration["GoogleAuth:RedirectUri"],
                codeModel = code
            };
            //var clientId = _configuration["GoogleAuth:ClientId"];
            //var clientSecret = _configuration["GoogleAuth:ClientSecret"];
            //var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            var result = await _userRepository.GoogleSignInAsync(googleSign);

            if (result.Success)
            {
                return Ok(new { Token = result.Token, Message = result.Message });
            }

            return BadRequest(new { Message = result.Message });
        }

        //[HttpPost("GoogleLogin")]
        //public async Task<IActionResult> GoogleLogin([FromBody] string googleToken)
        //{
        //    if (string.IsNullOrEmpty(googleToken))
        //    {
        //        return BadRequest(new { Message = "Google token is required." });
        //    }

        //    var result = await _userRepository.HandleGoogleLoginAsync(googleToken);

        //    if (result.Success)
        //    {
        //        return Ok(new { Token = result.Token, Message = result.Message });
        //    }

        //    return Unauthorized(new { Message = result.Message });
        //}
    }
}

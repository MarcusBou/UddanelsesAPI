using DTOModels;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UddanelsesAPI.Managers;
using UddanelsesAPI.Models;

namespace UddanelsesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration configuration;
        private AuthManager authmanager;
        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.authmanager = new AuthManager(configuration);
        }

        /// <summary>
        /// Register a new user to the api
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOUserModel user)
        {

            user.Username = user.Username.Trim();

            var usr = await authmanager.GetUserByUsername(user.Username);
            if (usr != null)
                return BadRequest("Username already exist");

            await authmanager.Register(user);
            return Ok();
        }

        /// <summary>
        /// Login, and acquire a token that is valid for the REST API
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] DTOUserModel user)
        {
            user.Username = user.Username.Trim();

            var usr = await authmanager.GetUserByUsername(user.Username);
            if (usr == null)
                return BadRequest("Username or Password was incorrect");

            var token = authmanager.Login(user.Password, usr);
            if (token == null)
                return BadRequest("Username or Password was incorrect");

            return Ok(new {token = token});
        }

       
    }
}

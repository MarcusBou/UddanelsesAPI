using DTOModels;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UddanelsesAPI.Models;

namespace UddanelsesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : MyBaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOUserModel user)
        {
            user.Username = user.Username.Trim();
            var usr = await db.Set<User>().Where(x => x.Username == user.Username).FirstOrDefaultAsync();
            if (usr != null)
                return BadRequest("Username already exist");

            var salt = GenerateRandomSalt();
            var hashed = HashPasswordArgon2(Encoding.UTF8.GetBytes(user.Password), salt);

            usr = new User { Username = user.Username, Password = hashed, salt = salt };
            await db.Set<User>().AddAsync(usr);
            await db.SaveChangesAsync();

            return Ok();

        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] DTOUserModel user)
        {
            user.Username = user.Username.Trim();
            var usr = await db.Set<User>().Where(x => x.Username == user.Username).FirstOrDefaultAsync();
            if (usr == null)
                return BadRequest("Username or Password was incorrect");
            var test = HashPasswordArgon2(Encoding.UTF8.GetBytes(user.Password), usr.salt);
            if (!test.SequenceEqual(usr.Password))
                return BadRequest("Username or Password was incorrect");             

            return Ok();
        }

        private byte[] GenerateRandomSalt()
        {
            var buffer = new byte[32];
            var rng = new RNGCryptoServiceProvider();   
            rng.GetBytes(buffer);
            return buffer;
        }

        private Byte[] HashPasswordArgon2(byte[] password, byte[] salt)
        {
            using var argon2 = new Argon2id(password);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8;
            argon2.Iterations = 4;
            argon2.MemorySize = 1024 * 128;

            return argon2.GetBytes(32);
        }
    }
}

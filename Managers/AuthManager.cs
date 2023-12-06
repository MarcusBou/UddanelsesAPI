using DTOModels;
using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UddanelsesAPI.Models;
using System.Security.Cryptography;

namespace UddanelsesAPI.Managers
{
    public class AuthManager : SuperManager
    {
        public AuthManager(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="user">User that needs to be created</param>
        /// <returns></returns>
        public async Task Register(DTOUserModel user)
        {
            var salt = GenerateRandomSalt();
            var hashed = HashPassword(Encoding.UTF8.GetBytes(user.Password), salt);

            // Creates the user model and saves it to the db
            var usr = new User { Username = user.Username, Password = hashed, salt = salt };
            await db.Set<User>().AddAsync(usr);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Try to login with a given password
        /// </summary>
        /// <param name="password">Password for user login</param>
        /// <param name="user">User that is wanted to be logged into</param>
        /// <returns></returns>
        public string? Login(string password, User user)
        {
            var passwrd = HashPassword(Encoding.UTF8.GetBytes(password), user.salt);
            if (!passwrd.SequenceEqual(user.Password))
                return null;

            var token = GenerateToken(user);
            return token;
        }

        /// <summary>
        /// Gets the user from the username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByUsername(string username)
        {
            var usr = await db.Set<User>().Where(x => x.Username == username).FirstOrDefaultAsync();
            return usr;
        }

        /// <summary>
        /// Hash the password, with a salt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public Byte[] HashPassword(byte[] password, byte[] salt)
        {
            using var argon2 = new Argon2id(password);
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8;
            argon2.Iterations = 4;
            argon2.MemorySize = 1024 * 128;

            return argon2.GetBytes(32);
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JwtSetttings:Key").Value!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Iss, configuration.GetSection("JwtSetttings:Issuer").Value!),
                new Claim(JwtRegisteredClaimNames.Aud, configuration.GetSection("JwtSetttings:Audience").Value!),
            };

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        /// <summary>
        /// Generate a random salt
        /// </summary>
        /// <returns></returns>
        private byte[] GenerateRandomSalt()
        {
            var buffer = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}

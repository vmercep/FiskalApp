using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Helpers;
using FiskalApp.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FiskalApp.Repository
{
    public class UserRepository: RepositoryBase<Users>, IUserRepository
    {
        
        public UserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
     
       

        private readonly AppDbContext _appDbContext;

        public AuthenticateResponse Authenticate(AuthenticateRequest model, AppSettings appsettings)
        {
            try
            {
                /* Fetch the stored value */
                string savedPasswordHash = _appDbContext.Users.SingleOrDefault(u => u.UserName == model.Username).Password;
                /* Extract the bytes */
                byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
                /* Get the salt */
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                /* Compute the hash on the password the user entered */
                var pbkdf2 = new Rfc2898DeriveBytes(model.Password, salt, 100000);
                byte[] hash = pbkdf2.GetBytes(20);
                /* Compare the results */
                for (int i = 0; i < 20; i++)
                    if (hashBytes[i + 16] != hash[i])
                        throw new UnauthorizedAccessException();




                var user = _appDbContext.Users.SingleOrDefault(x => x.UserName == model.Username && x.Password == savedPasswordHash);

                // return null if user not found
                if (user == null) return null;

                // authentication successful so generate jwt token
                var token = generateJwtToken(user, appsettings);

                return new AuthenticateResponse(user, token);

            }
            catch(Exception e)
            {
                throw new Exception("error ocured in user authentication "+e.Message);
            }
            
        }

        public IEnumerable<Users> GetAll()
        {
            return _appDbContext.Users.ToList();
        }

        public Users GetById(int id)
        {
            return _appDbContext.Users.FirstOrDefault(x => x.Id == id);
        }

        // helper methods

        private string generateJwtToken(Users user, AppSettings appSettings )
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public Users InsertUser(Users user)
        {
            try
            {
                var userExist = _appDbContext.Users.SingleOrDefault(x => x.UserName == user.UserName);
                if (userExist != null) return null;

                

                user.Password = CalculatePassword(user.Password);

                var userNew = _appDbContext.Users.Add(user);
                _appDbContext.SaveChanges();
                return userNew.Entity;

            }
            catch (Exception e)
            {
                throw new Exception("error ocured in adding user " + e.Message);
            }

        }

        public async Task<Users> UpdateUser(Users user)
        {
            try
            {
                var result = await _appDbContext.Users.FirstOrDefaultAsync(p => p.Id == user.Id);
                if (result != null)
                {
                    result.LastName = user.LastName;
                    result.Oib = user.Oib;
                    result.UserName = user.UserName;
                    result.FirstName = user.FirstName;

                    await _appDbContext.SaveChangesAsync();

                    return result;
                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Users> UpdateUserWithPassword(Users user)
        {
            try
            {
                var result = await _appDbContext.Users.FirstOrDefaultAsync(p => p.Id == user.Id);
                if (result != null)
                {
                    result.LastName = user.LastName;
                    result.Oib = user.Oib;
                    result.UserName = user.UserName;
                    result.FirstName = user.FirstName;
                    result.Password = CalculatePassword(user.Password);

                    await _appDbContext.SaveChangesAsync();

                    return result;
                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        private string CalculatePassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}

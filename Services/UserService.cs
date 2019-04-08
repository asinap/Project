using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using test2.DatabaseContext.Models;
using test2.Helpers;
using Google.Apis.Auth;
using test2.DatabaseContext;
using test2.Entities;

namespace test2.Services
{
    public interface IUserService
    {
        Task<Account> AuthenticateAsync(string _token);
        IEnumerable<Account> GetAll();
    }

    public class UserService : IUserService
    {
        private readonly LockerDbContext _dbContext;
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private readonly AppSettings _appSettings;

        public UserService(LockerDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        public async Task<Account> AuthenticateAsync(string _token)
        {
            try
            {
                GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(_token);
                string id = "";
                string domainmail = "@kmitl.ac.th";
                if (!validPayload.Email.Contains(domainmail))
                {
                    // detect domain email
                    return null;
                }
                id = validPayload.Email.Replace(domainmail, "");
                if (!int.TryParse(id, out int numberic))
                {
                    //before email is not studentID
                    return null;
                }
                if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == id) != null)
                {
                    var _user = GetToken(id);
                    Console.WriteLine("already exist");
                    return _user;
                }
                Account account = new Account()
                {
                    Id_account = id,
                    Email = validPayload.Email,
                    Name = validPayload.Name,
                    Phone = "",
                    Point = 100,
                    Role = Role.User
                };
                _dbContext.Accounts.Add(account);
                _dbContext.SaveChanges();
                var user = GetToken(id);
                return user;
            }
            catch
            {
                Console.WriteLine("AddUserAccount Error");
                return null;

            }
        }

        public Account GetToken (string id_account)
        { 
            var user = _dbContext.Accounts.SingleOrDefault(x => x.Id_account == id_account);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id_account),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            _dbContext.SaveChanges();
            return user;
        }

        public IEnumerable<Account> GetAll()
        {
            // return users without passwords
            return _dbContext.Accounts.Where(x =>  x.Token!=null);
        }

    }
}

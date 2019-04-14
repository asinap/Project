﻿using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Entities;
using test2.Helpers;

namespace test2.Services
{
    public interface IAdminService
    {
        Task<Account> AuthenticateAsync(string _token);
        IEnumerable<Account> GetAll();
    }
    public class AdminService : IAdminService
    {
        private readonly LockerDbContext _dbContext;
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private readonly AppSettings _appSettings;

        public AdminService(LockerDbContext dbContext, IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }

        public async Task<Account> AuthenticateAsync(string _token)
        {
            try
            {
                GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(_token);
                var admin = _dbContext.Accounts.FirstOrDefault(x => x.Email.ToLower() == validPayload.Email.ToLower() && x.Role == Role.Admin);
                if (admin != null)
                {
                    var _user = GetToken(admin.Id_account);
                    return _user;
                }
                else
                {
                    return null;
                }
                    
                
            }
            catch (Exception)
            {
                Console.WriteLine("Error check admin");
                return null;
            }
        }

        public Account GetToken(string id_account)
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
            throw new NotImplementedException();
        }
    }
}

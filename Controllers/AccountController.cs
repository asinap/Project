using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Repositories;
using test2.Services;
using test2.Entities;

namespace test2.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    // [Route("/api/[Controller]")]
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepo;
        private readonly LockerDbContext _dbContext;
        private IUserService _userService;
        private IAdminService _adminService;

        public AccountController(LockerDbContext lockerDbContext, IUserService userService, IAdminService adminService)
        {
            _userService = userService;
            _adminService = adminService;
            _dbContext = lockerDbContext;
            _accountRepo = new AccountRepository(_dbContext);
        }

        [AllowAnonymous]
        [Route("/mobile/usersauthenticate")]
        [HttpPost]
        public async Task<IActionResult> UserAuthenticate([FromBody] Token token)
        {
            var user = await _userService.AuthenticateAsync(token._Token);

            if (user == null)
                return BadRequest(new { message = "Access Denied." });

            return Ok(user.Token);
        }

        [AllowAnonymous]
        [Route("checkToken")]
        [HttpPost]
        public IActionResult CheckToken ([FromBody] Token token)
        {
            var user = _accountRepo.User_Information(token._Token);
            if (user == null)
            {
                return NotFound("Access Denied");
            }
            else
            {
                return Ok(Json(user));
            }
        }
       
        [AllowAnonymous]
        [Route("/web/adminsauthenticate")]
        [HttpPost]
        public async Task<IActionResult> AdminAuthenticate([FromBody] Token token)
        {
            try
            {
                GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(token._Token);
                var admin = await _adminService.AuthenticateAsync(token._Token);
                if (admin!=null)
                {

                    Log.Information("check {name} is admin from web {datetime}.", _dbContext.Accounts.FirstOrDefault(x => x.Email == validPayload.Email).Name, DateTime.Now);
                    return Ok(admin.Token);
                }
                else
                {
                    Log.Information("check {name} is not admin from web {datetime}.", validPayload.Email, DateTime.Now);
                    return BadRequest("No_admin");
                }
            }
            catch
            {
                Console.WriteLine("Error");
                return NotFound("No_admin");
            }
        }

        //[Authorize (Roles = Role.Admin)]
        [Route("/web/AddAdminAccount")]
        [HttpPost]
        public IActionResult AddAdminAccount([FromBody] Account account)
        {
            int result = _accountRepo.AddAdminAccount(account);
            switch (result)
            {
                case 1:
                    Log.Information("Add admin from web {name} {email} account_already_exist.", account.Name, account.Email);
                    return NotFound("account_already_exist");
                case 2:
                    Log.Information("Add user from mobile {name} {email} done.", account.Name, account.Email);
                    return Ok(account.Id_account);
                default:
                    Log.Information("Add user from mobile {name} {email} Error.", account.Name, account.Email);
                    return NotFound("Error");
            }
        }

        [Authorize (Roles = Role.User)]
        [Route("/mobile/AddPhoneNumber")]
        [HttpPut]
        public IActionResult AddPhoneNumber([FromQuery] string id_account, string phone)
        {
            if (_accountRepo.AddPhoneNumber(id_account, phone))
            {
                Log.Information("Add phone from mobile {name} {email} account_already_exist.", _dbContext.Accounts.FirstOrDefault(x=>x.Id_account==id_account).Name, phone );
                return Ok(id_account);
            }
            return NotFound("CannotAddphone");
        }

        [Authorize(Roles = Role.User)]
        [Route("/mobile/Getphone")]
        [HttpGet]
        public IActionResult Getphone (string id_account)
        {
            string result = _accountRepo.GetPhone(id_account);
            if (result!=null)
            {
                return Ok(result);
            }
            return NotFound("Error");
        }
        //[Route("UpdatePoint")]
        //[HttpPut]
        //public IActionResult UpdatePoint([FromQuery] string id, int num)
        //{
        //    if (_accountRepo.UpdatePoint(id, num))
        //    {
        //        return Ok(id);
        //    }
        //    return NotFound();
        //}

        //[Authorize(Roles = Role.User)]
        [Route("/web/UserAccountAll")]
        [HttpGet]
        public IActionResult GetUserAccount()
        {
            var list = _accountRepo.GetUserAccount();
            Log.Information("Get all user from web {datetime}.", DateTime.Now);
            if (list != null)
                return Ok(list);
            else
                return NotFound("No Account");
        }

        //[AllowAnonymous]
        [Authorize(Roles = Role.User)]
        [Route("/mobile/UserAccount")]
        [HttpGet]
        public JsonResult GetUserAccount(string id_account)
        {
            MemberAccount account = _accountRepo.GetUserAccount(id_account);
            Log.Information("Get user account from mobile {name}.", account.Name);
            if (account != null)
                return Json(account);
            else
                return Json(null);        }

        [Authorize( Roles = Role.Admin)]
        [Route("/web/UserOverview")]
        [HttpGet]
        public JsonResult GetUserOverview(string id_account)
        {
            UserOverview user = _accountRepo.GetUserOverview(id_account);
            Log.Information("Get user overview from web {name}.", _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id_account).Name);
            if (user != null)
                return Json(user);
            else
                return Json(null);
        }

        [Route("UserAccountAll")]
        [HttpGet]
        public IActionResult GetUserAccountdev()
        {

            var list = _accountRepo.GetUserAccountdev();
            if (list != null)
                return Ok(list);
            else
                return NotFound("No Account");

        }

        [Route("UserAccount")]
        [HttpGet]
        public IActionResult GetUserAccountdev(string id_account)
        {
            var list = _accountRepo.GetUserAccountdev(id_account);
            if (list != null)
                return Ok(list);
            else
                return NotFound("No Account");

        }

        [Route("AdminAccountAll")]
        [HttpGet]
        public IActionResult GetAdminAccount()
        {
            var list = _accountRepo.GetAdminAccount();
            if (list != null)
                return Ok(list);
            else
                return NotFound("No Account");

        }


        [Route("/web/Admin")]
        [HttpGet]
        public IActionResult GetAdmin ()
        {
            var admin = _accountRepo.GetAdmins();
            if ((admin != null) && (admin.Count() != 0))
            {
                Log.Information("Get all admin from web {datetime}.", DateTime.Now);
                return Ok(admin);
            }
            else
            {
                Log.Information("Cannot Get all admin from web {datetime}.", DateTime.Now);
                return NotFound("No_admin");
            }
        }

        

        [Route("AdminAccount")]
        [HttpGet]
        public IActionResult GetAdminAccount(string id_account)
        {
            var list = _accountRepo.GetAdminAccount(id_account);
            if (list != null)
                return Ok(list);
            else
                return NotFound("No Account");

        }

        [Route("DeleteAll")]
        [HttpDelete]
        public IActionResult Delete ()
        {
            if(_accountRepo.Delete())
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("Delete")]
        [HttpDelete]
        public IActionResult Delete (string id_account)
        {
            if(_accountRepo.Delete(id_account))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
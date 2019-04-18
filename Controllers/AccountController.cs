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
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            try
            {

                var user = await _userService.AuthenticateAsync(token._Token);

                if (user == null)
                {
                    Log.Information("Access Denied. {0}",dateTime);
                    return BadRequest(new { message = "Access Denied." });
                }

                Log.Information("user access {0}. {1}.",user.Name, dateTime);
                return Ok(user.Token);

            }
            catch 
            {
                Log.Information("Error User Authentication", dateTime);
                return NotFound("Error User Authentication");
            }
        }
        [AllowAnonymous]
        [Route("/test/usersauthenticate")]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] Account account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            try
            {

                int user = await _accountRepo.AddUserAccountAsync(account);

                if (user != 4)
                {
                    Log.Information("Access Denied. {0}", dateTime);
                    return BadRequest(new { message = "Access Denied." });
                }
                Log.Information("user access {0}. {1}.", account.Name, dateTime);
                return Ok(user);
            }
            catch
            {
                Log.Information("Error User Authentication");
                return NotFound("Error User Authentication");
            }
        }

        [AllowAnonymous]
        [Route("checkToken")]
        [HttpPost]
        public IActionResult CheckToken ([FromBody] Token token)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            try
            {
                var user = _accountRepo.User_Information(token._Token);
                if (user == null)
                {
                    Log.Information("Access Denied {0}.", dateTime);
                    return NotFound("Access Denied.");
                }
                else
                {
                    Log.Information("check token {0}., {1}.",user.Name, dateTime);
                    return Ok(user);
                }
            }
            catch
            {
                Log.Information("Error CHECKTOKEN");
                return NotFound("Error CHECKTOKEN");
            }
        }

        [AllowAnonymous]
        [Route("notitoken")]
        [HttpPost]
        public IActionResult NotiToken([FromBody] ExpoToken notiToken)
        {
            if (_accountRepo.NotiToken(notiToken))
            {
                //Log.Information("Add vacancy {no}, {location} OK.", vacant.No_vacancy, _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == vacant.Mac_address).Location);
                return Ok(notiToken.Id_account);
            }
            // Log.Information("Cannot Add vacancy {no}, {location} OK.", vacant.No_vacancy, _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == vacant.Mac_address).Location);
            return NotFound();
        }

        [AllowAnonymous]
        [Route("getnotitoken")]
        [HttpGet]
        public IActionResult GetNotiToken()
        {
            var list = _accountRepo.GetNotiToken();
            if (list == null)
            {
                //Log.Information("Add vacancy {no}, {location} OK.", vacant.No_vacancy, _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == vacant.Mac_address).Location);
                return NotFound();
            }            // Log.Information("Cannot Add vacancy {no}, {location} OK.", vacant.No_vacancy, _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == vacant.Mac_address).Location);
            return Ok(list);
        }

        [AllowAnonymous]
        [Route("/web/adminsauthenticate")]
        [HttpPost]
        public async Task<IActionResult> AdminAuthenticate([FromBody] Token token)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            try
            {
                GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(token._Token);
                var admin = await _adminService.AuthenticateAsync(token._Token);
                if (admin!=null)
                {

                    Log.Information("Admin access {0}., {1}.", admin.Name, dateTime);
                    return Ok(admin.Token);
                }
                else
                {
                    Log.Information("Access Denied {0}.", dateTime);
                    return BadRequest("Access Denied.");
                }
            }
            catch
            {
                Log.Information("Error Admin Authentication");
                return NotFound("Error Admin Authentication");
            }
        }

        [AllowAnonymous]
        [Route("/web/AddAdminAccount")]
        [HttpPost]
        public IActionResult AddAdminAccount([FromBody] Account account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            int result = _accountRepo.AddAdminAccount(account);
            switch (result)
            {
                case 1:
                    Log.Information("Add admin from web {Email} account already existed. {0}.", account.Email,dateTime);
                    return BadRequest("Account already existed.");
                case 2:
                    Log.Information("Add user from mobile {name} done. {0}.", account.Name, dateTime);
                    return Ok(account.Id_account);
                default:
                    Log.Information("Add user from mobile {name} error. {0}.", account.Name, dateTime);
                    return NotFound("Error add admin");
            }
        }

        [Authorize (Roles = Role.User)]
        [Route("/mobile/AddPhoneNumber")]
        [HttpPost]
        public IActionResult AddPhoneNumber([FromBody] PhoneUser phone)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            if (_accountRepo.AddPhoneNumber(phone.Id_account, phone.Phone))
            {
                Log.Information("Add phone from mobile {name}. {0}.", _dbContext.accounts.FirstOrDefault(x=>x.Id_account==phone.Id_account).Name, dateTime );
                return Ok(phone.Id_account);
            }
            Log.Information("Cannot Add phone from mobile {0}.", dateTime);
            return NotFound("Cannot Add phone.");
        }

        [Authorize(Roles = Role.User)]
        [Route("/mobile/Getphone")]
        [HttpGet]
        public IActionResult Getphone (string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            string result = _accountRepo.GetPhone(id_account);
            if (result!=null)
            {
                Log.Information("Get phone from mobile {name}. {0}.", _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account).Name, dateTime);
                return Ok(result);
            }
            Log.Information("Cannot Get phone from mobile {0}.", dateTime);
            return NotFound("Cannot Get phone.");
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

        [Authorize (Roles = Role.Admin)]
        [Route("/web/UserAccountAll")]
        [HttpGet]
        public IActionResult GetUserAccount()
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _accountRepo.GetUserAccount();

            if (list != null)
            {
                Log.Information("Get all user from web {0}.", dateTime);
                return Ok(list);
            }
            else
            {
                Log.Information("There is not user account in system {0}.", dateTime);
                return NotFound("There is not user account in system.");
            }
        }

        //[AllowAnonymous]
        [Authorize(Roles = Role.User)]
        [Route("/mobile/UserAccount")]
        [HttpGet]
        public JsonResult GetUserAccount(string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            MemberAccount account = _accountRepo.GetUserAccount(id_account);

            if (account != null)
            {
                Log.Information("Get user account from mobile {0}. {1).", account.Name,dateTime);
                return Json(account);
            }
            else
            {
                Log.Information("Get user account from mobile (0}.", dateTime);
                return Json(null);
            }
        }

        [Authorize( Roles = Role.Admin)]
        [Route("/web/UserOverview")]
        [HttpGet]
        public JsonResult GetUserOverview(string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            UserOverview user = _accountRepo.GetUserOverview(id_account);
            
            if (user != null)
            {
                //Log.Information("Get user overview from web {name}.", _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account).Name);
                return Json(user);
            }
            else
            {
                //Log.Information("Get user overview from web {name}.", _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account).Name);
                return Json(null);
            }
                
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

        [Authorize(Roles = Role.Admin)]
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
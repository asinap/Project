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


        /*Log in with google account for user*/
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
                //if there is no user in database
                if (user == null)
                {
                    Log.Information("Access Denied. {DateTime}",dateTime);
                    return BadRequest(new { message = "Access Denied." });
                }

                // there is user in database
                Log.Information("user access {name}. {DateTime}.",user.Name, dateTime);
                MemberAccount member = new MemberAccount()
                {
                    Id_account=user.Id_account,
                    Name=user.Name,
                    Point=user.Point,
                    Token=user.Token
                };
                return Ok(member);

            }
            catch 
            {
                //error
                Log.Information("Error User Authentication", dateTime);
                return NotFound("Error User Authentication");
            }
        }

        /*TEST Log in with google account for user*/
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
                //if there is no user in database
                if (user != 4)
                {
                    Log.Information("Access Denied. {DateTime}", dateTime);
                    return BadRequest(new { message = "Access Denied." });
                }
                // there is user in database
                Log.Information("user access {Name}. {DateTime}.", account.Name, dateTime);
                return Ok(account.Email);
            }
            catch
            {
                //Error
                Log.Information("Error User Authentication");
                return NotFound("Error User Authentication");
            }
        }

        /*Check token in database for log in automatically in application*/
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
                //if there is no user in database
                if (user == null)
                {
                    Log.Information("Access Denied {DateTime}.", dateTime);
                    return NotFound("Access Denied.");
                }
                else
                // there is user in database
                {
                    Log.Information("check token {Name}., {DateTime}.",user.Name, dateTime);
                    return Ok(user);
                }
            }
            catch
            {
                //Error
                Log.Information("Error CHECKTOKEN");
                return NotFound("Error CHECKTOKEN");
            }
        }

        /*Storing each user's notification token in database*/
        [AllowAnonymous]
        [Route("notitoken")]
        [HttpPost]
        public IActionResult NotiToken([FromBody] ExpoToken notiToken)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            // if it can add notification token 
            if (_accountRepo.NotiToken(notiToken))
            {
                Log.Information("Add notification token {Id} OK. {DateTime}.", notiToken.Id_account,dateTime);
                return Ok(notiToken.Id_account);
            }
            // if it can not add notification token 
            Log.Information("Add notification token {id} Error. {DateTime}.", notiToken.Id_account,dateTime);
            return NotFound("Error to add notification token");
        }

        /*TEST Get each user's notification token*/
        [AllowAnonymous]
        [Route("/test/getnotitoken")]
        [HttpGet]
        public IActionResult GetNotiToken()
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _accountRepo.GetNotiToken();
            //there is no user account in the database
            if (list == null)
            {
                Log.Information("Get notification token Error. {DateTime}.", dateTime);
                return NotFound();
            }
            //there are user accounts in the database
            Log.Information("Get notification token Error. {DateTime}.", dateTime);
            return Ok(list);
        }

        /*Log in with google account for administrator*/
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
                //there is admin account return back to this function
                //if there is admin return to this function
                if (admin!=null)
                {

                    Log.Information("Admin access {name}., {DateTime}.", admin.Name, dateTime);
                    return Ok(admin.Token);
                }
                // there is no admin return to this function
                else
                {
                    Log.Information("Access Denied {DateTime}.", dateTime);
                    return BadRequest("Access Denied.");
                }
            }
            catch
            {
                //error
                Log.Information("Error Admin Authentication");
                return NotFound("Error Admin Authentication");
            }
        }

        /*Register administrator through web application*/
        [AllowAnonymous]
        [Route("/web/AddAdminAccount")]
        [HttpPost]
        public IActionResult AddAdminAccount([FromBody] Account account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            int result = _accountRepo.AddAdminAccount(account);
            //there is a result return to this function
            switch (result)
            {
                case 1:
                    Log.Information("Add admin from web {Email} account already existed. {DateTime}.", account.Email,dateTime);
                    return BadRequest("Account already existed.");
                case 2:
                    Log.Information("Add user from mobile {name} done. {DateTime}.", account.Name, dateTime);
                    return Ok(account.Id_account);
                default:
                    Log.Information("Add user from mobile {name} error. {DateTime}.", account.Name, dateTime);
                    return NotFound("Error add admin");
            }
        }

        /*Add phone number from user through mobile application*/
        [Authorize (Roles = Role.User)]
        [Route("/mobile/AddPhoneNumber")]
        [HttpPost]
        public IActionResult AddPhoneNumber([FromBody] PhoneUser phone)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if success to edit mobile phone 
            if (_accountRepo.AddPhoneNumber(phone.Id_account, phone.Phone))
            {
                Log.Information("Add phone from mobile {name}. {DateTime}.", _dbContext.accounts.FirstOrDefault(x=>x.Id_account==phone.Id_account).Name, dateTime );
                return Ok(phone.Id_account);
            }
            //if not succes to edit mobile phone
            Log.Information("Cannot Add phone from mobile {id}, {DateTime}.",phone.Id_account, dateTime);
            return NotFound("Cannot Add phone.");
        }

        /*Get phone number from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route("/mobile/Getphone")]
        [HttpGet]
        public IActionResult Getphone (string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            string result = _accountRepo.GetPhone(id_account);
            //if there is result back to this function
            if (result!=null && result.Count() != 0)
            {
                Log.Information("Get phone from mobile {name}. {DateTime}.", _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account).Name, dateTime);
                return Ok(result);
            }
            //if there is no result back to this function
            Log.Information("Cannot Get phone from mobile {DateTime}.", dateTime);
            return NotFound("Cannot Get phone.");
        }
  
        /*Get all user account in this system from administrator through web application*/
        [Authorize (Roles = Role.Admin)]
        [Route("/web/UserAccountAll")]
        [HttpGet]
        public IActionResult GetUserAccount()
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _accountRepo.GetUserAccount();
            // if there are user accounts return to this function
            if (list != null&& list.Count()!=0)
            {
                Log.Information("Get all user from web {DateTime}.", dateTime);
                return Ok(list);
            }
            // if there is no user account return to this function
            else
            {
                Log.Information("There is not user account in system {DateTime}.", dateTime);
                return NotFound("There is not user account in system.");
            }
        }

        //[AllowAnonymous]
        /*Get specific user from administrator through web application*/
        [Authorize(Roles = Role.User)]
        [Route("/mobile/UserAccount")]
        [HttpGet]
        public JsonResult GetUserAccount(string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            MemberAccount account = _accountRepo.GetUserAccount(id_account);
            //if there is user account return to this function
            if (account != null )
            {
                Log.Information("Get user account from mobile {name}. {DateTime).", account.Name,dateTime);
                return Json(account);
            }
            //if there is no user account return to this function
            else
            {
                Log.Information("Get user account from mobile {name}, {DateTime}.",id_account, dateTime);
                return Json(null);
            }
        }


        /*Get user information from administrator through web application*/
        [Authorize( Roles = Role.Admin)]
        [Route("/web/UserOverview")]
        [HttpGet]
        public JsonResult GetUserOverview(string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            UserOverview user = _accountRepo.GetUserOverview(id_account);
            //if there is user account return to this function
            if (user != null )
            {
                Log.Information("Get user overview from web {name}.", _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account).Name);
                return Json(user);
            }
            //if there is no user account return to this function
            else
            {
                Log.Information("Get user overview from web {id}.", id_account);
                return Json(null);
            }
                
        }

        /*TEST get all user account in this system*/
        [Route("test/UserAccountAll")]
        [HttpGet]
        public IActionResult GetUserAccountdev()
        {
            var list = _accountRepo.GetUserAccountdev();
            //if there is user account return to this function
            if (list.Count()!= 0)
                return Ok(list);
            //if there is no user account return to this function
            else
                return NotFound("No Account");
        }

        /*TEST get specific user account*/
        [Route("UserAccount")]
        [HttpGet]
        public IActionResult GetUserAccountdev(string id_account)
        {
            var list = _accountRepo.GetUserAccountdev(id_account);
            //if there is user account return to this function
            if (list.Count() != 0)
                return Ok(list);
            //if there is no user account return to this function
            else
                return NotFound("No Account");

        }

        /*TEST get all admin account in this system*/
        [Route("AdminAccountAll")]
        [HttpGet]
        public IActionResult GetAdminAccount()
        {
            var list = _accountRepo.GetAdminAccount();
            //if there is admin account return to this function
            if (list.Count() != 0)
                return Ok(list);
            //if there is no admin account return to this function
            else
                return NotFound("No Account");

        }

        /*Get admin account from web application */
        [Authorize(Roles = Role.Admin)]
        [Route("/web/Admin")]
        [HttpGet]
        public IActionResult GetAdmin ()
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var admin = _accountRepo.GetAdmins();
            //if there is admin account return to this function
            if ((admin != null) && (admin.Count() != 0))
            {
                Log.Information("Get all admin from web {datetime}.", dateTime);
                return Ok(admin);
            }
            //if there is no admin account return to this function
            else
            {
                Log.Information("Cannot Get all admin from web {datetime}.", dateTime);
                return NotFound("No_admin");
            }
        }


        /*TEST get specific admin account in this system*/
        [Route("AdminAccount")]
        [HttpGet]
        public IActionResult GetAdminAccount(string id_account)
        {
            var list = _accountRepo.GetAdminAccount(id_account);
            //if there is admin account return to this function
            if (list.Count() != 0)
                return Ok(list);
            //if there is no admin account return to this function
            else
                return NotFound("No Account");

        }
        
    }
}
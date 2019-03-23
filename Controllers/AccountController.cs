using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Repositories;

namespace test2.Controllers
{
    [Route("/api/[Controller]")]
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepo;
        private readonly LockerDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;

        public AccountController(LockerDbContext lockerDbContext, ILogger<AccountController> logger)
        {
            _dbContext = lockerDbContext;
            _accountRepo = new AccountRepository(_dbContext);
            _logger = logger;
        }

        [Route("/mobile/AddUserAccount")]
        [HttpPost]
        public IActionResult AddUserAccount([FromBody] Account account)
        {
            int result = _accountRepo.AddUserAccount(account);
            switch(result)
            {
                case 1:
                    _logger.LogInformation("Add user from mobile {Name} {email} wrong_domainmail.", account.Name, account.Email);
                    return NotFound("wrong_domainmail");
                case 2:
                    _logger.LogInformation("Add user from mobile {Name} {email} not_student.", account.Name, account.Email);
                    return NotFound("not_student");
                case 3:
                    _logger.LogInformation("Add user from mobile {Name} {email} account_already_exist.", account.Name, account.Email);
                    return NotFound("account_already_exist");
                case 4:
                    _logger.LogInformation("Add user from mobile {Name} {email} done.", account.Name, account.Email);
                    return Ok(account.Id_account);
                default:
                    _logger.LogInformation("Add user from mobile {Name} {email} Error.", account.Name, account.Email);
                    return NotFound("Error");
            }
        }

        [Route("/web/AddAdminAccount")]
        [HttpPost]
        public IActionResult AddAdminAccount([FromBody] Account account)
        {
            int result = _accountRepo.AddAdminAccount(account);
            switch (result)
            {
                case 1:
                    _logger.LogInformation("Add admin from web {name} {email} account_already_exist.", account.Name, account.Email);
                    return NotFound("account_already_exist");
                case 2:
                    _logger.LogInformation("Add user from mobile {name} {email} done.", account.Name, account.Email);
                    return Ok(account.Id_account);
                default:
                    _logger.LogInformation("Add user from mobile {name} {email} Error.", account.Name, account.Email);
                    return NotFound("Error");
            }
        }


        [Route("/mobile/AddPhoneNumber")]
        [HttpPut]
        public IActionResult AddPhoneNumber([FromQuery] string id, string phone)
        {
            if (_accountRepo.AddPhoneNumber(id, phone))
            {
                _logger.LogInformation("Add phone from mobile {name} {email} account_already_exist.", _dbContext.Accounts.FirstOrDefault(x=>x.Id_account==id).Name, phone );
                return Ok(id);
            }
            return NotFound("CannotAddphone");
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

        [Route("/web/UserAccountAll")]
        [HttpGet]
        public IActionResult GetUserAccount()
        {
            var list = _accountRepo.GetUserAccount();
            _logger.LogInformation("Get all user from web {datetime}.", DateTime.Now);
            return Ok(list);
        }

        [Route("/mobile/UserAccount")]
        [HttpGet]
        public JsonResult GetUserAccount(string id_account)
        {
            MemberAccount account = _accountRepo.GetUserAccount(id_account);
            _logger.LogInformation("Get user account from mobile {name}.", _dbContext.Accounts.FirstOrDefault(x=>x.Id_account==id_account).Name);
            return Json(account);
        }

        [Route("/web/UserOverview")]
        [HttpGet]
        public JsonResult GetUserOverview(string id_account)
        {
            UserOverview user = _accountRepo.GetUserOverview(id_account);
            _logger.LogInformation("Get user overview from web {name}.", _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id_account).Name);
            return Json(user);
        }

        [Route("UserAccountAll")]
        [HttpGet]
        public IActionResult GetUserAccountdev()
        {
            var list = _accountRepo.GetUserAccountdev();
            return Ok(list);
        }

        [Route("UserAccount")]
        [HttpGet]
        public IActionResult GetUserAccountdev(string id_account)
        {
            var list = _accountRepo.GetUserAccountdev(id_account);
            return Ok(list);
        }

        [Route("AdminAccountAll")]
        [HttpGet]
        public IActionResult GetAdminAccount()
        {
            var list = _accountRepo.GetAdminAccount();
            return Ok(list);
        }

        [Route("/web/Admin")]
        [HttpGet]
        public IActionResult GetAdmin ()
        {
            var admin = _accountRepo.GetAdmins();
            if ((admin != null) && (admin.Count() != 0))
            {
                _logger.LogInformation("Get all admin from web {datetime}.", DateTime.Now);
                return Ok(admin);
            }
            else
            {
                _logger.LogInformation("Cannot Get all admin from web {datetime}.", DateTime.Now);
                return NotFound("No_admin");
            }
        }

        [Route("/web/IsAdmin")]
        [HttpGet]
        public IActionResult IsAdmin(string accountID)
        {
            if (_accountRepo.IsAdmin(accountID))
            {
                _logger.LogInformation("check {name} is admin from web {datetime}.",_dbContext.Accounts.FirstOrDefault(x=>x.Id_account==accountID).Name, DateTime.Now);
                return Ok(accountID);
            }
            else
            {
                _logger.LogInformation("check {name} is not admin from web {datetime}.", accountID, DateTime.Now);
                return NotFound("No_admin");
            }
        }
        [Route("AdminAccount")]
        [HttpGet]
        public IActionResult GetAdminAccount(string id_account)
        {
            var list = _accountRepo.GetAdminAccount(id_account);
            return Ok(list); 
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
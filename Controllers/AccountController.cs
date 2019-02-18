using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public AccountController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _accountRepo = new AccountRepository(_dbContext);
        }

        [Route("/mobile/AddUserAccount")]
        [HttpPost]
        public IActionResult AddUserAccount([FromBody] Account account)
        {
            if (_accountRepo.AddUserAccount(account))
            {
                return Ok(account.Id_account);
            }
            return NotFound("already_existed");
        }

        [Route("/web/AddAdminAccount")]
        [HttpPost]
        public IActionResult AddAdminAccount([FromBody] Account account)
        {
            if (_accountRepo.AddAdminAccount(account))
            {
                return Ok(account.Id_account);
            }
            return NotFound();
        }


        [Route("/mobile/AddPhoneNumber")]
        [HttpPut]
        public IActionResult AddPhoneNumber([FromQuery] string id, string phone)
        {
            if (_accountRepo.AddPhoneNumber(id, phone))
            {
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
            return Ok(list);
        }

        [Route("/mobile/UserAccount")]
        [HttpGet]
        public JsonResult GetUserAccount(string id)
        {
            MemberAccount account = _accountRepo.GetUserAccount(id);
            return Json(account);
        }

        [Route("/web/UserOverview")]
        [HttpGet]
        public JsonResult GetUserOverview(string id)
        {
            UserOverview user = _accountRepo.GetUserOverview(id);
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
        public IActionResult GetUserAccountdev(string id)
        {
            var list = _accountRepo.GetUserAccountdev(id);
            return Ok(list);
        }

        [Route("AdminAccountAll")]
        [HttpGet]
        public IActionResult GetAdminAccount()
        {
            var list = _accountRepo.GetAdminAccount();
            return Ok(list);
        }

        [Route("AdminAccount")]
        [HttpGet]
        public IActionResult GetAdminAccount(string id)
        {
            var list = _accountRepo.GetAdminAccount(id);
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
        public IActionResult Delete (string id)
        {
            if(_accountRepo.Delete(id))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
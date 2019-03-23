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
    public class LockerMetadataController : Controller
    {
        private readonly LockerMetadataRepository _lockerRepo;
        private readonly LockerDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;


        public LockerMetadataController(LockerDbContext lockerDbContext, ILogger<AccountController> logger)
        {
            _dbContext = lockerDbContext;
            _lockerRepo = new LockerMetadataRepository(_dbContext);
            _logger = logger;
        }

        
        [Route("AddLocker")]
        [HttpPost]
        public IActionResult AddLocker([FromBody] LockerMetadata locker)
        {
            if (_lockerRepo.AddLocker(locker))
            {
                _logger.LogInformation("Add Locker {Location} OK.", locker.Location);
                return Ok(locker.Mac_address);
            }
            _logger.LogInformation("Add Locker {Location} Error.", locker.Location);
            return NotFound();
        }

        [Route("DeleteLocker")]
        [HttpPut]
        public IActionResult DeleteLocker([FromQuery] string Mac_address)
        {
            if (_lockerRepo.DeleteLocker(Mac_address))
            {
                _logger.LogInformation("Delete Locker {Location} OK.", _dbContext.LockerMetadatas.FirstOrDefault(x=>x.Mac_address==Mac_address).Location);
                return Ok();
            }
            _logger.LogInformation("Delete Locker {Location} Error.", Mac_address);
            return NotFound();

        }

        [Route("RestoreLocker")]
        [HttpPut]
        public IActionResult RestoreLocker([FromQuery] string Mac_address)
        {
            if (_lockerRepo.RestoreLocker(Mac_address))
            {
                _logger.LogInformation("Restore Locker {Location} OK.", _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return Ok(Mac_address);
            }
            _logger.LogInformation("Restore Locker {Location} Error.", Mac_address);
            return NotFound();
        }

        [Route("/web/Locker")]
        [HttpGet]
        public IActionResult GetLocker()
        {
            var list = _lockerRepo.GetLocker();
            _logger.LogInformation("Get Locker from web {datetime}.", DateTime.Now);
            return Ok(list);
        }

        [Route("/web/lockerDetail")]
        [HttpGet]
        public JsonResult GetLockerDetail(string mac_address)
        {
            LockerDetail lockerDetail = _lockerRepo.GetLockerDetail(mac_address);
            _logger.LogInformation("Get Locker Detail from web {datetime}.", DateTime.Now);
            return Json(lockerDetail);
        }
        [Route("LockerMac")]
        [HttpGet]
        public IActionResult GetLocker(string mac_address)
        {
            var list = _lockerRepo.GetLocker(mac_address);
            return Ok(list);
        }

        [Route("ActiveLocker")]
        [HttpGet]
        public IActionResult ActiveLocker()
        {
            var list = _lockerRepo.GetActivelocker();
            return Ok(list);
        }

        [Route ("Inactivelocker")]
        [HttpGet]
        public IActionResult InactiveLocker()
        {
            var list = _lockerRepo.GetInactivelocker();
            return Ok(list);
        }

        [Route("DeleteAll")]
        [HttpDelete]
        public IActionResult Delete()
        {
            if (_lockerRepo.Delete())
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("Delete")]
        [HttpDelete]
        public IActionResult Delete(string mac)
        {
            if (_lockerRepo.Delete(mac))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
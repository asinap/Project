using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Entities;
using test2.Repositories;

namespace test2.Controllers
{

    [Route("/api/[Controller]")]
    public class LockerMetadataController : Controller
    {
        private readonly LockerMetadataRepository _lockerRepo;
        private readonly LockerDbContext _dbContext;


        public LockerMetadataController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _lockerRepo = new LockerMetadataRepository(_dbContext);
        }

        
        [Route("AddLocker")]
        [HttpPost]
        public IActionResult AddLocker([FromBody] LockerMetadata locker)
        {
            if (_lockerRepo.AddLocker(locker))
            {
                Log.Information("Add Locker {Location} OK.", locker.Location);
                return Ok(locker.Mac_address);
            }
            Log.Information("Add Locker {Location} Error.", locker.Location);
            return NotFound();
        }

        [Route("DeleteLocker")]
        [HttpPut]
        public IActionResult DeleteLocker([FromQuery] string Mac_address)
        {
            if (_lockerRepo.DeleteLocker(Mac_address))
            {
                Log.Information("Delete Locker {Location} OK.", _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return Ok();
            }
            if (_dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address) != null)
            {
                Log.Information("Delete Locker {Location} Error.", _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return NotFound();
            }
            else
            {
                Log.Information("Delete Locker No {Mac_addressn} Error.", Mac_address);
                return NotFound();
            }

        }

        [Route("RestoreLocker")]
        [HttpPut]
        public IActionResult RestoreLocker([FromQuery] string Mac_address)
        {
            if (_lockerRepo.RestoreLocker(Mac_address))
            {
                Log.Information("Restore Locker {Location} OK.", _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return Ok(Mac_address);
            }
            if (_dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address) != null)
            {
                Log.Information("Restore Locker {Location} Error.", _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return NotFound();
            }
            else
            {
                Log.Information("Restore Locker {Location} Error.", Mac_address);
                return NotFound();
            }
            
        }
        [Authorize(Roles = Role.Admin)]
        [Route("/web/Locker")]
        [HttpGet]
        public IActionResult GetLocker()
        {
            var list = _lockerRepo.GetLocker();
            Log.Information("Get Locker from web {datetime}.", DateTime.Now);
            return Ok(list);
        }
        [Authorize(Roles = Role.Admin)]
        [Route("/web/lockerDetail")]
        [HttpGet]
        public JsonResult GetLockerDetail(string mac_address)
        {
            LockerDetail lockerDetail = _lockerRepo.GetLockerDetail(mac_address);
            Log.Information("Get Locker Detail from web {datetime}.", DateTime.Now);
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
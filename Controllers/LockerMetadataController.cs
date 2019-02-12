using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
                return Ok(locker.Mac_address);
            }
            return NotFound();
        }

        [Route("DeleteLocker")]
        [HttpPut]
        public IActionResult DeleteLocker([FromQuery] string Mac_address)
        {
            if (_lockerRepo.DeleteLocker(Mac_address))
            {
                return Ok();
            }
            return NotFound();

        }

        [Route("RestoreLocker")]
        [HttpPut]
        public IActionResult RestoreLocker([FromQuery] string Mac_address)
        {
            if (_lockerRepo.RestoreLocker(Mac_address))
            {
                return Ok(Mac_address);
            }
            return NotFound();
        }

        [Route("LockerAll")]
        [HttpGet]
        public IActionResult GetLocker()
        {
            var list = _lockerRepo.GetLocker();
            return Ok(list);
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
        public IActionResult Delete(string id)
        {
            if (_lockerRepo.Delete(id))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
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
        [HttpDelete]
        public IActionResult DeleteLocker([FromQuery] string Mac_address)
        {
            if (_lockerRepo.DeleteLocker(Mac_address))
            {
                return Ok();
            }
            return NotFound();

        }

        [Route("UpdateActiveLocker")]
        [HttpPut]
        public IActionResult UpdateLocker([FromQuery] string Mac_address)
        {
            if (_lockerRepo.UpdateActive(Mac_address))
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
    }
}
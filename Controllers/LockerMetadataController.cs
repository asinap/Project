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

        /*Adding locker through web application by administrator*/
        [AllowAnonymous]
        [Route("/web/AddLocker")]
        [HttpPost]
        public IActionResult AddLocker([FromBody] LockerMetadata locker)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if adding locker success
            if (_lockerRepo.AddLocker(locker))
            {
                Log.Information("Add Locker {mac} OK. {DateTime}.", locker.Mac_address, dateTime);
                return Ok(locker.Mac_address);
            }
            //if adding locker fail
            Log.Information("Add Locker {mac} Error. {DateTime}.", locker.Mac_address, dateTime);
            return NotFound(locker.Mac_address);
        }

        /*Editing locker through web application by administrator*/
        [AllowAnonymous]
        [Route("/web/EditLocker")]
        [HttpPost]
        public IActionResult EditLocker([FromBody] LockerMetadata locker)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if editing locker success
            if (_lockerRepo.EditLocker(locker))
            {
                Log.Information("Edit Locker {mac} OK. {DateTime}.", locker.Mac_address, dateTime);
                return Ok(locker.Mac_address);
            }
            //if editing locker fail
            Log.Information("Edit Locker {mac} Error. {DateTime}.", locker.Mac_address, dateTime);
            return NotFound(locker.Mac_address);
        }

        /*Deleting locker through web application by administrator and set locker by active to be false*/
        [AllowAnonymous]
        [Route("/web/DeleteLocker")]
        [HttpPost]
        public IActionResult DeleteLocker([FromBody] LockerForm locker)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if deletinfg locker success
            if (_lockerRepo.DeleteLocker(locker.Mac_address))
            {
                Log.Information("Delete Locker {Mac_address} OK. {DateTime}.", locker.Mac_address, dateTime);
                return Ok(locker.Mac_address);
            }
            //if deletinfg locker fail
            else
            {
                Log.Information("Delete Locker No {Mac_address} Error. {DateTime}.", locker.Mac_address, dateTime);
                return NotFound(locker.Mac_address);
            }

        }

        /*Restore locker through web application by administrator and set locker by active to be true*/
        [AllowAnonymous]
        [Route("/web/RestoreLocker")]
        [HttpPost]
        public IActionResult RestoreLocker([FromBody] LockerForm locker)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if restore locker success
            if (_lockerRepo.RestoreLocker(locker.Mac_address))
            {
                Log.Information("Restore Locker {Mac_address} OK. {DateTime}.", locker.Mac_address, dateTime);
                return Ok(locker.Mac_address);
            }
            //if restore locker fail
            else
            {
                Log.Information("Restore Locker {Mac_address} Error. {DateTime}.", locker.Mac_address, dateTime);
                return NotFound(locker.Mac_address);
            }
            
        }

        /*Get locker from adminstrator through web application*/
        [Authorize(Roles = Role.Admin)]
        [Route("/web/Locker")]
        [HttpGet]
        public IActionResult GetLocker()
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _lockerRepo.GetLocker();
            Log.Information("Get Locker from web {datetime}.", dateTime);
            return Ok(list);
        }

        [Authorize(Roles = Role.User)]
        [Route("/mobile/Locker")]
        [HttpGet]
        public IActionResult GetLockerMobile()
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _lockerRepo.GetLockerMobile();
            Log.Information("Get Locker from mobile {datetime}.", dateTime);
            return Ok(list);
        }

        /*Get locker detail from adminstrator through web application*/
        [Authorize(Roles = Role.Admin + "," + Role.User)]
        [Route("/web/lockerDetail")]
        [HttpGet]
        public JsonResult GetLockerDetail(string mac_address)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            LockerDetail lockerDetail = _lockerRepo.GetLockerDetail(mac_address);
            Log.Information("Get Locker Detail from web {datetime}.", dateTime);
            return Json(lockerDetail);
        }

        /*TEST get all locker*/
        [Route("LockerMac")]
        [HttpGet]
        public IActionResult GetLocker(string mac_address)
        {
            var list = _lockerRepo.GetLocker(mac_address);
            return Ok(list);
        }

        /*TEST get active locker*/
        [Route("ActiveLocker")]
        [HttpGet]
        public IActionResult ActiveLocker()
        {
            var list = _lockerRepo.GetActivelocker();
            return Ok(list);
        }

        /*TEST get inactive locker*/
        [Route ("Inactivelocker")]
        [HttpGet]
        public IActionResult InactiveLocker()
        {
            var list = _lockerRepo.GetInactivelocker();
            return Ok(list);
        }

 
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using test2.DatabaseContext;
using test2.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace test2.Controllers
{
    [Route("/api/[Controller]")]
    public class HardwareController : Controller
    {
        private readonly HardwareRepository _hardwareRepo;
        private readonly LockerDbContext _dbContext;

        public HardwareController(LockerDbContext lockerDbContext, ILogger<AccountController> logger)
        {
            _dbContext = lockerDbContext;
            _hardwareRepo = new HardwareRepository(_dbContext);
        }

        [AllowAnonymous]
        /*Get information from hardware to open the locker*/
        [Route("/HW/getHardware")]
        [HttpGet]
        public JsonResult GetHardware(string userID, string code, string mac_address)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var result = _hardwareRepo.GetHardware(userID, code, mac_address);
            //if there is no result 
            if (result == null)
            {
                Log.Information("Cannot Get hardware from node-red. {DateTime}.", dateTime);
                return null;
            }
            //if there is result 
            else
            {
                Log.Information("Get hardware from node-red {name} {location} {no_vacancy}. {DateTime}."
                    , _dbContext.accounts.FirstOrDefault(x => x.Id_account == userID).Name
                    , _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location
                    , _dbContext.vacancies.FirstOrDefault(x=>x.Id_vacancy==_dbContext.reservations.FirstOrDefault(y=>y.Id_account==userID&&y.Code==code).Id_vacancy).No_vacancy
                    , dateTime);
                return Json(result);
            }
                
        }
    }
}
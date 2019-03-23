using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using test2.DatabaseContext;
using test2.Repositories;

namespace test2.Controllers
{
    [Route("/api/[Controller]")]
    public class HardwareController : Controller
    {
        private readonly HardwareRepository _hardwareRepo;
        private readonly LockerDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;

        public HardwareController(LockerDbContext lockerDbContext, ILogger<AccountController> logger)
        {
            _dbContext = lockerDbContext;
            _hardwareRepo = new HardwareRepository(_dbContext);
            _logger = logger;
        }

        [Route("/HW/getHardware")]
        [HttpGet]
        public JsonResult GetHardware(string userID, string code, string mac_address)
        {
            var result = _hardwareRepo.GetHardware(userID, code, mac_address);
            _logger.LogInformation("Get hardware from node-red {name} {location}.", _dbContext.Accounts.FirstOrDefault(x => x.Id_account == userID).Name, _dbContext.LockerMetadatas.FirstOrDefault(x=>x.Mac_address==mac_address).Location);
            return Json(result);
        }
    }
}
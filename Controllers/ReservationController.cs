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
    public class ReservationController : Controller
    {
        private readonly ReservationRepository _reserveRepo;
        private readonly LockerDbContext _dbContext;


        public ReservationController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _reserveRepo = new ReservationRepository(_dbContext);
        }

        [Route("AddReserve")]
        [HttpPost]
        public IActionResult AddReservation([FromBody] Reservation reserve)
        {
            int result = _reserveRepo.AddReservation(reserve);
            switch(result)
            {
                case 1: return NotFound("account is not existed.");
                case 2: return NotFound("No avaliable vacant.");
                case 3: return NotFound("Cannot find size requirement");
                case 4: return Ok(reserve.Id_reserve);
                default: return NotFound("Error");

            }
            //{
            //    return Ok(reserve.Id_reserve);
            //}
            //return NotFound();
        }

        [Route("CancelReserve")]
        [HttpDelete]
        public IActionResult CancelReservation([FromQuery] int id)
        {
            if (_reserveRepo.CancelReseveration(id))
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("ReserveAll")]
        [HttpGet]
        public IActionResult GetNotification()
        {
            var list = _reserveRepo.GetResverve();
            return Ok(list);
        }

    }
}
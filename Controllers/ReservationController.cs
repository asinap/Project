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
        public IActionResult AddReservation([FromBody] Reservation reserve,string size, int optional)
        {
            if (_reserveRepo.AddReservation(reserve,size,optional))
            {
                return Ok(reserve.Id_reserve);
            }
            return NotFound();
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

    }
}
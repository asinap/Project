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
                case 4: return NotFound("No point");
                case 5: return Ok(reserve.Id_reserve);
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
            int result = _reserveRepo.CancelReseveration(id);
            switch (result)
            {
                case 1: return Ok(id);
                case 2: return NotFound("Cannot cancel cause time");
                case 3: return NotFound("Reservation is not existed");
                default: return NotFound("Error");
            }
        }

        [Route ("SetCode")]
        [HttpPut]
        public IActionResult SetCode (int id,string code)
        {
            if(_reserveRepo.SetCode(id,code)==1)
            {
                string result = string.Format("id_reserve : {0}, code : {1}",id,code); 
                return Ok(result);
            }
            return NotFound("Error to set code");
        }

        [Route("ReserveAll")]
        [HttpGet]
        public IActionResult GetNotification()
        {
            var list = _reserveRepo.GetReserve();
            return Ok(list);
        }

        [Route("ReserveID")]
        [HttpGet]
        public IActionResult GetNotification(string id)
        {
            var list = _reserveRepo.GetReserve(id);
            return Ok(list);
        }

        [Route("Pending")]
        [HttpGet]
        public IActionResult Pending (string id)
        {
            var list = _reserveRepo.Pending(id);
            return Ok(list);
        }

        [Route("History")]
        [HttpGet]
        public IActionResult History (string id)
        {
            var list = _reserveRepo.History(id);
            return Ok(list);
        }

        [Route ("Count")]
        [HttpGet]
        public JsonResult CountUser (string id)
        {
            int unuse = _reserveRepo.Unuse(id);
            int use = _reserveRepo.Use(id);
            int penalty = _reserveRepo.Penalty(id);
            int expire = _reserveRepo.Expire(id);
 //           string result = String.Format("Unuse:{0},Use:{1},Penalty:{2},Expire:{3}",unuse,use,penalty,expire);
            Counter counter = new Counter()
            {
                Unuse = unuse,
                Use = use,
                Penalty = penalty,
                Expire = expire
            };


            return Json(counter);
        }

        [Route("SetState")]
        [HttpPut]
        public IActionResult SetState (int reserveID, string condition)
        {
            if(_reserveRepo.SetStatus(reserveID,condition)==1)
            {
                string result = String.Format("{0}:{1}",reserveID,condition);
                return Ok(result);
            }
            else if (_reserveRepo.SetStatus(reserveID,condition)==2)
            {
                string result = String.Format("{0}:{1}", reserveID, condition);
                return Ok(result);
            }
            else
            {
                return NotFound("Error to Set state");
            }

        }

        [Route("DeleteAll")]
        [HttpDelete]
        public IActionResult Delete()
        {
            if (_reserveRepo.Delete())
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("Delete")]
        [HttpDelete]
        public IActionResult Delete(int id_noti)
        {
            if (_reserveRepo.Delete(id_noti))
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
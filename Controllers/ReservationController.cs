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

        [Route("/mobile/AddReserve")]
        [HttpPost]
        public IActionResult AddReservation([FromBody] Reservation reserve)
        {
            int result = _reserveRepo.AddReservation(reserve);
            switch(result)
            {
                case 1: return NotFound("account_is_not_existed.");
                case 2: return NotFound("No_avaliable_vacant.");
                case 3: return NotFound("Cannot_find_size_requirement");
                case 4: return NotFound("No_point");
                case 5: return Ok(reserve.Id_reserve);
                default: return NotFound("Error");

            }
            //{
            //    return Ok(reserve.Id_reserve);
            //}
            //return NotFound();
        }

        [Route("/mobile/CancelReserve")]
        [HttpDelete]
        public IActionResult CancelReservation([FromQuery] int id)
        {
            int result = _reserveRepo.CancelReseveration(id);
            switch (result)
            {
                case 1: return Ok(id);
                case 2: return NotFound("Cannot_cancel_cause_time");
                case 3: return NotFound("Reservation_is_not_existed");
                default: return NotFound("Error");
            }
        }

        [Route ("/mobile/SetCode")]
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

        [Route ("/web/Activity")]
        [HttpGet]
        public JsonResult GetActivity()
        {
            var list = _reserveRepo.GetActivities();
            return Json(list);
        }

        [Route("/web/Notification")]
        [HttpGet]
        public JsonResult GetNotification()
        {
            var list = _reserveRepo.GetNotification();
            return Json(list);
        }

        [Route ("/web/ReserveDetail")]
        [HttpGet]
        public JsonResult GetReserveDetail(int id_reserve)
        {
            ReserveDetail detail = _reserveRepo.GetReserveDetail(id_reserve);
            return Json(detail);
        }

        [Route("ReserveAll")]
        [HttpGet]
        public IActionResult GetReserve()
        {
            var list = _reserveRepo.GetReserve();
            return Ok(list);
        }

        [Route("ReserveID")]
        [HttpGet]
        public IActionResult GetReserve(string id)
        {
            var list = _reserveRepo.GetReserve(id);
            return Ok(list);
        }

        [Route("/mobile/Pending")]
        [HttpGet]
        public JsonResult Pending (string id)
        {
            var list = _reserveRepo.Pending(id);
            return Json(list);
        }

        [Route("/mobile/History")]
        [HttpGet]
        public JsonResult History (string id)
        {
            var list = _reserveRepo.History(id);
            return Json(list);
        }

        [Route ("/mobile/BookingDetail")]
        [HttpGet]
        public JsonResult BookingDetail (int id)
        {
            BookingForm result = _reserveRepo.GetBookingDetail(id);
            return Json(result);
        }


        [Route ("Count")]
        [HttpGet]
        public JsonResult CountUser (string id)
        {
            int unuse = _reserveRepo.Unuse(id);
            int use = _reserveRepo.Use(id);
            int penalty = _reserveRepo.TimeUp(id);
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
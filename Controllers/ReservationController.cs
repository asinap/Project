﻿using System;
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
        public IActionResult SetCode (int id_reserve,string code)
        {
            int result = _reserveRepo.SetCode(id_reserve, code);
            if(result==1)
            {
                string _result = string.Format("id_reserve : {0}, code : {1}",id_reserve,code); 
                return Ok(_result);
            }
            else if(result==2)
            {
                return NotFound("Code_is_already_set");
            }
            else if (result==3)
            {
                return NotFound("Code_is_duplicated");
            }
            else
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
        public IActionResult GetReserve(string id_account)
        {
            var list = _reserveRepo.GetReserve(id_account);
            return Ok(list);
        }

        [Route("/mobile/Pending")]
        [HttpGet]
        public JsonResult Pending (string id_account)
        {
            var list = _reserveRepo.Pending(id_account);
            return Json(list);
        }

        [Route("/mobile/History")]
        [HttpGet]
        public JsonResult History (string id_account)
        {
            var list = _reserveRepo.History(id_account);
            return Json(list);
        }

        [Route ("/mobile/BookingDetail")]
        [HttpGet]
        public JsonResult BookingDetail (int id_reserve)
        {
            BookingForm result = _reserveRepo.GetBookingDetail(id_reserve);
            return Json(result);
        }


        [Route ("Count")]
        [HttpGet]
        public JsonResult CountUser (string id_account)
        {
            int unuse = _reserveRepo.Unuse(id_account);
            int use = _reserveRepo.Use(id_account);
            int penalty = _reserveRepo.TimeUp(id_account);
            int expire = _reserveRepo.Expire(id_account);
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

        [Route("SetBoolIsActive")]
        [HttpPut]
        public IActionResult SetBoolIsActive(int reserveID, bool _trueOr_false)
        {
            int result = _reserveRepo.SetBoolIsActive(reserveID, _trueOr_false);
            if (result == 2)
            {
                string _result = String.Format("{0}:{1}", reserveID, _trueOr_false);
                return Ok(result);
            }
            else if (result == 1)
            {

                return NotFound("No_reserveID");
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
        public IActionResult Delete(int id_reserve)
        {
            if (_reserveRepo.Delete(id_reserve))
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
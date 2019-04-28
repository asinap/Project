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
    public class ReservationController : Controller
    {
        private readonly ReservationRepository _reserveRepo;
        private readonly LockerDbContext _dbContext;


        public ReservationController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _reserveRepo = new ReservationRepository(_dbContext);
        }

        /*Add reservation from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route("/mobile/AddReserve")]
        [HttpPost]
        public IActionResult AddReservation([FromBody] ReservationForm reserve)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            int result = _reserveRepo.AddReservation(reserve);
            if(result == 0)
            {
                Log.Information("Add reservation Error. {DateTime}, {1}.", reserve.Id_account, dateTime);
                return NotFound("Error");
            }
            else
            {
                Log.Information("Add reservation {accountID} done. {DateTime}.", reserve.Id_account);
                return Ok(result);

            }

        }

        /*Cancel reservation from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route("/mobile/CancelReserve")]
        [HttpDelete]
        public IActionResult CancelReservation([FromQuery] int id)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            int result = _reserveRepo.CancelReseveration(id);
            switch (result)
            {
                case 1:
                    Log.Information("Cancel reservation from mobile {id} done. {DateTime}.", id,dateTime);
                    return Ok(id);
                case 2:
                    Log.Information("Cancel reservation from mobile {id} Cannot_cancel_cause_time. {DateTime}.", id, dateTime);
                    return NotFound("Cannot cancel cause time");
                case 3:
                    Log.Information("Cancel reservation from mobile {id} Reservation_is_not_existed.", id, dateTime);
                    return NotFound("Reservation is not existed");
                default:
                    Log.Information("Cancel reservation from mobile {id} Error.", id);
                    return NotFound("Error");
            }
        }

        /*check if reservation is set code from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route ("/mobile/IsSetCode")]
        [HttpGet]
        public IActionResult IsSetCode (int id_reserve)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            int result = _reserveRepo.IsSetCode(id_reserve);
            switch(result)
            {
                case 1:
                    Log.Information("check code is set? code is not set from mobile {id} done. {DateTime}.", id_reserve, dateTime);
                    return Ok("code==\"string\"");
                case 2:
                    Log.Information("check code is set? code is already set from mobile {id} done. {DateTime}.", id_reserve, dateTime);
                    return NotFound("code is already set");
                default:
                    Log.Information("check code is set? Error from mobile {id_reserve} {DateTime}.", id_reserve, dateTime);
                    return NotFound("Error");
            }
        }

        /*Set code in each reservation from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route ("/mobile/SetCode")]
        [HttpPost]
        public IActionResult SetCode ([FromBody]CodeUser codeUser)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            int result = _reserveRepo.SetCode(codeUser.Id_reserve, codeUser.Code);
            if (result == 1)
            {
                Log.Information("Set code OK from mobile {id} done. {DateTime}.", codeUser.Id_reserve, dateTime);
                return Ok(codeUser);
            }
            else if (result == 2)
            {
         
                Log.Information("Set code Code_is_duplicated from mobile {id} done. {DateTime}.", codeUser.Id_reserve, dateTime);
                return NotFound("Code_is_duplicated");
            }
            else
            {
                    Log.Information("Set code Error to set code from mobile {id_reserve}. {DateTime}.", codeUser.Id_reserve, dateTime);
                    return NotFound("Error to set code");

            }
        }

        /*Get code from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route ("/mobile/GetCode")]
        [HttpGet]
        public IActionResult GetCode (int id_reserve)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            string result = _reserveRepo.GetCode(id_reserve);
            if(result==null)
            {
                Log.Information("Get Code Error to get code from mobile {id}. {DateTime}.", id_reserve, dateTime);
                return NotFound("Cannot Get code");
            }
            else
            {
                Log.Information("Get Code from mobile {id} done. {DateTime}.", id_reserve, dateTime);
                return Ok(result);
            }
        }

        /*Get all reservation from administrator through web application*/
        [Authorize(Roles = Role.Admin)]
        [Route ("/web/Activity")]
        [HttpGet]
        public JsonResult GetActivity()
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _reserveRepo.GetActivities();
            Log.Information("Get Activity from web {DateTime}.", dateTime);
            return Json(list);
        }

        /*Get all notification from administrator through web application*/
        [Authorize(Roles = Role.Admin)]
        [Route("/web/Notification")]
        [HttpGet]
        public JsonResult GetNotification()
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _reserveRepo.GetNotification();
            Log.Information("Get noti from web {DateTime}.", dateTime);
            return Json(list);
        }

        /*Get specific reservastion detail from administrator through web application*/
        [Authorize(Roles = Role.Admin)]
        [Route ("/web/ReserveDetail")]
        [HttpGet]
        public JsonResult GetReserveDetail(int id_reserve)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            ReserveDetail detail = _reserveRepo.GetReserveDetail(id_reserve);
            Log.Information("Get reservation detail from web {id} {DateTime}.", id_reserve,dateTime);
            return Json(detail);
        }

        /*TEST Get all reservastion*/
        [Route("ReserveAll")]
        [HttpGet]
        public IActionResult GetReserve()
        {
            var list = _reserveRepo.GetReserve();
            return Ok(list);
        }

        /*TEST Get specific reservastion*/
        [Route("ReserveID")]
        [HttpGet]
        public IActionResult GetReserve(string id_account)
        {
            var list = _reserveRepo.GetReserve(id_account);
            return Ok(list);
        }

        /*Get pending reservation from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route("/mobile/Pending")]
        [HttpGet]
        public JsonResult Pending (string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _reserveRepo.Pending(id_account);
            Log.Information("Pending from mobile {id}. {0}.", id_account, dateTime);
            return Json(list);
        }

        /*Get history reservation from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route("/mobile/History")]
        [HttpGet]
        public JsonResult History (string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _reserveRepo.History(id_account);
            Log.Information("History from mobile {id}. {0}.", id_account, dateTime);

            return Json(list);
        }

        /*Get specific reservation detail from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route ("/mobile/BookingDetail")]
        [HttpGet]
        public JsonResult BookingDetail (int id_reserve)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            BookingForm result = _reserveRepo.GetBookingDetail(id_reserve);
            Log.Information("Booking detail from mobile {id}. {0}.",id_reserve, dateTime);
            return Json(result);
        }

        /*Set status in reservation from HW, if locker is used*/
        //[AllowAnonymous]
        [Route("/HW/SetState")]
        [HttpPut]
        public IActionResult SetState (int id_reserve, string condition)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            if (_reserveRepo.SetStatus(id_reserve,condition)==1)
            {
                string result = String.Format("{id}:{1}", id_reserve, condition);
                Log.Information("Set state unuse to use. {id}, {condition}, {DateTime}.", id_reserve, condition, dateTime);
                return Ok(result);
            }
            else if (_reserveRepo.SetStatus(id_reserve, condition) ==2)
            {
                string result = String.Format("{id}:{1}", id_reserve, condition);
                Log.Information("Set state use to use. {id}, {condition}, {DateTime}.", id_reserve, condition, dateTime);
                return Ok(result);
            }
            else
            {
                Log.Information("Set state Error to Set state. {id}, {condition}, {DateTime}.", id_reserve, condition, dateTime);
                return NotFound("Error to Set state");
            }

        }

        /*TEST set active status*/
        [Route("SetBoolIsActive")]
        [HttpPost]
        public IActionResult SetBoolIsActive([FromBody]SetIsActive setIsActive)
        {
            int result = _reserveRepo.SetBoolIsActive(setIsActive.Id_reserve,setIsActive.IsActive);
            if (result == 2)
            {
                string _result = String.Format("{DateTime}:{1}", setIsActive.Id_reserve, setIsActive.IsActive);
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

       
    }
}
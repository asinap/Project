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

        [Authorize(Roles = Role.User)]
        [Route("/mobile/AddReserve")]
        [HttpPost]
        public IActionResult AddReservation([FromBody] Reservation reserve)
        {
            int result = _reserveRepo.AddReservation(reserve);
            switch(result)
            {
                case 1:
                    Log.Information("Add reservation {accountID} account_is_not_existed.", reserve.Id_account);
                    return NotFound("account_is_not_existed.");
                case 2:
                    Log.Information("Add reservation {Name} {location} {size} No_avaliable_vacant."
                        , _dbContext.Accounts.FirstOrDefault(x=>x.Id_account==reserve.Id_account).Name
                        , reserve.Location, reserve.Size);
                    return NotFound("No_avaliable_vacant.");
                case 3:
                    Log.Information("Add reservation {Name} {location} {size} Cannot_find_size_requirement."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Name
                        , reserve.Location, reserve.Size);
                    return NotFound("Cannot_find_size_requirement");
                case 4:
                    Log.Information("Add reservation {Name} {reserveID} No_point.", _dbContext.Accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Name);
                    return NotFound("No_point");
                case 5:
                    Log.Information("Add reservation {Name}, {no_number}, {location}, {size}, {createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x=>x.Id_vacancy==_dbContext.Reservations.FirstOrDefault(y=>y.Id_reserve==reserve.Id_reserve).Id_vacancy).No_vacancy
                        , reserve.Location
                        , reserve.Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == reserve.Id_reserve).DateModified);
                    return Ok(reserve.Id_reserve);
                default:
                    Log.Information("Add reservation {userID} {reserveID} Error.", reserve.Id_account, reserve.Id_reserve);
                    return NotFound("Error");

            }
            //{
            //    return Ok(reserve.Id_reserve);
            //}
            //return NotFound();
        }

        [Authorize(Roles = Role.User)]
        [Route("/mobile/CancelReserve")]
        [HttpDelete]
        public IActionResult CancelReservation([FromQuery] int id)
        {
            int result = _reserveRepo.CancelReseveration(id);
            switch (result)
            {
                case 1:
                    Log.Information("Cancel reservation from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y=>y.Id_reserve==id).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).DateModified);
                    return Ok(id);
                case 2:
                    Log.Information("Cancel reservation from mobile {Name}, {no_number}, {location}, {size}, {createdTime} Cannot_cancel_cause_time."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id).DateModified);
                    return NotFound("Cannot_cancel_cause_time");
                case 3:
                    Log.Information("Cancel reservation from mobile {reserveID} Reservation_is_not_existed.", id);
                    return NotFound("Reservation_is_not_existed");
                default:
                    Log.Information("Cancel reservation from mobile {reserveID} Error.", id);
                    return NotFound("Error");
            }
        }

        [Authorize(Roles = Role.User)]
        [Route ("/mobile/IsSetCode")]
        [HttpGet]
        public IActionResult IsSetCode (int id_reserve)
        {
            int result = _reserveRepo.IsSetCode(id_reserve);
            switch(result)
            {
                case 1:
                    Log.Information("check code is set? code is not set from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).DateModified);
                    return Ok("code==\"string\"");
                case 2:
                    Log.Information("check code is set? code is already set from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                       , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_account).Name
                       , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_vacancy).No_vacancy
                       , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Location
                       , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Size
                       , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).DateModified);
                    return NotFound("code is already set");
                default:
                    if (_dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id_reserve) != null)
                    {
                        Log.Information("check code is set? Error from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                           , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_account).Name
                           , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_vacancy).No_vacancy
                           , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Location
                           , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Size
                           , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).DateModified);
                        return NotFound("Error");
                    }
                    else
                    {
                        Log.Information("check code is set? Error from mobile {id_reserve} done.", id_reserve);
                        return NotFound("Error");
                    }
                
            }
        }

        [Authorize(Roles = Role.User)]
        [Route ("/mobile/SetCode")]
        [HttpPost]
        public IActionResult SetCode ([FromBody]CodeUser codeUser)
        {
            int result = _reserveRepo.SetCode(codeUser.Id_reserve, codeUser.Code);
            if (result == 1)
            {
                Log.Information("Set code OK from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).DateModified);
                string _result = string.Format("id_reserve : {0}, code : {1}", codeUser.Id_reserve, codeUser.Code);
                return Ok(_result);
            }
            else if (result == 2)
            {
                Log.Information("Set code Code_is_already_set from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).DateModified);
                return NotFound("Code_is_already_set");
            }
            else if (result == 3)
            {
                Log.Information("Set code Code_is_duplicated from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                         , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Id_account).Name
                         , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Id_vacancy).No_vacancy
                         , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Location
                         , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Size
                         , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).DateModified);
                return NotFound("Code_is_duplicated");
            }
            else
            {
                if (_dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == codeUser.Id_reserve) != null)
                {
                    Log.Information("Set code Error to set code from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == codeUser.Id_reserve).DateModified);
                    return NotFound("Error to set code");
                }
                else
                {
                    Log.Information("Set code Error to set code from mobile {id_reserve} done.", codeUser.Id_reserve);
                    return NotFound("Error to set code");
                }

            }
        }

        [Authorize(Roles = Role.User)]
        [Route ("/mobile/GetCode")]
        [HttpGet]
        public IActionResult GetCode (int id_reserve)
        {
            string result = _reserveRepo.GetCode(id_reserve);
            if(result==null)
            {
                if (_dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id_reserve) != null)
                {
                    Log.Information("Get Code Error to get code from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                           , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_account).Name
                           , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_vacancy).No_vacancy
                           , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Location
                           , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Size
                           , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).DateModified);
                    return NotFound("Cannot Get code");
                }
                else
                {
                    Log.Information("Get Code Error to get code from mobile {id_reserve} done.", id_reserve);
                    return NotFound("Cannot Get code");
                }
            }
            else
            {
                Log.Information("Get Code from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                       , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_account).Name
                       , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_vacancy).No_vacancy
                       , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Location
                       , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Size
                       , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).DateModified);
                return Ok(result);
            }
        }

        [Authorize(Roles = Role.Admin)]
        [Route ("/web/Activity")]
        [HttpGet]
        public JsonResult GetActivity()
        {
            var list = _reserveRepo.GetActivities();
            Log.Information("Get Activity from web {DateTime}.", DateTime.Now);
            return Json(list);
        }

        [Authorize(Roles = Role.Admin)]
        [Route("/web/Notification")]
        [HttpGet]
        public JsonResult GetNotification()
        {
            var list = _reserveRepo.GetNotification();
            Log.Information("Get noti from web {DateTime}.", DateTime.Now);
            return Json(list);
        }

        [Authorize(Roles = Role.Admin)]
        [Route ("/web/ReserveDetail")]
        [HttpGet]
        public JsonResult GetReserveDetail(int id_reserve)
        {
            ReserveDetail detail = _reserveRepo.GetReserveDetail(id_reserve);
            Log.Information("Get reservation detail from web {DateTime} {Name} {No_vacant} {size} {location}.", detail.DateModified,detail.Name,detail.NumberVacancy,detail.Size,detail.Location);
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

        [Authorize(Roles = Role.User)]
        [Route("/mobile/Pending")]
        [HttpGet]
        public JsonResult Pending (string id_account)
        {
            var list = _reserveRepo.Pending(id_account);
            Log.Information("Pending from mobile {name}.", _dbContext.Accounts.FirstOrDefault(x=>x.Id_account== id_account).Name);
          
            return Json(list);
        }

        [Authorize(Roles = Role.User)]
        [Route("/mobile/History")]
        [HttpGet]
        public JsonResult History (string id_account)
        {
            var list = _reserveRepo.History(id_account);
            Log.Information("History from mobile {name}.", _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id_account).Name);

            return Json(list);
        }

        [Authorize(Roles = Role.User)]
        [Route ("/mobile/BookingDetail")]
        [HttpGet]
        public JsonResult BookingDetail (int id_reserve)
        {
            BookingForm result = _reserveRepo.GetBookingDetail(id_reserve);
            Log.Information("Booking detail from mobile {Name}, {no_number}, {location}, {size}, {createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == id_reserve).DateModified);
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
        [HttpPost]
        public IActionResult SetState ([FromBody]SetState setState)
        {
            if(_reserveRepo.SetStatus(setState.Id_reserve,setState.Condition)==1)
            {
                string result = String.Format("{0}:{1}", setState.Id_reserve, setState.Condition);
                Log.Information("Set state unuse to use. { Name}, { no_number}, { location}, { size}, { createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).DateModified);
                return Ok(result);
            }
            else if (_reserveRepo.SetStatus(setState.Id_reserve, setState.Condition) ==2)
            {
                string result = String.Format("{0}:{1}", setState.Id_reserve, setState.Condition);
                Log.Information("Set state use to use. { Name}, { no_number}, { location}, { size}, { createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).DateModified);
                return Ok(result);
            }
            else
            {
                Log.Information("Set state Error to Set state.{ Name}, { no_number}, { location}, { size}, { createdTime} done."
                        , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Id_account).Name
                        , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Id_vacancy).No_vacancy
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Location
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).Size
                        , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == setState.Id_reserve).DateModified);
                return NotFound("Error to Set state");
            }

        }

        [Route("SetBoolIsActive")]
        [HttpPost]
        public IActionResult SetBoolIsActive([FromBody]SetIsActive setIsActive)
        {
            int result = _reserveRepo.SetBoolIsActive(setIsActive.Id_reserve,setIsActive.IsActive);
            if (result == 2)
            {
                string _result = String.Format("{0}:{1}", setIsActive.Id_reserve, setIsActive.IsActive);
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
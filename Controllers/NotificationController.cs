using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Entities;
using test2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.HttpSys;

namespace test2.Controllers
{
    [Route("/api/[Controller]")]
    public class NotificationController : Controller
    {
        private readonly NotificationRepository _notiRepo;
        private readonly LockerDbContext _dbContext;

        public NotificationController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _notiRepo = new NotificationRepository(_dbContext);
        }

        //Delete notification by user through mobile application
        [Authorize (Roles = Role.User)]
        [Route("/mobile/DeleteNotificaiton")]
        [HttpPost]
        public IActionResult DeleteNotification([FromBody]NotificationIForm notification)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if setting notification is not show on user's mobile success
            if (_notiRepo.DeleteNotification(notification.Id_noti))
            {
                Log.Information("Delete noti from mobile {id_notification} OK. {DateTime}.", notification.Id_noti, dateTime);
                return Ok(notification.Id_noti);
            }
            //if setting notification is not show on user's mobile fail
            else
            {
                Log.Information("Error_Delete. {DateTime}.", dateTime);
                return NotFound("Error Delete notification");

            }

        }

        //set notification that is read by user through mobile application
        [Authorize(Roles = Role.User)]
        [Route("/mobile/SetRead")]
        [HttpPost]
        public IActionResult SetRead([FromBody]NotificationIForm notification)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if set notification is read success
            if (_notiRepo.SetRead(notification.Id_noti))
            {
                Log.Information("Set Read noti from mobile {id_noti} OK. {DateTime}.",notification.Id_noti,dateTime);
                return Ok(notification.Id_noti);
            }
            //if set notification is read fail
            else
            {
                Log.Information("Error_SetRead {noti}. {DateTime}.", notification.Id_noti,dateTime);
                return NotFound("Error_SetRead");

            }
        }

        /*TEST get all notification */
        [Route("NotificationAll")]
        [HttpGet]
        public IActionResult GetNotification()
        {
            var list = _notiRepo.GetNotification();
            return Ok(list);
        }

        /*TEST get specific notification*/
        [Route("UserAllNotification")]
        [HttpGet]
        public IActionResult GetNotification(int _noti)
        {
            var list = _notiRepo.GetNotification(_noti);
            return Ok(list);
        }

        /*Get notification from each user through mobile application */
        [Authorize(Roles = Role.User)]
        [Route ("/mobile/UserInbox")]
        [HttpGet]
        public JsonResult GetNotificationForm (string id_account)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            var list = _notiRepo.GetNotificationForm(id_account);
            Log.Information("Get user inbox from mobile {id} OK. {DateTime}.", id_account, dateTime);
            return Json(list);
        }

        /*Get specific notification from user through mobile application*/
        [Authorize(Roles = Role.User)]
        [Route("/mobile/UserInboxDetail")]
        [HttpGet]
        public JsonResult GetNotificationDetail (int id_noti)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            NotificationForm form = _notiRepo.GetNotificationDetail(id_noti);
            Log.Information("Get user inbox from mobile {id} OK. {DateTime}.", id_noti, dateTime);
            return Json(form);
        }

       

    }
 }
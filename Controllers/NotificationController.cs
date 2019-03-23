﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Repositories;

namespace test2.Controllers
{
    [Route("/api/[Controller]")]
    public class NotificationController : Controller
    {
        private readonly NotificationRepository _notiRepo;
        private readonly LockerDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;

        public NotificationController(LockerDbContext lockerDbContext, ILogger<AccountController> logger)
        {
            _dbContext = lockerDbContext;
            _notiRepo = new NotificationRepository(_dbContext);
            _logger = logger;
        }

        [Route("AddNotification")]
        [HttpPost]
        public IActionResult AddNotification([FromBody] Notification detail)
        {
            if (_notiRepo.AddNotification(detail))
            {
                _logger.LogInformation("Add notification {Name}, {No}, {Location}, {contentID} OK."
                    , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == detail.Id_account).Name
                    , _dbContext.Vacancies.FirstOrDefault(y => y.Id_vacancy == _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == detail.Id_reserve).Id_vacancy).No_vacancy
                    , _dbContext.Reservations.FirstOrDefault(y=>y.Id_reserve==detail.Id_reserve).Location
                    , detail.Id_content);
                return Ok(detail.Id_notification);
            }
            _logger.LogInformation("Cannot Add notification {Name}, {No}, {Location}, {contentID}."
                   , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == detail.Id_account).Name
                   , _dbContext.Vacancies.FirstOrDefault(y => y.Id_vacancy == _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == detail.Id_reserve).Id_vacancy).No_vacancy
                   , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == detail.Id_reserve).Location
                   , detail.Id_content);
            return NotFound();
        }

        [Route("/mobile/DeleteNotificaiton")]
        [HttpPut]
        public IActionResult DeleteNotification(int id_noti)
        {
            if (_notiRepo.DeleteNotification(id_noti))
            {
                _logger.LogInformation("Delete noti from mobile {Name}, {No}, {Location}, {contentID} OK."
                    , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Notifications.FirstOrDefault(y => y.Id_notification == id_noti).Id_account).Name
                    , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Id_vacancy).No_vacancy
                    , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Location
                    , _dbContext.Notifications.FirstOrDefault(x => x.Id_notification == id_noti).Id_content);
                return Ok(id_noti);
            }
            _logger.LogInformation("Cannot Delete noti from mobile {Name}, {No}, {Location}, {contentID}."
                    , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Notifications.FirstOrDefault(y => y.Id_notification == id_noti).Id_account).Name
                    , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Id_vacancy).No_vacancy
                    , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Location
                    , _dbContext.Notifications.FirstOrDefault(x => x.Id_notification == id_noti).Id_content);
            return NotFound("Error_Delete");
        }

        [Route("/mobile/SetRead")]
        [HttpPut]
        public IActionResult SetRead(int id_noti)
        {
            if(_notiRepo.SetRead(id_noti))
            {
                _logger.LogInformation("Set Read noti from mobile {Name}, {No}, {Location}, {contentID} OK."
                    , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Notifications.FirstOrDefault(y => y.Id_notification == id_noti).Id_account).Name
                    , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Id_vacancy).No_vacancy
                    , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Location
                    , _dbContext.Notifications.FirstOrDefault(x => x.Id_notification == id_noti).Id_content);
                return Ok(id_noti);
            }
            _logger.LogInformation("Cannot Set Read noti from mobile {Name}, {No}, {Location}, {contentID}."
                    , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Notifications.FirstOrDefault(y => y.Id_notification == id_noti).Id_account).Name
                    , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Id_vacancy).No_vacancy
                    , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Location
                    , _dbContext.Notifications.FirstOrDefault(x => x.Id_notification == id_noti).Id_content);
            return NotFound("Error_SetRead");
        }

        [Route("NotificationAll")]
        [HttpGet]
        public IActionResult GetNotification()
        {
            var list = _notiRepo.GetNotification();
            return Ok(list);
        }

        [Route("UserAllNotification")]
        [HttpGet]
        public IActionResult GetNotification(int _noti)
        {
            var list = _notiRepo.GetNotification(_noti);
            return Ok(list);
        }

        [Route ("/mobile/UserInbox")]
        [HttpGet]
        public JsonResult GetNotificationForm (string id_account)
        {
            var list = _notiRepo.GetNotificationForm(id_account);
            _logger.LogInformation("Get user inbox from mobile {Name} OK.", _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id_account).Name);
            return Json(list);
        }

        [Route("/mobile/UserInboxDetail")]
        [HttpGet]
        public JsonResult GetNotificationDetail (int id_noti)
        {
            NotificationForm form = _notiRepo.GetNotificationDetail(id_noti);
            _logger.LogInformation("Get user inbox detail from mobile {Name} , {No}, {Location}, {contentID} Ok."
                 , _dbContext.Accounts.FirstOrDefault(x => x.Id_account == _dbContext.Notifications.FirstOrDefault(y => y.Id_notification == id_noti).Id_account).Name
                    , _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Id_vacancy).No_vacancy
                    , _dbContext.Reservations.FirstOrDefault(y => y.Id_reserve == _dbContext.Notifications.FirstOrDefault(z => z.Id_notification == id_noti).Id_reserve).Location
                    , _dbContext.Notifications.FirstOrDefault(x => x.Id_notification == id_noti).Id_content);
            return Json(form);
        }

        [Route("UserActiveNotification")]
        [HttpGet]
        public IActionResult GetActiveNotification (string id_account)
        {
            var list = _notiRepo.GetActiveNoti(id_account);
            return Ok(list);
        }

        [Route("DeleteAll")]
        [HttpDelete]
        public IActionResult Delete()
        {
            if(_notiRepo.Delete())
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("Delete")]
        [HttpDelete]
        public IActionResult Delete(int id_noti)
        {
            if (_notiRepo.Delete(id_noti))
            {
                return Ok();
            }
            return NotFound();
        }

    }
 }
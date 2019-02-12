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
    public class NotificationController : Controller
    {
        private readonly NotificationRepository _notiRepo;
        private readonly LockerDbContext _dbContext;

        public NotificationController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _notiRepo = new NotificationRepository(_dbContext);
        }

        [Route("AddNotification")]
        [HttpPost]
        public IActionResult AddNotification([FromBody] Notification detail)
        {
            if (_notiRepo.AddNotification(detail))
            {
                return Ok(detail.Id_notification);
            }
            return NotFound();
        }

        [Route("DeleteNotificaiton")]
        [HttpPut]
        public IActionResult DeleteNotification(int id_noti)
        {
            if (_notiRepo.DeleteNotification(id_noti))
            {
                return Ok(id_noti);
            }
            return NotFound();
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
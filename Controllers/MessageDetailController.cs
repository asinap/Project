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
    public class MessageDetailController : Controller
    {
        private readonly MessageDetailRepository _messageRepo;
        private readonly LockerDbContext _dbContext;

        public MessageDetailController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _messageRepo = new MessageDetailRepository(_dbContext);
        }

        [Route("AddMessage")]
        [HttpPost]
        public IActionResult AddMessage([FromBody] MessageDetail detail)
        {
            if (_messageRepo.AddMessage(detail))
            {
                return Ok(detail.Id_message);
            }
            return NotFound();
        }
    }
 }
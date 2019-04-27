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
using test2.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace test2.Controllers
{
    [Route("/api/[Controller]")]
    public class ContentController : Controller
    {
        private readonly ContentRepository _contentRepo;
        private readonly LockerDbContext _dbContext;

        public ContentController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _contentRepo = new ContentRepository(_dbContext);
        }

        [AllowAnonymous]
        /*Adding content for notification*/
        [Route("AddContent")]
        [HttpPost]
        public IActionResult AddContent([FromBody] Content _cont)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if adding content success
            if (_contentRepo.AddContent(_cont))
            {
                Log.Information("Add content {id} OK. {DateTime}.", _cont.Id_content, dateTime);
                return Ok(_cont.Id_content);
            }
            //if adding content fail
            Log.Information("Cannot Add content {id}. {DateTime}.", _cont.Id_content, dateTime);
            return NotFound();
        }

        [AllowAnonymous]
        /*Deleting content by set active to be false*/
        [Route("DeleteContent")]
        [HttpPost]
        public IActionResult DeleteContent([FromBody] ContentForm _cont)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if deleting content success
            if (_contentRepo.DeleteContent(_cont.Id_content))
            {
                Log.Information("Delete content {id} OK. {DateTime}.", _cont.Id_content, dateTime);
                return Ok(_cont.Id_content);
            }
            //if deleting content fail
            Log.Information("Cannot Delete content {id}. {DateTime}.", _cont.Id_content, dateTime);
            return NotFound(_cont.Id_content);

        }

        [AllowAnonymous]
        /*Restoring content by set active to be true*/
        [Route("RestoreContent")]
        [HttpPost]
        public IActionResult RestoreContent([FromBody] ContentForm _cont)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //if restore content success
            if (_contentRepo.RestoreContent(_cont.Id_content))
            {
                Log.Information("Restore content {id} OK. {DateTime}.", _cont.Id_content, dateTime);
                return Ok(_cont.Id_content);
            }
            //if restore content fail
            Log.Information("Cannot Restore content {id}. {DateTime}.", _cont.Id_content, dateTime);
            return NotFound(_cont.Id_content);
        }

        /*TEST show all content*/
        [Route("ContentAll")]
        [HttpGet]
        public IActionResult GetContent()
        {
            var list = _contentRepo.GetAllContent();
            return Ok(list);
        }

        /*TEST show all content that active is true*/
        [Route("ActiveContent")]
        [HttpGet]
        public IActionResult ActiveContent()
        {
            var list = _contentRepo.GetContent();
            return Ok(list);
        }

        /*TEST show specific content that active is true*/
        [Route("ActiveContentID")]
        [HttpGet]
        public IActionResult GetContent(int id_content)
        {
            var list = _contentRepo.GetContent(id_content);
            return Ok(list);
        }

    }
}
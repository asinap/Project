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

        [Route("AddContent")]
        [HttpPost]
        public IActionResult AddContent([FromBody] Content _cont)
        {
            if (_contentRepo.AddContent(_cont))
            {
                Log.Information("Add content {id} OK.", _cont.Id_content);
                return Ok(_cont.Id_content);
            }
            Log.Information("Cannot Add content {id}.", _cont.Id_content);
            return NotFound();
        }

        [Route("DeleteContent")]
        [HttpPost]
        public IActionResult DeleteContent([FromBody] ContentForm _cont)
        {
            if (_contentRepo.DeleteContent(_cont.Id_content))
            {
                Log.Information("Delete content {id} OK.", _cont.Id_content);
                return Ok();
            }
            Log.Information("Cannot Delete content {id}.", _cont.Id_content);
            return NotFound();

        }

        [Route("RestoreContent")]
        [HttpPost]
        public IActionResult RestoreContent([FromBody] ContentForm _cont)
        {
            if (_contentRepo.RestoreContent(_cont.Id_content))
            {
                Log.Information("Restore content {id} OK.", _cont.Id_content);
                return Ok(_cont.Id_content);
            }
            Log.Information("Cannot Restore content {id}.", _cont.Id_content);
            return NotFound();
        }

        [Route("ContentAll")]
        [HttpGet]
        public IActionResult GetContent()
        {
            var list = _contentRepo.GetAllContent();
            return Ok(list);
        }

        [Route("ActiveContent")]
        [HttpGet]
        public IActionResult ActiveContent()
        {
            var list = _contentRepo.GetContent();
            return Ok(list);
        }

        [Route("ActiveContentID")]
        [HttpGet]
        public IActionResult GetContent(int id_content)
        {
            var list = _contentRepo.GetContent(id_content);
            return Ok(list);
        }

        [Route("DeleteAll")]
        [HttpDelete]
        public IActionResult Delete()
        {
            if (_contentRepo.Delete())
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("Delete")]
        [HttpDelete]
        public IActionResult Delete(int id_content)
        {
            if (_contentRepo.Delete(id_content))
            {
                return Ok();
            }
            return NotFound();
        }

    }
}
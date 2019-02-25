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
                return Ok(_cont.Id_content);
            }
            return NotFound();
        }

        [Route("DeleteContent")]
        [HttpPut]
        public IActionResult DeleteContent([FromQuery] int id_content)
        {
            if (_contentRepo.DeleteContent(id_content))
            {
                return Ok();
            }
            return NotFound();

        }

        [Route("RestoreContent")]
        [HttpPut]
        public IActionResult RestoreContent([FromQuery] int id_content)
        {
            if (_contentRepo.RestoreContent(id_content))
            {
                return Ok(id_content);
            }
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
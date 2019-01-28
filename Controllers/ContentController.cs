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
        public IActionResult AddLocker([FromBody] Content _cont)
        {
            if (_contentRepo.AddContent(_cont))
            {
                return Ok(_cont.Id_content);
            }
            return NotFound();
        }

        [Route("DeleteContent")]
        [HttpDelete]
        public IActionResult DeleteLocker([FromQuery] int id)
        {
            if (_contentRepo.DeleteContent(id))
            {
                return Ok();
            }
            return NotFound();

        }

        [Route("SetActiveContent")]
        [HttpPut]
        public IActionResult SetActiveContent([FromQuery] int id)
        {
            if (_contentRepo.SetActive(id))
            {
                return Ok(id);
            }
            return NotFound();
        }

        [Route("ContentAll")]
        [HttpGet]
        public IActionResult GetContent()
        {
            var list = _contentRepo.GetContent();
            return Ok(list);
        }

        [Route("ContentId")]
        [HttpGet]
        public IActionResult GetContent(int id)
        {
            var list = _contentRepo.GetContent(id);
            return Ok(list);
        }

    }
}
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
    public class VacancyController : Controller
    {
        private readonly VacancyRepository _vacancyRepo;
        private readonly LockerDbContext _dbContext;


        public VacancyController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _vacancyRepo = new VacancyRepository(_dbContext);
        }

        [Route("AddVacant")]
        [HttpPost]
        public IActionResult AddVacancy([FromBody] Vacancy vacant)
        {
            if (_vacancyRepo.AddVacancy(vacant))
            {
                return Ok(vacant.Id_vacancy);
            }
            return NotFound();
        }

        [Route("DeleteVacant")]
        [HttpDelete]
        public IActionResult DeleteVacancy([FromQuery] string No_vacant, string Mac_address)
        {
            if (_vacancyRepo.DeleteVacancy(No_vacant, Mac_address))
            {
                return Ok();
            }
            return NotFound();

        }

        [Route("UpadateActiveVacant")]
        [HttpPut]
        public IActionResult UpdateActive([FromQuery] string No_vacant, string Mac_address)
        {
            if (_vacancyRepo.UpdateActive(No_vacant, Mac_address))
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("UpdateSizeVacant")]
        [HttpPut]
        public IActionResult UpdateSize([FromQuery] string No_vacant, string Mac_address, string size)
        {
            if (_vacancyRepo.UpdateSize(No_vacant, Mac_address, size))
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("VacancyAll")]
        [HttpGet]
        public IActionResult GetVacancy()
        {
            var list = _vacancyRepo.GetVacancy();
            return Ok(list);
        }

        [Route("VacancyId")]
        [HttpGet]
        public IActionResult GetHistory(int id)
        {
            var list = _vacancyRepo.GetVacancy(id);
            return Ok(list);
        }

        [Route("DeleteAll")]
        [HttpDelete]
        public IActionResult Delete()
        {
            if (_vacancyRepo.Delete())
            {
                return Ok();
            }
            return NotFound();
        }

        [Route("Delete")]
        [HttpDelete]
        public IActionResult Delete(int id_noti)
        {
            if (_vacancyRepo.Delete(id_noti))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
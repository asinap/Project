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
    public class VacancyController : Controller
    {
        private readonly VacancyRepository _vacancyRepo;
        private readonly LockerDbContext _dbContext;

        public VacancyController(LockerDbContext lockerDbContext)
        {
            _dbContext = lockerDbContext;
            _vacancyRepo = new VacancyRepository(_dbContext);
        }

        [AllowAnonymous]
        [Route("/web/AddVacant")]
        [HttpPost]
        public IActionResult AddVacancy([FromBody] Vacancy vacant)
        {
            if (_vacancyRepo.AddVacancy(vacant))
            {
                Log.Information("Add vacancy {no}, {location} OK.",vacant.No_vacancy,_dbContext.LockerMetadatas.FirstOrDefault(x=>x.Mac_address==vacant.Mac_address).Location);
                return Ok(vacant.Id_vacancy);
            }
            Log.Information("Cannot Add vacancy {no}, {location} OK.", vacant.No_vacancy, _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == vacant.Mac_address).Location);
            return NotFound();
        }

        [AllowAnonymous]
        [Route("/web/EditVancancy")]
        [HttpPost]
        public IActionResult EditVacancy([FromBody] Vacancy vacant)
        {
            if (_vacancyRepo.EditVacancy(vacant))
            {
                Log.Information("Edit Locker {Location} OK.", _dbContext.LockerMetadatas.FirstOrDefault(x=>x.Mac_address== vacant.Mac_address).Location);
                return Ok(vacant.Mac_address);
            }
            Log.Information("Edit Locker {Location} Error.", _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == vacant.Mac_address).Location);
            return NotFound();
        }

        [Route("DeleteVacant")]
        [HttpDelete]
        public IActionResult DeleteVacancy([FromQuery] string No_vacant, string Mac_address)
        {
            if (_vacancyRepo.DeleteVacancy(No_vacant, Mac_address))
            {
                Log.Information("Delete vacancy {no}, {location} OK.", No_vacant, _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return Ok();
            }
            if (_dbContext.Vacancies.FirstOrDefault(x => x.No_vacancy == No_vacant && x.Mac_address == Mac_address) != null)
            {
                Log.Information("Cannot Delete vacancy {no}, {location} OK.", No_vacant, _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return NotFound();
            }
            else
            {
                Log.Information("Cannot Delete vacancy {no}, {location} OK.", No_vacant, Mac_address);
                return NotFound();
            }

        }

        [Route("UpadateActiveVacant")]
        [HttpPost]
        public IActionResult UpdateActive([FromBody] string No_vacant, string Mac_address)
        {
            if (_vacancyRepo.UpdateActive(No_vacant, Mac_address))
            {
                Log.Information("Set Active vacancy {no}, {location} OK.", No_vacant, _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return Ok();
            }
            if (_dbContext.Vacancies.FirstOrDefault(x => x.No_vacancy == No_vacant && x.Mac_address == Mac_address) != null)
            {
                Log.Information("Cannot set Active vacancy {no}, {location} OK.", No_vacant, _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return NotFound();
            }
            else
            {
                Log.Information("Cannot set Active vacancy {no}, {location} OK.", No_vacant, Mac_address);
                return NotFound();
            }
        }

        [Route("UpdateSizeVacant")]
        [HttpPost]
        public IActionResult UpdateSize([FromBody] UpdateSize updateSize)
        {
            if (_vacancyRepo.UpdateSize(updateSize.No_vacant, updateSize.Mac_address, updateSize.Size))
            {
                Log.Information("Set Size vacancy {size}, {no}, {location} OK.", updateSize.Size, updateSize.No_vacant, _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == updateSize.Mac_address).Location);
                return Ok();
            }
            if (_dbContext.Vacancies.FirstOrDefault(x => x.No_vacancy == updateSize.No_vacant && x.Mac_address == updateSize.Mac_address && x.Size== updateSize.Size) != null)
            {
                Log.Information("Cannot Set Size vacancy {size}, {no}, {location} OK.", updateSize.Size, updateSize.No_vacant, _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == updateSize.Mac_address).Location);
                return NotFound();
            }
            else
            {
                Log.Information("Cannot Set Size vacancy {size}, {no}, {location} OK.", updateSize.No_vacant, updateSize.Mac_address, updateSize.Size);
                return NotFound();
            }
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
        public IActionResult GetHistory(int id_vacant)
        {
            var list = _vacancyRepo.GetVacancy(id_vacant);
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
        public IActionResult Delete(int id_vacant)
        {
            if (_vacancyRepo.Delete(id_vacant))
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
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

        /*Add vacancy from administrator through web application*/
        [AllowAnonymous]
        [Route("/web/AddVacant")]
        [HttpPost]
        public IActionResult AddVacancy([FromBody] Vacancy vacant)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            if (_vacancyRepo.AddVacancy(vacant))
            {
                Log.Information("Add vacancy {id} OK. {datetime}.",vacant.Id_vacancy,dateTime);
                return Ok(vacant.Id_vacancy);
            }
            Log.Information("Cannot Add vacancy {id}. {datetime}.", vacant.Id_vacancy, dateTime);
            return NotFound("Cannot add vacancy");
        }

        /*Edit vacancy from administrator through web application*/
        [AllowAnonymous]
        [Route("/web/EditVancancy")]
        [HttpPost]
        public IActionResult EditVacancy([FromBody] Vacancy vacant)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            if (_vacancyRepo.EditVacancy(vacant))
            {
                Log.Information("Edit Locker {id} OK. {datetime}.", vacant.Id_vacancy, dateTime);
                return Ok(vacant.Mac_address);
            }
            Log.Information("Edit Locker {id} Error. {datetime}.", vacant.Id_vacancy, dateTime);
            return NotFound("Cannot edit locker");
        }

        /*Delete vacancy from administrator thriugh web application*/
        [AllowAnonymous]
        [Route("/web/DeleteVacant")]
        [HttpDelete]
        public IActionResult DeleteVacancy([FromQuery] string No_vacant, string Mac_address)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            if (_vacancyRepo.DeleteVacancy(No_vacant, Mac_address))
            {
                Log.Information("Delete vacancy {no}, {mac} OK. {datetime}", No_vacant, Mac_address,dateTime);
                return Ok("Delete vacancy complete.");
            }
            else
            {
                Log.Information("Cannot Delete vacancy {no}, {mac}. {datetime}.", No_vacant, Mac_address, dateTime);
                return NotFound("Delete vacancy fail.");
            }

        }

        /*TEST*/
        [Route("UpadateActiveVacant")]
        [HttpPost]
        public IActionResult UpdateActive([FromBody] string No_vacant, string Mac_address)
        {
            if (_vacancyRepo.UpdateActive(No_vacant, Mac_address))
            {
                Log.Information("Set Active vacancy {no}, {location} OK.", No_vacant, _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return Ok();
            }
            if (_dbContext.vacancies.FirstOrDefault(x => x.No_vacancy == No_vacant && x.Mac_address == Mac_address) != null)
            {
                Log.Information("Cannot set Active vacancy {no}, {location} OK.", No_vacant, _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).Location);
                return NotFound();
            }
            else
            {
                Log.Information("Cannot set Active vacancy {no}, {location} OK.", No_vacant, Mac_address);
                return NotFound();
            }
        }

        /*TEST*/
        [Route("UpdateSizeVacant")]
        [HttpPost]
        public IActionResult UpdateSize([FromBody] UpdateSize updateSize)
        {
            if (_vacancyRepo.UpdateSize(updateSize.No_vacant, updateSize.Mac_address, updateSize.Size))
            {
                Log.Information("Set Size vacancy {size}, {no}, {location} OK.", updateSize.Size, updateSize.No_vacant, _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == updateSize.Mac_address).Location);
                return Ok();
            }
            if (_dbContext.vacancies.FirstOrDefault(x => x.No_vacancy == updateSize.No_vacant && x.Mac_address == updateSize.Mac_address && x.Size== updateSize.Size) != null)
            {
                Log.Information("Cannot Set Size vacancy {size}, {no}, {location} OK.", updateSize.Size, updateSize.No_vacant, _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == updateSize.Mac_address).Location);
                return NotFound();
            }
            else
            {
                Log.Information("Cannot Set Size vacancy {size}, {no}, {location} OK.", updateSize.No_vacant, updateSize.Mac_address, updateSize.Size);
                return NotFound();
            }
        }

        /*TEST*/
        [Route("VacancyAll")]
        [HttpGet]
        public IActionResult GetVacancy()
        {
            var list = _vacancyRepo.GetVacancy();
            return Ok(list);
        }


        /*TEST*/
        [Route("VacancyId")]
        [HttpGet]
        public IActionResult GetHistory(int id_vacant)
        {
            var list = _vacancyRepo.GetVacancy(id_vacant);
            return Ok(list);
        }

       
    }
}
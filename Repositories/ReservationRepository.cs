using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;

namespace test2.Repositories
{
    public class ReservationRepository
    {
        LockerDbContext _dbContext;

        public ReservationRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public int AddReservation(Reservation reserve)
        {
            try
            {
                switch (reserve.Optional)
                {
                    case 1: return ReserveByDay(reserve);
                    case 2: return ReserveByTime(reserve);
                    default: Console.WriteLine("Out of optional");
                        return 0;
                }
                //if (CheckId_studentRef(reserve.Id_account))
                //{
                //    return false;
                //}
                ////else if (CheckId_vacancyRef(reserve.Id_vacancyRef))
                ////{
                ////    return false;
                ////}
                //else
                //{
                //    _dbContext.Reservations.Add(reserve);
                //    _dbContext.SaveChanges();
                //    return true;
                //}
            }
            catch (Exception)
            {
                Console.WriteLine("AddReservation Error");
                return 0;
            }


        }


        public int ReserveByDay(Reservation reserve)
        {
            //1. check account exist.
            if (CheckId_account(reserve.Id_account))
            {
                return 1;
            }

            //2. find non-overlap locker; check available day, free vacancy and right location return list of vacancy
            var nonOverlap = CheckAvailableDay(reserve);
            if (nonOverlap == null)
            {
                return 2;
            }

            //3.find size
            var inSize = nonOverlap.FirstOrDefault(x => x.Size == reserve.Size);
            if(inSize==null)
            {
                return 3;
            }
            reserve.Id_vacancy = inSize.Id_vacancy;
            reserve.Status = "Unuse";
            _dbContext.Reservations.Add(reserve);
            _dbContext.SaveChanges();
            return 4;
        }

        public int ReserveByTime(Reservation reserve)
        {
            if (CheckId_account(reserve.Id_account))
            {
                return 0;
            }
            return 1;
        }

        public bool CheckId_account(string id)
        {
            return _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id) == null;
        }

        public List<Vacancy> CheckAvailableDay (Reservation reserve)
        {
            var overlap = from reservelist in _dbContext.Reservations
                             where reservelist.StartDay <= reserve.StartDay && reservelist.EndDay >= reserve.StartDay
                             select reservelist;
            var availableVacant = from vacantlist in _dbContext.Vacancies join lockerlist in _dbContext.LockerMetadatas
                                  on vacantlist.Mac_address equals lockerlist.Mac_address
                                  where !(overlap.Any(x => x.Id_vacancy == vacantlist.Id_vacancy)) && lockerlist.Location == reserve.Location
                                  select vacantlist;
            return availableVacant.ToList();
        }

        //public List<Vacancy> CheckAvailableTime (Reservation reserve)
        //{
        //    var overlap = from reservelist in _dbContext.Reservations
        //                  where 
        //}
        //public bool CheckId_vacancyRef(int id)
        //{
        //    return _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == id) == null;
        //}

        public bool CancelReseveration(int id)
        {
            try
            {
                if (_dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id) == null)
                {
                    _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id).IsActive = false;
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                Console.WriteLine("Cancel reservation error");
                return false;
            }
        }

        public List<Reservation> GetResverve()
        {
            return _dbContext.Reservations.ToList();
        }

        ///// <summary>
        /////     Delete employee in database
        ///// </summary>
        ///// <param name="staffId">An id of the one who will be deleted</param>
        ///// <returns>
        /////     true - if success
        /////     false - if no id found
        ///// </returns>
        //public bool DeleteEmployee(string staffId)
        //{
        //    if (_dbContext.Employees.Where(x => x.StaffId == staffId) == null)
        //        return false;
        //    _dbContext.Employees.FirstOrDefault(x => x.StaffId == staffId).IsActive = false;
        //    _dbContext.SaveChanges();
        //    return true;
        //}

        ///// <summary>
        /////     Get basic information of the employee
        ///// </summary>
        ///// <param name="staffId">An id of the employee</param>
        ///// <returns>
        /////     Employee - An instance of the employee
        /////     null - if no employee found
        ///// </returns>
        //public Employee GetProfile(string staffId)
        //{
        //    var emp = _dbContext.Employees.FirstOrDefault(x => x.StaffId == staffId);
        //    if (emp == null)
        //        return null;
        //    if (emp.IsActive == false)
        //        return null;
        //    return emp;
        //}
    }
}

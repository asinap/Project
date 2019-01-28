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


        public bool AddReservation(Reservation reserve, string size, int optional)
        {
            try
            {
                switch (optional)
                {
                    case 1: return ReserveByDay(reserve, size);
                    case 2: return ReserveByTime(reserve, size);
                    default: Console.WriteLine("Out of optional");
                        return false;
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
                return false;
            }


        }


        public bool ReserveByDay(Reservation reserve, string size)
        {
            if (CheckId_account(reserve.Id_account))
            {
                return false;
            }
            var nonOverlap = CheckAvailableDay(reserve);
            if (nonOverlap == null)
            {
                return false;
            }
            var inSize = nonOverlap.FirstOrDefault(x => x.Size == size);
            reserve.Id_vacancy = inSize.Id_vacancy;
            reserve.Status = "Unuse";
            _dbContext.Reservations.Add(reserve);
            _dbContext.SaveChanges();
            return true;
        }

        public bool ReserveByTime(Reservation reserve, string size)
        {
            if (CheckId_account(reserve.Id_account))
            {
                return false;
            }
            return true;
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
            var availableVacant = from vacantlist in _dbContext.Vacancies
                                  where !(overlap.Any(x => x.Id_vacancy == vacantlist.Id_vacancy))
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

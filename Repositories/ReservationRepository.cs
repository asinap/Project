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
                if (!reserve.Optional)
                {
                    return ReserveByDay(reserve);
                }
                else {
                    return ReserveByTime(reserve);
                }
                Console.WriteLine("Out of optional");
                return 0;
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
            //4.out of point
            if(_dbContext.Accounts.FirstOrDefault(x=>x.Id_account==reserve.Id_account).Point==0)
            {
                return 4;
            }
            _dbContext.Accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Point -= 5;
            //reserve.DateModified = DateTime.Now;
            _dbContext.Reservations.Add(reserve);
            _dbContext.SaveChanges();
            return 5;
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

        public int CancelReseveration(int id)
        {
            try
            {
                if (_dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id) != null)
                {
                    DateTime date = DateTime.Now;
                    var reserve = _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id);
                    if(reserve.StartDay>date)
                    {
                        _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id).IsActive = false;
                        _dbContext.Accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Point += 5;
                        _dbContext.SaveChanges();
                        return 1;
                    }

                    return 2;
                }
                return 3;
            }
            catch (Exception)
            {
                Console.WriteLine("Cancel reservation error");
                return 0;
            }
        }

        public int SetCode (int id_reserve,string code)
        {
            try
            {
                var list = _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id_reserve);
                if(list.Code.ToLower()=="string")
                {
                    list.Code = code;
                    _dbContext.SaveChanges();
                    return 1;
                }
                return 0;

            }
            catch (Exception)
            {
                Console.WriteLine("Error to set code");
                return 0;
            }
        }
        public List<Reservation> GetReserve()
        {
            return _dbContext.Reservations.ToList();
        }

        public List<Reservation> GetReserve(string id)
        {
            return _dbContext.Reservations.Where(x => x.Id_account == id).ToList();
        }

        public List<Reservation> Pending(string id)//order by recent date
        {
            var list = GetReserve(id);
            return list.Where(x => x.IsActive == true).ToList();
        }

        public List<Reservation> History(string id)//order by recent day
        {
            var list = GetReserve(id);
            return list.Where(x => x.IsActive == false).ToList();
        }

        public int Unuse (string id)
        {
            var list = GetReserve(id);
            return list.Count(x => x.Status == "Unuse");
        }

        public int Use (string id)
        {
            var list = GetReserve(id);
            return list.Count(x => x.Status == "Use");
        }

        public int Penalty (string id)
        {
            var list = GetReserve(id);
            return list.Count(x => x.Status == "Penalty");
        }
        
        public int Expire (string id)
        {
            var list = GetReserve(id);
            return list.Count(x => x.Status == "Expire");
        }

        public int SetStatus(int reserve, string condition)
        {
            try
            {
                if(condition.ToLower()=="use")
                {
                    _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == reserve).Status = condition;
                    var user = _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == reserve);
                    _dbContext.Accounts.FirstOrDefault(x => x.Id_account == user.Id_account).Point += 5;
                    _dbContext.SaveChanges();
                    return 1;
                }
                else
                {
                    _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == reserve).Status = condition;
                    _dbContext.SaveChanges();
                    return 2;
                }
                //return 0;
                
            }
            catch (Exception)
            {
                Console.WriteLine("Error to set state");
                return 0;
            }

        }

        public bool Delete()
        {
            try
            {
                var data = from list in _dbContext.Reservations select list;
                _dbContext.Reservations.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                Console.Write("Cannot delete all Reservation database");
                return false;
            }
        }

        public bool Delete(int id_reserve)
        {
            try
            {
                if (_dbContext.Reservations.Where(x => x.Id_reserve == id_reserve) == null)
                {
                    return false;
                }
                var data = from list in _dbContext.Reservations
                           where list.Id_reserve == id_reserve
                           select list;
                _dbContext.Reservations.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.Write("Cannot delete %s", id_reserve);
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

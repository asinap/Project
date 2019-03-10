using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.Class;
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
                //Console.WriteLine("Out of optional");
                //return 0;
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
            try
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
                var inSize = nonOverlap.FirstOrDefault(x => x.Size == reserve.Size.ToUpper());
                if (inSize == null)
                {
                    return 3;
                }
                reserve.Id_vacancy = inSize.Id_vacancy;
                reserve.Status = "Unuse";
                //4.out of point
                if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Point == 0)
                {
                    return 4;
                }
                _dbContext.Accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Point -= 5;
                //reserve.DateModified = DateTime.Now;
                _dbContext.Reservations.Add(reserve);
                _dbContext.SaveChanges();
                return 5;
            }
            catch(Exception)
            {
                return 0;
            }
        }

        /*not finnish yet!!!!!!!!!!!!!!!*/
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
                        _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id).Status = "Cancel";
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
                //find owner reservation
                string accountID = _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id_reserve).Id_account;
                //find all reservation that owner has booked the reservation and check that Code is not replicated.
                var reservelist = _dbContext.Reservations.FirstOrDefault(x => x.Id_account == accountID && x.Code == code && x.IsActive == true);
                if (reservelist != null)
                {
                    return 3;
                }
                else
                {
                    if(_dbContext.Reservations.FirstOrDefault(x=>x.Id_reserve==id_reserve).Code.ToLower()=="string")
                    {
                        _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id_reserve).Code = code;
                        _dbContext.SaveChanges();
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }


            }
            catch (Exception)
            {
                Console.WriteLine("Error to set code");
                return 0;
            }
        }

        /*web all activity*/
        public List<WebForm> GetActivities()
        {
            try
            {
                var list = _dbContext.Reservations.OrderByDescending(x => x.DateModified);
                List<WebForm> result = new List<WebForm>();
                foreach (var run in list)
                {
                    WebForm tmp = new WebForm()
                    {
                        Status = run.Status,
                        Id_booking = run.Id_reserve,
                        Id_user = run.Id_account,
                        Location = run.Location,
                        DateModified = run.DateModified
                    };
                    result.Add(tmp);
                }
                return result.ToList();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public List<WebForm> GetNotification()
        {
            try
            {
                var list = _dbContext.Reservations.Where(x => x.Status.ToLower() == "timeup").OrderByDescending(x => x.DateModified);
                List<WebForm> result = new List<WebForm>();
                foreach (var run in list)
                {
                    WebForm tmp = new WebForm()
                    {
                        Status = run.Status,
                        Id_booking = run.Id_reserve,
                        Id_user = run.Id_account,
                        Location = run.Location,
                        DateModified = run.DateModified
                    };
                    result.Add(tmp);
                }
                return result.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<BookingForm> Pending(string id)//order by recent date
        {
            try
            {
                //var list = GetReserve(id);
                //if(list==null)
                //{
                //    return null;
                //}
                //var intime = list.Where(x => x.EndDay > DateTime.Now).OrderBy(x => x.StartDay); // change; isActive==true
                var intime = from list in _dbContext.Reservations
                             where list.Id_account == id && list.IsActive == true
                             orderby list.StartDay
                             select list;
                if(intime==null)
                {
                    return null;
                }
                List < BookingForm > result = new List<BookingForm>();
                foreach (var run in intime)
                {
                    string no_vacancy = _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).No_vacancy;
                    BookingForm tmp = new BookingForm()
                    {
                        BookingID = run.Id_reserve,
                        StartDate = run.StartDay,
                        EndDate = run.EndDay,
                        Location = run.Location,
                        Size = run.Size,
                        NumberVacancy = no_vacancy
                    };
                    result.Add(tmp);
                }
                return result.ToList();
            }
            catch(Exception)
            {
                return null;
            }
        }

        public List<BookingForm> History(string id)//order by recent day
        {
            try
            {

                //var list = GetReserve(id);
                //var intime = list.Where(x => x.StartDay < DateTime.Now).OrderByDescending(x => x.EndDay); // change; isActive==false
                var intime = from list in _dbContext.Reservations
                             where list.Id_account == id && list.IsActive == false
                             orderby list.EndDay descending
                             select list;
                if (intime == null)
                {
                    return null;
                }
                List<BookingForm> result = new List<BookingForm>();
                foreach (var run in intime)
                {
                    string no_vacancy = _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).No_vacancy;
                    BookingForm tmp = new BookingForm()
                    {
                        BookingID = run.Id_reserve,
                        StartDate = run.StartDay,
                        EndDate = run.EndDay,
                        Location = run.Location,
                        Size = run.Size,
                        NumberVacancy = no_vacancy
                    };
                    result.Add(tmp);
                }
                return result.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public BookingForm GetBookingDetail (int id_reserve)
        {
            try
            {
                var list = _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id_reserve);
                if(list==null)
                {
                    return null;
                }
                string no_vacancy = _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == list.Id_vacancy).No_vacancy;
                BookingForm result = new BookingForm()
                {
                    UserID = list.Id_account,
                    BookingID = list.Id_reserve,
                    StartDate = list.StartDay,
                    EndDate = list.EndDay,
                    Location = list.Location,
                    Size = list.Size,
                    NumberVacancy = no_vacancy
                };
                return result;
            }
            catch (Exception)
            {
                return null; 
            }
        }

        public ReserveDetail GetReserveDetail(int id_reserve)
        {
            try
            {
                var reserve = _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == id_reserve); //BookingID,UserID,StartDate,EndDate,DateModified,Location,Size
                if(reserve==null)
                {
                    return null;
                }
                string numberVacant = _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == reserve.Id_vacancy).No_vacancy;//NumberVacancy
                string name = _dbContext.Accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Name;
                ReserveDetail result = new ReserveDetail()
                {
                    Id_user = reserve.Id_account,
                    Name = name,
                    BookingID = reserve.Id_reserve,
                    StartDate = reserve.StartDay,
                    EndDate = reserve.EndDay,
                    DateModified = reserve.DateModified,
                    Status = reserve.Status,
                    Location = reserve.Location,
                    Size = reserve.Size,
                    NumberVacancy = numberVacant
                };
                return result;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public List<Reservation> GetReserve()
        {
            return _dbContext.Reservations.ToList();
        }

        public List<Reservation> GetReserve(string id) //by id account
        {
            return _dbContext.Reservations.Where(x => x.Id_account == id).ToList();
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

        public int TimeUp (string id)
        {
            var list = GetReserve(id);
            return list.Count(x => x.Status == "TimeUp");
        }
        
        public int Expire (string id)
        {
            var list = GetReserve(id);
            return list.Count(x => x.Status == "Expire");
        }

        public int Cancel(string id)
        {
            var list = GetReserve(id);
            return list.Count(x => x.Status == "Cancel");
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

        public int SetBoolIsActive (int reserveID,bool isActive)
        {
            try
            {
                var reserve = _dbContext.Reservations.Where(x => x.Id_reserve == reserveID);
                if (reserve == null)
                {
                    return 1;
                }
                _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == reserveID).IsActive = isActive;
                _dbContext.SaveChanges();
                return 2;
            }catch (Exception)
            {
                Console.WriteLine("Error to set state of IsActive");
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

        

    
    }
}

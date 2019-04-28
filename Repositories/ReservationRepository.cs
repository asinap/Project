using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Entities;

namespace test2.Repositories
{
    public class ReservationRepository
    {
        LockerDbContext _dbContext;

        public ReservationRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public int AddReservation(ReservationForm reserve)
        {
            try
            {
                //1. check account exist.
                if (CheckId_account(reserve.Id_account))
                {
                    return 0;
                }

                //2. find non-overlap locker; check available day, free vacancy and right location return list of vacancy
                var nonOverlap = CheckAvailableDay(reserve);
                if (nonOverlap.Count() == 0)
                {
                    return 0;
                }

                //3.find size
                var inSize = nonOverlap.FirstOrDefault(x => x.Size.ToLower() == reserve.Size.ToLower());
                if (inSize == null)
                {
                    return 0;
                }
                reserve.Id_vacancy = inSize.Id_vacancy;
                reserve.Status = Status.Unuse;
                //4.out of point
                if (_dbContext.accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Point <= 0)
                {
                    return 0;
                }

                _dbContext.accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Point -= 5;

                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                //reserve.DateModified = DateTime.Now;
                reserve.DateModified = dateTime;
                //Log.Information("{0}", dateTime);
                Reservation reservation = new Reservation()
                {
                    Id_reserve = reserve.Id_reserve,
                    Id_account = reserve.Id_account,
                    DateModified = reserve.DateModified,
                    StartDay = reserve.StartDay,
                    EndDay = reserve.EndDay,
                    Code = reserve.Code,
                    IsActive = reserve.IsActive,
                    Status = reserve.Status,
                    Id_vacancy = reserve.Id_vacancy
                };
                _dbContext.reservations.Add(reservation);
                _dbContext.SaveChanges();
                return reservation.Id_reserve;
            }
            catch (Exception)
            {
                Console.WriteLine("AddReservation Error");
                return 0;
            }

        }

        /*check if there is an account*/
        public bool CheckId_account(string id)
        {
            return _dbContext.accounts.FirstOrDefault(x => x.Id_account == id) == null;
        }

        /*find available vacancy during the day reservation*/
        public List<Vacancy> CheckAvailableDay(ReservationForm reserve)
        {
            var overlap = from reservelist in _dbContext.reservations
                          where reservelist.StartDay <= reserve.StartDay && reservelist.EndDay >= reserve.StartDay && (reservelist.Status == Status.Use || reservelist.Status == Status.Unuse)
                          select reservelist;
            var availableVacant = from vacantlist in _dbContext.vacancies join lockerlist in _dbContext.lockerMetadatas
                                  on vacantlist.Mac_address equals lockerlist.Mac_address
                                  where !(overlap.Any(x => x.Id_vacancy == vacantlist.Id_vacancy)) && lockerlist.Location == reserve.Location
                                  select vacantlist;
            return availableVacant.ToList();
        }

        /*Cancel reservation from user through mobile application*/
        public int CancelReseveration(int id)
        {
            try
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                var reserve = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id);
                if (reserve!=null)
                {
                    
                    if(DateTime.Compare(reserve.StartDay,dateTime)>0)
                    {
                        _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id).IsActive = false;
                        _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id).Status = Status.Cancel;
                        _dbContext.accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Point += 5;
                        _dbContext.SaveChanges();
                        return 1; //return cancel reservation success
                    }

                    return 2; //return cannot cancel reservation cause time
                }
                return 3; //return if there is no reservation
            }
            catch (Exception)
            {
                //error
                Console.WriteLine("Cancel reservation error");
                return 0;
            }
        }

        /*Check if code is set*/
        public int IsSetCode (int reserveID)
        {
            try
            {
                //if code is not set
                if(_dbContext.reservations.FirstOrDefault(x=>x.Id_reserve==reserveID).Code.ToLower()=="string")
                {
                    return 1;
                }
                //if code is set
                return 2;
            }
            catch
            {
                //error
                Console.WriteLine("Error to check code");
                return 0;
            }
        }

        /*Set Code in order to opent the locker*/
        public int SetCode (int id_reserve,string code)
        {
            try
            {
                //find owner reservation
                string accountID = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id_reserve).Id_account;
                //find all reservation that owner has booked the reservation and check that Code is not replicated.
                var reservelist = _dbContext.reservations.FirstOrDefault(x => x.Id_account == accountID && x.Code == code && x.IsActive == true);
                if (reservelist != null)
                {
                    return 2;
                }
                else
                {
                        _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id_reserve).Code = code;
                        _dbContext.SaveChanges();
                        return 1;
                }


            }
            catch (Exception)
            {
                //error
                Console.WriteLine("Error to set code");
                return 0;
            }
        }

        /*Get code from database to return to user*/
        public string GetCode (int reserveID)
        {
            try
            {
                //find reservation by id
                var result = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == reserveID);
                //if there is a code and locker still active
                if (result.Code.ToLower() != "string" && result.IsActive == true)
                {
                    return result.Code;
                }
                //if there is a code and locker is not active
                else if (result.Code.ToLower() != "string" && result.IsActive == false)
                {
                    return result.Code;
                }
                //out of condition
                else
                {
                    return null;
                }

            }
            catch
            {
                Console.WriteLine("Error to get code");
                return null;
            }
        }
        /*web all activity*/
        public List<WebForm> GetActivities()
        {
            try
            {
                /*Get all reservation*/
                var list = _dbContext.reservations.OrderByDescending(x => x.DateModified);
                //create reservation form to return to administrator
                List<WebForm> result = new List<WebForm>();
                foreach (var run in list)
                {
                    string mac_address = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).Mac_address;
                    string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
                    WebForm tmp = new WebForm()
                    {
                        Status = run.Status,
                        Id_booking = run.Id_reserve,
                        Id_user = run.Id_account,
                        Location = location,
                        DateModified = run.DateModified
                    };
                    result.Add(tmp);
                }
                return result.ToList();
            }
            catch (Exception)
            {
                //error
                return null;
            }

        }

        public List<WebForm> GetNotification()
        {
            try
            {
                /*get all reservation that is time up and sort by datemodified*/
                var list = _dbContext.reservations.Where(x => x.Status == Status.Timeup).OrderByDescending(x => x.DateModified);
                //create form to return to administrator
                List<WebForm> result = new List<WebForm>();
                foreach (var run in list)
                {
                    string mac_address = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).Mac_address;
                    string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
                    WebForm tmp = new WebForm()
                    {
                        Status = run.Status,
                        Id_booking = run.Id_reserve,
                        Id_user = run.Id_account,
                        Location = location,
                        DateModified = run.DateModified
                    };
                    result.Add(tmp);
                }
                return result.ToList();
            }
            catch (Exception)
            {
                //error
                return null;
            }
        }

        /*Get all reservation each user that still active and not expire*/
        public List<BookingForm> Pending(string id)//order by recent date
        {
            try
            {
                /*get all reservation each user that has status active is true*/
                var intime = from list in _dbContext.reservations
                             where list.Id_account == id && list.IsActive == true
                             orderby list.StartDay
                             select list;
                //if reservation is null
                if(intime==null)
                {
                    return null;
                }
                //create form to return to user
                List < BookingForm > result = new List<BookingForm>();
                foreach (var run in intime)
                {
                    string mac_address = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).Mac_address;
                    string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;


                    string no_vacancy = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).No_vacancy;

                    string size = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).Size;
                    BookingForm tmp = new BookingForm()
                    {
                        BookingID = run.Id_reserve,
                        StartDate = run.StartDay,
                        EndDate = run.EndDay,
                        Location = location,
                        Size = size,
                        NumberVacancy = no_vacancy
                    };
                    result.Add(tmp);
                }
                return result.ToList();
            }
            catch(Exception)
            {
                //error
                return null;
            }
        }

        /*Get all reservation each user that inactive and expire*/
        public List<BookingForm> History(string id)//order by recent day
        {
            try
            {

                /*get all reservation each user that has status isactive is false*/
                var intime = from list in _dbContext.reservations
                             where list.Id_account == id && list.IsActive == false
                             orderby list.EndDay descending
                             select list;
                //if reservation is null
                if (intime == null)
                {
                    return null;
                }
                //create form to return to user
                List<BookingForm> result = new List<BookingForm>();
                foreach (var run in intime)
                {
                    string mac_address = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).Mac_address;
                    string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;


                    string no_vacancy = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).No_vacancy;

                    string size = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).Size;
                    BookingForm tmp = new BookingForm()
                    {
                        BookingID = run.Id_reserve,
                        StartDate = run.StartDay,
                        EndDate = run.EndDay,
                        Location = location,
                        Size = size,
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

        /*Get reservation detail for mobile*/
        public BookingForm GetBookingDetail (int id_reserve)
        {
            try
            {
                //find reservation
                var list = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id_reserve);
                //if it is null
                if(list==null)
                {
                    return null;
                }
                int id_vacant = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id_reserve).Id_vacancy;
                string mac_address = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == id_vacant).Mac_address;
                string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
                string no_vacancy = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == list.Id_vacancy).No_vacancy;
                string size = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == id_vacant).Size;
                //create form in order to return to the user
                BookingForm result = new BookingForm()
                {
                    UserID = list.Id_account,
                    BookingID = list.Id_reserve,
                    StartDate = list.StartDay,
                    EndDate = list.EndDay,
                    Location = location,
                    Size = size,
                    NumberVacancy = no_vacancy
                };
                return result;
            }
            catch (Exception)
            {
                //error
                return null; 
            }
        }

        /*Get reservation detail for web*/
        public ReserveDetail GetReserveDetail(int id_reserve)
        {
            try
            {
                //find reservation
                var reserve = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id_reserve); //BookingID,UserID,StartDate,EndDate,DateModified,Location,Size
                //if it is null
                if (reserve==null)
                {
                    return null;
                }
                int id_vacant = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == id_reserve).Id_vacancy;
                string mac_address = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == id_vacant).Mac_address;
                string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
                string no_vacancy = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == id_vacant).No_vacancy;
                string size = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == id_vacant).Size;
                string name = _dbContext.accounts.FirstOrDefault(x => x.Id_account == reserve.Id_account).Name;
                //create form in order to return to the user
                ReserveDetail result = new ReserveDetail()
                {
                    Id_user = reserve.Id_account,
                    Name = name,
                    BookingID = reserve.Id_reserve,
                    StartDate = reserve.StartDay,
                    EndDate = reserve.EndDay,
                    DateModified = reserve.DateModified,
                    Status = reserve.Status,
                    Location = location,
                    Size = size,
                    NumberVacancy = no_vacancy
                };
                return result;
            }
            catch (Exception)
            {
                //error
                return null;
            }

        }

        /*TEST get all reservation*/
        public List<Reservation> GetReserve()
        {
            return _dbContext.reservations.ToList();
        }

        /*TEST get specidic reservation*/
        public List<Reservation> GetReserve(string id) //by id account
        {
            return _dbContext.reservations.Where(x => x.Id_account == id).ToList();
        }

        /*Set status reservation*/
        public int SetStatus(int reserve, string condition)
        {
            try
            {
                //if reervation's status = unuse and codition = use
                if(_dbContext.reservations.FirstOrDefault(x=>x.Id_reserve==reserve).Status==Status.Unuse&&condition==Status.Use)
                {
                    _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == reserve).Status = condition;
                    var user = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == reserve);
                    _dbContext.accounts.FirstOrDefault(x => x.Id_account == user.Id_account).Point += 5;
                    _dbContext.SaveChanges();
                    return 1;
                }
                //other condition
                else
                {
                    _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == reserve).Status = condition;
                    _dbContext.SaveChanges();
                    return 2;
                }
                //return 0;
                
            }
            catch (Exception)
            {
                //error
                Console.WriteLine("Error to set state");
                return 0;
            }

        }

        /*TEST set about active and inactive reservation*/
        public int SetBoolIsActive (int reserveID,bool isActive)
        {
            try
            {
                var reserve = _dbContext.reservations.Where(x => x.Id_reserve == reserveID);
                //if there is no reservation
                if (reserve == null)
                {
                    return 1;
                }
                //if there is a reservation
                _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == reserveID).IsActive = isActive;
                _dbContext.SaveChanges();
                return 2;
            }catch (Exception)
            {
                //error
                Console.WriteLine("Error to set state of IsActive");
                return 0;
            }
        }

    
    }
}

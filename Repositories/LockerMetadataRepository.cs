using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;

namespace test2.Repositories
{
    public class LockerMetadataRepository
    {
        LockerDbContext _dbContext;

        public LockerMetadataRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* Add Locker                                                                   *
         * input = locker                                                               *
         *          Attributes => string Mac_address, string Location, bool IsActive    *
         *                          string Mac_address can not be dupilcated and null   */
        public bool AddLocker(LockerMetadata locker)
        {

            try
            {
                //if mac is replicated
                if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address) != null)
                {
                    //if mac is replicated and locker is active.
                    if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address && x.IsActive == true) != null)
                    {
                        return false;
                    }
                    //if mac is replicated and locker is inactive.
                    else
                    {
                        //location is replicated.
                        if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Location.ToLower() == locker.Location.ToLower()) != null)
                        {
                            return false;
                        }
                        else
                        {
                            _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address).Location = locker.Location;
                            _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address).IsActive = true;
                            _dbContext.SaveChanges();
                            return true;
                        }
                    }

                }
                //if mac is not replicated
                else
                {
                    //location is replicated.
                    if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Location.ToLower() == locker.Location.ToLower()) != null)
                    {
                        return false;
                    }
                    else
                    {
                        _dbContext.lockerMetadatas.Add(locker);
                        _dbContext.SaveChanges();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                //error
                Console.WriteLine("Have %s it already", locker.Mac_address.ToString());
                return false;
            }
        }

        /* Delete Locker                *
         * Input = string Mac_address   *
         *      set IsActive = false    */
        public bool DeleteLocker(string Mac_address)
        {
            try
            {
                //checking locker is existed
                if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address) != null)
                {
                    //checking locker is used
                    if (CheckVacancyInuse(Mac_address) != null)
                    {
                        return false;
                    }

                    _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).IsActive = false;

                    // set all vacancy at that locker can not be used ; set all IsActive = false 
                    var vacancylocker = from vacantlist in _dbContext.vacancies
                                        where vacantlist.Mac_address == Mac_address
                                        select vacantlist;
                    if (vacancylocker != null)
                    {
                        vacancylocker.ToList().ForEach(x => x.IsActive = false);
                    }
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                //error
                Console.WriteLine("No have %s\t%s it already", Mac_address);
                return false;
            }
        }

        /*check that there are a vacancy on working*/
        public List<Reservation> CheckVacancyInuse(string mac_address)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            //string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
            var vacant = _dbContext.vacancies.Where(x => x.Mac_address == mac_address);
            var inUse = from reservelist in _dbContext.reservations
                        where reservelist.StartDay > dateTime && vacant.Any(x=>x.Id_vacancy==reservelist.Id_vacancy)
                        select reservelist;
            //if there is no used vacancy
            if (inUse.Count()==0)
                return null;
            //if there is used vacancy
            else
                return inUse.ToList();
        }

        /* Restore Locker               *
         * Input = string Mac_address   *
         *      set IsActive = true     */
        public bool RestoreLocker(string Mac_address)//consider no_vacant and mac_address to set active
        {
            try
            {
                //check locker is existed
                if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address) != null)
                {
                    _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).IsActive = true;

                    // set all vacancy at that locker can be used ; set all IsActive = true
                    var vacancylocker = from vacantlist in _dbContext.vacancies
                                        where vacantlist.Mac_address == Mac_address
                                        select vacantlist;
                    if (vacancylocker != null)
                    {
                        vacancylocker.ToList().ForEach(x => x.IsActive = true);
                    }
                    _dbContext.SaveChanges();
                    return true;
                }
                //if locker doesn't existed
                return false;

            }
            catch (Exception)
            {
                //error
                Console.WriteLine("No have %s\t%s it already", Mac_address);
                return false;
            }
        }

        /* Edit Locker                          *
         * Input = locker                       *
         *      edit some info about the locker */
        public bool EditLocker(LockerMetadata locker)
        {
            try
            {

                var _locker = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address);
                //check locker is existed
                if(_locker!=null)
                {
                    _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address).IsActive = locker.IsActive;
                    _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address).Location = locker.Location;
                    _dbContext.SaveChanges();
                    return true;
                }
                //if locker is not existed
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                //error
                Console.WriteLine("Cannot Edit Locker");
                return false;
            }

        }
        /* Get all locker                       *
         *  return all locker to string  */
        public List<LockerMetadata> GetLocker()
        {
            return _dbContext.lockerMetadatas.ToList();
        }

        /* Get all locker                       *
         *  return all locker and IsActive=true to string  */
        public List<LockerMetadata> GetLockerMobile()
        {
            return _dbContext.lockerMetadatas.Where(x => x.IsActive == true).ToList();
        }
        /* Get specific locker                          *
         * Input = string Mac_address                   *
         * return locker that has Mac_address = input   */
        public List<LockerMetadata> GetLocker(string mac_address)
        {

            return _dbContext.lockerMetadatas.Where(x => x.Mac_address == mac_address).ToList();
        }

        /* Get active locker                        *
         * return locker that has IsActive = true   */
         public List<LockerMetadata> GetActivelocker()
        {
            var activelocker = from lockerlist in _dbContext.lockerMetadatas
                               where lockerlist.IsActive == true
                               select lockerlist;
            return activelocker.ToList();
        }

        /* Get inactive locker                      *
         * return locker that has IsActive = false  */
         public List<LockerMetadata> GetInactivelocker()
        {
            var inactivelocker = from lockerlist in _dbContext.lockerMetadatas
                                 where lockerlist.IsActive == false
                                 select lockerlist;
            return inactivelocker.ToList();
        }

        /*  Get locker detail from web application by administrator */
        public LockerDetail GetLockerDetail (string mac_address)
        {
            try
            {
                //if locker is not existed
                if(_dbContext.lockerMetadatas.FirstOrDefault(x=>x.Mac_address==mac_address)==null)
                {
                    return null;
                }
                //if locker is existed
                var vacantlist = _dbContext.vacancies.Where(x => x.Mac_address == mac_address);
                string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
                //create vacancy list
                List<VacancyDetail> vacancyDetails = GetVacantList(mac_address);
                LockerDetail lockerDetail = new LockerDetail()
                {
                    LockerID = mac_address,
                    Location = location,
                    Vacancieslist = vacancyDetails
                };
                return lockerDetail;
            }
            catch (Exception)
            {
                //error
                return null;
            }
        }

        /*Find all vacancy in the locker*/
        public List<VacancyDetail> GetVacantList(string mac_address)
        {
            try
            {
                //if locker is not existed
                if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address) == null)
                {
                    return null;
                }
                //if locker is existed
                var vacantlist = _dbContext.vacancies.Where(x => x.Mac_address == mac_address).OrderBy(x => x.Id_vacancy);
                //create vacancy list in vacancy form
                List<VacancyDetail> result = new List<VacancyDetail>();
                foreach (var run in vacantlist)
                {
                    VacancyDetail lockerDetail = new VacancyDetail()
                    {
                        VacancyID = run.Id_vacancy,
                        No_vacancy = run.No_vacancy,
                        Size = run.Size,
                        IsActive = run.IsActive
                    };
                    result.Add(lockerDetail);
                }
                return result;
            }
            catch(Exception)
            {
                //error
                return null;
            }
        }
    }
}

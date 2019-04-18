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
                //Mac_address is primary key, can not be duplicated and can not be null
                //if ((_dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address)) != null)
                //{
                //    return false;
                //}
                if(_dbContext.lockerMetadatas.FirstOrDefault(x=>x.Location.ToLower()==locker.Location.ToLower())!=null)
                {
                    return false;
                }
                _dbContext.lockerMetadatas.Add(locker);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
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
                if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address) != null)
                {
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
                Console.WriteLine("No have %s\t%s it already", Mac_address);
                return false;
            }
        }

        public List<Reservation> CheckVacancyInuse(string mac_address)
        {
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
            string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
            var inUse = from reservelist in _dbContext.reservations
                        where reservelist.StartDay > dateTime && reservelist.Location ==location
                        select reservelist;
            if (inUse.Count()==0)
                return null;
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
                return false;

            }
            catch (Exception)
            {
                Console.WriteLine("No have %s\t%s it already", Mac_address);
                return false;
            }
        }
        public bool EditLocker(LockerMetadata locker)
        {
            try
            {
                var _locker = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address);

                if(_locker!=null)
                {
                    _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address).IsActive = locker.IsActive;
                    _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == locker.Mac_address).Location = locker.Location;
                    _dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Cannot Edit Locker");
                return false;
            }

        }
        /* Get all locker                       *
         *  return all locker detail to string  */
        public List<LockerMetadata> GetLocker()
        {
            return _dbContext.lockerMetadatas.ToList();
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

        /*Delete*/
        public bool Delete()
        {
            try
            {
                var data = from list in _dbContext.lockerMetadatas select list;
                _dbContext.lockerMetadatas.RemoveRange(data);
                var vacant = from list in _dbContext.vacancies select list;
                _dbContext.vacancies.RemoveRange(vacant);
                _dbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                Console.Write("Cannot delete all LockerMetadatas database");
                return false;
            }
        }

        public bool Delete(string mac_address)
        {
            try
            {
                if (_dbContext.lockerMetadatas.Where(x => x.Mac_address == mac_address) == null)
                {
                    return false;
                }
                var data = from list in _dbContext.lockerMetadatas
                           where list.Mac_address == mac_address
                           select list;
                _dbContext.lockerMetadatas.RemoveRange(data);
                var vacant = from list in _dbContext.vacancies
                             where list.Mac_address == mac_address
                             select list;
                _dbContext.vacancies.RemoveRange(vacant);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.Write("Cannot delete %s", mac_address);
                return false;
            }
        }

        public LockerDetail GetLockerDetail (string mac_address)
        {
            try
            {
                if(_dbContext.lockerMetadatas.FirstOrDefault(x=>x.Mac_address==mac_address)==null)
                {
                    return null;
                }
                var vacantlist = _dbContext.vacancies.Where(x => x.Mac_address == mac_address);
                string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
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
                return null;
            }
        }

        public List<VacancyDetail> GetVacantList(string mac_address)
        {
            try
            {
                if(_dbContext.lockerMetadatas.FirstOrDefault(x=>x.Mac_address==mac_address)==null)
                {
                    return null;
                }
                var vacantlist = _dbContext.vacancies.Where(x => x.Mac_address == mac_address).OrderBy(x => x.Id_vacancy);
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
                return null;
            }
        }
    }
}

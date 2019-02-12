using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                _dbContext.LockerMetadatas.Add(locker);
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
                if (_dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address) != null)
                {
                    _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).IsActive = false;

                    // set all vacancy at that locker can not be used ; set all IsActive = false 
                    var vacancylocker = from vacantlist in _dbContext.Vacancies
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

        /* Restore Locker               *
         * Input = string Mac_address   *
         *      set IsActive = true     */
        public bool RestoreLocker(string Mac_address)//consider no_vacant and mac_address to set active
        {
            try
            {

                if (_dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address) != null)
                {
                    _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == Mac_address).IsActive = true;

                    // set all vacancy at that locker can be used ; set all IsActive = true
                    var vacancylocker = from vacantlist in _dbContext.Vacancies
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

        /* Get all locker                       *
         *  return all locker detail to string  */
        public List<LockerMetadata> GetLocker()
        {
            return _dbContext.LockerMetadatas.ToList();
        }

        /* Get specific locker                          *
         * Input = string Mac_address                   *
         * return locker that has Mac_address = input   */
        public List<LockerMetadata> GetLocker(string mac_address)
        {

            return _dbContext.LockerMetadatas.Where(x => x.Mac_address == mac_address).ToList();
        }

        /* Get active locker                        *
         * return locker that has IsActive = true   */
         public List<LockerMetadata> GetActivelocker()
        {
            var activelocker = from lockerlist in _dbContext.LockerMetadatas
                               where lockerlist.IsActive == true
                               select lockerlist;
            return activelocker.ToList();
        }

        /* Get inactive locker                      *
         * return locker that has IsActive = false  */
         public List<LockerMetadata> GetInactivelocker()
        {
            var inactivelocker = from lockerlist in _dbContext.LockerMetadatas
                                 where lockerlist.IsActive == false
                                 select lockerlist;
            return inactivelocker.ToList();
        }

        /*Delete*/
        public bool Delete()
        {
            try
            {
                var data = from list in _dbContext.LockerMetadatas select list;
                _dbContext.LockerMetadatas.RemoveRange(data);
                var vacant = from list in _dbContext.Vacancies select list;
                _dbContext.Vacancies.RemoveRange(vacant);
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
                if (_dbContext.LockerMetadatas.Where(x => x.Mac_address == mac_address) == null)
                {
                    return false;
                }
                var data = from list in _dbContext.LockerMetadatas
                           where list.Mac_address == mac_address
                           select list;
                _dbContext.LockerMetadatas.RemoveRange(data);
                var vacant = from list in _dbContext.Vacancies
                             where list.Mac_address == mac_address
                             select list;
                _dbContext.Vacancies.RemoveRange(vacant);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.Write("Cannot delete %s", mac_address);
                return false;
            }
        }
    }
}

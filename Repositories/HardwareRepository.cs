using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.Class;
using test2.DatabaseContext;
using test2.Entities;

namespace test2.Repositories
{
    public class HardwareRepository
    {
        LockerDbContext _dbContext;

        public HardwareRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Hardware GetHardware(string userID, string code, string mac_address)
        {
            try
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                //checking user is existed
                if (_dbContext.accounts.FirstOrDefault(x=>x.Id_account==userID)==null)
                {
                    return null;
                }
                //checking locker is existed
                if (_dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address) == null)
                {
                    return null;
                }

                //find location
                string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;

                //find vacancy in the locker
                var vacantlist = _dbContext.vacancies.Where(x => x.Mac_address == mac_address);

                /*find reservation that 1. id_account == userID 
                                        2. isActive = true
                                        3. status = unuse / use 
                                        4. reservation  startday<now and endday>now
                                        5. id_vacancy in vacantlist*/
                var allreserve = from reservelist in _dbContext.reservations
                                 where  reservelist.Id_account == userID 
                                        && reservelist.Code == code
                                        && reservelist.IsActive == true 
                                        && (reservelist.Status == Status.Use || reservelist.Status == Status.Unuse)
                                        && reservelist.StartDay<= dateTime && reservelist.EndDay>= dateTime
                                        && vacantlist.Any(x=>x.Id_vacancy==reservelist.Id_vacancy) 
                                 select reservelist;
                //check code = code
                var reserve = allreserve.FirstOrDefault(x => x.Code == code);
                //if there is no reservation
                if (reserve == null)
                {
                    Hardware result = new Hardware()
                    {
                        ReserveID = reserve.Id_reserve,
                        State = reserve.Status,
                        No_vacancy = "None",
                        Ok = false
                    };
                    return result;
                }
                //if there is reservation
                else
                {
                    string no_vacant = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == reserve.Id_vacancy).No_vacancy;
                    Hardware result = new Hardware()
                    {
                        ReserveID = reserve.Id_reserve,
                        State = reserve.Status,
                        No_vacancy = no_vacant,
                        Ok = true
                    };
                    return result;
                }
            }
            catch (Exception)
            {
                //error
                return null;
            }
        }
    }
}

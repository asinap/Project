using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.Class;
using test2.DatabaseContext;

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
                if(_dbContext.Accounts.FirstOrDefault(x=>x.Id_account==userID)==null)
                {
                    return null;
                }
                if (_dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address) == null)
                {
                    return null;
                }
                string location = _dbContext.LockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
                var allreserve = from reservelist in _dbContext.Reservations
                                 where reservelist.Id_account == userID && reservelist.Location == location && reservelist.IsActive == true && (reservelist.Status == "Use" || reservelist.Status == "Unuse")
                                 select reservelist;
                var reserve = allreserve.FirstOrDefault(x => x.Code == code);
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
                else
                {
                    string no_vacant = _dbContext.Vacancies.FirstOrDefault(x => x.Id_vacancy == reserve.Id_vacancy).No_vacancy;
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
                return null;
            }
        }
    }
}

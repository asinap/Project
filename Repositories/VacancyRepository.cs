using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;

namespace test2.Repositories
{
    public class VacancyRepository
    {
        LockerDbContext _dbContext;

        public VacancyRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* Add vacancy for each locker                                                                  *
         * input = int Id_vacancy, string No_vacancy, string Mac_address, string Size, bool IsActive    *
         *          int Id_vacancy is prinmary key can not be duplicated and null                       *
         *          srtring Mac_address is foreign key, Mac_addresa must be in LockerMetadata database  */
        public bool AddVacancy(Vacancy vacant)
        {
            try
            {
                if (CheckMac_address(vacant.Mac_address))
                {
                    return false;
                }
                else
                {
                    if(!CheckNo_vacant(vacant.Mac_address,vacant.No_vacancy))
                    {
                        return false;
                    }
                    vacant.Size = vacant.Size.ToLower();
                    _dbContext.vacancies.Add(vacant);
                    _dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Have %s\t%s it already on AddVacancy()", vacant.Mac_address, vacant.No_vacancy);
                return false;
            }
        }

        /* Check Mac_address                                                    *
         * check that there is mac_address in lockermetadata database           *
         * return true if there is not mac_address in lockermetadata database   */
        public bool CheckMac_address(string mac_address)
        {
            return _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address) == null;
        }

        public bool CheckNo_vacant (string mac_address, string no_vacant)
        {
            var list = from vacantlist in _dbContext.vacancies
                       where vacantlist.Mac_address == mac_address && vacantlist.No_vacancy == no_vacant
                       select vacantlist;
            if (list == null)
            {
                return false;
            }
            else
                return true;

        }

        public bool EditVacancy(Vacancy vacant)
        {
            try
            {
                var _vacant = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == vacant.Id_vacancy);
                if(vacant!=null)
                {
                    _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == vacant.Id_vacancy).IsActive = vacant.IsActive;
                    _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == vacant.Id_vacancy).Mac_address = vacant.Mac_address;
                    _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == vacant.Id_vacancy).No_vacancy = vacant.No_vacancy;
                    _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == vacant.Id_vacancy).Size = vacant.Size;
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
                Console.WriteLine("Cannot Edit Vacancy");
                return false;
            }
        }

        /* Delete vacancy                                                                           *
         * input = string Mac_address, string No_vacancy                                            *
         *  vacancy has that mac_address and that No_vacancy was assigned false value to IsActive   */
        public bool DeleteVacancy(string No_vacant, string Mac_address)
        {
            try
            {
                var result = from vacantlist in _dbContext.vacancies
                             where vacantlist.Mac_address == Mac_address
                             select vacantlist;

                result.FirstOrDefault(x => x.No_vacancy == No_vacant).IsActive = false;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Have %s\t%s it already on DeleteVacancy", Mac_address, No_vacant);
                return false;
            }
        }

        /* Update active vacancy                                                                *
         * input = string No_vacancy, string Mac_address                                        *
         * vacancy has that mac_address and that No_vacancy was assigned true value to IsActive */
        public bool UpdateActive(string No_vacant, string Mac_address)//consider no_vacant and mac_address to set active
        {
            try
            {
                /* var result = from vacantlist in _dbContext.Vacancies
                              where vacantlist.Mac_addressRef == Mac_address
                              select vacantlist;*/

                var result = from vacanlist in _dbContext.vacancies
                             join lockerlist in _dbContext.lockerMetadatas on vacanlist.Mac_address equals lockerlist.Mac_address
                             where lockerlist.IsActive == true && vacanlist.Mac_address == Mac_address
                             select vacanlist;
                if (result != null)
                {
                    result.FirstOrDefault(x => x.No_vacancy == No_vacant).IsActive = true;
                }

                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                //Console.WriteLine("No have %s\t%s it already on UpdateActive()", Mac_address, No_vacant);
                return false;
            }
        }

        /* Update size                                                                      *
         * input = string No_vacacy, string Mac_address, string size                        *
         * change size of vacancy that there are No_vacancy = input and Mac_address = input */
        public bool UpdateSize(string No_vacant, string Mac_address, string Size)
        {
            try
            {
                var result = from vacantlist in _dbContext.vacancies
                             where vacantlist.Mac_address == Mac_address && vacantlist.No_vacancy == No_vacant && vacantlist.IsActive == true
                             select vacantlist;
                if (result != null)
                {
                    result.FirstOrDefault(x => x.No_vacancy == No_vacant).Size = Size;
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                Console.WriteLine("No have %s\t%s it process  UpdateSize()", Mac_address, No_vacant);
                return false;
            }
        }

        /* Get all vacancy      * 
         * return all vacancy   */
        public List<Vacancy> GetVacancy()
        {
            return _dbContext.vacancies.ToList();
        }

        /* Get Specific vacancy                     *
         * input = int id                           *
         * return vacancy that has id_vacancy = id  */
        public List<Vacancy> GetVacancy(int id)
        {

            return _dbContext.vacancies.Where(x => x.Id_vacancy == id).ToList();
        }

  
        /* Get active vacancy                   *
         * return vacancy has IsActive = true   */
        public List<Vacancy> GetActive()
        {
            return _dbContext.vacancies.Where(x => x.IsActive == true).ToList();
        }

        /* Get inactive vacancy                 *
         * return vacancy has IsActive = false  */
        public List<Vacancy> GetInactive()
        {
            return _dbContext.vacancies.Where(x => x.IsActive == false).ToList();
        }

        public bool Delete()
        {
            try
            {
                var data = from list in _dbContext.vacancies select list;
                _dbContext.vacancies.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                Console.Write("Cannot delete all Vacancy database");
                return false;
            }
        }

        public bool Delete(int id_vacant)
        {
            try
            {
                if (_dbContext.vacancies.Where(x => x.Id_vacancy == id_vacant) == null)
                {
                    return false;
                }
                var data = from list in _dbContext.vacancies
                           where list.Id_vacancy == id_vacant
                           select list;
                _dbContext.vacancies.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.Write("Cannot delete %s", id_vacant);
                return false;
            }
        }


    }
}

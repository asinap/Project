using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;

namespace test2.Repositories
{
    public class AccountRepository
    {
        LockerDbContext _dbContext;

        public AccountRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* Add new account from register user
         * input = account 
                    attributes => Id_account, Name, Phone, Email, Role, Point
                    default Point = 100 */
        public bool AddUserAccount(Account account)
        {
            try
            {
                if(_dbContext.Accounts.FirstOrDefault(x=>x.Id_account == account.Id_account)==null)
                {
                    Console.WriteLine("already exist");
                    return false;
                }
                account.Point = 100;
                account.Role = "User";
                _dbContext.Accounts.Add(account);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("AddAccount Error");
                return false;
            }
        }

        /* Add new account from register Administrator (optional adding random id for assign each administrator)    *
         * input = account                                                                                          *
                    attributes => Id_account, Name, Phone, Email, Role, Point                                       *
                    default Point = 100                                                                             */
        public bool AddAdminAccount(Account account)
        {
            try
            {
                if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == account.Id_account) == null)
                {
                    Console.WriteLine("already exist");
                    return false;
                }
                account.Point = 0;
                account.Role = "Administrator";
                _dbContext.Accounts.Add(account);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("AddAccount Error");
                return false;
            }
        }

        /* Add phone number in setting that belong to each user
         * input = Id_account and Phone Number */
        public bool AddPhoneNumber(string id, string phone)
        {
            try
            {
                if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == id) != null)
                {
                    _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id).Phone = phone;
                    _dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                Console.WriteLine("AddPhoneNumber Error");
                return false;
            }
        }

        /*Not finish yet*/
        public bool UpdatePoint(string id, int num)
        {
            try
            {
                if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == id) != null)
                {
                    _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id).Point = num;
                    _dbContext.SaveChanges();
                    return true;

                }
                return false;
            }
            catch (Exception)
            {
                Console.WriteLine("AddPhoneNumber Error");
                return false;
            }
        }

        /* Get All User Account 
         * return all account to string */
        public List<Account> GetUserAccount()
        {
            var userlist = from accountlist in _dbContext.Accounts
                           where accountlist.Role == "User"
                           select accountlist;
            return userlist.ToList();
        }

        /* Get specific User Account 
         * Input = Id_student 
           return account that has Id_student equal input*/
        public List<Account> GetUserAccount(string id)
        {

            return _dbContext.Accounts.Where(x => x.Id_account == id).ToList();
        }

        /*Admin*/
        public List<Account> GetAdminAccount()
        {
            var adminlist = from accountlist in _dbContext.Accounts
                             where accountlist.Role == "Administrator"
                             select accountlist;
            return adminlist.ToList();
        }
    }
}

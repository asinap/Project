using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.Class;
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
        public int AddUserAccount(Account account)
        {
            try
            {
                string id = "";
                string domainmail = "@kmitl.ac.th";
                if (!account.Email.Contains(domainmail))
                {
                    // detect domain email
                    return 1;
                }
                id = account.Email.Replace(domainmail, "");
                int numberic;
                if (!int.TryParse(id, out numberic))
                {
                    //before email is not studentID
                    return 2;
                }
                account.Id_account = id;
                if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == account.Id_account) != null)
                {
                    //already have account
                    Console.WriteLine("already exist");
                    return 3;
                }
                account.Point = 100;
                account.Role = "User";
                _dbContext.Accounts.Add(account);
                _dbContext.SaveChanges();
                return 4;
            }
            catch (Exception)
            {
                Console.WriteLine("AddUserAccount Error");
                return 0;
            }
        }

        /* Add new account from register Administrator (optional adding random id for assign each administrator)    *
         * input = account                                                                                          *
                    attributes => Id_account, Name, Phone, Email, Role, Point                                       *
                    default Point = 100                                                                             */
        public int AddAdminAccount(Account account)
        {
            try
            {
                if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == account.Id_account) != null)
                {
                    Console.WriteLine("already exist");
                    return 1;
                }
                int rnd;
                do
                {
                    Random rand = new Random();
                    rnd = rand.Next(100000000, 999999999);
                } while (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == rnd.ToString()) != null);
                account.Id_account = rnd.ToString();
                account.Point = 0;
                account.Role = "Administrator";
                _dbContext.Accounts.Add(account);
                _dbContext.SaveChanges();
                return 2;
            }
            catch (Exception)
            {
                Console.WriteLine("AddAdminAccount Error");
                return 0;
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
        //public bool UpdatePoint(string id, int num)
        //{
        //    try
        //    {
        //        if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == id) != null)
        //        {
        //            _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id).Point = num;
        //            _dbContext.SaveChanges();
        //            return true;

        //        }
        //        return false;
        //    }
        //    catch (Exception)
        //    {
        //        Console.WriteLine("Cannot update point");
        //        return false;
        //    }
        //}

        /* Get All User Account 
         * return all account to string */
        public List<Member> GetUserAccount()
        {
            try
            {
                var userlist = from accountlist in _dbContext.Accounts
                               where accountlist.Role.ToString().ToLower() == "user"
                               select accountlist;
                List<Member> resultlist = new List<Member>();

                foreach (var run in userlist)
                {
                    int usingCount = _dbContext.Reservations.Count(x => x.Id_account == run.Id_account && x.Status.ToLower() == "use");
                    int bookedCount = _dbContext.Reservations.Count(x => x.Id_account == run.Id_account);
                    int timeUpCount = _dbContext.Reservations.Count(x => x.Id_account == run.Id_account && x.Status.ToLower() == "timeup") + _dbContext.Reservations.Count(x => x.Id_account == run.Id_account && x.Status.ToLower() == "expire");
                    Member newby = new Member()
                    {
                        Id_account = run.Id_account,
                        Name = run.Name,
                        Using = usingCount,
                        Booked = bookedCount,
                        TimeUp = timeUpCount
                    };
                    resultlist.Add(newby);
                }
                return resultlist;
            }
            catch(Exception)
            {
                return null;

            }
        }

        /* Get specific User Account 
         * Input = Id_student 
           return account that has Id_student equal input*/
        public MemberAccount GetUserAccount(string id)
        {
            try
            {
                var user = _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id);
                if(user==null)
                {
                    return null;
                }
                MemberAccount newby = new MemberAccount()
                {
                    Id_account = user.Id_account,
                    Name = user.Name,
                    Point = user.Point
                };
                return newby;
            }
            catch (Exception)
            {
                return null;

            }
        }

        
        public UserOverview GetUserOverview (string id_account)
        {
            try
            {
                var user = _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id_account); // ID,Name,point
                if (user == null)
                {
                    return null;
                }
                int usingCount = _dbContext.Reservations.Count(x => x.Id_account == id_account && x.Status.ToLower() == "use"); //Using count
                                                                                                                                //TimeUp+Expire count 
                int timeUpCount = _dbContext.Reservations.Count(x => x.Id_account == id_account && x.Status.ToLower() == "timeup") + _dbContext.Reservations.Count(x => x.Id_account == id_account && x.Status.ToLower() == "expire");
                //all reservation
                var reserve = _dbContext.Reservations.Where(x => x.Id_account == id_account).OrderByDescending(x => x.DateModified);
                List<WebForm> tmp = new List<WebForm>();
                foreach (var run in reserve)
                {
                    WebForm newby = new WebForm()
                    {
                        Status = run.Status,
                        Id_booking = run.Id_reserve,
                        Id_user = run.Id_account,
                        Location = run.Location,
                        DateModified = run.DateModified
                    };
                    tmp.Add(newby);
                }

                UserOverview overview = new UserOverview()
                {
                    UserID = user.Id_account,
                    Name = user.Name,
                    Using = usingCount,
                    TimeUp = timeUpCount,
                    Point = user.Point,
                    BookingList = tmp
                };
                return overview;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /*user*/
        public List<Account> GetUserAccountdev()
        {
            var adminlist = from accountlist in _dbContext.Accounts
                            where accountlist.Role == "User"
                            select accountlist;
            return adminlist.ToList();
        }

        /*Admin*/
        public List<Account> GetUserAccountdev(string id)
        {
            var adminlist = from accountlist in _dbContext.Accounts
                            where accountlist.Role == "User" && accountlist.Id_account == id
                            select accountlist;
            return adminlist.ToList();
        }

        /*Admin*/
        public List<Account> GetAdminAccount()
        {
            var adminlist = from accountlist in _dbContext.Accounts
                            where accountlist.Role == "Administrator"
                            select accountlist;
            return adminlist.ToList();
        }


        public List<Admin> GetAdmins()
        {
            try
            {
                var adminlist = _dbContext.Accounts.Where(x => x.Role == "Administrator");
                List<Admin> result = new List<Admin>();
                foreach (var run in adminlist)
                {
                    Admin admin = new Admin()
                    {
                        Name = run.Name,
                        Email = run.Email
                    };
                    result.Add(admin);
                }
                return result;
            }
            catch (Exception)
            {
                return null;

            }
        }

        public bool IsAdmin (string accountID)
        {
            try
            {
                if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == accountID && x.Role == "Administrator") != null)
                    return true;
                else
                    return false;
            }catch (Exception)
            {
                Console.WriteLine("Error check admin");
                return false;
            }
        }
        /*Admin*/
        public List<Account> GetAdminAccount(string id)
        {
            var adminlist = from accountlist in _dbContext.Accounts
                            where accountlist.Role == "Administrator" && accountlist.Id_account == id
                            select accountlist;
            return adminlist.ToList();
        }

        /*DeleteAll*/

        public bool Delete()
        {
            try
            {
                var data = from list in _dbContext.Accounts select list;
                _dbContext.Accounts.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                Console.Write("Cannot delete all account database");
                return false;
            }
        }

        public bool Delete(string id)
        {
            try
            {
                if(_dbContext.Accounts.Where(x=>x.Id_account==id)==null)
                {
                    return false;
                }
                var data = from list in _dbContext.Accounts
                           where list.Id_account == id
                           select list;
                _dbContext.Accounts.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.Write("Cannot delete %s", id);
                return false;
            }
        }
    }
}

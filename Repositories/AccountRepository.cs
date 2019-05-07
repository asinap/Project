using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test2.Class;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Entities;

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
        public async Task<int> AddUserAccountAsync(Account account)
        {
            try
            {
                //GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(token);
                string id = "";
                string domainmail = "@kmitl.ac.th";
                //if domain account is not "@kmitl.ac.th"
                if (!account.Email.Contains(domainmail))
                {
                    // detect domain email
                    return 1;
                }
                
                id = account.Email.Replace(domainmail, "");
                //if id account is not numberic
                if (!int.TryParse(id, out int numberic))
                {
                    //before email is not studentID
                    return 2;
                }
                //if account already exist
                if (_dbContext.accounts.FirstOrDefault(x => x.Id_account == id) != null)
                {
                    //already have account
                    Console.WriteLine("already exist");
                    return 3;
                }

                //create account to store in database
                Account _account = new Account()
                {
                    Id_account = id,
                    Email = account.Email,
                    Name = account.Name,
                    Phone = "",
                    Point = 100,
                    Role="User"
                };

                //add account into database
                _dbContext.accounts.Add(_account);

                //save database
                _dbContext.SaveChanges();
                return 4;
               
            }
            catch (Exception)
            {
                //error
                Console.WriteLine("AddUserAccount Error");
                return 0;
            }
        }

        /*generate the token*/
        
        /* Add new account from register Administrator (optional adding random id for assign each administrator)    *
         * input = account                                                                                          *
                    attributes => Id_account, Name, Phone, Email, Role, Point                                       *
                    default Point = 100                                                                             */
        public int AddAdminAccount(Account account)
        {
            try
            {
                //if the account is already existed
                if (_dbContext.accounts.FirstOrDefault(x => x.Email.ToLower() == account.Email.ToLower()) != null)
                {
                    Console.WriteLine("already exist");
                    return 1;
                }
                //generate number id
                int rnd;
                do
                {
                    Random rand = new Random();
                    rnd = rand.Next(100000000, 999999999);
                } while (_dbContext.accounts.FirstOrDefault(x => x.Id_account == rnd.ToString()) != null);
                //save number id to account
                account.Id_account = rnd.ToString();
                //phone number and point are not used
                account.Phone = "";
                account.Point = 0;
                //set role to administrator
                account.Role = Role.Admin;
                //add account into database
                _dbContext.accounts.Add(account);
                //save database
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
        public bool AddPhoneNumber(string id_account, string phone)
        {
            try
            {
                //if there is user account in the database
                if (_dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account) != null)
                {
                    if(phone.Length!=10)
                    {
                        return false;
                    }

                    _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account).Phone = phone;
                    _dbContext.SaveChanges();
                    return true;
                }
                //if there is no user account in the database
                return false;
            }
            catch (Exception)
            {
                //error
                Console.WriteLine("AddPhoneNumber Error");
                return false;
            }
        }

        /* Get phone number in setting that belong to each user
        * input = Id_account 
        * return phone number to user */
        public string GetPhone(string id_account)
        {
            try
            {
                //if there is user account in the database
                if (_dbContext.accounts.FirstOrDefault(x=>x.Id_account==id_account)!=null)
                {
                    return _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account).Phone;
                }
                //if there is no user account in the database
                return null;
            }
            catch(Exception)
            {
                //error
                Console.WriteLine("Error Get phone");
                return null;
            }
        }

        /* Store notification token from Expo react in order to send notification to user*/
        public bool NotiToken(ExpoToken _token)
        {

            try
            {
                //if there is user account in the database
                if (_dbContext.accounts.FirstOrDefault(x => x.Id_account == _token.Id_account) != null)
                {
                    _dbContext.accounts.FirstOrDefault(x => x.Id_account == _token.Id_account).ExpoToken = _token.Expotoken;
                }
                //if there is no user account in the database
                else
                {
                    return false;
                }
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                //error
                Console.WriteLine("Cannot add notitoken");
                return false;
            }
        }

        /*TEST Get all notification token from Expo react in order to send notification to user*/
        public List<ExpoToken> GetNotiToken()
        {

            try
            {
                var list = _dbContext.accounts.ToList();
                //get all user account
                List<ExpoToken> expolist = new List<ExpoToken>();
                //loop for forming new form in order to return to dev 
                foreach (var run in list)
                {
                    ExpoToken expoToken = new ExpoToken()
                    {
                        Id_account=run.Id_account,
                        Expotoken=run.ExpoToken
                    };
                    expolist.Add(expoToken);
                }
                //if there is no any account
                if (expolist.Count() == 0)
                {
                    return null;
                }
                //if there is account
                else
                {
                    return expolist;
                }
            }
            catch (Exception)
            {
                //error
                return null;
            }
        }

        /*For checking token APIs, Finding id account and return some information*/
        public MemberAccount User_Information(string token)
        {
            //find account
            var user = _dbContext.accounts.SingleOrDefault(x => x.Token.Contains(token));
            //if there is no user
            if(user==null)
            {
                return null;
            }
            //if there is user, create user form in order to return to user
            MemberAccount _user = new MemberAccount()
            {
                Id_account =user.Id_account,
                Name=user.Name,
                Point=user.Point
            };
            return _user;
        }
        /* Get All User Account 
         * return all account to string */
        public List<Member> GetUserAccount()
        {
            try
            {
                //find account that role is user
                var userlist = from accountlist in _dbContext.accounts
                               where accountlist.Role == Role.User
                               select accountlist;
                List<Member> resultlist = new List<Member>();

                //create form in order to return to administrator to show in member page
                foreach (var run in userlist)
                {
                    int usingCount = _dbContext.reservations.Count(x => x.Id_account == run.Id_account && x.Status.ToLower() == "use");
                    int bookedCount = _dbContext.reservations.Count(x => x.Id_account == run.Id_account);
                    int timeUpCount = _dbContext.reservations.Count(x => x.Id_account == run.Id_account && x.Status.ToLower() == "timeup") + _dbContext.reservations.Count(x => x.Id_account == run.Id_account && x.Status.ToLower() == "expire");
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
                //error
                return null;

            }
        }

        /* Get specific User Account 
         * Input = Id_student 
           return account that has Id_student equal input*/
        public MemberAccount GetUserAccount(string id_account)
        {
            try
            {
                //find user account
                var user = _dbContext.accounts.SingleOrDefault(x => x.Id_account==id_account);
                //if there is no account
                if(user==null)
                {
                    return null;
                }
                //if there is account in database
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
                //error
                return null;

            }
        }

        /* Get information ABOUT USER include some detail like their reservation and how many time that they reserved*/
        public UserOverview GetUserOverview (string id_account)
        {
            try
            {
                //find account
                var user = _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account); // ID,Name,point
                //if there is no account
                if (user == null)
                {
                    return null;
                }
                //if there is an account

                int usingCount = _dbContext.reservations.Count(x => x.Id_account == id_account && x.Status.ToLower() == "use"); //Using count
                                                                                                                                //TimeUp+Expire count 
                int timeUpCount = _dbContext.reservations.Count(x => x.Id_account == id_account && x.Status.ToLower() == "timeup") + _dbContext.reservations.Count(x => x.Id_account == id_account && x.Status.ToLower() == "expire");
                //all reservation
                var reserve = _dbContext.reservations.Where(x => x.Id_account == id_account).OrderByDescending(x => x.DateModified);

                //create form for returning to administrator
                List<WebForm> tmp = new List<WebForm>();
                foreach (var run in reserve)
                {
                    string mac_address = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == run.Id_vacancy).Mac_address;
                    string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == mac_address).Location;
                    WebForm newby = new WebForm()
                    {
                        Status = run.Status,
                        Id_booking = run.Id_reserve,
                        Id_user = run.Id_account,
                        Location = location,
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
                //error
                return null;
            }

        }

        /*TEST get all user*/
        public List<Account> GetUserAccountdev()
        {
            var adminlist = from accountlist in _dbContext.accounts
                            where accountlist.Role == Role.User
                            select accountlist;
            return adminlist.ToList();
        }

        /*TEST get specific user*/
        public List<Account> GetUserAccountdev(string id)
        {
            var adminlist = from accountlist in _dbContext.accounts
                            where accountlist.Role == Role.User && accountlist.Id_account == id
                            select accountlist;
            return adminlist.ToList();
        }

        /*TEST get all admin*/
        public List<Account> GetAdminAccount()
        {
            var adminlist = from accountlist in _dbContext.accounts
                            where accountlist.Role == Role.Admin
                            select accountlist;
            return adminlist.ToList();
        }

        /*For Admin page return list of admin*/
        public List<Admin> GetAdmins()
        {
            try
            {
                //find all admin account
                var adminlist = _dbContext.accounts.Where(x => x.Role == Role.Admin);

                //create form for returning to administrator
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
                //error
                return null;

            }
        }

        ///*For checking admin authentication*/
        //public async Task<bool> IsAdmin (string token)
        //{
        //    try
        //    {
        //        GoogleJsonWebSignature.Payload validPayload = await GoogleJsonWebSignature.ValidateAsync(token);
        //        if (_dbContext.accounts.FirstOrDefault(x => x.Email== validPayload.Email && x.Role.ToLower() == "administrator") != null)
        //            return true;
        //        else
        //            return false;
        //    }catch (Exception)
        //    {
        //        Console.WriteLine("Error check admin");
        //        return false;
        //    }
        //}

        /*TEST get specific admin*/
        public List<Account> GetAdminAccount(string id)
        {
            var adminlist = from accountlist in _dbContext.accounts
                            where accountlist.Role == Role.Admin && accountlist.Id_account == id
                            select accountlist;
            return adminlist.ToList();
        }

    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;

namespace test2.Repositories
{
    public class NotificationRepository
    {
        LockerDbContext _dbContext;

        public NotificationRepository(LockerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /* Add notification => Create new notification                                                                               *
         * input = _message                                                                                                 *
         *      Attrinbutes = int Id_message, DateTime Date, DateTime Time, bool IsShow, int id_content, string Id_account  */
        public bool AddNotification(Notification _noti)
        {

            try
            {
                if(CheckId_content(_noti.Id_content))
                {
                    return false;
                }
                if(CheckId_account(_noti.Id_account))
                {
                    return false;
                }
                /*search reservation for create notification and assign no_vacancy and mac_address*/
                _dbContext.Notifications.Add(_noti);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception error");
                return false;
            }
        }

        /* CheckId_content that has id_content in Contents database     *
         *  input = int id_content                                      *
         * return true if there aren't id_content in database           */
        public bool CheckId_content (int id_content)
        {
            var list = from contentlist in _dbContext.Contents
                       where contentlist.Id_content == id_content && contentlist.IsActive == true
                       select contentlist;
            if (list==null)
            {
                return true;
            }
            else
            {
                return false;
            }
            //var list=_dbContext.Contents.FirstOrDefault(x => x.Id_content == id_content) == null;
        }

        /* CheckId_account that has id_account in Accounts database     *
         *  input = int id_account                                      *
         * return true if there aren't id_account in database           */
        public bool CheckId_account (string id_account)
        {
            return _dbContext.Accounts.FirstOrDefault(x => x.Id_account == id_account) == null;
        }

        /* DeleteMessage            *
         *  input = int id          *
         *      set IsShow = false  */
        public bool DeleteNotification(int id)
        {
            try
            {
                _dbContext.Notifications.FirstOrDefault(x => x.Id_notification == id).IsShow = false;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception error");
                return false;
            }
        }

        /* Get all message                      *
         * return all message detail to string  */
        public List<Notification> GetNotification()
        {
            return _dbContext.Notifications.ToList();
        }

        /* Get specific message                             *
         *  input = int id_message                          *
         * return message that has Id_message = id_message  */
        public List<Notification> GetNotification(int id_noti)
        {
            var usernotilist = from notilist in _dbContext.Notifications
                                 where notilist.Id_notification == id_noti
                                 select notilist;
            return usernotilist.ToList();

        }
        
        /* Get active message for one user                  *
         *  input = string id_account                       *
         * return idActivemessage to list                   */
        public List<Notification> GetActiveNoti (string id_account)
        {
            var userActiveNoti = from notilist in _dbContext.Notifications
                                  where notilist.Id_account == id_account && notilist.IsShow == true
                                  select notilist;
            return userActiveNoti.ToList();
        }

        /*Delete*/
        public bool Delete()
        {
            try
            {
                var data = from list in _dbContext.Notifications select list;
                _dbContext.Notifications.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                Console.Write("Cannot delete all Notifications database");
                return false;
            }
        }

        public bool Delete(int id_noti)
        {
            try
            {
                if (_dbContext.Notifications.Where(x => x.Id_notification == id_noti) == null)
                {
                    return false;
                }
                var data = from list in _dbContext.Notifications
                           where list.Id_notification == id_noti
                           select list;
                _dbContext.Notifications.RemoveRange(data);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.Write("Cannot delete %s", id_noti);
                return false;
            }
        }

    }
}

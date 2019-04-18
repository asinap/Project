using System;
using System.Collections.Generic;
using System.Linq;
using test2.Class;
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
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                _noti.CreateTime = dateTime;
                _noti.Read = false;
                _dbContext.notifications.Add(_noti);
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
            var list = from contentlist in _dbContext.contents
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
            return _dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account) == null;
        }

        /* DeleteMessage            *
         *  input = int id          *
         *      set IsShow = false  */
        public bool DeleteNotification(int id)
        {
            try
            {
                _dbContext.notifications.FirstOrDefault(x => x.Id_notification == id).IsShow = false;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Exception error");
                return false;
            }
        }

        public bool SetRead(int id)
        {
            try
            {
                _dbContext.notifications.FirstOrDefault(x => x.Id_notification == id).Read = true;
                _dbContext.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                Console.WriteLine("Error");
                return false;
            }
        }

        /* Get all message                      *
         * return all message detail to string  */
        public List<Notification> GetNotification()
        {
            return _dbContext.notifications.ToList();
        }

        /* Get specific message                             *
         *  input = int id_message                          *
         * return message that has Id_message = id_message  */
        public List<Notification> GetNotification(int id_noti)
        {
            var usernotilist = from notilist in _dbContext.notifications
                                 where notilist.Id_notification == id_noti
                                 select notilist;
            return usernotilist.ToList();

        }

        public List<NotificationForm> GetNotificationForm (string id_account)
        {
            try
            {
                if (_dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account) == null)
                {
                    return null;
                }
                var list = _dbContext.notifications.Where(x => x.Id_account == id_account && x.IsShow == true).OrderByDescending(x => x.Id_notification).ThenBy(x => x.Read);
                List<NotificationForm> result = new List<NotificationForm>();
                foreach (var run in list)
                {
                    var content = _dbContext.contents.FirstOrDefault(x => x.Id_content == run.Id_content);
                    var reservelist = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve);
                    var vacant = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == reservelist.Id_vacancy);
                    string _content = content.PlainText;
                    _content = _content.Replace("%p", reservelist.Location);
                    _content = _content.Replace("%v", vacant.No_vacancy);
                    NotificationForm form = new NotificationForm()
                    {
                        Id_account = run.Id_account,
                        Id_noti = run.Id_notification,
                        CreateTime = run.CreateTime,
                        Content = _content,
                        Read = run.Read
                    };
                    result.Add(form);

                }
                return result.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public NotificationForm GetNotificationDetail (int id_noti)
        {
            try
            {
                if(_dbContext.notifications.FirstOrDefault(x=>x.Id_notification==id_noti)==null)
                {
                    return null;
                }
                var list = _dbContext.notifications.FirstOrDefault(x => x.Id_notification == id_noti);
                var content = _dbContext.contents.FirstOrDefault(x => x.Id_content == list.Id_content);
                var reservelist = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == list.Id_reserve);
                var vacant = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == reservelist.Id_vacancy);
                string _content = content.PlainText;
                _content = _content.Replace("%p", reservelist.Location);
                _content = _content.Replace("%v", vacant.No_vacancy);
                NotificationForm form = new NotificationForm()
                {
                    Id_account = list.Id_account,
                    CreateTime = list.CreateTime,
                    Content = _content,
                    Read = list.Read
                };
                return form;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public List<NotificationForm> GetNotificationForm (string id_account, int id_noti)
        //{
        //    if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == id_account) == null)
        //    {
        //        return null;
        //    }
            

        //}
        
        /* Get active message for one user                  *
         *  input = string id_account                       *
         * return idActivemessage to list                   */
        public List<Notification> GetActiveNoti (string id_account)
        {
            var userActiveNoti = from notilist in _dbContext.notifications
                                  where notilist.Id_account == id_account && notilist.IsShow == true
                                  select notilist;
            return userActiveNoti.ToList();
        }

        /*Delete*/
        public bool Delete()
        {
            try
            {
                var data = from list in _dbContext.notifications select list;
                _dbContext.notifications.RemoveRange(data);
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
                if (_dbContext.notifications.Where(x => x.Id_notification == id_noti) == null)
                {
                    return false;
                }
                var data = from list in _dbContext.notifications
                           where list.Id_notification == id_noti
                           select list;
                _dbContext.notifications.RemoveRange(data);
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

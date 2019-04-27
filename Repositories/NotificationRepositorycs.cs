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

        /* DeleteMessage                            *
         * input = int id                           *
         *      set IsShow = false                  *
         *     from user through mobile application */
        public bool DeleteNotification(int id)
        {
            try
            {
                //if there is a notification ID
                if (_dbContext.notifications.FirstOrDefault(x => x.Id_notification == id) != null)
                {
                    _dbContext.notifications.FirstOrDefault(x => x.Id_notification == id).IsShow = false;
                    _dbContext.SaveChanges();
                    return true;
                }
                //if there is no a notification ID
                return false;
            }
            catch (Exception)
            {
                //error
                Console.WriteLine("Exception error");
                return false;
            }
        }

        /* Set read Message                          *
        *  input = int id                            *
        *      set Read = true                       *
        *       from user through mobile application */
        public bool SetRead(int id)
        {
            try
            {
                //if there is a notification ID
                if (_dbContext.notifications.FirstOrDefault(x => x.Id_notification == id) != null)
                {
                    _dbContext.notifications.FirstOrDefault(x => x.Id_notification == id).Read = true;
                    _dbContext.SaveChanges();
                    return true;
                }
                //if there is no a notification ID
                return false;
            }
            catch(Exception)
            {
                //error
                Console.WriteLine("Error");
                return false;
            }
        }

        /* TEST Get all message                      *
         * return all message detail to string  */
        public List<Notification> GetNotification()
        {
            return _dbContext.notifications.ToList();
        }

        /* TEST Get specific message                             *
         *  input = int id_message                          *
         * return message that has Id_message = id_message  */
        public List<Notification> GetNotification(int id_noti)
        {
            var usernotilist = from notilist in _dbContext.notifications
                                 where notilist.Id_notification == id_noti
                                 select notilist;
            return usernotilist.ToList();

        }

        /*Get notification from each user through mobile application */
        public List<NotificationForm> GetNotificationForm (string id_account)
        {
            try
            {
                //check if there is no user account
                if (_dbContext.accounts.FirstOrDefault(x => x.Id_account == id_account) == null)
                {
                    return null;
                }
                //if there is user account
                //find notification that there is a same id account and show is true then order by descending with id_notification and read that order by false first and following by true 
                var list = _dbContext.notifications.Where(x => x.Id_account == id_account && x.IsShow == true).OrderByDescending(x => x.Id_notification).ThenBy(x => x.Read);
                //create form in order to return to user
                List<NotificationForm> result = new List<NotificationForm>();
                foreach (var run in list)
                {
                    var content = _dbContext.contents.FirstOrDefault(x => x.Id_content == run.Id_content);
                    var reservelist = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve);
                    var vacant = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == reservelist.Id_vacancy);
                    string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == vacant.Mac_address).Location;
                    string _content = content.PlainText;
                    _content = _content.Replace("%p", location);
                    _content = _content.Replace("%v", vacant.No_vacancy);
                    //create notification form to user
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
                //error
                return null;
            }
        }

        /*Get specific notification from user through mobile application*/
        public NotificationForm GetNotificationDetail(int id_noti)
        {
            try
            {
                //check if there is no user account
                if (_dbContext.notifications.FirstOrDefault(x => x.Id_notification == id_noti) == null)
                {
                    return null;
                }
                //if there is user account
                //find specific notification
                var list = _dbContext.notifications.FirstOrDefault(x => x.Id_notification == id_noti);
                //find notification content
                var content = _dbContext.contents.FirstOrDefault(x => x.Id_content == list.Id_content);
                //find reservation
                var reservelist = _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == list.Id_reserve);
                //find vacancy
                var vacant = _dbContext.vacancies.FirstOrDefault(x => x.Id_vacancy == reservelist.Id_vacancy);
                //find location
                string location = _dbContext.lockerMetadatas.FirstOrDefault(x => x.Mac_address == vacant.Mac_address).Location;

                //create form in order to return to user
                string _content = content.PlainText;
                _content = _content.Replace("%p", location);
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
                //error
                return null;
            }
        }

  

    }
}

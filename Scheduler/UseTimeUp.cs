using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Helpers;

namespace test2.Scheduler
{
    /* Set isActive be false in Use Reservation ,then Create notification to User */
    public class UseTimeUp : ScheduledProcessor
    {
        LockerDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public UseTimeUp(IServiceScopeFactory serviceScopeFactory,IOptions<AppSettings> appsetting) : base(serviceScopeFactory, appsetting)
        {
            _appSettings = appsetting.Value;
        }

        protected override string Schedule => "*/1 * * * *"; // every minute

        public override Task ProcessInScope(IServiceProvider serviceProvider, DbContextOptions<test2.DatabaseContext.LockerDbContext> dbOption)
        {
            //var processor = serviceProvider.GetRequiredService();
            try
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                _dbContext = new LockerDbContext(dbOption);
                var reservelist = from list in _dbContext.reservations
                                  where list.EndDay < dateTime && list.IsActive == true && list.Status.ToLower() == "use"
                                  select list;
                
                if (reservelist == null)
                {
                    Log.Information("Use Time up every 1 mins {0} No data to set.", dateTime);
                }
                else
                {
                    foreach (var run in reservelist)
                    {
                        //_dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve).Status = "TimeUp";
                        _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve).IsActive = false;
                        _dbContext.SaveChanges();
                        Notification notification = new Notification()
                        {
                            Id_account = run.Id_account,
                            CreateTime = dateTime,
                            Id_content = _appSettings.EndContent,
                            Id_reserve = run.Id_reserve,
                            IsShow = true,
                            Read = true

                        };
                        _dbContext.notifications.Add(notification);
                        _dbContext.SaveChanges();
                        SendPushNotification(_dbContext.accounts.FirstOrDefault(x => x.Id_account == run.Id_account).ExpoToken);
                        if (_dbContext.accounts.FirstOrDefault(x => x.Id_account == run.Id_account).Point <= 0)
                        {
                            _dbContext.accounts.FirstOrDefault(x => x.Id_account == run.Id_account).Point = 0;
                            _dbContext.SaveChanges();
                            Notification p_notification = new Notification()
                            {
                                Id_account = run.Id_account,
                                CreateTime = dateTime,
                                Id_content = _appSettings.PenaltyContent,
                                IsShow = true,
                                Read = true
                            };
                            _dbContext.notifications.Add(p_notification);
                            _dbContext.accounts.FirstOrDefault(x => x.Id_account == run.Id_account).Point = 0;
                            _dbContext.SaveChanges();

                            SendPushNotificationP(_dbContext.accounts.FirstOrDefault(x => x.Id_account == run.Id_account).ExpoToken);

                            Log.Information("Create noti point {0} {1}.", dateTime, run.Id_account);
                            Log.Information("Set point to zero {0} {1}.", dateTime, run.Id_account);
                        }
                        Log.Information("Use Time up every 1 mins {0} {1} Data to be set and create notification.", dateTime, run.Id_reserve);
                    }
                }
            }
            catch
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                Log.Information("Use Time up every 1 mins {0} Error.", dateTime);
            }
            //List<Reservation> reservations = _dbContext.Reservations.ToList();
            //Console.WriteLine("Processing starts here");
            //var run = reservations;
            //foreach (var list in run)

            return Task.CompletedTask;
        }
        public static dynamic SendPushNotification(string ExpoToken)
        {
            dynamic body = new
            {
                to = ExpoToken,
                title = "Notification",
                body = "End of service.",
                sound = "default",
                data = new { some = "daaaata" }
            };
            string response = null;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("accept", "application/json");
                client.Headers.Add("accept-encoding", "gzip, deflate");
                client.Headers.Add("Content-Type", "application/json");
                response = client.UploadString("https://exp.host/--/api/v2/push/send", JsonExtensions.SerializeToJson(body));
            }
            var json = JsonExtensions.DeserializeFromJson<dynamic>(response);
            return json;
        }
        public static dynamic SendPushNotificationP(string ExpoToken)
        {
            dynamic body = new
            {
                to = ExpoToken,
                title = "Notification",
                body = "You point is 0.",
                sound = "default",
                data = new { some = "daaaata" }
            };
            string response = null;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("accept", "application/json");
                client.Headers.Add("accept-encoding", "gzip, deflate");
                client.Headers.Add("Content-Type", "application/json");
                response = client.UploadString("https://exp.host/--/api/v2/push/send", JsonExtensions.SerializeToJson(body));
            }
            var json = JsonExtensions.DeserializeFromJson<dynamic>(response);
            return json;
        }
    }
}

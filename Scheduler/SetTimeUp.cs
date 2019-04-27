
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using System.Linq;
using test2.Helpers;
using Microsoft.Extensions.Options;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using test2.Entities;

namespace test2.Scheduler
{
    /* Set Time up Status and isActive be false in Unuse Reservation ,then Create notification to User */
    public class SetTimeUp : ScheduledProcessor
    {
        LockerDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public SetTimeUp(IServiceScopeFactory serviceScopeFactory, IOptions<AppSettings> appsetting) : base(serviceScopeFactory,appsetting)
        {
            _appSettings = appsetting.Value;
        }

        protected override string Schedule => "*/1 * * * *"; //every minutes

        public override Task ProcessInScope(IServiceProvider serviceProvider, DbContextOptions<test2.DatabaseContext.LockerDbContext> dbOption)
        {
            //var processor = serviceProvider.GetRequiredService();
            try
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);

                _dbContext = new LockerDbContext(dbOption);
                
                var reservelist = from list in _dbContext.reservations
                                  where list.EndDay < dateTime && list.IsActive == true && list.Status == Status.Unuse
                                  select list;

               
                if (reservelist == null)
                {
                    Log.Information("Set time up every 1 mins {0} No data to set.", dateTime);
                }
                else
                {
                    foreach (var run in reservelist)
                    {
                        _dbContext.reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve).Status = Status.Timeup;
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
                        Log.Information("Set time up every 1 mins {0} {1} Data to be set and create notification.", dateTime, run.Id_reserve);
                    }
                }
            }
            catch
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                Log.Information("Set time up every 1 mins {0} Error.", dateTime);
            }
            //List<Reservation> reservations = _dbContext.Reservations.ToList();
            //Console.WriteLine("Processing starts here");
            //var run = reservations;
            //foreach (var list in run)
            //{
            //    Log.Information("every 1 mins {0}",DateTime.Now);
            //}
            
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

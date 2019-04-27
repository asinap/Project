using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using test2.DatabaseContext;
using test2.DatabaseContext.Models;
using test2.Helpers;

namespace test2.Scheduler
{
    public class CheckFiveMins : ScheduledProcessor
    {
        LockerDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public CheckFiveMins(IServiceScopeFactory serviceScopeFactory, IOptions<AppSettings> appsetting) : base(serviceScopeFactory, appsetting)
        {
            _appSettings = appsetting.Value;
        }

        protected override string Schedule => "*/1 * * * *"; // every minute

        public override Task ProcessInScope(IServiceProvider serviceProvider, DbContextOptions<LockerDbContext> dbOption)
        {
            try
            {
                _dbContext = new LockerDbContext(dbOption);
                
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);

                //get active reservation
                var reservelist = from row in _dbContext.reservations
                                  where row.IsActive == true
                                  select row;

                if (reservelist == null)
                {
                    Log.Information("Check five mins every minute {0} No data to set.", dateTime);
                }
                else
                {
                    foreach (var run in reservelist)
                    {
                        //if there is no notification in each reservation 
                        if (_dbContext.notifications.FirstOrDefault(x => x.Id_reserve == run.Id_reserve && x.Id_content == _appSettings.FiveContent) == null)
                        {
                            //check different time = 5 minutes
                            TimeSpan diff = (run.EndDay - dateTime).Duration();
                            if (diff.TotalMinutes < 6 && diff.TotalMinutes > 4)
                            {
                                Notification notification = new Notification()
                                {
                                    Id_account = run.Id_account,
                                    CreateTime = dateTime,
                                    Id_content = _appSettings.FiveContent,
                                    Id_reserve = run.Id_reserve,
                                    IsShow = true,
                                    Read = false

                                };
                                _dbContext.notifications.Add(notification);
                                _dbContext.SaveChanges();
                                SendPushNotification(_dbContext.accounts.FirstOrDefault(x=>x.Id_account==run.Id_account).ExpoToken);
                                Log.Information("Check five mins every 1 mins {0} {1} Data to be set and create notification.", dateTime, run.Id_reserve);
                            }
                        }
                    }
                }
            }
            catch
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                Log.Information("Check five mins every minute {0} Error.", dateTime);
            }
            return Task.CompletedTask;
        }
        public static dynamic SendPushNotification(string ExpoToken)
        {
            dynamic body = new
            {
                to = ExpoToken,
                title = "Notification",
                body = "5 minutes before end of service.",
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

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
using test2.Entities;
using test2.Helpers;

namespace test2.Scheduler
{
    /* Count Time up status, then change the status to Expire, Finally minus penalty from user point*/
    public class SetExpire : ScheduledProcessor
    {
        LockerDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public SetExpire(IServiceScopeFactory serviceScopeFactory, IOptions<AppSettings> appsetting) : base(serviceScopeFactory, appsetting)
        {
            _appSettings = appsetting.Value;
        }

        protected override string Schedule => "*/5 * * * *"; //every 5 minute

        public override Task ProcessInScope(IServiceProvider serviceProvider, DbContextOptions<LockerDbContext> dbOption)
        {
            try
            {
                _dbContext = new LockerDbContext(dbOption);
                var reservelist = (from row in _dbContext.reservations
                                   where row.Status == Status.Timeup
                                   select row.Id_account).Distinct();

                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);

                if (reservelist == null)
                {
                    Log.Information("Set Expire every 5 min {0} No data to set.", dateTime);
                }
                else
                {
                    foreach (var run in reservelist)
                    {
                        int timeupcount = _dbContext.reservations.Count(x => x.Id_account == run.ToString()&&x.Status==Status.Timeup);
                        int penalty = timeupcount / 4;
                        if (penalty==0)
                        {
                            Log.Information("Set Expire every 5 min {0} {1} No data to set.", dateTime, run.ToString());
                        }
                        else
                        {
                            int i;
                            for (i=0;i<4*penalty;i++)
                            {
                                _dbContext.reservations.FirstOrDefault(x => x.Id_account == run.ToString() && x.Status == Status.Timeup).Status = Status.Expire;
                                _dbContext.SaveChanges();
                                Log.Information("Set Expire every 1 hour {0} {1} Data to be set", dateTime,run.ToString());
                            }
                            _dbContext.accounts.FirstOrDefault(x => x.Id_account == run.ToString()).Point -= _appSettings.PenaltyPoint * penalty;
                            _dbContext.SaveChanges();

                            // point less than 0
                            if (_dbContext.accounts.FirstOrDefault(x => x.Id_account == run.ToString()).Point <= 0)
                            {
                                Notification notification = new Notification()
                                {
                                    Id_account = run.ToString(),
                                    CreateTime = dateTime,
                                    Id_content = _appSettings.PenaltyContent,
                                    IsShow = true,
                                    Read = false
                                };
                                _dbContext.notifications.Add(notification);
                                _dbContext.accounts.FirstOrDefault(x => x.Id_account == run.ToString()).Point = 0;
                                _dbContext.SaveChanges();
                                SendPushNotification(_dbContext.accounts.FirstOrDefault(x => x.Id_account == run.ToString()).ExpoToken);
                                Log.Information("Create noti point {0} {1}.", dateTime, run.ToString());
                            }
                            Log.Information("Set Expire every 1 hour {0} {1} Data to be set", dateTime, run.ToString());
                        }
        
                    }
                }
            }
            catch
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, zone);
                Log.Information("Set Expire every 1 hour {0} Error.", dateTime);
            }

            return Task.CompletedTask;
        }
        public static dynamic SendPushNotification(string ExpoToken)
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

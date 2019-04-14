using System;
using System.Collections.Generic;
using System.Linq;
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
                var reservelist = (from row in _dbContext.Reservations
                                   where row.Status.ToLower() == "timeup"
                                   select row.Id_account).Distinct();
                
                if (reservelist == null)
                {
                    Log.Information("Set Expire every 5 min {0} No data to set.", DateTime.Now);
                }
                else
                {
                    foreach (var run in reservelist)
                    {
                        int timeupcount = _dbContext.Reservations.Count(x => x.Id_account == run.ToString()&&x.Status.ToLower()=="timeup");
                        int penalty = timeupcount / 4;
                        if (penalty==0)
                        {
                            Log.Information("Set Expire every 5 min {0} {1} No data to set.", DateTime.Now,run.ToString());
                        }
                        else
                        {
                            int i;
                            for (i=0;i<4*penalty;i++)
                            {
                                _dbContext.Reservations.FirstOrDefault(x => x.Id_account == run.ToString() && x.Status.ToLower() == "timeup").Status = "Expire";
                                _dbContext.SaveChanges();
                                //Log.Information("Set Expire every 1 hour {0} {1} Data to be set", DateTime.Now,run.ToString());
                            }
                            _dbContext.Accounts.FirstOrDefault(x => x.Id_account == run.ToString()).Point -= _appSettings.PenaltyPoint * penalty;
                            _dbContext.SaveChanges();
                            if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == run.ToString()).Point <= 0)
                            {
                                Notification notification = new Notification()
                                {
                                    Id_account = run.ToString(),
                                    CreateTime = DateTime.Now,
                                    Id_content = _appSettings.PenaltyContent,
                                    Id_reserve = -1,
                                    IsShow = true,
                                    Read = true
                                };
                                _dbContext.Notifications.Add(notification);
                                _dbContext.Accounts.FirstOrDefault(x => x.Id_account == run.ToString()).Point = 0;
                                _dbContext.SaveChanges();
                                Log.Information("Create noti point {0} {1}.", DateTime.Now, run.ToString());
                            }
                            Log.Information("Set Expire every 1 hour {0} {1} Data to be set", DateTime.Now, run.ToString());
                        }
                    //    _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve).IsActive = false;
                    //    _dbContext.SaveChanges();
                        //Log.Information("Set Expire every 1 hour {0} Data to be set", DateTime.Now);
                    }
                }
            }
            catch
            {
                Log.Information("Set Expire every 1 hour {0} Error.", DateTime.Now);
            }

            return Task.CompletedTask;
        }
    }
}

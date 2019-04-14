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
    public class CheckTenMins : ScheduledProcessor
    {
        LockerDbContext _dbContext;
        private readonly AppSettings _appSettings;
        public CheckTenMins(IServiceScopeFactory serviceScopeFactory, IOptions<AppSettings> appsetting) : base(serviceScopeFactory, appsetting)
        {
            _appSettings = appsetting.Value;
        }

        protected override string Schedule => "*/1 * * * *"; // every minute

        public override Task ProcessInScope(IServiceProvider serviceProvider, DbContextOptions<LockerDbContext> dbOption)
        {
            try
            {
                _dbContext = new LockerDbContext(dbOption);
                var reservelist = from row in _dbContext.Reservations
                                  where row.IsActive == true
                                  select row;
                if(reservelist==null)
                {
                    Log.Information("Check ten mins every minute {0} No data to set.", DateTime.Now);
                }
                else
                {
                    foreach (var run in reservelist)
                    {
                        if (_dbContext.Notifications.FirstOrDefault(x => x.Id_reserve == run.Id_reserve && x.Id_content==_appSettings.TenContent) == null)
                        {
                            DateTime dateTime = DateTime.Now;
                            TimeSpan diff = (run.EndDay - dateTime).Duration();
                            if(diff.TotalMinutes<11&&diff.TotalMinutes>9)
                            {
                                Notification notification = new Notification()
                                {
                                    Id_account = run.Id_account,
                                    CreateTime = DateTime.Now,
                                    Id_content = _appSettings.TenContent,
                                    Id_reserve = run.Id_reserve,
                                    IsShow = true,
                                    Read = true

                                };
                                _dbContext.Notifications.Add(notification);
                                _dbContext.SaveChanges();
                                Log.Information("Check ten mins every 1 mins {0} {1} Data to be set and create notification.", DateTime.Now, run.Id_reserve);
                            }
                        }
                    }
                }
            }
            catch
            {
                Log.Information("Check ten mins every minute {0} Error.", DateTime.Now);
            }
            return Task.CompletedTask;
        }
    }
}

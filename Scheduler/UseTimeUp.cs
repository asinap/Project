using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
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
                _dbContext = new LockerDbContext(dbOption);
                var reservelist = from list in _dbContext.Reservations
                                  where list.EndDay < DateTime.Now && list.IsActive == true && list.Status.ToLower() == "use"
                                  select list;
                if (reservelist == null)
                {
                    Log.Information("Use Time up every 1 mins {0} No data to set.", DateTime.Now);
                }
                else
                {
                    foreach (var run in reservelist)
                    {
                        //_dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve).Status = "TimeUp";
                        _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve).IsActive = false;
                        _dbContext.SaveChanges();
                        Notification notification = new Notification()
                        {
                            Id_account = run.Id_account,
                            CreateTime = DateTime.Now,
                            Id_content = _appSettings.EndContent,
                            Id_reserve = run.Id_reserve,
                            IsShow = true,
                            Read = true

                        };
                        _dbContext.Notifications.Add(notification);
                        _dbContext.SaveChanges();
                        if (_dbContext.Accounts.FirstOrDefault(x => x.Id_account == run.Id_account).Point <= 0)
                        {
                            _dbContext.Accounts.FirstOrDefault(x => x.Id_account == run.ToString()).Point = 0;
                            _dbContext.SaveChanges();
                            Log.Information("Set point to zero {0} {1}.", DateTime.Now, run.Id_account);
                        }
                        Log.Information("Use Time up every 1 mins {0} {1} Data to be set and create notification.", DateTime.Now, run.Id_reserve);
                    }
                }
            }
            catch
            {
                Log.Information("Use Time up every 1 mins {0} Error.", DateTime.Now);
            }
            //List<Reservation> reservations = _dbContext.Reservations.ToList();
            //Console.WriteLine("Processing starts here");
            //var run = reservations;
            //foreach (var list in run)

            return Task.CompletedTask;
        }
    }
}


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
                _dbContext = new LockerDbContext(dbOption);
                
                var reservelist = from list in _dbContext.Reservations
                                  where list.EndDay < DateTime.Now && list.IsActive == true && list.Status.ToLower() == "unuse"
                                  select list;
                if (reservelist == null)
                {
                    Log.Information("Set time up every 1 mins {0} No data to set.", DateTime.Now);
                }
                else
                {
                    foreach (var run in reservelist)
                    {
                        _dbContext.Reservations.FirstOrDefault(x => x.Id_reserve == run.Id_reserve).Status = "TimeUp";
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
                        Log.Information("Set time up every 1 mins {0} {1} Data to be set and create notification.", DateTime.Now, run.Id_reserve);
                    }
                }
            }
            catch
            {
                Log.Information("Set time up every 1 mins {0} Error.", DateTime.Now);
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
    }
}

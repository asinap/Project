using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using test2.DatabaseContext;
using test2.Helpers;

namespace test2.BackgroundService
{
    public abstract class ScopedProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly AppSettings _appSettings;
  
        public ScopedProcessor(IServiceScopeFactory serviceScopeFactory,IOptions<AppSettings> appsetting) : base()
        {
            _serviceScopeFactory = serviceScopeFactory;
            _appSettings = appsetting.Value;
 
        }

        protected override async Task Process()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {

                var dbOption = scope.ServiceProvider.GetRequiredService<DbContextOptions<test2.DatabaseContext.LockerDbContext>>();
                await ProcessInScope(scope.ServiceProvider,dbOption);
                
            }
        }

        public abstract Task ProcessInScope(IServiceProvider serviceProvider, DbContextOptions<test2.DatabaseContext.LockerDbContext> dbOption);
    }
}

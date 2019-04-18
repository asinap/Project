using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test2.DatabaseContext.Models;

namespace test2.DatabaseContext
{
    public class LockerDbContext : DbContext
    {
        public LockerDbContext(DbContextOptions<LockerDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Reservation> reservations { get; set; }
        public DbSet<Vacancy> vacancies { get; set; }
        public DbSet<LockerMetadata> lockerMetadatas { get; set; }
        public DbSet<Content> contents { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public DbSet<Account> accounts { get; set; }
        
            

    }
}

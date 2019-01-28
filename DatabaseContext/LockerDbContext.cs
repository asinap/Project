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
        
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<LockerMetadata> LockerMetadatas { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<MessageDetail> MessageDetails { get; set; }
        public DbSet<Account> Accounts { get; set; }

    }
}

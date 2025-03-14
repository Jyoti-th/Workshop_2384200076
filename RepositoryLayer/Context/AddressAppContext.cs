using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Context
{
    public class AddressAppContext : DbContext
    {
        public AddressAppContext(DbContextOptions<AddressAppContext> options) : base(options)
        {
        }

        // Address Book Table ke liye DbSet
        public DbSet<AddressBookEntity> AddressBooks { get; set; }

        //Users table ke liye DbSet
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}


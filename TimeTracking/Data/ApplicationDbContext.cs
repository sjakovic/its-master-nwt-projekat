using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TimeTracking.Models;

namespace TimeTracking.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        private Func<object, object> p;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext(Func<object, object> p)
        {
            this.p = p;
        }

        public DbSet<Project> Project { get; set; }
    }
}

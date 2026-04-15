using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;


namespace DatabaseQuiz.Models
{
    public class MyDBContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public MyDBContext() : base("name=MyDBContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // PostgreSQL 映射到 public schema
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
    }
}
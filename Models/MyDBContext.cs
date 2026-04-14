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
    }
}
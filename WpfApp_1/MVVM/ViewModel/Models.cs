using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WpfApp_1.MVVM.ViewModel
{
    public class VipApartament : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Clients> Clients { get; set; }

        public string ConnectionString { get; }

        public VipApartament()
        {
            this.ConnectionString = @"Data Source=DESKTOP-RFJRJ41;Initial Catalog=VipApartaments1;Integrated Security=True";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            options.UseSqlServer(ConnectionString);
        }
    }
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Pass { get; set; }
    }

    public class Clients
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }


    }
}

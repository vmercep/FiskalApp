using Fiskal.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FiskalApp.Model
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Racun> Racun { get; set; }
        public DbSet<Artikli> Artikli { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<StavkeRacuna> StavkeRacuna { get; set; }        
        public DbSet<Users> Users { get; set; }
        public DbSet<FiskalLogMessages> LogMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Settings>().HasData(new Settings { Id = 1, Email = "test@dot.com", Godina = 2020, NaplatniUredjaj = "1", Naziv = "Test", Oib = "12345678901", TipJedinica = "1", Vlasnik = "Test" });
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes("admin123", salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            modelBuilder.Entity<Users>().HasData(new Users { Id = 1, UserName="Admin", FirstName="Administrator", LastName="Admin", Oib="1234569871", Password= savedPasswordHash });

        }

    }
}

using Fiskal.Model;
using FiskalApp.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Model
{
    public class RacunRepository  : RepositoryBase<Racun>, IRacunRepository
    {
        private readonly ILogger<RacunRepository> _logger;
        public RacunRepository(AppDbContext appDbContext, ILogger<RacunRepository> logger) : base(appDbContext)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }



        private readonly AppDbContext _appDbContext;

        public Racun PopulateRacun(Racun racun)
        {
            _logger.LogInformation("Generate bill number");
            try
            {
                
                racun.Godina = DateTime.Now.Year;
                racun.DatumRacuna = DateTime.Now;
                
                if (string.IsNullOrEmpty( racun.BrojRacuna))
                    racun.BrojRacuna = GetRacunBroj();

                if (CheckIfBrojExist(racun.BrojRacuna))
                    throw new Exception("Broj Računa postoji u bazi!");

                decimal total = 0;
                foreach (var artikl in racun.StavkeRacuna)
                {
                    total = total+(artikl.Cijena * artikl.Kolicina);
                }
                racun.Iznos = total;

                return racun;
                
            }
            catch(Exception e)
            {
                _logger.LogInformation("Error ocured in bill number - populate generation "+e.Message);
                throw new Exception(e.Message);
            }

        }

        private bool CheckIfBrojExist(string brojRacuna)
        {
            try
            {
                var racuni = _appDbContext.Racun.Where(p => p.Godina == DateTime.Now.Year && p.BrojRacuna==brojRacuna).FirstOrDefault();
                if (racuni != null)
                    return true;
                return false;

            }
            catch (Exception e)
            {
                _logger.LogInformation("Error ocured in bill number generation " + e.Message);
                throw new Exception(e.Message);
            }
        }

        public string GetRacunBroj()
        {
            try
            {
                string brojRacuna = "";
                var settings = _appDbContext.Settings.FirstOrDefault();
                var racuni = _appDbContext.Racun.Where(p => p.Godina == DateTime.Now.Year).OrderByDescending(p => p.Id).FirstOrDefault();
                if (DateTime.Now.Year != settings.Godina) throw new Exception("Please check year in application settings");

                if (racuni == null)
                {
                    brojRacuna = string.Format("{0}/{1}/{2}", 1 , settings.TipJedinica, settings.NaplatniUredjaj);
                    _logger.LogInformation("Generate bill number  " + brojRacuna);
                    return brojRacuna;
                }
                string[] brojRacunaZadnji = racuni.BrojRacuna.Split('/');
                brojRacuna = string.Format("{0}/{1}/{2}", Convert.ToInt32(brojRacunaZadnji[0]) + 1, settings.TipJedinica, settings.NaplatniUredjaj);
                _logger.LogInformation("Generate bill number " + brojRacuna);
                return brojRacuna;

            }
            catch (Exception e)
            {
                _logger.LogInformation("Error ocured in bill number generation " + e.Message);
                throw new Exception(e.Message);
            }
        }


    }
}

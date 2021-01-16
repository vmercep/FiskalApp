using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Repository
{
    public class ArtikliRepository: RepositoryBase<Artikli>, IArtikliRepository
    {
        
        public ArtikliRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }



        private readonly AppDbContext _appDbContext;


        public async Task<Artikli> UpdateArtikl(Artikli artikl)
        {
            try
            {
                var result = await _appDbContext.Artikli.FirstOrDefaultAsync(p => p.Id == artikl.Id);
                if (result != null)
                {
                    result.Naziv = artikl.Naziv;
                    result.Sifra = artikl.Sifra;
                    result.SifraMjere = artikl.SifraMjere;
                    result.VrstaArtikla = artikl.VrstaArtikla;
                    result.Cijena = artikl.Cijena;

                    await _appDbContext.SaveChangesAsync();

                    return result;
                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }

        }

        public async Task InsertArtikl(Artikli artikl)
        {
            try
            {
                artikl.Sifra= GetNewSifruArtikla(artikl.VrstaArtikla);
                _appDbContext.Artikli.Add(artikl);
                _appDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception("Error ocured in database save "+e.Message);
            }


        }

        private string GetNewSifruArtikla(VrstaArtikla vrstaArtikla)
        {
            string sifraArtikla = "";
            Artikli artikl=null;

            var maxId = (_appDbContext.Artikli.Select(x => (int?)x.Id).Max() ?? 0);

            if(maxId!=0)
                artikl = _appDbContext.Artikli.Where(p => p.Id == maxId).FirstOrDefault();

            switch(vrstaArtikla)
            {
                case (VrstaArtikla.Artikl):
                    if (artikl == null)
                        sifraArtikla = "A000001";
                    else
                    {
                        string sifra = artikl.Sifra.Substring(1);
                        int sifraNo = Convert.ToInt32(sifra);
                        sifraNo = sifraNo + 1;
                        return "A" + sifraNo.ToString("000000");
                    }
                    break;
                case VrstaArtikla.Usluga:
                    if (artikl == null)
                        sifraArtikla = "U000001";
                    else
                    {
                        string sifra = artikl.Sifra.Substring(1);
                        int sifraNo = Convert.ToInt32(sifra);
                        sifraNo = sifraNo + 1;
                        return "U" + sifraNo.ToString("000000");
                    }
                    break;
                default:
                    break;

            }
            
            return sifraArtikla;
        }
    }
}

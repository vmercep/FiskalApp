using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Controllers;
using FiskalApp.Model;
using FiskalApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raverus.FiskalizacijaDEV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;

namespace FiskalApp.Helpers
{
    public class Fiskalizacija : IFiskalizacija
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<Fiskalizacija> _logger;

        public Fiskalizacija(AppDbContext appDbContext, ILogger<Fiskalizacija> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }


        public Task<Racun> FiskalizirajRacun(Racun racunZaFiskalizaciju)
        {
            _logger.Log(LogLevel.Information, "Starting fiskalization module");

            var result = _appDbContext.Settings.FirstOrDefault();
            X509Certificate2 cert = null;
            if (result != null)
            {
                if (result.Certificate == "" || result.Certificate == null) throw new Exception("Certificate dose not exist");
                byte[] bytes = Convert.FromBase64String(result.Certificate);
                cert = new X509Certificate2(bytes, result.CertificatePassword);
                _logger.Log(LogLevel.Information, "Get certificate " + cert.Issuer);


            }

            var user = _appDbContext.Users.Where(p => p.Id == racunZaFiskalizaciju.UserId).FirstOrDefault();
            if(user==null) throw new Exception("User dose not exist");




            CultureInfo culture = new CultureInfo("hr-HR");
            string vrijeme = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.DohvatiFormatiranoTrenutnoDatumVrijeme();
            string[] formatRacuna = racunZaFiskalizaciju.BrojRacuna.Split('/');
            racunZaFiskalizaciju.DatumRacuna = DateTime.Now;

            string jir = "";
            var postavke = _appDbContext.Settings.FirstOrDefault();


            Raverus.FiskalizacijaDEV.Schema.RacunType racun = new Raverus.FiskalizacijaDEV.Schema.RacunType() { Oib = postavke.Oib, USustPdv = false, DatVrijeme = vrijeme, OznSlijed = Raverus.FiskalizacijaDEV.Schema.OznakaSlijednostiType.P };
            Raverus.FiskalizacijaDEV.Schema.BrojRacunaType br = new Raverus.FiskalizacijaDEV.Schema.BrojRacunaType() { BrOznRac = formatRacuna[0], OznPosPr = formatRacuna[1], OznNapUr = formatRacuna[2] };

            racun.BrRac = br;
            //Raverus.FiskalizacijaDEV.Schema.PdvType pdv = new Raverus.FiskalizacijaDEV.Schema.PdvType();
            //Raverus.FiskalizacijaDEV.Schema.PorezType porez = new Raverus.FiskalizacijaDEV.Schema.PorezType() { Stopa = "0", Osnovica = "0", Iznos = "0" };
            //pdv.Porez.Add(porez);
            //racun.Pdv.Add(porez);

            racun.IznosUkupno = racunZaFiskalizaciju.Iznos.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            racun.NacinPlac = Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.G;
            racun.OibOper = user.Oib;

            string zki = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun(cert, racun.Oib, racun.DatVrijeme.Replace('T', ' '), racun.BrRac.BrOznRac, racun.BrRac.OznPosPr, racun.BrRac.OznNapUr, racun.IznosUkupno.ToString());
            _logger.Log(LogLevel.Information, "ZKI calculated " + zki);
            racun.ZastKod = zki;
            racun.NakDost = false;

            CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

#if !DEBUG
            cis.CisUrl = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService";
#endif
            //cis.NazivAutoGeneriranje = true;
            //cis.NazivMapeZahtjev = "C:\\Temp\\Fiskal\\Out\\";

            racunZaFiskalizaciju.Zki = zki;
            _appDbContext.Racun.Update(racunZaFiskalizaciju);
            _appDbContext.SaveChanges();


            try
            {
                _logger.Log(LogLevel.Information, "sending bill to server ");
                XmlDocument request = cis.KreirajRacunZahtjev(racun, cert);
                
                _appDbContext.LogMessages.Add(new FiskalLogMessages() {Direction="OUT", MesasgeDateTime=DateTime.Now, Message= JsonConvert.SerializeObject(request), BillId=racunZaFiskalizaciju.Id.ToString() });
                _appDbContext.SaveChanges();

                XmlDocument doc = cis.PosaljiSoapPoruku(request);

                if (doc != null)
                {
                    jir = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(doc);
                    _appDbContext.LogMessages.Add(new FiskalLogMessages() { Direction = "IN", MesasgeDateTime = DateTime.Now, Message = JsonConvert.SerializeObject(doc), BillId = racunZaFiskalizaciju.Id.ToString() });
                    _appDbContext.SaveChanges();
                    _logger.Log(LogLevel.Information, "jir calculated "+jir);
                    racunZaFiskalizaciju.Jir = jir;
                    _appDbContext.Racun.Update(racunZaFiskalizaciju);
                    _appDbContext.SaveChanges();
                }
                else
                {
                    // jebat ga
                    _logger.Log(LogLevel.Information, String.Format("Problem u fiskalizaciji"));
                }

            }
            catch (Exception ex)
            {

                if (cis.OdgovorGreska != null)
                {
                    _appDbContext.LogMessages.Add(new FiskalLogMessages() { Direction = "IN", MesasgeDateTime = DateTime.Now, Message = JsonConvert.SerializeObject(cis.OdgovorGreska), BillId = racunZaFiskalizaciju.Id.ToString() });
                    _appDbContext.SaveChanges();
                    _logger.Log(LogLevel.Information, cis.OdgovorGreska.InnerXml);
                    _logger.Log(LogLevel.Information, Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiSifruGreske(cis.OdgovorGreska, Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.RacunOdgovor));
                    _logger.Log(LogLevel.Information, Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiPorukuGreske(cis.OdgovorGreska, Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.RacunOdgovor));
                    _logger.Log(LogLevel.Information, Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiGreskuRezultataZahtjeva(cis.OdgovorGreska));

                }
                _logger.Log(LogLevel.Information, String.Format("Greška: {0}", ex.Message));
            }


            return Task.FromResult(racunZaFiskalizaciju);
        }

        public Task<Racun> FiskalizirajRacun(int id)
        {
            var racun = _appDbContext.Racun.Where(p => p.Id == id).FirstOrDefault();
            if (racun == null) return null;
            if (racun.Zki != null) return NaknadnoFiskaliziraj(racun);
            if (racun.Zki != null && racun.Jir != null) return Task.FromResult(racun);
            return FiskalizirajRacun(racun);

        }

        public Task<Racun> NaknadnoFiskaliziraj(Racun racunZaFiskalizaciju)
        {
            _logger.Log(LogLevel.Information, "Starting fiskalization module");

            var result = _appDbContext.Settings.FirstOrDefault();
            X509Certificate2 cert = null;
            if (result != null)
            {
                if (result.Certificate == "" || result.Certificate == null) throw new Exception("Certificate dose not exist");
                byte[] bytes = Convert.FromBase64String(result.Certificate);
                cert = new X509Certificate2(bytes, result.CertificatePassword);
                _logger.Log(LogLevel.Information, "Get certificate " + cert.Issuer);


            }

            var user = _appDbContext.Users.Where(p => p.Id == racunZaFiskalizaciju.UserId).FirstOrDefault();
            if (user == null) throw new Exception("User dose not exist");




            CultureInfo culture = new CultureInfo("hr-HR");
            string vrijeme = Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.FormatirajDatumVrijeme(racunZaFiskalizaciju.DatumRacuna);
            string[] formatRacuna = racunZaFiskalizaciju.BrojRacuna.Split('/');
            //racunZaFiskalizaciju.DatumRacuna = DateTime.Now;

            string jir = "";
            var postavke = _appDbContext.Settings.FirstOrDefault();


            Raverus.FiskalizacijaDEV.Schema.RacunType racun = new Raverus.FiskalizacijaDEV.Schema.RacunType() { Oib = postavke.Oib, USustPdv = false, DatVrijeme = vrijeme, OznSlijed = Raverus.FiskalizacijaDEV.Schema.OznakaSlijednostiType.P };
            Raverus.FiskalizacijaDEV.Schema.BrojRacunaType br = new Raverus.FiskalizacijaDEV.Schema.BrojRacunaType() { BrOznRac = formatRacuna[0], OznPosPr = formatRacuna[1], OznNapUr = formatRacuna[2] };

            racun.BrRac = br;
            //Raverus.FiskalizacijaDEV.Schema.PdvType pdv = new Raverus.FiskalizacijaDEV.Schema.PdvType();
            //Raverus.FiskalizacijaDEV.Schema.PorezType porez = new Raverus.FiskalizacijaDEV.Schema.PorezType() { Stopa = "0", Osnovica = "0", Iznos = "0" };
            //pdv.Porez.Add(porez);
            //racun.Pdv.Add(porez);

            racun.IznosUkupno = racunZaFiskalizaciju.Iznos.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            racun.NacinPlac = Raverus.FiskalizacijaDEV.Schema.NacinPlacanjaType.G;
            racun.OibOper = user.Oib;

            string zki = racunZaFiskalizaciju.Zki; //Raverus.FiskalizacijaDEV.PopratneFunkcije.Razno.ZastitniKodIzracun(cert, racun.Oib, racun.DatVrijeme.Replace('T', ' '), racun.BrRac.BrOznRac, racun.BrRac.OznPosPr, racun.BrRac.OznNapUr, racun.IznosUkupno.ToString());
            _logger.Log(LogLevel.Information, "ZKI calculated from bill" + zki);
            racun.ZastKod = zki;
            racun.NakDost = true;

            CentralniInformacijskiSustav cis = new CentralniInformacijskiSustav();

#if !DEBUG
            cis.CisUrl = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService";
#endif
            //cis.NazivAutoGeneriranje = true;
            //cis.NazivMapeZahtjev = "C:\\Temp\\Fiskal\\Out\\";

            //racunZaFiskalizaciju.Zki = zki;
            //_appDbContext.Racun.Update(racunZaFiskalizaciju);
            //_appDbContext.SaveChanges();


            try
            {
                _logger.Log(LogLevel.Information, "sending bill to server ");
                XmlDocument request = cis.KreirajRacunZahtjev(racun, cert);

                _appDbContext.LogMessages.Add(new FiskalLogMessages() { Direction = "OUT", MesasgeDateTime = DateTime.Now, Message = JsonConvert.SerializeObject(request), BillId = racunZaFiskalizaciju.Id.ToString() });
                _appDbContext.SaveChanges();

                XmlDocument doc = cis.PosaljiSoapPoruku(request);

                if (doc != null)
                {
                    jir = Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiJir(doc);
                    _appDbContext.LogMessages.Add(new FiskalLogMessages() { Direction = "IN", MesasgeDateTime = DateTime.Now, Message = JsonConvert.SerializeObject(doc), BillId = racunZaFiskalizaciju.Id.ToString() });
                    _appDbContext.SaveChanges();
                    _logger.Log(LogLevel.Information, "jir calculated " + jir);
                    racunZaFiskalizaciju.Jir = jir;
                    _appDbContext.Racun.Update(racunZaFiskalizaciju);
                    _appDbContext.SaveChanges();
                }
                else
                {
                    // jebat ga
                    _logger.Log(LogLevel.Information, String.Format("Problem u fiskalizaciji"));
                }

            }
            catch (Exception ex)
            {

                if (cis.OdgovorGreska != null)
                {
                    _appDbContext.LogMessages.Add(new FiskalLogMessages() { Direction = "IN", MesasgeDateTime = DateTime.Now, Message = JsonConvert.SerializeObject(cis.OdgovorGreska), BillId = racunZaFiskalizaciju.Id.ToString() });
                    _appDbContext.SaveChanges();
                    _logger.Log(LogLevel.Information, cis.OdgovorGreska.InnerXml);
                    _logger.Log(LogLevel.Information, Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiSifruGreske(cis.OdgovorGreska, Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.RacunOdgovor));
                    _logger.Log(LogLevel.Information, Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiPorukuGreske(cis.OdgovorGreska, Raverus.FiskalizacijaDEV.PopratneFunkcije.TipDokumentaEnum.RacunOdgovor));
                    _logger.Log(LogLevel.Information, Raverus.FiskalizacijaDEV.PopratneFunkcije.XmlDokumenti.DohvatiGreskuRezultataZahtjeva(cis.OdgovorGreska));

                }
                _logger.Log(LogLevel.Information, String.Format("Greška: {0}", ex.Message));
            }


            return Task.FromResult(racunZaFiskalizaciju);

        }
    }
}

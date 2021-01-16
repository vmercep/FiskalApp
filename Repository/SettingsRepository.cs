using Fiskal.Model;
using FiskalApp.Contracts;
using FiskalApp.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FiskalApp.Repository
{
    public class SettingsRepository:ISettingsRepository
    {
        private readonly AppDbContext appDbContext;

        public SettingsRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<CertificateDetails> GetCertificateDetails()
        {
            CertificateDetails details = new CertificateDetails();
            try
            {
                var result = await appDbContext.Settings.FirstOrDefaultAsync();
                if (result != null)
                {
                    if (result.Certificate == "" || result.Certificate == null) return null;
                    byte[] bytes = Convert.FromBase64String(result.Certificate);
                    var cert = new X509Certificate2(bytes, result.CertificatePassword);

                    details.CertificateDn = cert.Subject;
                    details.CertificateName = cert.FriendlyName;
                    details.CertSn = cert.SerialNumber;
                    details.CertValidity = cert.NotAfter.ToString();

                }
                return details;

            }
            catch (Exception e)
            {
                throw new Exception("Exception ocured in GetCertificateDetails Probably password is wrong "+e.Message);
                return null;
            }

        }

        public async Task<Settings> GetSettings()
        {
            return await appDbContext.Settings.FirstOrDefaultAsync();
        }

        public async Task<Settings> SaveCertificateAsync(string fileBytes, string filename)
        {
            try
            {
                var result = await appDbContext.Settings.FirstOrDefaultAsync();
                if (result != null)
                {
                    result.Certificate = fileBytes;
                    result.CertificateName = filename;

                    await appDbContext.SaveChangesAsync();

                    return result;
                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Settings> UpdateCertificatePassword(string password)
        {
            try
            {
                var result = await appDbContext.Settings.FirstOrDefaultAsync();
                if (result != null)
                {
                    result.CertificatePassword = password;


                    await appDbContext.SaveChangesAsync();

                    return result;
                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Settings> UpdateSettings(Settings settings)
        {
            try
            {
                var result = await appDbContext.Settings.FirstOrDefaultAsync(p => p.Id == settings.Id);
                if (result != null)
                {
                    result.Godina = settings.Godina;
                    result.NaplatniUredjaj = settings.NaplatniUredjaj;
                    result.Naziv = settings.Naziv;
                    result.Oib = settings.Oib;
                    result.TipJedinica = settings.TipJedinica;
                    result.Vlasnik = settings.Vlasnik;
                    result.Email = settings.Email;

                    await appDbContext.SaveChangesAsync();

                    return result;
                }
                return null;

            }
            catch(Exception e)
            {
                return null;
            }
            
        }
    }
}

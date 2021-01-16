using Fiskal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Contracts
{
    public interface ISettingsRepository
    {
        Task<Settings> GetSettings();
        Task<Settings> UpdateSettings(Settings settings);
        Task<Settings> SaveCertificateAsync(string fileBytes, string filename);
        Task<Settings> UpdateCertificatePassword(string password);
        Task<CertificateDetails> GetCertificateDetails();
    }
}

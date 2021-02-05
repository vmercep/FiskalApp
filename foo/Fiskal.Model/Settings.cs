using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fiskal.Model
{
    [Table("settings")]
    public class Settings
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        
        public string Vlasnik { get; set; }
        public string Oib { get; set; }

        public string Email { get; set; }

        public string TipJedinica { get; set; }

        public string NaplatniUredjaj { get; set; }

        public int Godina { get; set; }

        public string Certificate { get; set; }

        public string CertificatePassword { get; set; }

        public string CertificateName { get; set; }

    }
}

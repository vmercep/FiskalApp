using System;
using System.Collections.Generic;
using System.Text;

namespace Fiskal.Model
{
    public class CertificateDetails
    {
        public string CertificateName { get; set; }
        public string CertificateDn { get; set; }
        public string CertValidity { get; set; }

        public string CertSn { get; set; }
    }
}

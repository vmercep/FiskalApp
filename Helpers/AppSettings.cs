using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiskalApp.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }

        public string FiskalScheduler { get; set; }
        public string ReportScheduler { get; set; }

        public string Connection { get; set; }
    }
}

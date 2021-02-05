using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fiskal.Model
{
    [Table("log")]
    public class FiskalLogMessages
    {
        public int id { get; set; }
        public DateTime MesasgeDateTime { get; set; }

        public string Direction { get; set; }

        public string BillId { get; set; }

        public string Message { get; set; }

    }
}

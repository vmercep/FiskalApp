using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fiskal.Model
{
    [Table("racun")]
    public class Racun
    {
        public int Id { get; set; }

        public string BrojRacuna { get; set; }
        public int Godina { get; set; }

        public DateTime DatumRacuna { get; set; }

        public NacinPlacanja NacinPlacanja { get; set; }
        public decimal Iznos { get; set; }
        public string Zki { get; set; }
        public string Jir { get; set; }
        public string Operater { get; set; }

        [ForeignKey(nameof(Users))]
        public int UserId { get; set; }
        public Users User { get; set; }

        public ICollection<StavkeRacuna> StavkeRacuna { get; set; }
    }
}

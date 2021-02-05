using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fiskal.Model
{
    [Table("stavkeracuna")]
    public class StavkeRacuna
    {
        public int Id { get; set; }

        public int Kolicina { get; set; }

        public decimal Cijena { get; set; }

        [ForeignKey(nameof(Artikli))]
        public int ArtiklId { get; set; }
        public Artikli Artikl { get; set; }

        [ForeignKey(nameof(Racun))]
        public int RacunId { get; set; }
        public Racun Racun { get; set; }
    }
}

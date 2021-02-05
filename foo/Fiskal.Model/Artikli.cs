using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Fiskal.Model
{
    [Table("artikli")]
    public class Artikli
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv is required")]
        public string Naziv { get; set; }

        public string Sifra { get; set; }

        [Required(ErrorMessage = "Cijena is required")]
        public decimal Cijena { get; set; }

        public SifraMjera SifraMjere { get; set; }

        public VrstaArtikla VrstaArtikla { get; set; }
    }
}

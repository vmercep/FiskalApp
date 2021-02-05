using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Fiskal.Model
{
    [Table("users")]
    public class Users
    {
        public int Id { get; set; }
        [StringLength(40)]
        public string FirstName { get; set; }
        [StringLength(40)]
        public string LastName { get; set; }
        [Required]
        [StringLength(10)]
        public string UserName { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 11)]
        public string Oib { get; set; }

        [Required]
        public string Password { get; set; }




    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LearnOData.Model
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [StringLength(150)]
        [Required]
        public string Title { get; set; }

        [StringLength(150)]
        [Required]
        public string Director { get; set; }

        [StringLength(50)]
        public string CatalogNumber { get; set; }

        public int? Year { get; set; }

        //public PressingDetail PressingDetail { get; set; }

        public virtual Person Person { get; set; }
    }
}

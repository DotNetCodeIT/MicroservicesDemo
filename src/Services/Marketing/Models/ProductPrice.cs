using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Marketing.Models
{
    [Table("ProductPrices")]
    public class ProductPrice
    {
        public ProductPrice()
        {
            DateTime now = DateTime.UtcNow;
            this.PeriodFromUtc = new DateTime(now.Year, now.Month, now.Day);
            this.PeriodToUtc = PeriodFromUtc.AddYears(10);

        }

        [Key]
        public int ProductPriceId { get; set; }
        
        [Required]
        public int ProductId { get; set; }

        [Required]
        public DateTime PeriodFromUtc { get; set; }

        [Required] 
        public DateTime PeriodToUtc { get; set; }

        [Required] 
        [Range(0.01,999999.99)]
        public double Price { get; set; }

    }
}

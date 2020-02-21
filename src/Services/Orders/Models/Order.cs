using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Orders.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }


        public virtual ICollection<OrderItem> Items { get; set; }


        public OrderStatus OrderStatus { get; set; }

    }

    public enum OrderStatus
    {
        Inserted,
        Completed

    }
}

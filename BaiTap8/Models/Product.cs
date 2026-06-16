using System.ComponentModel.DataAnnotations;

namespace BaiTap8.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public ICollection<ProductImage>? Images { get; set; }
    }
}
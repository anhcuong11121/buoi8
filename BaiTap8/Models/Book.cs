using System.ComponentModel.DataAnnotations;

namespace BaiTap8.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public ICollection<BookImage>? Images { get; set; }
    }
}
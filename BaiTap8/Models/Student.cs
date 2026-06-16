using System.ComponentModel.DataAnnotations;

namespace BaiTap8.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        public ICollection<StudentImage>? Images { get; set; }
    }
}
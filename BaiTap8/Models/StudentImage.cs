namespace BaiTap8.Models
{
    public class StudentImage
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }
    }
}
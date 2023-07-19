using System.ComponentModel.DataAnnotations;


namespace Group_Project.Data.Entities
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        public DateTime Date { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}

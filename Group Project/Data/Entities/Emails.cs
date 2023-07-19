using System.ComponentModel.DataAnnotations;

namespace Group_Project.Data.Entities
{
    public class Emails
    {
        [Key]
        public int EmailID { get; set; }

        public string EmailAddress { get; set; } = string.Empty;
    }
}

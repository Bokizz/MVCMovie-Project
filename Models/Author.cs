using BookProject1.Models;
using System.ComponentModel.DataAnnotations;

namespace BookProject1.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required] public string? FirstName { get; set; }
        [Required] public string? LastName { get; set; }

        [Display(Name = "Date of birth:")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public string? Nationality { get; set; }
        public string? Gender { get; set; }
        public ICollection<Book>? Books { get; set; }

        public string? FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }

    }
}

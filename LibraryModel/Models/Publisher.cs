using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Sirbu_Iulia_Lab2.Models
{
    public class Publisher
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Publisher name is required.")]
        [StringLength(50, ErrorMessage = "Publisher name cannot be longer than 50 characters.")]
        public string PublisherName { get; set; }

        [StringLength(100, ErrorMessage = "Address cannot be longer than 100 characters.")]
        public string Adress { get; set; }

        public ICollection<PublishedBook> PublishedBooks { get; set; } = new List<PublishedBook>();
    }
}
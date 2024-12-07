using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sirbu_Iulia_Lab2.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        //public string Author { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }
        public int? GenreID { get; set; }
        public Genre? Genre { get; set; }
        public int? AuthorID { get; set; }
        public Author? Author { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<PublishedBook>? PublishedBook { get; set; }
    }
}
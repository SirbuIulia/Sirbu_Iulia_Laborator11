using Microsoft.EntityFrameworkCore;
using Sirbu_Iulia_Lab2.Models;
using System;

namespace Sirbu_Iulia_Lab2.Data
{
    public class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new LibraryContext(
                serviceProvider.GetRequiredService<DbContextOptions<LibraryContext>>()))
            {
                // Check if the database has already been seeded
                //if (context.Book.Any())
                //{
                //    return; // Database was already created
                //}
            }
        }
    }
}

                //    // Seed Books
                //    context.Book.AddRange(
                //       new Book { Title = "Baltagul", Price = 22, AuthorID = 1, GenreID = 1 },
                //       new Book { Title = "Enigma Otiliei", Price = 18, AuthorID = 2, GenreID = 2 }
                //   );

                //    context.Author.AddRange(
                //       new Author { FirstName = "Mihail", LastName = "Sadoveanu" },
                //       new Author { FirstName = "George", LastName = "Calinescu" }
                //   );


                //    // Seed Genres
                //    context.Genre.AddRange(
                //        new Genre { Name = "Roman" },
                //        new Genre { Name = "Nuvela" },
                //        new Genre { Name = "Poezie" }
                //    );

                //    // Seed Customers
                //    context.Customer.AddRange(
                //        new Customer
                //        {
                //            Name = "Popescu Marcela",
                //            Adress = "Str. Plopilor, nr. 24",
                //            BirthDate = DateTime.Parse("1979-09-01")
                //        },
                //        new Customer
                //        {
                //            Name = "Mihailescu Cornel",
                //            Adress = "Str. Bucuresti, nr. 45, ap. 2",
                //            BirthDate = DateTime.Parse("1969-07-08")
                //        }
                //    );

                //    // Save changes to the database
                //    context.SaveChanges();

 
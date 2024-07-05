
using BookProject1.Data;
using BookProject1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;            
using System.Linq;
using System.Threading.Tasks;
using BookProject1.Areas.Identity.Data;
namespace BookProject1.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<BookProject1User>>();
                IdentityResult roleResult;
                var registeredRoleCheck = await RoleManager.RoleExistsAsync("Registered");
                if (!registeredRoleCheck)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("Registered"));
                }
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            BookProject1User user = await UserManager.FindByEmailAsync("admin@bookstoread.com");
            if (user == null)
            { 
                var User = new BookProject1User();
                User.Email = "admin@bookstoread.com";
                User.UserName = "BojanBeast";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BookProject1Context(
                serviceProvider.GetRequiredService<DbContextOptions<BookProject1Context>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Book.Any() || context.Author.Any() || context.Genre.Any() || context.UserBooks.Any() || context.BookGenre.Any())
                {
                    return;
                }
                context.Author.AddRange(
 new Author { /*Id = 1, */FirstName = "Chris", LastName = "Columbus", BirthDate = DateTime.Parse("1965-7-31"), Nationality = "American", Gender = "Male"},
 new Author { /*Id = 2, */FirstName = "Mike", LastName = "Newell", BirthDate = DateTime.Parse("1942-3-28"), Nationality = "English", Gender = "Male" },
  new Author { /*Id = 3, */FirstName = "David", LastName = "Yates", BirthDate = DateTime.Parse("1960-4-21"), Nationality = "American", Gender = "Male" },
    new Author { /*Id = 4, */FirstName = "Stanley", LastName = "Kubrick", BirthDate = DateTime.Parse("1928-7-26"), Nationality = "American", Gender = "Male" },
  new Author { /*Id = 5, */FirstName = "Andres", LastName = "Muschietti", BirthDate = DateTime.Parse("1976-3-3"), Nationality = "Italian", Gender = "Male" },
  new Author { /*Id = 6, */FirstName = "Mary", LastName = "Lamber", BirthDate = DateTime.Parse("1943-5-1"), Nationality = "American", Gender = "Female" },
  new Author { /*Id = 7, */FirstName = "David", LastName = "Fincher", BirthDate = DateTime.Parse("1950-9-22"), Nationality = "American", Gender = "Male" },
  new Author { /*Id = 8, */FirstName = "Denis", LastName = "Villeneuve", BirthDate = DateTime.Parse("1963-2-11"), Nationality = "American", Gender = "Male" },
    new Author { /*Id = 9, */FirstName = "Boris", LastName = "Yelsin", BirthDate = DateTime.Parse("1931-2-1"), Nationality = "American", Gender = "Male" }


);
                context.SaveChanges();

                context.Genre.AddRange(
new Genre { GenreName = "Thriller" },
new Genre { GenreName = "Drama" },
new Genre { GenreName = "Science-Fiction" },
new Genre { GenreName = "Horror" },
new Genre { GenreName = "Fantasy" },
new Genre { GenreName = "Detective" },
new Genre { GenreName = "Adventure" }
                    );
                context.SaveChanges();

                context.Book.AddRange(
new Book
{
    Title = "Harry Potter and the Philosopher's Stone",
    YearPublished = 2001,
    NumPages = 1024,
    Description = "Harry Potter and the Philosopher's Stone (also known as Harry Potter and the Sorcerer's Stone in the United States) is a 2001 fantasy film directed by Chris Columbus and produced by David Heyman, from a screenplay by Steve Kloves, based on the 1997 novel of the same name by J. K. Rowling. It is the first instalment in the Harry Potter film series. The film stars Daniel Radcliffe as Harry Potter, with Rupert Grint as Ron Weasley, and Emma Watson as Hermione Granger. Its story follows Harry's first year at Hogwarts School of Witchcraft and Wizardry as he discovers that he is a famous wizard and begins his formal wizarding education.",
    Publisher = "Warner Bros",
    FrontPage = "https://flixpatrol.com/runtime/cache/files/posters/w/w350/wumc08ipkeatf9rnmnxvidxqp4w.jpg",
    DownloadUrl = "https://www.imdb.com/title/tt0241527/",
    AuthorId = 1
},
new Book
{
    Title = "Harry Potter and the Goblet of Fire",
    YearPublished = 2005,
    NumPages = 900,
    Description = "Harry Potter and the Goblet of Fire is a fantasy novel written by British author J. K. Rowling and the fourth novel in the Harry Potter series. It follows Harry Potter, a wizard in his fourth year at Hogwarts School of Witchcraft and Wizardry, and the mystery surrounding the entry of Harry's name into the Triwizard Tournament, in which he is forced to compete.",
    Publisher = "Warner Bros",
    FrontPage = "https://cdn.europosters.eu/image/750/104198.jpg",
    DownloadUrl = "https://www.imdb.com/title/tt0330373/",
    AuthorId = 2
},
new Book
{
    Title = "Fantastic Beasts and Where to Find Them",
    YearPublished = 2016,
    NumPages = 814,
    Description = "Fantastic Beasts and Where to Find Them is a 2016 fantasy film directed by David Yates and written by J. K. Rowling. It is the first instalment in the Fantastic Beasts film series and the ninth overall in the Wizarding World franchise, serving as a spin-off of and prequel to the Harry Potter film series; it is inspired by the 2001 guide book of the same name by Rowling. The film features an ensemble cast that includes Eddie Redmayne, Katherine Waterston, Dan Fogler, Alison Sudol, Ezra Miller, Samantha Morton, Jon Voight, Carmen Ejogo, Ron Perlman, Colin Farrell and Johnny Depp.",
    Publisher = "Warner Bros",
    FrontPage = "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcTWaFwwbwLjWLUEySM5qFAQLpYwn-2MomhDuWn_qODgKhbCnDkG53aDU-6pMCnPOqY5Qhce_XmcZ8XKxBGv0LxiBn8z6fwmCkVrUZ9JJAg",
    DownloadUrl = "https://www.imdb.com/title/tt3183660/",
    AuthorId = 3
},
new Book
{
    Title = "The Shining",
    YearPublished = 1980,
    NumPages = 50,
    Description = "The Shining is a 1980 psychological horror film[7] produced and directed by Stanley Kubrick and co-written with novelist Diane Johnson. It is based on Stephen King's 1977 novel of the same name and stars Jack Nicholson, Danny Lloyd, Shelley Duvall, and Scatman Crothers. Nicholson plays Jack Torrance, a writer and recovering alcoholic who accepts a new position as the off-season caretaker of the Overlook Hotel. Lloyd plays his young son Danny, who has psychic abilities (\"the shining\"), which he learns about from head chef Dick Hallorann (Crothers). Danny's imaginary friend Tony warns him the hotel is haunted before a winter storm leaves the family snowbound in the Colorado Rockies. Jack's sanity deteriorates under the influence of the hotel and the residents, and Danny and his mother Wendy (Duvall) face mortal danger.",
    Publisher = "Warner Bros,Columbia Pictures",
    FrontPage = "https://resizing.flixster.com/sSiEFHeE5OsgpnfPGXSGd8m9CnE=/fit-in/352x330/v2/https://resizing.flixster.com/-XZAfHZM39UwaGJIFWKAE8fS0ak=/v3/t/assets/p40_v_h9_sm.jpg",
    DownloadUrl = "https://www.amazon.com/Shining-Stephen-King/dp/0307743659",
    AuthorId = 4
},
new Book
{
    Title = "It",
    YearPublished = 2017,
    NumPages = 702,
    Description = "It is a 1986 horror novel by American author Stephen King. It was his 22nd book and the 17th novel written under his own name. The story follows the experiences of seven children as they are terrorized by an evil entity that exploits the fears of its victims to disguise itself while hunting its prey. \"It\" primarily appears in the form of Pennywise the Dancing Clown to attract its preferred prey of young children.",
    Publisher = "Warner Bros",
    FrontPage = "https://upload.wikimedia.org/wikipedia/en/5/5a/It_%282017%29_poster.jpg",
    DownloadUrl = "https://www.imdb.com/title/tt1396484/",
    AuthorId = 5
},
new Book
{
    Title = "Pet Sematary",
    YearPublished = 1983,
    NumPages = 58,
    Description = "Pet Sematary is a 1989 American supernatural horror film and the first adaptation of Stephen King's 1983 novel of the same name. Directed by Mary Lambert, with King writing the screenplay, it stars Dale Midkiff, Denise Crosby, Blaze Berdahl, Fred Gwynne, and Miko Hughes as Gage Creed. The title is a sensational spelling of \"pet cemetery\".\r\n\r\nThe film was released on April 21, 1989, and grossed $57.5 million at the box office on a budget of $11.5 million. A sequel, Pet Sematary Two, was released in 1992 and a second film adaptation was released in 2019.",
    Publisher = "Paramount Pictures",
    FrontPage = "https://upload.wikimedia.org/wikipedia/en/7/75/Pet_sematary_poster.jpg",
    DownloadUrl = "https://www.imdb.com/title/tt0098084/",
    AuthorId = 6
},
new Book
{
    Title = "Se7en",
    YearPublished = 1995,
    NumPages = 330,
    Description = "When retiring police Detective William Somerset (Morgan Freeman) tackles a final case with the aid of newly transferred David Mills (Brad Pitt), they discover a number of elaborate and grizzly murders. They soon realize they are dealing with a serial killer (Kevin Spacey) who is targeting people he thinks represent one of the seven deadly sins. Somerset also befriends Mills' wife, Tracy (Gwyneth Paltrow), who is pregnant and afraid to raise her child in the crime-riddled city.",
    Publisher = "New Line Cinema",
    FrontPage = "https://static.tvtropes.org/pmwiki/pub/images/se7en_poster.png",
    DownloadUrl = "https://www.imdb.com/title/tt0114369/",
    AuthorId = 7
},
new Book
{
    Title = "Zodiac",
    YearPublished = 2007,
    NumPages = 85,
    Description = "Robert Graysmith, a cartoonist by profession, finds himself obsessively thinking about the zodiac killer. He uses his puzzle-solving abilities to get closer to revealing the identity of the killer.",
    Publisher = "Warner Bros",
    FrontPage = "https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcSxDq2JMofpl5yhYPsApK77JKLCki65c-mS2VD56nPxwJpZ_k5wk5n_e5f9FzIElaVSjruVI2rIDH59eG7rDviHehjEcgb6ztEBj9mPtg",
    DownloadUrl = "https://www.imdb.com/title/tt0443706/",
    AuthorId = 8
},
new Book
{
    Title = "Prisoners",
    YearPublished = 2013,
    NumPages = 122,
    Description = "When the police take time to find Keller Dover's daughter and her friend, he decides to go on a search himself. His desperation leads him closer to finding the truth and also jeopardises his own life.",
    Publisher = "Warner Bros",
    FrontPage = "https://upload.wikimedia.org/wikipedia/en/6/63/Prisoners2013Poster.jpg",
    DownloadUrl = "https://www.imdb.com/title/tt1392214/",
    AuthorId = 9
});
                context.SaveChanges();

                context.BookGenre.AddRange(
                    new BookGenre { GenreId = 7, BookId = 1 },
 new BookGenre { GenreId = 5, BookId = 1 },
 new BookGenre { GenreId = 7, BookId = 2 },
 new BookGenre { GenreId = 5, BookId = 2 },
 new BookGenre { GenreId = 7, BookId = 3 },
 new BookGenre { GenreId = 5, BookId = 3 },
 new BookGenre { GenreId = 3, BookId = 4 },
 new BookGenre { GenreId = 4, BookId = 4 },
new BookGenre { GenreId = 3, BookId = 5 },
new BookGenre { GenreId = 4, BookId = 5 },
new BookGenre { GenreId = 3, BookId = 6 },
new BookGenre { GenreId = 4, BookId = 6 },
new BookGenre { GenreId = 1, BookId = 7 },
new BookGenre { GenreId = 6, BookId = 7 },
new BookGenre { GenreId = 1, BookId = 8 },
new BookGenre { GenreId = 6, BookId = 8 },
new BookGenre { GenreId = 1, BookId = 9 },
new BookGenre { GenreId = 6, BookId = 9 });
                context.SaveChanges();

                context.UserBooks.AddRange(
                      new UserBooks
                      {
                          BookId = 1,
                          AppUser = "User 1"
                      },
                      new UserBooks
                      {
                          BookId = 1,
                          AppUser = "User 2"
                      },
                      new UserBooks
                      {
                          BookId = 2,
                          AppUser = "User 3"
                      },
                      new UserBooks
                      {
                          BookId = 3,
                          AppUser = "User 4"
                      },
                      new UserBooks
                      {
                          BookId = 3,
                          AppUser = "User 5"
                      },
                      new UserBooks
                      {
                          BookId = 3,
                          AppUser = "User 5"
                      },
                      new UserBooks
                      {
                          BookId = 4,
                          AppUser = "User 6"
                      },
                      new UserBooks
                      {
                          BookId = 5,
                          AppUser = "User 7"
                      },
                      new UserBooks
                      {
                          BookId = 5,
                          AppUser = "User 8"
                      },
                      new UserBooks
                      {
                          BookId = 6,
                          AppUser = "User 9"
                      },
                      new UserBooks
                      {
                          BookId = 7,
                          AppUser = "User 10"
                      },
                      new UserBooks
                      {
                          BookId = 9,
                          AppUser = "User 11"
                      },
                      new UserBooks
                      {
                          BookId = 8,
                          AppUser = "User 12"
                      }
                  );
                context.SaveChanges();

                context.Review.AddRange(
                    new Review
                    {
                        BookId = 1,
                        AppUser = "User 1",
                        Comment = "Love it",
                        Rating = 5
                    },
                    new Review
                    {
                        BookId = 1,
                        AppUser = "User 2",
                        Comment = "I did not understand it",
                        Rating = 1
                    },
                    new Review
                    {
                        BookId = 2,
                        AppUser = "User 3",
                        Comment = "Boring",
                        Rating = 2
                    },
                    new Review
                    {
                        BookId = 1,
                        AppUser = "User 4",
                        Comment = "Well this is stupid",
                        Rating = 2
                    },
                    new Review
                    {
                        BookId = 3,
                        AppUser = "User 5",
                        Comment = "No comment",
                        Rating = 5
                    },
                    new Review
                    {
                        BookId = 3,
                        AppUser = "User 6",
                        Comment = "Will read 3 more times!",
                        Rating = 5
                    },
                    new Review
                    {
                        BookId = 5,
                        AppUser = "User 5",
                        Comment = "Scary",
                        Rating = 3
                    },
                    new Review
                    {
                        BookId = 4,
                        AppUser = "User 7",
                        Comment = "what is this",
                        Rating = 4
                    },
                    new Review
                    {
                        BookId = 6,
                        AppUser = "User 9",
                        Comment = "Not for me so scary",
                        Rating = 3
                    },
                    new Review
                    {
                        BookId = 9,
                        AppUser = "User 10",
                        Comment = "Wicked",
                        Rating = 3
                    },
                    new Review
                    {
                        BookId = 7,
                        AppUser = "User 12",
                        Comment = "Wicked",
                        Rating = 3
                    },
                     new Review
                     {
                         BookId = 8,
                         AppUser = "User 69",
                         Comment = "Inspirational",
                         Rating = 5
                     },
                    new Review
                    {
                        BookId = 9,
                        AppUser = "User 8",
                        Comment = "Na Jovana i e omilena",
                        Rating = 5
                    }
                );
                context.SaveChanges();
            }
        }
    }
}


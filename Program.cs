using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SuncoastMovies
{
    class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PrimaryDirector { get; set; }
        public int YearReleased { get; set; }
        public string Genre { get; set; }

        // This is the column in the database
        public int RatingId { get; set; }
        // This is the related object we can use from our code (if properly used with Include)
        public Rating Rating { get; set; }

        // This is the related list of roles we an use (if properly used with Include)
        public List<Role> Roles { get; set; }
    }

    class Rating
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    class Role
    {
        public int Id { get; set; }
        public string CharacterName { get; set; }

        // This is the column in the database
        public int MovieId { get; set; }
        // This is the related object we can use from our code (if properly used with Include)
        public Movie Movie { get; set; }

        // This is the column in the database
        public int ActorId { get; set; }
        // This is the related object we can use from our code (if properly used with Include)
        public Actor Actor { get; set; }
    }

    class Actor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }

        // This is the related list of roles we an use (if properly used with Include)
        public List<Role> Roles { get; set; }
    }

    // Define a database context for our Suncoast Movies database.
    // It derives from (has a parent of) DbContext so we get all the
    // abilities of a database context from EF Core.
    class SuncoastMoviesContext : DbContext
    {
        // Define a movies property that is a DbSet.
        public DbSet<Movie> Movies { get; set; }

        // Define a Ratings property that is a DbSet.
        public DbSet<Rating> Ratings { get; set; }

        // Define a Roles property that is a DbSet.
        public DbSet<Role> Roles { get; set; }

        // Define an Actors property that is a DbSet.
        public DbSet<Actor> Actors { get; set; }

        // Define a method required by EF that will configure our connection
        // to the database.
        //
        // DbContextOptionsBuilder is provided to us. We then tell that object
        // we want to connect to a postgres database named suncoast_movies on
        // our local machine.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            optionsBuilder.UseLoggerFactory(loggerFactory);

            optionsBuilder.UseNpgsql("server=localhost;database=SuncoastMovies");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var context = new SuncoastMoviesContext();

            var keepGoing = true;

            while (keepGoing)
            {
                Console.Write("(L)ist movies. (C)reate movie. (D)elete movie. (U)pdate movie. (Q)uit: ");
                var option = Console.ReadLine().ToUpper();

                switch (option)
                {
                    case "Q":
                        keepGoing = false;
                        break;

                    case "L":
                        var movies = context.Movies.Include(movie => movie.Rating).Include(movie => movie.Roles).ThenInclude(role => role.Actor);

                        var movieCount = movies.Count();
                        Console.WriteLine($"There are {movieCount} movies!");

                        foreach (var movie in movies)
                        {
                            if (movie.Rating == null)
                            {
                                Console.WriteLine($"There is a movie named {movie.Title} and has not been rated yet");
                            }
                            else
                            {
                                Console.WriteLine($"There is a movie named {movie.Title} and a rating of {movie.Rating.Description}");
                            }

                            foreach (var role in movie.Roles)
                            {
                                Console.WriteLine($" - Has a character named {role.CharacterName} played by {role.Actor.FullName}");
                            }
                        }
                        break;

                    case "C":
                        Console.Write("What is the name of the new movie: ");
                        var title = Console.ReadLine();

                        Console.Write("What is the name of the primary director: ");
                        var primaryDirector = Console.ReadLine();

                        Console.Write("What is the genre: ");
                        var genre = Console.ReadLine();

                        Console.Write("In what year was the movie released: ");
                        var yearReleased = int.Parse(Console.ReadLine());

                        Console.Write("What is the ID of the movie rating: ");
                        var ratingID = int.Parse(Console.ReadLine());

                        var newMovie = new Movie
                        {
                            Title = title,
                            PrimaryDirector = primaryDirector,
                            Genre = genre,
                            YearReleased = yearReleased,
                            RatingId = ratingID
                        };

                        context.Movies.Add(newMovie);
                        context.SaveChanges();
                        break;

                    case "U":
                        Console.Write("What is the name of the movie you want to update: ");
                        var titleOfMovieToUpdate = Console.ReadLine();

                        var existingMovieToUpdate = context.Movies.FirstOrDefault(movie => movie.Title == titleOfMovieToUpdate);

                        if (existingMovieToUpdate != null)
                        {
                            Console.Write("Enter a new name for the movie: ");
                            existingMovieToUpdate.Title = Console.ReadLine();

                            context.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine($"No movie with title {titleOfMovieToUpdate} to update");
                        }
                        break;

                    case "D":
                        Console.Write("What is the name of the movie you want to delete: ");
                        var titleOfMovieToDelete = Console.ReadLine();

                        var existingMovieToDelete = context.Movies.FirstOrDefault(movie => movie.Title == titleOfMovieToDelete);

                        if (existingMovieToDelete != null)
                        {
                            context.Movies.Remove(existingMovieToDelete);
                            context.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine($"No movie with title {titleOfMovieToDelete} to update");
                        }
                        break;
                }
            }
        }
    }
}
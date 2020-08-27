using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace SuncoastMovies
{
    class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PrimaryDirector { get; set; }
        public int YearReleased { get; set; }
        public string Genre { get; set; }
        public int RatingId { get; set; }
        public Rating Rating { get; set; }
        public List<Role> Roles { get; set; }
    }
    // Define a database context for our Suncoast Movies database.
    // It derives from (has a parent of) DbContext so we get all the
    // abilities of a database context from EF Core.
    class SuncoastMoviesContext : DbContext
    {
        // Define a movies property that is a DbSet
        public DbSet<Movie> Movies { get; set; }

        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Actor> Actors { get; set; }

        // Define a method required by EF that will configure our connection to the database.
        // 
        // DbContextOptionsBuilder is provided to us. We then tell that object 
        //  we want to connect to a PostgreSQL database named SuncoastMovies on our local machine
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("server=localhost;database=SuncoastMovies");

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }
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
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int ActorId { get; set; }
        public Actor Actor { get; set; }
    }

    class Actor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public List<Role> Roles { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Get a new context which will connect to the database
            var context = new SuncoastMoviesContext();

            // Get a reference to our collection of movies/
            // NOTE: This doesn't yet access any of them, just
            // gives us a variable that knows how
            var movies = context.Movies.Include(movie => movie.Rating).Include(movie => movie.Roles).ThenInclude(role => role.Actor);

            // Count the movies
            var movieCount = movies.Count();
            Console.WriteLine($"There are {movieCount} movies");

            // List the movies
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


        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MovieCatalogue.Models;

namespace MovieApp.Web;

public class MovieAppContext(DbContextOptions<MovieAppContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

    public DbSet<Movie> Movies { get; set; }
}

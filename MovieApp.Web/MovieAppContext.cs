using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MovieApp.Web.Models;
using System;
using System.Collections.Generic;

namespace MovieApp.Web;

public class MovieAppContext(DbContextOptions<MovieAppContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieContent>().Property(p => p.Length)
            .HasConversion<long>();
        modelBuilder.Entity<MovieContent>().Property(p => p.LastTimeStamp)
            .HasConversion<long?>();

        // Deleting a collection keeps its movies; they just become uncollected.
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Collection)
            .WithMany(c => c.Movies)
            .HasForeignKey(m => m.CollectionId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieContent> MovieContent { get; set; }
    public DbSet<MovieCollection> Collections { get; set; }
}

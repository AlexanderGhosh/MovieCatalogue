using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Models;

namespace MovieApp.Web;

public class MovieAppContext(DbContextOptions<MovieAppContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<WatchHistory> WatchHistories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(movie =>
        {
            movie.HasMany(d => d.Tags)
                .WithMany(p => p.Movies)
                .UsingEntity("MovieTag");
        });
    }
}

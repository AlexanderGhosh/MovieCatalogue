using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Models;

namespace MovieApp.Web.Services.Implementations;

public class MovieAppContext(DbContextOptions<MovieAppContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Movie 1 : 0..1 MovieContent
        builder.Entity<Movie>()
            .HasOne(m => m.Content)
            .WithOne(c => c.Movie)
            .HasForeignKey<MovieContent>(c => c.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        // Movie <-> MovieCollection (many-to-many via Movie_Collection, composite key)
        builder.Entity<Movie>()
            .HasMany(m => m.Collections)
            .WithMany(c => c.Movies)
            .UsingEntity<Movie_Collection>(
                right => right.HasOne<MovieCollection>().WithMany().HasForeignKey(mc => mc.CollectionId),
                left => left.HasOne<Movie>().WithMany().HasForeignKey(mc => mc.MovieId),
                join =>
                {
                    join.ToTable("Movie_Collection");
                    join.HasKey(mc => new { mc.MovieId, mc.CollectionId });
                });

        // MovieCollection <-> MovieCollection (self-referencing many-to-many)
        builder.Entity<MovieCollection>()
            .HasMany(c => c.ChildCollections)
            .WithMany(c => c.ParentCollections)
            .UsingEntity<Collection_Collection>(
                right => right.HasOne<MovieCollection>().WithMany()
                    .HasForeignKey(cc => cc.ChildId).OnDelete(DeleteBehavior.Restrict),
                left => left.HasOne<MovieCollection>().WithMany()
                    .HasForeignKey(cc => cc.ParentId).OnDelete(DeleteBehavior.Restrict),
                join =>
                {
                    join.ToTable("Collection_Collection");
                    join.HasKey(cc => new { cc.ParentId, cc.ChildId });
                });
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieContent> MovieContent { get; set; }
    public DbSet<MovieCollection> Collections { get; set; }
}
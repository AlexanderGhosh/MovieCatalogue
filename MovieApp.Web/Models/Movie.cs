using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCatalogue.Models;

[Table("Movie")]
public class Movie : IMovie
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public DateTime AddedOn { get; set; }

    [Column("BlobGuid")]
    public Guid? MovieGuid { get; set; }
    public int? ThumbnailId { get; set; }

    public virtual ICollection<_WatchHistory> WatchHistories { get; set; } = new HashSet<_WatchHistory>();

    public virtual ICollection<_Tag> Tags { get; set; } = new HashSet<_Tag>();

    public virtual _Thumbnail? Thumbnail { get; set; } = null!;
}

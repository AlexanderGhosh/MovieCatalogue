using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Web.Models;

[Table("Movie")]
public class Movie
{
    public int Id { get; set; }

    public required string Title { get; set; }

    [Column(TypeName = "BIGINT")]
    public required TimeSpan Length { get; set; }

    [Column(TypeName = "BIGINT")]
    public TimeSpan? LastTimeStamp { get; set; }

    public DateTime AddedOn { get; set; }

    [Column("BlobGuid")]
    public Guid? MovieGuid { get; set; }
    public int? ThumbnailId { get; set; }

    public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new HashSet<WatchHistory>();

    public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

    public virtual Thumbnail? Thumbnail { get; set; } = null!;
}

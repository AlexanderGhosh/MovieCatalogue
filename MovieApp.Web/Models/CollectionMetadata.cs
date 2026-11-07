namespace MovieApp.Web.Models
{
    public class AgeRating
    {
        public required string Rating { get; set; }
        public required string[] Reasons { get; set; }
    }
    public class CollectionMetadata
    {
        public required string Description { get; set; }
        public required string[] Genres { get; set; }
        public required AgeRating AgeRating { get; set; }
        public required string[] Creators { get; set; }
        public required string[] MainCast { get; set; }
    }
}

namespace MovieCatalogue
{
    public interface IMovie
    {
        public int Id { get; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
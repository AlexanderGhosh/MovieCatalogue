namespace MovieCatalogue
{
    public interface IMovieContent
    {
        TimeSpan Length { get; }
        TimeSpan? LastTimeStamp { get; }
        Stream Stream { get; }
    }
}

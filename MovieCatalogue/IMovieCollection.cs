namespace MovieCatalogue;

public interface IMovieCollection
{
    int Id { get; }
    string Name { get; }
    DateTime AddedOn { get; }
}

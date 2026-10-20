namespace MovieApp.Web.Models;

public interface IMovieItem
{
    public string Name { get; }
    public DateTime AddedOn { get; set; }
}
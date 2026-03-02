using MP4.Parser.NET;

namespace MovieApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MovieFolderRepository movies = new(@"C:\Users\AGWDW\Desktop\Programing\C#\MovieCatalogue\Movies");
            movies.LoadMovies();
            foreach (var movie in movies)
            {
                Console.WriteLine(movie.Title);
            }

            using FileStream movieOutput = new(@"C:\Users\AGWDW\Desktop\Programing\C#\MovieCatalogue\OutputMovie.dat", FileMode.Create, FileAccess.Write);
            int head = 0;
            int i = 0;
            MP4StreamReader reader = new();
            foreach (Chunk chunk in movies.PlayMovie(1))
            {
                Console.WriteLine($"Playing chunk {i++}");
                var box = reader.ReadBox(chunk.Data, 0);


                movieOutput.Write(chunk.Data, 0, chunk.Size);
                head += chunk.Size;
            }
        }
    }
}

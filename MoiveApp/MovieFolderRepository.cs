using Microsoft.VisualBasic;
using MovieCatalogue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MovieApp
{
    internal class MovieFolderRepository(string baseDirectory) : MovieRepository, IEnumerable<MovieFile>
    {
        public override void AddMovie(IMovie movie) => throw new InvalidOperationException();

        public void LoadMovies()
        {
            DirectoryInfo baseDir = new(baseDirectory);
            if(!baseDir.Exists) throw new DirectoryNotFoundException($"The directory '{baseDirectory}' does not exist.");
            foreach (FileInfo movieFile in baseDir.EnumerateFiles())
            {
                IMovie movie = new MovieFile() 
                { 
                    Title = movieFile.Name, 
                    FilePath = movieFile.FullName 
                };
                base.AddMovie(movie);
            }
        }

        const int MAX_CHUNK_SIZE = 1024; // 1KB
        public IEnumerable<Chunk> PlayMovie(int id)
        {
            byte[] currentChunk = new byte[MAX_CHUNK_SIZE];
            if (GetMovie(id) is not MovieFile movie) 
                throw new ArgumentException($"No movie with id {id} found.");

            using FileStream stream = new(movie.FilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(stream);
            int actualReadSize = int.MaxValue;
            while (actualReadSize >= MAX_CHUNK_SIZE)
            {
                actualReadSize = reader.Read(currentChunk);
                yield return new Chunk(currentChunk, actualReadSize);
            }
        }


        public IEnumerator<MovieFile> GetEnumerator() => GetAllMovies().Cast<MovieFile>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class Chunk
    {
        public byte[] Data { get; }
        public int Size { get; }
        public Chunk(byte[] data, int actualSize)
        {
            Data = data;
            Size = actualSize;
        }
    }
}

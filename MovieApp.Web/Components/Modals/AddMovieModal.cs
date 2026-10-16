using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Web.Models;
using System.Diagnostics.CodeAnalysis;

namespace MovieApp.Web.Components.Modals
{
    public record AddMovie(Movie Movie, MovieContent? Content = null);
    public partial class AddMovieModal : ComponentBase
    {
        [CascadingParameter] 
        BlazoredModalInstance BlazoredModal { get; set; } = default!;
        [BindProperty]
        public string? MovieTitle { get; set; }
        private IBrowserFile? _movieContent;

        private async Task AddMovie()
        {
            AddMovie data = new(MakeMovie(), MakeContent());
            ModalResult res = ModalResult.Ok(data);
            await BlazoredModal.CloseAsync(res);
        }
        
        private Task Close() => BlazoredModal.CancelAsync();

        [MemberNotNull(nameof(_movieContent))]
        private void MovieContentChange(InputFileChangeEventArgs args)
        {
            _movieContent = args.File;
        }

        private Movie MakeMovie() => new Movie()
        {
            Title = MovieTitle ?? "null title"
        };

        private MovieContent? MakeContent()
        {
            if (_movieContent is null) return null;
            return new MovieContent(_movieContent);
        }
    }
}

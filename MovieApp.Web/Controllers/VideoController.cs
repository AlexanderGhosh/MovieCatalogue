using Microsoft.AspNetCore.Mvc;
using MovieApp.Web.Models;

namespace MovieApp.Web.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VideoController(IMovieService movieSvc) : ControllerBase
    {
        private readonly IMovieService _movieSvc = movieSvc;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo(int id)
        {
            var content = await _movieSvc.GetMovieContent(id);
            if (content is null) return NotFound();
            return File(content.Stream, "video/mp4", true);
        }

        [HttpPost("upload/stream")]
        public async Task<IActionResult> UploadVideo([FromForm] IFormFile formFile)
        {
            await UploadVideo(formFile.OpenReadStream());
            return Ok();
        }

        public record UploadedFile(string Path);
        [HttpPost("upload/file")]
        public async Task<IActionResult> UploadVideo([FromBody] UploadedFile formFile)
        {
            var stream = System.IO.File.OpenRead(formFile.Path);
            await UploadVideo(stream);
            return Ok();
        }

        private async Task UploadVideo(Stream stream)
        {
            var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); ;
            var movie = new Movie()
            {
                Title = $"Movie {id}"
            };

            var content = new MovieContent()
            {
                Stream = stream
            };

            await _movieSvc.UploadMovieAsync(movie, content, null);
        }
    }
}

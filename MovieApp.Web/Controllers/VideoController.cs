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
        public async Task<IActionResult> GetVideo(int id, [FromQuery] bool download = false)
        {
            var content = await _movieSvc.GetMovieContent(id);
            if (content is null) return NotFound();
            var stream = content.Stream;

            if (download)
            {
                var movie = await _movieSvc.GetMovie(id);
                var fileName = Path.ChangeExtension(movie!.Title, "mp4");

                Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"; filename*=UTF-8''{Uri.EscapeDataString(fileName)}";
                Response.Headers.XContentTypeOptions = "nosniff";
                Response.Headers.AcceptRanges = "bytes";
            }


            return File(stream, "video/mp4", true);
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

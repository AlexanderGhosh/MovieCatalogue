using Agent.Agentic.Abstractions.Extensions;
using BlobStorage;
using Microsoft.AspNetCore.Mvc;
using MovieApp.Web.Agents;
using MovieApp.Web.Extensions;
using MovieApp.Web.Models;
using MovieApp.Web.Services.Abstractions;

namespace MovieApp.Web.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VideoController(IMovieService movieSvc, IWebHostEnvironment webHost, IServiceProvider serviceProvider) : ControllerBase
    {
        private readonly IMovieService _movieSvc = movieSvc;
        private const string DEFAULT_IMAGE = "missing_thumbnail.png";


        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo(int id, [FromQuery] bool download = false)
        {
            var content = await _movieSvc.GetMovieContent(id);
            if (content is null) return NotFound();

            if (download)
            {
                var movie = await _movieSvc.GetMovie(id);
                var fileName = Path.ChangeExtension(movie!.Title, "mp4");

                Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"; filename*=UTF-8''{Uri.EscapeDataString(fileName)}";
                Response.Headers.XContentTypeOptions = "nosniff";
                Response.Headers.AcceptRanges = "bytes";
            }


            return File(content.Stream, "video/mp4", true);
        }

        public record UploadedFile(string Path, string? Name = null);
        [HttpPost("upload/file")]
        public async Task<IActionResult> UploadVideo([FromBody] UploadedFile formFile)
        {
            var stream = System.IO.File.OpenRead(formFile.Path);
            await UploadVideo(stream, formFile.Name);
            return Ok();
        }

        private async Task UploadVideo(Stream stream, string? name)
        {
            var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); ;
            var movie = new Movie()
            {
                Title = name ?? $"Movie {id}",
                ShowThumbnail = true
            };

            var content = new MovieContent()
            {
                Stream = stream
            };

            await _movieSvc.UploadMovieAsync(movie, content);
        }

        [HttpGet("{videoId}/thumbnail")]
        public async Task<IActionResult> GetVideoThumbnail(int videoId, [FromQuery] bool isCollection = false)
        {
            var thumbnail = isCollection
                ? await _movieSvc.GetCollectionThumbnail(videoId)
                : await _movieSvc.GetMovieThumbnail(videoId);
            if (thumbnail?.Stream?.IsEmptyOrDefault() is true)
                thumbnail = null;
            var stream = thumbnail ?? GetDefaultThumbnail();

            return File(stream.Stream, stream.ContentType);
        }

        [HttpPut("{videoId}/description")]
        public async Task<IActionResult> PutVideoDescription(int videoId, [FromBody] string? description, [FromQuery] bool aiGen = false, [FromQuery] string? name = null)
        {
            if (aiGen)
            {
                name ??= (await _movieSvc.GetMovie(videoId))?.Title;
                if (name is null)
                    return BadRequest("Either the video cannot be found or you did not provide a name");
                var message = $"""
                    movie or tv show name: {name}
                    """;
                var agent = serviceProvider.GetRequiredAgent<VideoDescriptionAgent>();
                var response = await agent.RunAsync(message);
                description ??= response.Text;
            }
            await _movieSvc.UpdateMovie(videoId, m => m.Description = description);
            return Ok();
        }

        private Thumbnail GetDefaultThumbnail()
        {
            var image = webHost.WebRootFileProvider.GetFileInfo(DEFAULT_IMAGE);
            var stream = image.CreateReadStream();
            var contentType = BlobContentTypes.FromExtension(Path.GetExtension(DEFAULT_IMAGE));
            return new(stream, contentType);
        }
    }
}

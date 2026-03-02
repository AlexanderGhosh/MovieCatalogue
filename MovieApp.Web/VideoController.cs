using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Web
{

    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class VideoController(IMovieService movieSvc) : ControllerBase
    {
        private readonly IMovieService _movieSvc = movieSvc;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo(int id)
        {
            var url = await _movieSvc.GetMovieUrlAsync(id);
            if (url is null) return NotFound();
            return PhysicalFile(url, "video/mp4");
        }
    }
}

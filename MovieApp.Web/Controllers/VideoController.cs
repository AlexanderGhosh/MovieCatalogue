using Microsoft.AspNetCore.Mvc;

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
    }
}

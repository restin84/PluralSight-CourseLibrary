using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
  [ApiController]
  [Route("api/authors")]
  public class AuthorsController : ControllerBase
  {
    private readonly ICourseLibraryRepository courseLibraryRepository;

    public AuthorsController(ICourseLibraryRepository courseLibraryRepository) {
      this.courseLibraryRepository = courseLibraryRepository ??
        throw new ArgumentNullException(nameof(courseLibraryRepository));
    }

    [HttpGet]
    public IActionResult GetAuthors() {
      var authorsFromRepo = courseLibraryRepository.GetAuthors();
      return new JsonResult(authorsFromRepo);
    }

    [HttpGet("{authorId}")]
    public IActionResult GetAuthor(Guid authorId) {
      var authorFromRepo = courseLibraryRepository.GetAuthor(authorId);
      return new JsonResult(authorFromRepo);
    }
  }
}

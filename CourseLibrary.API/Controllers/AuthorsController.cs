using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
  [ApiController]
  public class AuthorsController : ControllerBase
  {
    private readonly ICourseLibraryRepository courseLibraryRepository;

    public AuthorsController(ICourseLibraryRepository courseLibraryRepository) {
      this.courseLibraryRepository = courseLibraryRepository ??
        throw new ArgumentNullException(nameof(courseLibraryRepository));
    }

    public IActionResult GetAuthors() {
      var authorsFromRepo = courseLibraryRepository.GetAuthors();
      return new JsonResult(authorsFromRepo);

    }
  }
}

using AutoMapper;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Controllers
{
  [ApiController]
  [Route("api/authors/{authorId}/courses")]
  public class CoursesController : Controller
  {
    private readonly IMapper mapper;
    private readonly ICourseLibraryRepository courseLibraryRepository;
    
    public CoursesController(ICourseLibraryRepository courseLibraryRepository,
      IMapper mapper) {
      this.mapper = mapper ??
        throw new ArgumentNullException(nameof(mapper));
      this.courseLibraryRepository = courseLibraryRepository ??
        throw new ArgumentNullException(nameof(courseLibraryRepository));
    }

    [HttpGet] //authorId will automatically be filled with the Guid from the URI
    [HttpHead]
    public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId) {
      if (!courseLibraryRepository. AuthorExists(authorId)) {
        return NotFound();
      }

      var coursesForAuthorFromRepo = courseLibraryRepository.GetCourses(authorId);

      //return the courses from the repo mapped to an IEnumerable of CourseDtos
      return Ok(mapper.Map<IEnumerable<CourseDto>>(coursesForAuthorFromRepo));
    }

    [HttpGet("{courseId}")]
    [HttpHead]
    public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId) {
      if (!courseLibraryRepository.AuthorExists(authorId)) {
        return NotFound();
      }

      var courseForAuthorFromRepo = courseLibraryRepository.GetCourse(authorId, courseId);

      if (courseForAuthorFromRepo == null) {
        return NotFound();
      }

      return Ok(mapper.Map<CourseDto>(courseForAuthorFromRepo));
    }
  }
}

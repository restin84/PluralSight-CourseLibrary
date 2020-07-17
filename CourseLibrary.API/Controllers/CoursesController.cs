using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

    [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
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

    [HttpPost]
    public ActionResult<CourseDto> CreateCourseForAuthor(
      Guid authorId, CourseForCreationDto course) {
      if (!courseLibraryRepository.AuthorExists(authorId)) {
        return NotFound();
      }

      var courseEntity = mapper.Map<Entities.Course>(course);
      courseLibraryRepository.AddCourse(authorId, courseEntity);
      courseLibraryRepository.Save();

      var courseToReturn = mapper.Map<CourseDto>(courseEntity);
      return CreatedAtRoute(
        "GetCourseForAuthor",
        new { authorId = authorId, courseId = courseToReturn.Id }, 
        courseToReturn);
    }

    [HttpPut("{courseId}")]
    public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto course) {
      if (!courseLibraryRepository.AuthorExists(authorId)) {
        return NotFound();
      }

      var courseForAuthorFromRepo = courseLibraryRepository.GetCourse(authorId, courseId);

      if (courseForAuthorFromRepo == null) {
        var courseToAdd = mapper.Map<Course>(course);
        courseToAdd.Id = courseId;

        courseLibraryRepository.AddCourse(authorId, courseToAdd);

        courseLibraryRepository.Save();

        var courseToReturn = mapper.Map<CourseDto>(courseToAdd);

        return CreatedAtRoute("GetCourseForAuthor",
          new { authorId, courseId = courseToReturn.Id },
          courseToReturn);
      }

      mapper.Map(course, courseForAuthorFromRepo);

      courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

      courseLibraryRepository.Save();

      return NoContent();
    }

    [HttpPatch("{courseId}")]
    public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId,
      Guid courseId, 
      JsonPatchDocument<CourseForUpdateDto> patchDocument) {

      if (!courseLibraryRepository.AuthorExists(authorId)) {
        return NotFound();
      }

      var courseForAuthorFromRepo = courseLibraryRepository.GetCourse(authorId, courseId);

      if (courseForAuthorFromRepo == null) {
        var courseDto = new CourseForUpdateDto();
        patchDocument.ApplyTo(courseDto, ModelState);

        if (!TryValidateModel(courseDto)) {
          return ValidationProblem(ModelState);
        }
        var courseToAdd = mapper.Map<Course>(courseDto);
        courseToAdd.Id = courseId;

        courseLibraryRepository.AddCourse(authorId, courseToAdd);
        courseLibraryRepository.Save();

        var courseToReturn = mapper.Map<CourseDto>(courseToAdd);

        return CreatedAtRoute("GetCourseForAuthor",
          new { authorId, courseId = courseToReturn.Id },
          courseToReturn);
      }

      var courseToPatch = mapper.Map<CourseForUpdateDto>(courseForAuthorFromRepo);

      //add validation
      //because we are passing the ModelState
      patchDocument.ApplyTo(courseToPatch, ModelState);

      if (!TryValidateModel(courseToPatch)) {
        return ValidationProblem(ModelState);
      }

      mapper.Map(courseToPatch, courseForAuthorFromRepo);

      courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

      courseLibraryRepository.Save();

      return NoContent();
    }

    public override ActionResult ValidationProblem(
      [ActionResultObjectValue] ModelStateDictionary modelStateDictionary) {
      var options = HttpContext.RequestServices
        .GetRequiredService<IOptions<ApiBehaviorOptions>>();
      return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
    }
  }
}

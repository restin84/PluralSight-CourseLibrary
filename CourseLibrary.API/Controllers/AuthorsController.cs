using AutoMapper;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
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
    private readonly IMapper mapper;
    private readonly ICourseLibraryRepository courseLibraryRepository;

    public AuthorsController(ICourseLibraryRepository courseLibraryRepository,
      IMapper mapper) {
      this.mapper = mapper ?? 
        throw new ArgumentException(nameof(mapper));
      this.courseLibraryRepository = courseLibraryRepository ??
        throw new ArgumentNullException(nameof(courseLibraryRepository));
    }

    [HttpGet]
    [HttpHead]
    public ActionResult<IEnumerable<AuthorDto>> GetAuthors(
      [FromQuery]AuthorsResourceParameters authorsResourceParameters) {
      var authorsFromRepo = courseLibraryRepository.GetAuthors(authorsResourceParameters);
      //the IMapper instance knows how to do this mapping because of 
      //this AuthorsProfile 
      return Ok(mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
    }

    [HttpGet("{authorId}", Name = "GetAuthor")]
    [HttpHead]
    public ActionResult<AuthorDto> GetAuthor(Guid authorId) {
      var authorFromRepo = courseLibraryRepository.GetAuthor(authorId);

      if (authorFromRepo == null) {
        return NotFound();
      }

      return Ok(mapper.Map<AuthorDto>(authorFromRepo));
    }

    [HttpPost]
    public ActionResult<AuthorDto> CreateAuthor(AuthorForCreationDto author) {
      var authorEntity = mapper.Map<Entities.Author>(author);
      courseLibraryRepository.AddAuthor(authorEntity);
      courseLibraryRepository.Save();

      var authorToReturn = mapper.Map<AuthorDto>(authorEntity);
      //the first parameter refers to the route with the name 
      //of 'GetAuthor'
      return CreatedAtRoute("GetAuthor",
        new { authorId = authorToReturn.Id },
        authorToReturn
        );
    }

    [HttpOptions]
    public IActionResult GetAuthorsOptions() {
      Response.Headers.Add("Allow", "GET,OPTIONS,POST");
      return Ok();
    }
  }
}

using AutoMapper;
using CourseLibrary.API.Helpers;
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
      string mainCategory, string searchQuery) {
      var authorsFromRepo = courseLibraryRepository.GetAuthors(mainCategory, searchQuery);
      //the IMapper instance knows how to do this mapping because of 
      //this AuthorsProfile 
      return Ok(mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo));
    }

    [HttpGet("{authorId}")]
    [HttpHead]
    public ActionResult<AuthorDto> GetAuthor(Guid authorId) {
      var authorFromRepo = courseLibraryRepository.GetAuthor(authorId);

      if (authorFromRepo == null) {
        return NotFound();
      }

      return Ok(mapper.Map<AuthorDto>(authorFromRepo));
    }
  }
}

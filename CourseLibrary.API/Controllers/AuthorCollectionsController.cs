using AutoMapper;
using CourseLibrary.API.Entities;
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
  [Route("api/authorcollections")]
  public class AuthorCollectionsController : ControllerBase
  {
    private readonly ICourseLibraryRepository courseLibraryRepository;
    private readonly IMapper mapper;

    public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper) {
      this.courseLibraryRepository = courseLibraryRepository ??
        throw new ArgumentNullException(nameof(courseLibraryRepository));
      this.mapper = mapper ??
        throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPost]
    public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(
      IEnumerable<AuthorForCreationDto> authorCollection) {

      var authorEntities = mapper.Map<IEnumerable<Author>>(authorCollection);

      foreach(var author in authorEntities) {
        courseLibraryRepository.AddAuthor(author);
      }

      courseLibraryRepository.Save();

      return Ok();
    }
  }
}

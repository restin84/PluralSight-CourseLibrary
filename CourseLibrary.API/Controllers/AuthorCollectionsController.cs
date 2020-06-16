using AutoMapper;
using CourseLibrary.API.Entities;
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

    [HttpGet("({ids})", Name = "GetAuthorCollection")]
    public ActionResult GetAuthorCollection(
      [FromRoute] //the ids will come from the route
      [ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids) {

      if (ids == null) {
        return BadRequest();
      }

      var authorEntities = courseLibraryRepository.GetAuthors(ids);

      if (ids.Count() != authorEntities.Count()) { //invalid key because some of the ids are invalid
        return NotFound();
      }

      var authorsToReturn = mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

      return Ok(authorsToReturn);
    }

    [HttpPost]
    public ActionResult<IEnumerable<AuthorDto>> CreateAuthorCollection(
      IEnumerable<AuthorForCreationDto> authorCollection) {

      var authorEntities = mapper.Map<IEnumerable<Author>>(authorCollection);

      foreach(var author in authorEntities) {
        courseLibraryRepository.AddAuthor(author);
      }

      courseLibraryRepository.Save();

      var authorCollectionToReturn = mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
      var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

      return CreatedAtRoute("GetAuthorCollection",
        new { ids = idsAsString },
        authorCollectionToReturn);
    }
  }
}

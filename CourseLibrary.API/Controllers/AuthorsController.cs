using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {

        private readonly ICourseLibraryRepository _courseLibraryRepository;
        public AuthorsController(ICourseLibraryRepository courseLibraryRepository)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authors = _courseLibraryRepository.GetAuthors();
            return Ok(authors); //changed JsonResult() to Ok() Method 
        }

        [HttpGet("{authorId}")] // or [HttpGet("{authorId:guid}")] Os dois pontos seguidos do tipo (guid) é para eliminar ambiguidades, ou seja, só irá aceitar objetos que podem ser convertidos para o tipo gui
        public IActionResult GetAuthor(Guid authorId)
        {
            var author = _courseLibraryRepository.GetAuthor(authorId);
            if(author == null) return NotFound();
            return Ok(author); //changed JsonResult() to Ok() Method 
        }
    }
}

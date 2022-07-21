using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using CourseLibrary.API.Helpers;

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
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors() //Always Use ActionResult<T> instead of IActionResult if is possible.
        {
            var authors = _courseLibraryRepository.GetAuthors();
            var authorsDto = new List<AuthorDto>();

            foreach(var author in authors)
            {
                authorsDto.Add(new AuthorDto()
                {
                    Id = author.Id,
                    Name = $"{author.FirstName} {author.LastName}",
                    MainCategory = author.MainCategory,
                    Age = author.DateOfBirth.GetCurrentAge()
                });
            }

            return Ok(authorsDto); //changed JsonResult() to Ok() Method 
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

using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using CourseLibrary.API.Helpers;
using AutoMapper;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {

        private readonly ICourseLibraryRepository _courseLibraryRepository;

        private readonly IMapper _mapper;
        public AuthorsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet()]
        [HttpHead] // This will make the code be executed but without fill the response body. Used to get API information.
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors(string mainCategory) //Always Use ActionResult<T> instead of IActionResult if is possible. 
            // To implemente QueryParams we can use the same property name for the function param. or can use ([FromQuery] string mainCategory). If the name is the same
            // as the property we can ommit the '[FromQuery]'
        {
            var authors = _courseLibraryRepository.GetAuthors(mainCategory);
                        

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors)); //changed JsonResult() to Ok() Method. Used AutoMapper
        }

        [HttpGet("{authorId}")] // or [HttpGet("{authorId:guid}")] Os dois pontos seguidos do tipo (guid) é para eliminar ambiguidades, ou seja, só irá aceitar objetos que podem ser convertidos para o tipo gui
        public IActionResult GetAuthor(Guid authorId)
        {
            var author = _courseLibraryRepository.GetAuthor(authorId);
            if(author == null) return NotFound();
            return Ok(_mapper.Map<AuthorDto>(author)); //changed JsonResult() to Ok() Method 
        }
    }
}

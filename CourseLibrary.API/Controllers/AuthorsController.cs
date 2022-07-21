using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using CourseLibrary.API.Helpers;
using AutoMapper;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Entities;

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
        public ActionResult<IEnumerable<AuthorDto>> GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParams) //Always Use ActionResult<T> instead of IActionResult if is possible. 
            // To implemente QueryParams we can use the same property name for the function param. or can use ([FromQuery] string mainCategory). If the name is the same
            // as the property we can ommit the '[FromQuery]'
        {
            var authors = _courseLibraryRepository.GetAuthors(authorsResourceParams);
                        

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authors)); //changed JsonResult() to Ok() Method. Used AutoMapper
        }

        [HttpGet("{authorId}", Name ="GetAuthor")] // or [HttpGet("{authorId:guid}")] Os dois pontos seguidos do tipo (guid) é para eliminar ambiguidades, ou seja, só irá aceitar objetos que podem ser convertidos para o tipo gui
        public ActionResult<AuthorDto> GetAuthor(Guid authorId)
        {
            var author = _courseLibraryRepository.GetAuthor(authorId);
            if(author == null) return NotFound();
            return Ok(_mapper.Map<AuthorDto>(author)); //changed JsonResult() to Ok() Method 
        }

        [HttpPost]
        public ActionResult<AuthorDto> CreatAuthor(AuthorForCreationDto author)
        {
            // if (author == null) return BadRequest(); No need this anymore because of the [ApiController] attribute
            var authorEntity = _mapper.Map<Author>(author);

            _courseLibraryRepository.AddAuthor(authorEntity); //Added to DbContext

            _courseLibraryRepository.Save(); //Actually saves on the Database;

            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
             
            // CreateAtRoute( Route to refer to a single Resource, id of the the resource, value to response body) -> This Create a URI !
            return CreatedAtRoute("GetAuthor", new {authorId = authorToReturn.Id }, authorToReturn);

        }
       
    }
}

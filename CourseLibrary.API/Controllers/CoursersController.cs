using AutoMapper;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursersController: ControllerBase
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        private readonly IMapper _mapper;

        public CoursersController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _courseLibraryRepository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CourseDto>> GetCoursesForAuthor(Guid authorId)
        {
            var courses = _courseLibraryRepository.GetCourses(authorId);
            if (courses.Count() == 0) return NotFound();
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));

        }

        [HttpGet("{courseId}", Name ="GetCourseForAuthor")]
        public ActionResult<CourseDto> GetCourseForAuthor(Guid authorId, Guid courseId)
        {
            var course = _courseLibraryRepository.GetCourse(authorId, courseId);
            if (course == null) return NotFound();
            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpPost]
        public ActionResult<CourseDto> CreateCourseForAuthor(Guid authorId, CourseForCreationDto course)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();

            var courseEntity = _mapper.Map<Course>(course);

            _courseLibraryRepository.AddCourse(authorId, courseEntity);
            _courseLibraryRepository.Save();

            var courseToReturn = _mapper.Map<CourseDto>(courseEntity);
            return CreatedAtRoute(
                "GetCourseForAuthor",
                new { authorId = authorId ,courseId = courseToReturn.Id },
                courseToReturn
                );
        }

        [HttpPut("{courseId}")]
        public IActionResult UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdateDto course)
        {

            if (!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();

            var courseForAuthorFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (courseForAuthorFromRepo == null) // Support for UpInsert
            {
                var courseToAdd = _mapper.Map<Course>(course);
                courseToAdd.Id = courseId;

                _courseLibraryRepository.AddCourse(authorId, courseToAdd);

                _courseLibraryRepository.Save();

                var courseToReturn = _mapper.Map<CourseDto>(courseToAdd);

                return CreatedAtRoute(
                    "GetCourseForAuthor",
                    new { authorId = authorId, courseId = courseToReturn.Id },
                     courseToReturn);
            }

            // We must not use the same dto for creation to update even if this models have the same properties. We do this because the same rules applied for POST
            // are not he same for PUT

            // map the entity to a CourseForUpdateDto
            // apply the updated field values to that dto
            // map the CourseForUpdateDto back to an Entity

            _mapper.Map(course, courseForAuthorFromRepo);

            _courseLibraryRepository.UpdateCourse(courseForAuthorFromRepo);

            _courseLibraryRepository.Save();

            return NoContent();


        }

        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId, JsonPatchDocument<CourseForUpdateDto> patchDocument)
        {
            if (!_courseLibraryRepository.AuthorExists(authorId)) return NotFound();

            var coursesFromAuthorFromRepo = _courseLibraryRepository.GetCourse(authorId, courseId);

            if (coursesFromAuthorFromRepo == null) return NotFound();

            var courseToPatch = _mapper.Map<CourseForUpdateDto>(coursesFromAuthorFromRepo);

            // add validation
            patchDocument.ApplyTo(courseToPatch);

            _mapper.Map(courseToPatch, coursesFromAuthorFromRepo); // mapping to an Entity again

            _courseLibraryRepository.UpdateCourse(coursesFromAuthorFromRepo);

            _courseLibraryRepository.Save();

            return NoContent();



        }
    }
}

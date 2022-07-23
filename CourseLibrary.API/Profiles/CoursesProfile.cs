using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Profiles
{
    public class CoursesProfile: Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, CourseDto>();

            CreateMap<CourseForCreationDto, Course>();

            CreateMap<CourseForUpdateDto, Course>();

            CreateMap<Course, CourseForUpdateDto>();
        }
    }
}

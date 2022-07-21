using AutoMapper;
using CourseLibrary.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class AuthorsProfile: Profile
    {
        public AuthorsProfile()
        {
            //CreateMap<sourceObject, destinationObject>()
            CreateMap<Entities.Author, Models.AuthorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}")) //This is called Projection
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()));
        }
    }
}

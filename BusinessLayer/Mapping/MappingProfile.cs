using AutoMapper;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddressBookEntity, AddressBookDTO>().ReverseMap();

            // UserDTO to UserEntity Mapping
             CreateMap<UserDTO, UserEntity>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)); // Map Password to PasswordHash
        }
    }
    }


using AutoMapper;
using BooksBase.DataAccess;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Web.Constants;
using Web.Permissions;
using Web.Resources;

namespace Web.Controllers.Users
{
    [ApiController]
    public class GetUser : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NSwag.Annotations.OpenApiTag(CoreConstants.Users)]
        [Authorize(CorePermissions.DisplayUsers)]
        [HttpGet("users/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new Query { Id = id });
            return result.Process();
        }

        public class Query : IRequest<Result<UserDto>>
        {
            public Guid Id { get; set; }
        }

        public class UserDto
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<RoleDto> Roles { get; set; }
        }

        public class RoleDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<UserDto>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<UserDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _db.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
                if(user == null)
                {
                    return Result.Error<UserDto>(Resource.UserNotFound);
                }
                var result = _mapper.Map<UserDto>(user);

                return Result.Ok(result);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<User, UserDto>()
                    .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles));
                CreateMap<UserRole, RoleDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RoleId))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name));
            }
        }
    }
}

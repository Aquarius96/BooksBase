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

namespace Web.Controllers.Roles
{
    [ApiController]
    public class GetRole : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetRole(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NSwag.Annotations.OpenApiTag(CoreConstants.Roles)]
        [Authorize(CorePermissions.DisplayRoles)]
        [HttpGet("roles/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new Query { Id = id });
            return result.Process();
        }

        public class Query : IRequest<Result<RoleDto>>
        {
            public Guid Id { get; set; }
        }

        public class RoleDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public List<string> Claims { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<RoleDto>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }
            public async Task<Result<RoleDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var role = await _db.Roles
                    .Include(r => r.RoleClaims)
                    .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
                if(role == null)
                {
                    return Result.Error<RoleDto>(Resource.RoleNotFound);
                }

                var result = _mapper.Map<RoleDto>(role);
                return Result.Ok(result);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Role, RoleDto>()
                    .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.RoleClaims));
                CreateMap<RoleClaim, string>()
                    .ConvertUsing(claim => claim.ClaimValue);
            }
        }
    }
}

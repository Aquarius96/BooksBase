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
using Web.Permissions;

namespace Web.Controllers.Roles
{
    [ApiController]
    public class GetRoles : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetRoles(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(CorePermissions.DisplayRoles)]
        [HttpGet("roles")]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new Query());
            return result.Process();
        }

        public class Query : IRequest<Result<List<RoleDto>>>
        {
        }

        public class RoleDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<RoleDto>>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<RoleDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var roles = _db.Roles;
                var result = await _mapper.ProjectTo<RoleDto>(roles)
                    .ToListAsync(cancellationToken);

                return Result.Ok(result);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Role, RoleDto>();
            }
        }
    }
}

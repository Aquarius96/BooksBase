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

namespace Web.Controllers.Users
{
    [ApiController]
    public class GetUsers : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetUsers(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new Query());
            return result.Process();
        }

        public class Query : IRequest<Result<List<UserDto>>>
        {
        }

        public class UserDto
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserDto>>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<UserDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = _db.Users;
                var result = await _mapper.ProjectTo<UserDto>(users)
                    .ToListAsync(cancellationToken);

                return Result.Ok(result);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<User, UserDto>();
            }
        }
    }
}

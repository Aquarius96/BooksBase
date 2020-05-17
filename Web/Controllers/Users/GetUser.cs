using AutoMapper;
using BooksBase.DataAccess;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
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
                var user = await _db.Users.FindAsync(request.Id);
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
                CreateMap<User, UserDto>();
            }
        }
    }
}

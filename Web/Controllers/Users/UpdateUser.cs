using AutoMapper;
using AutoMapper.EquivalencyExpression;
using BooksBase.DataAccess;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using FluentValidation;
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
    public class UpdateUser : ControllerBase
    {
        private readonly IMediator _mediator;

        public UpdateUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NSwag.Annotations.OpenApiTag(CoreConstants.Users)]
        [Authorize(CorePermissions.ManageUsers)]
        [HttpPut("users/{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateUserCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return result.Process();
        }

        public class UpdateUserCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<Guid> Roles { get; set; }
        }

        public class Handler : IRequestHandler<UpdateUserCommand, Result>
        {
            private readonly DataContext _db;            
            private readonly IMapper _mapper;

            public Handler(DataContext db,                
                IMapper mapper)
            {
                _db = db;                
                _mapper = mapper;
            }

            public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _db.Users
                    .Include(u => u.UserRoles)
                    .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
                if(user == null)
                {
                    return Result.Error(Resource.UserNotFound);
                }

                _mapper.Map(request, user);
                await _db.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateUserCommand, User>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.Roles));
                CreateMap<Guid, UserRole>()
                    .EqualityComparison((id, role) => role.RoleId == id)
                    .ConvertUsing(id => new UserRole { RoleId = id });
            }
        }

        public class Validator : AbstractValidator<UpdateUserCommand>
        {
            public Validator()
            {
                RuleFor(e => e.Email)
                    .EmailAddress()
                    .NotEmpty();                
                RuleFor(e => e.FirstName)
                    .NotEmpty();
                RuleFor(e => e.LastName)
                    .NotEmpty();
            }
        }
    }
}

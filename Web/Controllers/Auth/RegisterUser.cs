using AutoMapper;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Controllers.Auth
{
    [ApiController]
    public class RegisterUser : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegisterUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("accounts/register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Process();
        }

        public class RegisterCommand : IRequest<Result>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Handler : IRequestHandler<RegisterCommand, Result>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager,
                IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var user = _mapper.Map<User>(request);

                var result = await _userManager.CreateAsync(user, request.Password);
                if(result.Succeeded == false)
                {
                    return Result.Error(result.Errors);
                }

                return Result.Ok();
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<RegisterCommand, User>();
            }
        }

        public class Validator : AbstractValidator<RegisterCommand>
        {
            public Validator()
            {
                RuleFor(e => e.Email)
                    .EmailAddress()
                    .NotEmpty();
                RuleFor(e => e.Password)
                    .NotEmpty();
            }
        }
    }
}

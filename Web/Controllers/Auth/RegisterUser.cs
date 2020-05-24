using AutoMapper;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Web.Constants;

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

        [NSwag.Annotations.OpenApiTag(CoreConstants.Accounts)]
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
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Handler : IRequestHandler<RegisterCommand, Result>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;
            private readonly IEmailService _emailService;

            public Handler(UserManager<User> userManager,
                IMapper mapper,
                IEmailService emailService)
            {
                _userManager = userManager;
                _mapper = mapper;
                _emailService = emailService;
            }

            public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                var user = _mapper.Map<User>(request);

                var result = await _userManager.CreateAsync(user, request.Password);
                if(result.Succeeded == false)
                {
                    return Result.Error(result.Errors);
                }

                await _userManager.AddToRoleAsync(user, "User");
                await _emailService.SendAsync("Register", new RecipientData
                {
                    Email = user.Email,
                    Name = user.UserName
                }, new { }, cancellationToken);

                return Result.Ok();
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<RegisterCommand, User>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
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
                RuleFor(e => e.FirstName)
                    .NotEmpty();
                RuleFor(e => e.LastName)
                    .NotEmpty();
            }
        }
    }
}

using BooksBase.Models.Auth;
using BooksBase.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Controllers.Auth
{
    [ApiController]
    public class LoginUser : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoginUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("accounts/login")]
        public async Task<IActionResult> Login(Query query)
        {
            var result = await _mediator.Send(query);
            if (result.Success)
            {
                return result.Process();
            }

            return Unauthorized(result);
        }

        public class Query : IRequest<Result<UserTokenInfo>>
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<UserTokenInfo>>
        {
            private readonly UserManager<User> _userManager;
            private readonly ILoginService _loginService;

            public Handler(UserManager<User> userManager,
                ILoginService loginService)
            {
                _userManager = userManager;
                _loginService = loginService;
            }

            public async Task<Result<UserTokenInfo>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(request.Email);

                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(request.Email);
                }

                if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    var token = _loginService.GetToken(user);
                    return Result.Ok(token);
                }

                return Result.Error<UserTokenInfo>("Unauthorized");
            }
        }
    }
}

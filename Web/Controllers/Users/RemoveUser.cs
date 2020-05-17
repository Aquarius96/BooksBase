using BooksBase.DataAccess;
using BooksBase.Models.Auth;
using BooksBase.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Permissions;
using Web.Resources;

namespace Web.Controllers.Users
{
    [ApiController]
    public class RemoveUser : ControllerBase
    {
        private readonly IMediator _mediator;

        public RemoveUser(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(CorePermissions.ManageUsers)]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await _mediator.Send(new RemoveUserCommand { Id = id });
            return result.Process();
        }

        public class RemoveUserCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<RemoveUserCommand, Result>
        {
            private readonly DataContext _db;

            public Handler(DataContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _db.Users.FindAsync(request.Id);
                if(user == null)
                {
                    return Result.Error(Resource.UserNotFound);
                }

                _db.Users.Remove(user);
                await _db.SaveChangesAsync(cancellationToken);
                
                return Result.Ok();
            }
        }
    }
}

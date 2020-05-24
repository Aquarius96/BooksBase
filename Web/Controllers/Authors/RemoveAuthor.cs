using BooksBase.DataAccess;
using BooksBase.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Constants;
using Web.Permissions;
using Web.Resources;

namespace Web.Controllers.Authors
{
    [ApiController]
    public class RemoveAuthor : ControllerBase
    {
        private readonly IMediator _mediator;

        public RemoveAuthor(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NSwag.Annotations.OpenApiTag(CoreConstants.Authors)]
        [Authorize(CorePermissions.ManageAuthors)]
        [HttpDelete("authors/{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await _mediator.Send(new RemoveAuthorCommand { Id = id });
            return result.Process();
        }

        public class RemoveAuthorCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<RemoveAuthorCommand, Result>
        {
            private readonly DataContext _db;

            public Handler(DataContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(RemoveAuthorCommand request, CancellationToken cancellationToken)
            {
                var author = await _db.Authors.FindAsync(request.Id);
                if(author == null)
                {
                    return Result.Error(Resource.AuthorNotFound);
                }

                _db.Remove(author);
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
        }
    }
}

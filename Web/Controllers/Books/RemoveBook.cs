using BooksBase.DataAccess;
using BooksBase.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Permissions;
using Web.Resources;

namespace Web.Controllers.Books
{
    [ApiController]
    public class RemoveBook : ControllerBase
    {
        private readonly IMediator _mediator;

        public RemoveBook(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(CorePermissions.ManageBooks)]
        [HttpDelete("books/{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await _mediator.Send(new RemoveBookCommand { Id = id });
            return result.Process();
        }

        public class RemoveBookCommand : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<RemoveBookCommand, Result>
        {
            private readonly DataContext _db;

            public Handler(DataContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(RemoveBookCommand request, CancellationToken cancellationToken)
            {
                var book = await _db.Books.FindAsync(request.Id);
                if(book == null)
                {
                    return Result.Error(Resource.BookNotFound);
                }

                _db.Remove(book);
                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok();
            }
        }
    }
}

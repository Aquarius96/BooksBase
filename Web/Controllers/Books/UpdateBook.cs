using AutoMapper;
using BooksBase.DataAccess;
using BooksBase.Models;
using BooksBase.Shared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Constants;
using Web.Permissions;
using Web.Resources;

namespace Web.Controllers.Books
{
    [ApiController]
    public class UpdateBook : ControllerBase
    {
        private readonly IMediator _mediator;

        public UpdateBook(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NSwag.Annotations.OpenApiTag(CoreConstants.Books)]
        [Authorize(CorePermissions.ManageBooks)]
        [HttpPost("books")]
        public async Task<IActionResult> Add(UpdateBookCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Process();
        }

        [Authorize]
        [HttpPut("books/{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateBookCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return result.Process();
        }

        public class UpdateBookCommand : IRequest<Result<Guid>>
        {
            public Guid? Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Genre { get; set; }
            public Guid AuthorId { get; set; }
        }

        public class Handler : IRequestHandler<UpdateBookCommand, Result<Guid>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
            {
                Book book;
                if(request.Id == null)
                {
                    book = _mapper.Map<Book>(request);
                    await _db.AddAsync(book, cancellationToken);
                }
                else
                {
                    book = await _db.Books.FindAsync(request.Id);
                    if(book == null)
                    {
                        return Result.Error<Guid>(Resource.BookNotFound);
                    }
                    _mapper.Map(request, book);
                }

                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok(book.Id);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateBookCommand, Book>();
            }
        }

        public class Validator : AbstractValidator<UpdateBookCommand>
        {
            public Validator(DataContext db)
            {
                RuleFor(e => e.Title)
                    .NotEmpty();
                RuleFor(e => e.Description)
                    .NotEmpty();
                RuleFor(e => e.Genre)
                    .NotEmpty();
                RuleFor(e => e.AuthorId)
                    .NotEmpty()
                    .MustAsync(async (authorId, cancellationToken) => await db.Authors.AnyAsync(author => author.Id == authorId, cancellationToken))
                    .WithMessage(Resource.AuthorNotFound);
            }
        }
    }
}

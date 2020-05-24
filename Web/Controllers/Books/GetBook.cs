using AutoMapper;
using BooksBase.DataAccess;
using BooksBase.Models;
using BooksBase.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Constants;
using Web.Permissions;
using Web.Resources;

namespace Web.Controllers.Books
{
    [ApiController]
    public class GetBook : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetBook(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NSwag.Annotations.OpenApiTag(CoreConstants.Books)]
        [Authorize(CorePermissions.DisplayBooks)]
        [HttpGet("books/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new Query { Id = id });
            return result.Process();
        }

        public class Query : IRequest<Result<BookDto>>
        {
            public Guid Id { get; set; }
        }

        public class BookDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Genre { get; set; }
            public Guid AuthorId { get; set; }
            public AuthorDto Author { get; set; }
        }

        public class AuthorDto
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<BookDto>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<BookDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var book = await _db.Books
                    .Include(b => b.Author)
                    .FirstOrDefaultAsync(b => b.Id == request.Id);
                if(book == null)
                {
                    return Result.Error<BookDto>(Resource.BookNotFound);
                }

                var result = _mapper.Map<BookDto>(book);
                return Result.Ok(result);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Book, BookDto>();
                CreateMap<Author, AuthorDto>();
            }
        }
    }
}

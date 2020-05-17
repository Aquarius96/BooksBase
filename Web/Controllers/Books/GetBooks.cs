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
using System.Threading;
using System.Threading.Tasks;
using Web.Permissions;

namespace Web.Controllers.Books
{
    [ApiController]
    public class GetBooks : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetBooks(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(CorePermissions.DisplayBooks)]
        [HttpGet("books")]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new Query());
            return result.Process();
        }

        public class Query : IRequest<Result<List<BookDto>>>
        {
        }

        public class BookDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Genre { get; set; }
            public Guid AuthorId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<BookDto>>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<BookDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var books = _db.Books;
                var result = await _mapper.ProjectTo<BookDto>(books)
                    .ToListAsync(cancellationToken);

                return Result.Ok(result);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Book, BookDto>();
            }
        }
    }
}

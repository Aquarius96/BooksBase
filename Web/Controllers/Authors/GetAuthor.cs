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
using Web.Constants;
using Web.Permissions;
using Web.Resources;

namespace Web.Controllers.Authors
{
    [ApiController]
    public class GetAuthor : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetAuthor(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NSwag.Annotations.OpenApiTag(CoreConstants.Authors)]
        [Authorize(CorePermissions.DisplayAuthors)]
        [HttpGet("authors/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new Query { Id = id });
            return result.Process();
        }

        public class Query : IRequest<Result<AuthorDto>>
        {
            public Guid Id { get; set; }
        }

        public class AuthorDto
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<BookDto> Books { get; set; }
        }

        public class BookDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<AuthorDto>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<AuthorDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var author = await _db.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
                if(author == null)
                {
                    return Result.Error<AuthorDto>(Resource.AuthorNotFound);
                }

                var result = _mapper.Map<AuthorDto>(author);
                return Result.Ok(result);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Author, AuthorDto>();
                CreateMap<Book, BookDto>();
            }
        }
    }
}

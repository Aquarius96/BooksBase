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

namespace Web.Controllers.Authors
{
    [ApiController]
    public class GetAuthors : ControllerBase
    {
        private readonly IMediator _mediator;

        public GetAuthors(IMediator mediator)
        {
            _mediator = mediator;
        }

        [NSwag.Annotations.OpenApiTag(CoreConstants.Authors)]
        [Authorize(CorePermissions.DisplayAuthors)]
        [HttpGet("authors")]
        public async Task<IActionResult> Get()
        {
            var result = await _mediator.Send(new Query());
            return result.Process();
        }

        public class Query : IRequest<Result<List<AuthorDto>>>
        {
        }

        public class AuthorDto
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<AuthorDto>>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<List<AuthorDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var authors = _db.Authors;
                var result = await _mapper.ProjectTo<AuthorDto>(authors)
                    .ToListAsync(cancellationToken);

                return Result.Ok(result);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Author, AuthorDto>();
            }
        }
    }
}

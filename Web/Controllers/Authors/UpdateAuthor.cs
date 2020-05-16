using AutoMapper;
using BooksBase.DataAccess;
using BooksBase.Models;
using BooksBase.Shared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Resources;

namespace Web.Controllers.Authors
{
    [ApiController]
    public class UpdateAuthor : ControllerBase
    {
        private readonly IMediator _mediator;

        public UpdateAuthor(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("authors")]
        public async Task<IActionResult> Add(UpdateAuthorCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Process();
        }

        [Authorize]
        [HttpPut("authors/{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateAuthorCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return result.Process();
        }

        public class UpdateAuthorCommand : IRequest<Result<Guid>>
        {
            public Guid? Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Handler : IRequestHandler<UpdateAuthorCommand, Result<Guid>>
        {
            private readonly DataContext _db;
            private readonly IMapper _mapper;

            public Handler(DataContext db,
                IMapper mapper)
            {
                _db = db;
                _mapper = mapper;
            }

            public async Task<Result<Guid>> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
            {
                Author author;
                if (request.Id == null)
                {
                    author = _mapper.Map<Author>(request);
                    await _db.AddAsync(author, cancellationToken);
                }
                else
                {
                    author = await _db.Authors.FindAsync(request.Id);
                    if(author == null)
                    {
                        return Result.Error<Guid>(Resource.AuthorNotFound);
                    }
                    _mapper.Map(request, author);
                }

                await _db.SaveChangesAsync(cancellationToken);
                return Result.Ok(author.Id);
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<UpdateAuthorCommand, Author>();
            }
        }

        public class Validator : AbstractValidator<UpdateAuthorCommand>
        {
            public Validator()
            {
                RuleFor(e => e.FirstName)
                    .NotEmpty();
                RuleFor(e => e.LastName)
                    .NotEmpty();
            }
        }
    }
}

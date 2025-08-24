using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Activities.DTOs;
using Application.Core;
using Application.interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Command
{
    public class AddComment
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public required string Body { get; set; }
            public string ActivityId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private readonly AppDbContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;

            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .Include(x => x.Comments)
                    .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);
                if (activity == null) return Result<CommentDto>.Failure("could find activity", 400);
                var user =  await _userAccessor.GetUserAsync();
                var comment = new Comment
                {
                    UserId = user.Id,
                    ActivityId = activity.Id,
                    Body = request.Body

                };
                activity.Comments.Add(comment);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<CommentDto>.Failure("Failed to add Comment", 400);
                return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));
            }
        }
    }

}

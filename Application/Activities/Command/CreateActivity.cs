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
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Command
{
    public class CreateActivity
    {
        public class Command : IRequest<Result<string>>
        {
            public required CreateActivityDto createActivityDto { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<string>>
        {
            private readonly AppDbContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(AppDbContext context, IMapper mapper,IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;

            }

            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userAccessor.GetUserAsync();
                var activity = _mapper.Map<Activity>(request.createActivityDto);
                
                _context.Activities.Add(activity);
                var attendee = new ActivityAttendee()
                {
                    ActivityId = activity.Id,
                    UserId = user.Id,
                    IsHost = true
                };
                activity.Attendees.Add(attendee);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<string>.Failure("Failed to Create Activity", 400);
                return Result<string>.Success(activity.Id);
            }
        }
    }
}

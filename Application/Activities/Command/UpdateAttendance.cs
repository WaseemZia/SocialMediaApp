using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using Application.interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Command
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AppDbContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(AppDbContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .Include(x => x.Attendees).
                    ThenInclude(x => x.User).
                    SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                var user = await _userAccessor.GetUserAsync();
                var attendee = activity.Attendees.FirstOrDefault(x => x.UserId == user.Id);
                var isHost = activity.Attendees.Any(x => x.IsHost && x.UserId == user.Id);
                if (attendee != null)
                {
                    if (isHost)
                    {
                        activity.IsCancelled = !activity.IsCancelled;
                    }
                    else { activity.Attendees.Remove(attendee); }
                }
                else
                {
                    activity.Attendees.Add(new ActivityAttendee()
                    {
                        UserId = user.Id,
                        ActivityId = request.Id,
                        IsHost = false
                    });
                }

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Unit>.Failure("Problem Updating the DB", 400);

                return Result<Unit>.Success(Unit.Value);


            }
        }
    }
}

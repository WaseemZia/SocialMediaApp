using Application.Activities.DTOs;
using Application.Core;
using Application.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries
{
    public class GetActivityList
    {
        

        public class Query : IRequest<Result<PagedList<ActivityDto, DateTime?>>>
        {   
            public required ActivityParams ActivityParams { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto, DateTime?>>>
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

            public async Task<Result<PagedList<ActivityDto, DateTime?>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Activities.OrderBy(x => x.Date)
                    .Where(x=>x.Date>=(request.ActivityParams.Cursor??request.ActivityParams.startDate))
                    .AsQueryable();
                if(!string.IsNullOrEmpty(request.ActivityParams.filter))
                {
                    query = request.ActivityParams.filter switch
                    {
                        "isGoing"=>query.Where(x=>x.Attendees.Any(x=>x.UserId==_userAccessor.GetUserId())),
                        "isHost"=>query.Where(x=>x.Attendees.Any(x=>x.IsHost && x.UserId==_userAccessor.GetUserId())),
                        _=>query
                    };
                }

                var projectActivities = query.ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new
                {
                    CurrentUserId = _userAccessor.GetUserId(),
                });
                var activities = await projectActivities.
                    Take(request.ActivityParams.PageSize + 1)
                .ToListAsync(cancellationToken);
                DateTime? nextCursor = null;
                if (activities.Count > request.ActivityParams.PageSize)
                {
                    nextCursor = activities.Last().Date;
                    activities.RemoveAt(activities.Count - 1);
                }
                return Result<PagedList<ActivityDto, DateTime?>>.Success(
                    new PagedList<ActivityDto, DateTime?>
                    {
                        Items = activities,
                        nextCursor = nextCursor
                    }
                    );
            }
        }
    }
}

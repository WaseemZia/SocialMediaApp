using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Activities.DTOs;
using Application.Core;
using Application.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Persistence;

namespace Application.Activities.Queries
{
    public class GetActivityDetail
    {
        public class Query : IRequest<Result<ActivityDto>>
        {
            public required string Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ActivityDto>>
        {
            private readonly AppDbContext context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(AppDbContext context,IMapper mapper,IUserAccessor userAccessor)
            {
                this.context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public  async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                //var activites = await context.Activities.FindAsync([request.Id],cancellationToken);
                var activites = await context.Activities
                    //.Include(x => x.Attendees)
                    //.ThenInclude(x => x.User)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new
                    {
                        CurrentUserId = _userAccessor.GetUserId(),
                    })
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (activites == null) return Result<ActivityDto>.Failure("Activity Not Found", 404);
                return Result<ActivityDto>.Success(_mapper.Map<ActivityDto>(activites));
            } 
        }
    }
}

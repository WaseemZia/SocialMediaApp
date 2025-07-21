using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using Application.interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries
{
    public class GetProfile
    {
        public class Query : IRequest<Result<UserProfile>>
        {

            public required string UserId { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<UserProfile>>
        {
            private readonly AppDbContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(AppDbContext context, IMapper mapper ,IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<UserProfile>> Handle(Query request, CancellationToken cancellationToken)
            {
                //return await _context.Activities
                //    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider).
                //ToListAsync(cancellationToken);
                var profile= await _context.Users.ProjectTo<UserProfile>(_mapper.ConfigurationProvider,
                new
                {
                        CurrentUserId = _userAccessor.GetUserId(),
                    })
                    .SingleOrDefaultAsync(x=>x.Id==request.UserId,cancellationToken);
                if (profile == null) {return Result<UserProfile>.Failure("Could Find Profile",400); }
                else
                {
                    return Result<UserProfile>.Success(profile);
                }
            }
        }
    }
}

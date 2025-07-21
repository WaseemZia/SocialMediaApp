using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries
{
    public class GetProfilePhotos
    {
        public class Query: IRequest<Result<List<Photo>>>
        {

        public required string UserId { get; set; } }
        public class Handler : IRequestHandler<Query, Result<List<Photo>>>
        {
            private readonly AppDbContext _context;
            private readonly IMapper _mapper;
            public Handler(AppDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<Photo>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //return await _context.Activities
                //    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider).
                //ToListAsync(cancellationToken);
                var photos = await _context.Users.Where(x => x.Id == request.UserId)
                    .SelectMany(x => x.Photos).
                    ToListAsync(cancellationToken);
                return Result<List<Photo>>.Success(photos);
            }
        }
    }
    
}

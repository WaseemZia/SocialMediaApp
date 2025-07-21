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

namespace Application.Activities.Command
{
    public class EditActivity
    {
        public class Command : IRequest<Result<Unit>>
        {
            public EditActivityDto Activitydto { get; set; }
        }
        public class Handler : IRequestHandler<Command ,Result<Unit>>
        {
            private readonly AppDbContext _context;
            private readonly IMapper _mapper;

            public Handler(AppDbContext context,IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync([request.Activitydto.Id], cancellationToken);
                if (activity == null) return Result<Unit>.Failure("Activity Not Found.", 404);
               
                //_mapper.Map(request.Activity, activity);
                //Console.WriteLine(_context.ChangeTracker.DebugView.ShortView);

                //_context.Entry(activity).State = EntityState.Modified;
                Console.WriteLine("Before Mapping: " + activity.Date);

                _mapper.Map(request.Activitydto, activity);
                Console.WriteLine("After Mapping: " + activity.Date);
               var result= await _context.SaveChangesAsync(cancellationToken)>0;
                if (!result) return Result<Unit>.Failure("Activity Can not not be Edited",400);
                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}

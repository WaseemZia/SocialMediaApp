using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Command
{
    public class DeleteActivity
    {
        public class Command :IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }
        public class Handler : IRequestHandler<Command,Result<Unit>>
        {
            private readonly AppDbContext _context;
            public Handler(AppDbContext context)
            {
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync([request.Id], cancellationToken);
                if ((activity == null)) return Result<Unit>.Failure("Activity Not Found", 404);
               

                _context.Remove(activity);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return  Result<Unit>.Failure("Failed to Delete Activity", 400);

                return Result<Unit>.Success(Unit.Value);

                
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using Application.interfaces;
using Application.Interfaces;
using FluentValidation.Results;
using MediatR;
using Persistence;

namespace Application.Profiles.Command
{
    public class SetMainPhoto
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string PhotoId { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {

            private readonly AppDbContext _context;
            private readonly IPhotoService _photoService;
            private readonly IUserAccessor _userAccessor;
            public Handler(AppDbContext context, IPhotoService photoService, IUserAccessor userAccessor)
            {
                _context = context;
                _photoService = photoService;
                _userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {

                var user = await _userAccessor.GetUserWithPhotosAsync();
                var photo = user.Photos.FirstOrDefault(x=>x.Id== request.PhotoId);
                if ((photo == null)) return Result<Unit>.Failure("Cannot find photo", 400);
                user.ImageUrl = photo.Url;

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                return result
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Problem Updating photo", 400);
            }
        }
    }
}

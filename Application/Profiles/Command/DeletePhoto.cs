using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;
using Application.interfaces;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Profiles.Command
{
    public class DeletePhoto
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
                var photo =   user.Photos.FirstOrDefault(x=>x.Id == request.PhotoId);
                if ((photo == null)) return Result<Unit>.Failure("Cannot find photo", 400);
                if(user.ImageUrl== photo.Url) { return Result<Unit>.Failure("Cannot delete main photo", 400); }
                 await _photoService.DeletePhoto(photo.PublicId);

                //if (deleteResult != null)
                //    return Result<Unit>.Failure(deleteResult.Error.Message, 400);

                user.Photos.Remove(photo);

                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                return result
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Problem deleting photo", 400);
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Activities.DTOs;
using Application.Core;
using Application.interfaces;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Persistence;

namespace Application.Profiles.Command
{
    public class AddPhoto
    {

        public class Command : IRequest<Result<Photo>>
        {
            public required IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Photo>>
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
            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                var uploadResult = await _photoService.UploadPhoto(request.File);
                if (uploadResult == null) {
                    return Result<Photo>.Failure("Failure to upload Photo", 400);};
                var user = await _userAccessor.GetUserAsync();
                var photo = new Photo
                {

                    PublicId = uploadResult.PublicId,
                    UserId = user.Id,
                    Url = uploadResult.Url
                };
                user.ImageUrl ??= photo.Url;
                _context.Photos.Add(photo);
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                return result
                    ? Result<Photo>.Success(photo)
                    : Result<Photo>.Failure("Problem saving photo to DB", 400);
            }
        }
    }
}

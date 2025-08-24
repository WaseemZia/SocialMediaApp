using Application.Profiles.Command;
using Application.Profiles.Commands;
using Application.Profiles.Queries;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseApiController
    {

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto(IFormFile file)
        {
            return HandleRequest(await Mediator.Send(new AddPhoto.Command { File = file }));
        }


        [HttpGet("{id}/photo")]
        public async Task<ActionResult<Photo>> GetUserProfilePhoto(string id)
        {
            return HandleRequest(await Mediator.Send(new GetProfilePhotos.Query{UserId=id}));
        }

        [HttpDelete("{PhotoId}/photo")]
        public async Task<ActionResult<Photo>> DeleteUserProfilePhoto(string PhotoId)
        {
            return HandleRequest(await Mediator.Send(new DeletePhoto.Command { PhotoId = PhotoId }));
        }

        [HttpPut("{PhotoId}/setMain")]
        public async Task<ActionResult<Photo>> SetMainPhoto(string PhotoId)
        {
            return HandleRequest(await Mediator.Send(new SetMainPhoto.Command { PhotoId = PhotoId }));
        }
        [HttpGet("{UserId}")]
        public async Task<ActionResult<Photo>> GetUserProfile(string UserId)
        {
            return HandleRequest(await Mediator.Send(new GetProfile.Query { UserId = UserId }));
        }

        [HttpPost("{UserId}/follow")]
        public async Task<ActionResult> FollowToggle(string UserId)
        {
            return HandleRequest(await Mediator.Send(new FollowToggles.Command{ TargetUserId = UserId }));
        }
        [HttpGet("{UserId}/follow-list")]
        public async Task<ActionResult> GetFollowing(string UserId, string predicate)
        {
            return HandleRequest(await Mediator.Send(new GetFollowings.Query
            {
                UserId = UserId,
                Predicate = predicate
            }));
        }
        [HttpGet("{userId}/activities")]
        public async Task<IActionResult> GetUserActivities(string userId, string filter)
        {
            return HandleRequest(await Mediator.Send(new GetUserActivities.Query
            { UserId = userId, Filter = filter }));
        }
    }
}

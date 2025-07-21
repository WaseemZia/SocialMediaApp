using Application.Activities.Command;
using Application.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class CommentHub:Hub
    {
        private IMediator _mediator;
        public CommentHub( IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendComment(AddComment.Command command)
        {
            var comment =await _mediator.Send(command);
            await Clients.Group(command.ActivityId).SendAsync("ReceiveComment", comment.Value);
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var activityId = httpContext?.Request.Query["activityId"];
            if (string.IsNullOrEmpty(activityId)) throw new HubException("Activity Not Found");
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            //await Task.Delay(5000000); // 5 s
            var result =await _mediator.Send(new GetComments.Query { ActivityId=activityId! });
            await  Clients.Caller   .SendAsync("LoadingComments",result.Value);

        }
    }
}

using Application.Activities.Command;
using Application.Activities.DTOs;
using Application.Activities.Queries;
using Application.Core;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
       
        [HttpGet]
        public async Task<ActionResult<PagedList<ActivityDto,DateTime?>>> GetActivities([FromQuery]ActivityParams activityParams)
        {
            var result = await Mediator.Send(new GetActivityList.Query{ActivityParams=activityParams });
            return HandleRequest(result);   
        }
      
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivitiesById(string id)
        {
            //var activities = await context.Activities.FindAsync(Id);
            //if (activities == null) { return NotFound(); }
            
            var result= await Mediator.Send(new GetActivityDetail.Query { Id = id });
            return HandleRequest(result);
        }
        [HttpPost]
        public async Task<ActionResult<string>> CreateActivity(CreateActivityDto createActivityDto)
        {
            return HandleRequest(await Mediator.Send(new CreateActivity.Command { createActivityDto = createActivityDto }));
           
        }
        [HttpPost("{id}/attend")]


        public async Task<ActionResult<string>> UpdateAttendence(string id)
        {
            return HandleRequest(await Mediator.Send(new UpdateAttendance.Command { Id=id }));

        }

        [HttpPut("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Activity>> EditActivity(string id,EditActivityDto activity)
        {
            activity.Id = id;
          return  HandleRequest(await Mediator.Send(new EditActivity.Command { Activitydto = activity })); 
        }
        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult> DeleteActivity(string id)
        {
            return HandleRequest(await Mediator.Send(new DeleteActivity.Command {Id=id}));
        }
    }
}

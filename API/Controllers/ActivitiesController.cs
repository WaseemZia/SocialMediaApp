using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {

        private readonly AppDbContext context;
        public ActivitiesController(AppDbContext context)
        {
            this.context = context;
        }
        [HttpGet]

        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            
            return await context.Activities.ToListAsync();
        }

        [HttpGet("{Id}")]

        public async Task<ActionResult<Activity>> GetActivitiesById(string Id)
        {
            var activities = await context.Activities.FindAsync(Id);
            if (activities == null) { return NotFound(); }
            return activities;
        }
    }
}

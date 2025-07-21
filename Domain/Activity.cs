using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    [Index(nameof(Date))]
    public class Activity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Title { get; set; }
        public DateTime Date { get; set; }
        public  string Description { get; set; }
        public  string Category { get; set; }
        public bool IsCancelled { get; set; } 
        // location props
        public  string City { get; set; }
        public  string Venue { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        //Navigation Properties 
        public ICollection<ActivityAttendee> Attendees { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}

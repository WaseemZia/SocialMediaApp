using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Core;

namespace Application.Activities.Queries
{
    public class ActivityParams :PaginationParams<DateTime?>
    {
        public string? filter { get; set; }
        public DateTime? startDate { get; set; }
    }
}

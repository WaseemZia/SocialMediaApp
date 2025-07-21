using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Activities.Command;
using Application.Activities.DTOs;
using FluentValidation;

namespace Application.Activities.Validators
{
    public class CreateActivityValidator: BaseActivityValidator<CreateActivity.Command, CreateActivityDto>
    {
        public CreateActivityValidator():base(x=>x.createActivityDto)
        {
            
        }
    }
}

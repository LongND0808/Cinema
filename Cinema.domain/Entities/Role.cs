using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public string Code { get; set; }
    }
}

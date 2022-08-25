using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MicrosoftIdentityTemplate.Models
{
    public class CustomIdentityDbContext :IdentityDbContext<CustomIdentityUser , CustomIdentityRole , string>
    {


        //dependency için parametre geçiyoruz options'u 
        public CustomIdentityDbContext(DbContextOptions<CustomIdentityDbContext> options):base(options)
        {
            
        }
    }
}

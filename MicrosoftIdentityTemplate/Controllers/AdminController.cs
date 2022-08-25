using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MicrosoftIdentityTemplate.Models;

namespace MicrosoftIdentityTemplate.Controllers
{
    public class AdminController : Controller
    {
        public AdminController(UserManager<CustomIdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        private UserManager<CustomIdentityUser> userManager { get; }
        public IActionResult Index()
        {
            return View(userManager.Users.ToList());
        }
    }
}

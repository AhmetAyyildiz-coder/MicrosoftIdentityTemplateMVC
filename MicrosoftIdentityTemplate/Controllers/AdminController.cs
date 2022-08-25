using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MicrosoftIdentityTemplate.Models;
using MicrosoftIdentityTemplate.ViewModels;

namespace MicrosoftIdentityTemplate.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(UserManager<CustomIdentityUser> userManager, RoleManager<CustomIdentityRole> roleManager) : base(userManager,null, roleManager)
        {
            this.userManager = userManager;
        }

        private UserManager<CustomIdentityUser> userManager { get; }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View(userManager.Users.ToList());
        }



        //role olusturma
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RoleCreate(RoleViewModel roleViewModel)
        {
            CustomIdentityRole role = new CustomIdentityRole();
            role.Name = roleViewModel.Name;
            IdentityResult result = roleManager.CreateAsync(role).Result;

            if (result.Succeeded)

            {
                return RedirectToAction("Roles");
            }
            else
            {
                AddModelError(result);
            }

            return View(roleViewModel);
        }

        public IActionResult Roles()
        {
            return View(roleManager.Roles.ToList());
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MicrosoftIdentityTemplate.Models;
using MicrosoftIdentityTemplate.ViewModels;
using Mapster;

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

        public IActionResult RoleDelete(string id)
        {
            CustomIdentityRole role = roleManager.FindByIdAsync(id).Result;
            if (role != null)
            {
                IdentityResult result = roleManager.DeleteAsync(role).Result;
            }

            return RedirectToAction("Roles");
        }


        public IActionResult RoleUpdate(string id)
        {
            CustomIdentityRole role = roleManager.FindByIdAsync(id).Result;

            if (role != null)
            {
                return View(role.Adapt<RoleViewModel>());
            }

            return RedirectToAction("Roles");
        }

        [HttpPost]
        public IActionResult RoleUpdate(RoleViewModel roleViewModel)
        {
            CustomIdentityRole role = roleManager.FindByIdAsync(roleViewModel.Id).Result;

            if (role != null)
            {
                role.Name = roleViewModel.Name;
                IdentityResult result = roleManager.UpdateAsync(role).Result;

                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Güncelleme işlemi başarısız oldu.");
            }

            return View(roleViewModel);
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

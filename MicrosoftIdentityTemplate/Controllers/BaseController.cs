using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MicrosoftIdentityTemplate.Models;

namespace MicrosoftIdentityTemplate.Controllers
{
    public class BaseController : Controller
    {
        protected UserManager<CustomIdentityUser> userManager { get; }
        protected SignInManager<CustomIdentityUser> signInManager { get; }
        protected CustomIdentityUser CurrentUser => userManager.FindByNameAsync(User.Identity.Name).Result;

        public BaseController(UserManager<CustomIdentityUser> userManager, SignInManager<CustomIdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public void AddModelError(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
        }
    }
}

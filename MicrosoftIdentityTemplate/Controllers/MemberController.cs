using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MicrosoftIdentityTemplate.Models;

namespace MicrosoftIdentityTemplate.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        public UserManager<CustomIdentityUser> userManager { get; }
        public SignInManager<CustomIdentityUser> signInManager { get; }

        public MemberController(UserManager<CustomIdentityUser> userManager, SignInManager<CustomIdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            CustomIdentityUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            UserViewModel userViewModel = user.Adapt<UserViewModel>();

            return View(userViewModel);
        }



    }
}

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


        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (ModelState.IsValid)
            {
                CustomIdentityUser user = userManager.FindByNameAsync(User.Identity.Name).Result;

                bool exist = userManager.CheckPasswordAsync(user, passwordChangeViewModel.PasswordOld).Result;

                if (exist)
                {
                    IdentityResult result = userManager.ChangePasswordAsync(user, passwordChangeViewModel.PasswordOld, passwordChangeViewModel.PasswordNew).Result;

                    if (result.Succeeded)
                    {
                        userManager.UpdateSecurityStampAsync(user); //sifre degistikten sonra mutlaka 
                        //securityStamp değiştirilmelidir.

                        signInManager.SignOutAsync(); //ardından kullanıcı cıkıs yaptırılıp tekrar girilmeli ki yeni 
                        //security stamp ile girilsin cookie
                        signInManager.PasswordSignInAsync(user, passwordChangeViewModel.PasswordNew, true, false);

                        ViewBag.success = "true";
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Eski şifreniz yanlış");
                }
            }

            return View(passwordChangeViewModel);
        }
    }
}

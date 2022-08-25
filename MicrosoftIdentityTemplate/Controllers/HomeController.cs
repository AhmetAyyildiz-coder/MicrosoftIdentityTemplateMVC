using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MicrosoftIdentityTemplate.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MicrosoftIdentityTemplate.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MicrosoftIdentityTemplate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public UserManager<CustomIdentityUser> UserManager { get; }
        public SignInManager<CustomIdentityUser> SignInManager { get; }
        public HomeController(ILogger<HomeController> logger, UserManager<CustomIdentityUser> userManager, SignInManager<CustomIdentityUser> signInManager)
        {
            _logger = logger;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult SignUp() //Kayıt ol
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(IdentitySignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                CustomIdentityUser user = new CustomIdentityUser();
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;


                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("LogIn");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("",item.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(TempData["ReturnUrl"].ToString());
            }
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
               CustomIdentityUser user = await UserManager.FindByEmailAsync(viewModel.Email);
                if (user != null)
                {
                    await SignInManager.SignOutAsync();
                    SignInResult result = await SignInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (TempData["ReturnUrl"] !=null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");
                    }
                }
               
            }
            else
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre");

            }
            return View(viewModel);
        }
    }
}

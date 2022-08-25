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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Member");
            }
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
                    //eğer kullanıcı şifre girerek  kendini kilitlettirdiyse belirli başarısız 
                    // Bunu bildirmeliyiz.

                    //bu if koşulu kullanıcının kilitli olup olmadıgını bize dondurur
                    if (await UserManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınız bir süreliğine kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");

                        return View(viewModel);
                    }

                    await SignInManager.SignOutAsync();
                    SignInResult result = await SignInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);
                    if (result.Succeeded)
                    {

                        //eger kullanıcı basarili sekilde sisteme girdiyse bunu sıfırlayalım
                        await UserManager.ResetAccessFailedCountAsync(user);

                        if (TempData["ReturnUrl"] !=null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");
                    }
                    else //eğer sistemde bu emailde bi kullanıcı varsa ve şifre girmede 
                    //başarısız oluyorsa , bu durumda bu arkadaşın başarısız girişini arttırmalıyız.
                    {
                        await UserManager.AccessFailedAsync(user); //+1 arttır başarısız giriş sayısını diyoruz

                        int fail = await UserManager.GetAccessFailedCountAsync(user);
                        ModelState.AddModelError("", $" {fail} kez başarısız giriş.");
                        if (fail == 3)
                        {//eğer 3 başarısız giriş yaparsak belirli tarihe kadar kullanıcıyı kilitle diyoruz.
                            //default olarak 20dakika yaptık bunu sistemde
                            await UserManager.SetLockoutEndDateAsync(user, new System.DateTimeOffset(DateTime.Now.AddMinutes(20)));

                            ModelState.AddModelError("", "Hesabınız 3 başarısız girişten dolayı 20 dakika süreyle kitlenmiştir. Lütfen daha sonra tekrar deneyiniz.");
                        }
                        else
                        {//eğer başarısız giriş sayısı daha 3 olmadıysa tekrar email yanlış bildirimi verelim
                            //burda isteğe bağlı max 3 hakkınız var yazabiliriz.
                            ModelState.AddModelError("", $"Email adresiniz veya şifreniz yanlıştır. " +
                                $"Eğer {3-fail} kez daha başarısız giriş yaparsanız hesabınız 20 dakika kilitlenicektir");
                        }
                    }
                }
               
            }
            else
            {
                ModelState.AddModelError("", "Bu email adresine kayıtlı kullanıcı bulunamamıştır.");

            }
            return View(viewModel);
        }
    }
}

using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MicrosoftIdentityTemplate.Enums;
using MicrosoftIdentityTemplate.Models;
using MicrosoftIdentityTemplate.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MicrosoftIdentityTemplate.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
       

        public MemberController(UserManager<CustomIdentityUser> userManager, SignInManager<CustomIdentityUser> signInManager) :base(userManager,signInManager)
        {
         
        }
        public IActionResult Index()
        {
            CustomIdentityUser user = CurrentUser;
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
                CustomIdentityUser user = CurrentUser;

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
                        //foreach (var item in result.Errors)
                        //{
                        //    ModelState.AddModelError("", item.Description);
                        //}
                        AddModelError(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Eski şifreniz yanlış");
                }
            }

            return View(passwordChangeViewModel);
        }


        public IActionResult UserEdit()
        {
            CustomIdentityUser user = CurrentUser;
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            UserViewModel userViewModel = user.Adapt<UserViewModel>();

            return View(userViewModel);
        }



        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel userViewModel , IFormFile userPicture)
        {
            ModelState.Remove("Password");
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            if (ModelState.IsValid)
            {
                CustomIdentityUser user = CurrentUser;



                //kullanıcı için profil fotoğrafı ekleme mekanizması 
                if (userPicture != null && userPicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await userPicture.CopyToAsync(stream);

                        user.Picture = "/UserPicture/" + fileName;
                    }
                }

                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;
                

                user.BirthDay = userViewModel.BirthDay;

                user.Gender = (int)userViewModel.Gender;
                IdentityResult result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await userManager.UpdateSecurityStampAsync(user);
                    await signInManager.SignOutAsync();
                    await signInManager.SignInAsync(user, true);

                    ViewBag.success = "true";
                }
                else
                {
                    //foreach (var item in result.Errors)
                    //{
                    //    ModelState.AddModelError("", item.Description);
                    //}
                    AddModelError(result);
                }
            }

            return View(userViewModel);
        }

        public void LogOut()
        {
            signInManager.SignOutAsync();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

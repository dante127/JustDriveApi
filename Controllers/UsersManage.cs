using JustDrive.Data;
using JustDrive.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JustDrive.Controllers
{
    public class UsersManage : Controller
    {
        UserManager<AppUser> userManager;
        IPasswordHasher<AppUser> passwordHasher;
        public UsersManage(UserManager<AppUser> user, IPasswordHasher<AppUser> pass)
        {
            userManager = user;
            passwordHasher = pass;
        }
        public IActionResult Index()
        {
            var AllUsers = userManager.Users;
            return View(AllUsers);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    Email = registerViewModel.Email,
                    UserName = registerViewModel.Email,
                    PhoneNumber = registerViewModel.Phone,
                    DriveCert = registerViewModel.DriveCert,
                    ImageId = registerViewModel.ImageId
                };

                IdentityResult result = await userManager.CreateAsync(appUser, registerViewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(registerViewModel);
        }


        public async Task<IActionResult> Details(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if(user != null)
            {
                return View(user);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }


            var EditModel = new RegisterViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                Password = user.PasswordHash,
                DriveCert = user.DriveCert,
                ImageId = user.ImageId
            };
            return View(EditModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(RegisterViewModel registerViewModel)
        {
            if(ModelState.IsValid)
            {
                var email = await userManager.FindByIdAsync(registerViewModel.Id);
                email.FirstName = registerViewModel.FirstName;
                email.LastName = registerViewModel.LastName;
                email.Email = registerViewModel.Email;
                email.PasswordHash = passwordHasher.HashPassword(email, registerViewModel.Password);
                email.PhoneNumber = registerViewModel.Phone;
                email.DriveCert = registerViewModel.DriveCert;
                email.ImageId = registerViewModel.ImageId;

                var isSucc = await userManager.UpdateAsync(email);
                if(isSucc.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in isSucc.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }

            return View(registerViewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            if(user!=null)
            {
                var DeletedUser = new RegisterViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };
                return View(DeletedUser);
            }

            return NotFound();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RegisterViewModel registerViewModel)
        {
            var Deleted = await userManager.FindByIdAsync(registerViewModel.Id);
            if(Deleted!=null)
            {
                var result = await userManager.DeleteAsync(Deleted);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(registerViewModel);
        }



    }
}

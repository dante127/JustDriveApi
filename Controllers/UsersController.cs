using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JustDrive.Data;
using JustDrive.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JustDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private ApplicationDbContext db;
        private UserManager<User> userManager;
        private readonly IHostingEnvironment hostingEnvironment;

        IPasswordHasher<User> passwordHasher;
        public UsersController(ApplicationDbContext applicationDbContext, 
            UserManager<User> userManager,
              IPasswordHasher<User> passwordHasher, IHostingEnvironment hostingEnvironment
            )
        {
            db = applicationDbContext;
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
            this.hostingEnvironment = hostingEnvironment;
        }


        [HttpPost]
      
        [Route("Register")]
        public async Task<string> Register([FromForm] RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var result2 = new List<FileUploadResult>();
                string path = "";
                    
                
                User appUser = new User
                {
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    Email = registerViewModel.Email,
                    UserName = registerViewModel.Email,
                    PhoneNumber = registerViewModel.Phone,
                    DriverCertificate = registerViewModel.Password,
                };
               
                IdentityResult result = await userManager.CreateAsync(appUser, registerViewModel.Password);
                if (result.Succeeded)
                {
                    string projectRootPath = hostingEnvironment.ContentRootPath;
                    Console.WriteLine(projectRootPath);
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/user_img",  registerViewModel.PdfFile.FileName);
                    var stream = new FileStream(path+"_"+appUser.Email, FileMode.Create);
                    await registerViewModel.PdfFile.CopyToAsync(stream);
                    result2.Add(new FileUploadResult() { Name = registerViewModel.PdfFile.FileName, Length = registerViewModel.PdfFile.Length });
                    User s = db.User.FirstOrDefault(f => f.Id == appUser.Id);
                    s.ImageId = path;
                    db.SaveChanges();
                    await userManager.AddToRoleAsync(appUser, "user");
                    return "1";
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        return error.Description;
                    }
                }
            }
            return "0";
        }


        [HttpPost]
        [Route("Login")]

        public async Task<string> Login(LoginViewModel loginViewModel)
        {
           
             var user = await userManager.FindByEmailAsync(loginViewModel.Email);
            if (user == null)
            {
                return "Invalid Email";
            }
             
            else if (!await userManager.IsEmailConfirmedAsync(user))
            {
                return "Wait for Email Confirm";
            }
      
            var pass = await userManager.CheckPasswordAsync(user, loginViewModel.Password);
      
            if (pass == false)
            {
                return "Invalid pass";
            }

            else if (user != null && pass != false)
                {
                    return user.Id;
                }

            return"";
            
        }

        [HttpPost]
        [Route("EditProfile")]
        public async Task<int> Edit(EditViewModel model)
        {
            var IsEx = await userManager.FindByIdAsync(model.Id);
            if (IsEx != null)
            {
                var EditModel = new EditViewModel
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                };

                User u = await userManager.FindByIdAsync(EditModel.Id);

                u.FirstName = EditModel.FirstName;
                u.LastName = EditModel.LastName;
                u.PhoneNumber = EditModel.Phone;
                var IsDone = await userManager.UpdateAsync(u);
                if (IsDone.Succeeded)
                {
                    return 1;
                }
            }
            return 0;
        }

    }
}
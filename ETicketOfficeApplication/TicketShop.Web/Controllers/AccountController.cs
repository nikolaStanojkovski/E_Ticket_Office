using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketShop.Domain.Domain;
using TicketShop.Domain.DTO;
using TicketShop.Domain.Identity;
using TicketShop.Repository.Interface;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System.IO;
using TicketShop.Domain.Enumerations;

namespace TicketShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<EShopUser> userManager;
        private readonly SignInManager<EShopUser> signInManager;
        private readonly IUserRepository userRepository;
        private readonly List<string> _roles = new List<string>() { "Administrator", "User" };

        public AccountController(UserManager<EShopUser> _userManager,
            SignInManager<EShopUser> _signInManager,
            IUserRepository _userRepository)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            this.userRepository = _userRepository;
        }

        // -------------------------------------------
        // ------------LOGIN-REGISTER-----------------
        // -------------------------------------------

        [HttpGet]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed");
                    return View(model);
                }
                if (await userManager.CheckPasswordAsync(user, model.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
                    false, true);

                if (result.Succeeded)
                {
                    await userManager.AddClaimAsync(user, new Claim("UserRole", "Admin"));
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(UserRegistrationDto request)
        {
            var result = RegisterUser(request, "User");
            if(!result.Result)
                return View(request);

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // -------------------------------------------
        // ------------------OTHER--------------------
        // -------------------------------------------

        [Authorize(Roles = "Administrator")]
        public IActionResult AddUserToRole()
        {
            AddUserToRoleDTO model = new AddUserToRoleDTO
            {
                Roles = _roles,
                Users = userRepository.GetAllMails(),
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddUserToRole([Bind("UserMail,Role")] AddUserToRoleDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.UserMail);

            await userManager.RemoveFromRolesAsync(user, _roles);
            await userManager.AddToRoleAsync(user, model.Role);

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult ImportUsers()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> ImportUsers(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\Files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            await CreateUsersFromFile(file.FileName);

            return RedirectToAction("Index", "Home");
        }

        public async Task<bool> CreateUsersFromFile(string fileName)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}\\Files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        var model = new UserRegistrationDto
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(1).ToString(),
                            FirstName = reader.GetValue(3).ToString(),
                            MiddleName = reader.GetValue(4).ToString(),
                            LastName = reader.GetValue(5).ToString(),
                            Address = reader.GetValue(6).ToString(),
                            Age = Int32.Parse(reader.GetValue(7).ToString()),
                            Gender = (Gender)Enum.Parse(typeof(Gender), reader.GetValue(8).ToString())
                        };

                        await RegisterUser(model, reader.GetValue(2).ToString());
                    }
                }
            }

            return true;
        }

        public async Task<bool> RegisterUser(UserRegistrationDto request, string role)
        {
            var userCheck = await userManager.FindByEmailAsync(request.Email);
            if (userCheck == null)
            {
                var user = new EShopUser
                {
                    UserName = request.Email,
                    NormalizedUserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,

                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    Address = request.Address,
                    Age = request.Age,
                    Gender = request.Gender,
                    Cart = new ShoppingCart()
                };
                var result = await userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    return true;
                }
                else
                {
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("message", error.Description);
                        }
                    }
                    return false;
                }
            }
            else
            {
                ModelState.AddModelError("message", "Email already exists");
                return false;
            }
        }
    }
}

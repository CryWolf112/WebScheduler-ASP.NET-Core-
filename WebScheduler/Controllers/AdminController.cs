using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WebScheduler.Database;
using WebScheduler.Interfaces;
using WebScheduler.Models;
using WebScheduler.ViewModels;

namespace WebScheduler.Controllers
{
    [Auth(roleType:"Admin")]
    public class AdminController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly DataContext dataContext;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<User> logger;
        public AdminController(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<User> logger)
        {
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            IdentityRole role = await roleManager.FindByNameAsync("User");
            IEnumerable<User> users = unitOfWork.UserRepository.GetAllUsersInRole(role.Id);

            ViewBag.Users = users.ToList();

            return View();
        }

        public async Task<IActionResult> Reset(AdminDashboardViewModel viewModel)
        {
            User user = await userManager.FindByIdAsync(viewModel.Id);
            await userManager.RemovePasswordAsync(user);
            await userManager.AddPasswordAsync(user, viewModel.Password);
            
            return RedirectToAction("dashboard", "admin");
        }

        public async Task<IActionResult> Disable(AdminDashboardViewModel viewModel)
        {
            User user = await userManager.FindByIdAsync(viewModel.Id);
            await userManager.SetLockoutEndDateAsync(user, DateTime.MaxValue);

            return RedirectToAction("dashboard", "admin");
        }

        public async Task<IActionResult> Enable(AdminDashboardViewModel viewModel)
        {
            User user = await userManager.FindByIdAsync(viewModel.Id);
            user.LockoutEnd = null;

            unitOfWork.UserRepository.Update(user);
            await unitOfWork.SaveAsync();

            return RedirectToAction("dashboard", "admin");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
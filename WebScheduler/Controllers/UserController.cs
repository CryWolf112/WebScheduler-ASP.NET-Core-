using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebScheduler.Database;
using WebScheduler.Interfaces;
using WebScheduler.Models;
using WebScheduler.ViewModels;

namespace WebScheduler.Controllers
{
    [Auth(roleType: "User")]
    public class UserController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly DataContext dataContext;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<User> logger;
        public UserController(
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
            DateTime date;
            if (TempData["Date"] != null)
            {
                date = ((DateTime)TempData["Date"]).Date;
            }
            else
            {
                date = DateTime.Now.Date;
                TempData["Date"] = date;
            }

            User user = await userManager
                .FindByNameAsync(User.Identity.Name);

            IEnumerable<Appointment> appointments = await unitOfWork.AppointmentRepository
                .GetAllAsync(app => app.UserId == user.Id && app.StartsAt.Date == date);

            appointments = appointments.OrderBy(appointment => appointment.StartsAt);
            ViewBag.Appointments = appointments.ToList();
                
            return View();
        }

        public IActionResult Search(AppointmentsViewModel viewModel)
        {
            if (viewModel.Date != null)
            {
                TempData["Date"] = viewModel.Date;
            }

            return RedirectToAction("dashboard", "user");
        }

        public async Task<IActionResult> Create(AppointmentsViewModel viewModel, DateTime date)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager
                    .FindByNameAsync(User.Identity.Name);

                DateTime startsAt = new DateTime(
                        date.Year,
                        date.Month,
                        date.Day,
                        viewModel.Appointment.StartsAt.Hour,
                        viewModel.Appointment.StartsAt.Minute,
                        viewModel.Appointment.StartsAt.Second);

                DateTime? endsAt = null;

                if (viewModel.Appointment.EndsAt != null)
                {
                    endsAt = new DateTime(
                    date.Year,
                    date.Month,
                    date.Day,
                    viewModel.Appointment.EndsAt.Value.Hour,
                    viewModel.Appointment.EndsAt.Value.Minute,
                    viewModel.Appointment.EndsAt.Value.Second);
                }

                await unitOfWork.AppointmentRepository.AddAsync(new Appointment()
                {
                    Title = viewModel.Appointment.Title,
                    StartsAt = startsAt,
                    EndsAt = endsAt,
                    Description = viewModel.Appointment.Description,
                    UserId = user.Id
                });

                await unitOfWork.SaveAsync();

                TempData["Date"] = date;
                return RedirectToAction("dashboard", "user");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Update(AppointmentsViewModel viewModel, DateTime date)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager
                    .FindByNameAsync(User.Identity.Name);

                DateTime startsAt = new DateTime(
                    date.Year,
                    date.Month,
                    date.Day,
                    viewModel.Appointment.StartsAt.Hour,
                    viewModel.Appointment.StartsAt.Minute,
                    viewModel.Appointment.StartsAt.Second);

                DateTime? endsAt = null;

                if (viewModel.Appointment.EndsAt != null)
                {
                    endsAt = new DateTime(
                    date.Year,
                    date.Month,
                    date.Day,
                    viewModel.Appointment.EndsAt.Value.Hour,
                    viewModel.Appointment.EndsAt.Value.Minute,
                    viewModel.Appointment.EndsAt.Value.Second);
                }

                Appointment appointment = await unitOfWork.AppointmentRepository
                    .GetAsync(appointment => appointment.UserId == user.Id && appointment.Id == viewModel.Appointment.Id);
                appointment.Title = viewModel.Appointment.Title;
                appointment.StartsAt = startsAt;
                appointment.EndsAt = endsAt;
                appointment.Description = viewModel.Appointment.Description;

                unitOfWork.AppointmentRepository.Update(appointment);
                await unitOfWork.SaveAsync();

                TempData["Date"] = date;

                return RedirectToAction("dashboard", "user");
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(AppointmentsViewModel viewModel, DateTime date)
        {
            User user = await userManager
                .FindByNameAsync(User.Identity.Name);

            Appointment appointment = await unitOfWork.AppointmentRepository
                .GetAsync(appointment => appointment.Id == viewModel.Appointment.Id && appointment.UserId == user.Id);

            unitOfWork.AppointmentRepository.Remove(appointment);
            await unitOfWork.SaveAsync();

            TempData["Date"] = date;
            return RedirectToAction("dashboard", "user");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using WebScheduler.Database;
using WebScheduler.Interfaces;
using WebScheduler.Models;
using WebScheduler.ViewModels;

namespace WebScheduler.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly DataContext dataContext;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<User> logger;
        public AccountController(
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

        // GET: AccountController
        [Auth(guestOnly:true)]
        public ActionResult Login()
        {
            return View();
        }

        // GET: AccountController/Register
        [Auth(guestOnly: true)]
        public ActionResult Register()
        {
            Country[] countries = unitOfWork.CountryRepository
                .GetAll()
                .OrderBy(country => country.Name)
                .ToArray();

            ViewData["Countries"] = countries;

            return View();
        }

        // GET: AccountController/MailSent
        [Auth(guestOnly: true)]
        public IActionResult MailSent()
        {
            return View();
        }

        // GET: AccountController/Confirm
        public async Task<ActionResult> Confirm(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key"));

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "https://localhost:7205/",
                    ValidateAudience = false,
                    ValidateLifetime = true,
                }, out SecurityToken validatedToken); ;

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
                string userId = jwtToken.Claims.First(claim => claim.Type == "id").Value;

                User user = await userManager.FindByIdAsync(userId);
                user.EmailConfirmed = true;

                await userManager.UpdateAsync(user);

                return View();
            }
            catch (Exception exception)
            {
                logger.LogError("JwtTokenValidationFailed", exception);
                return StatusCode(419);
            }
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Auth(guestOnly: true)]
        public async Task<ActionResult> Create(RegistrationViewModel viewModel)
        {
            try
            {
                string googleCaptcha = Request.Form["g-recaptcha-response"];
                string secretKey = configuration.GetValue<string>("Captcha:SecretKey");

                HttpClient httpClient = new HttpClient();
                string url = "https://www.google.com/recaptcha/api/siteverify?secret=" + secretKey + "&response=" + googleCaptcha;
                HttpResponseMessage response = await httpClient.PostAsync(url, null);
                response.EnsureSuccessStatusCode();
                
                var result = await response.Content.ReadAsStringAsync();
                bool captchaValid = JsonObject.Parse(result)["success"] == null ? false : JsonObject.Parse(result)["success"].GetValue<bool>();

                if (!captchaValid)
                {
                    ModelState.AddModelError("GoogleRecaptcha", "The captcha validation failed.");
                }

                if (ModelState.IsValid)
                {
                    IdentityRole role = await roleManager.FindByNameAsync("User");

                    User newUser = viewModel.User;
                    newUser.DateCreated = DateTime.UtcNow;
                    
                    await userManager.CreateAsync(newUser, viewModel.Password);
                    await userManager.AddToRoleAsync(newUser, "User");

                    string token = GenerateJwtToken(newUser);
                    SendMail(viewModel.User.Email, token);
                    
                    TempData["Email"] = viewModel.User.Email;
                    return RedirectToAction("MailSent");
                }
                else
                {
                    TempData["Error"] = ModelState.Values.SelectMany(values => values.Errors).First().ErrorMessage;
                    return RedirectToAction("Register");
                }
            }
            catch(Exception exception)
            {
                logger.LogError("UserRegistrationFailed", exception);
                return BadRequest();
            }
        }

        // POST: AccountController/Authenticate
        [HttpPost]
        [Auth(guestOnly: true)]
        public async Task<IActionResult> Authenticate(LoginViewModel viewModel)
        {
            try
            {
                User user; 
                if (Regex.IsMatch(viewModel.Input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    user = await userManager.FindByEmailAsync(viewModel.Input);
                }
                else
                {
                    user = await userManager.FindByNameAsync(viewModel.Input);
                }

                if (user != null)
                {

                    if (await userManager.IsLockedOutAsync(user))
                    {
                        ViewBag.Error = "Your account has been blocked";
                        return View("Login");
                    }

                    var result = await signInManager
                        .CheckPasswordSignInAsync(user, viewModel.Password, false);

                    if (result.Succeeded)
                    {
                        await signInManager
                            .SignInAsync(user, viewModel.Remember);
                        
                        if (await userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToAction("dashboard", "admin");
                        }

                        return RedirectToAction("dashboard", "user");
                    }
                }

                ViewBag.Error = "Login failed. Please, try again.";
                return View("Login");
            }
            catch(Exception exception)
            {
                logger.LogError("UserLoginFailed", exception);
                return BadRequest();
            }
        }

        // GET: AccountController/Authenticate+
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            
            return RedirectToAction("login", "account");
        }

        private string Hash(string plainText)
        {
            StringBuilder stringBuilder = new StringBuilder();
            SHA256 hash = SHA256.Create();
            byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

            foreach(byte b in result )
            {
                stringBuilder.Append(b.ToString("x2"));
            }
            
            return stringBuilder.ToString();
        }

        private string GenerateJwtToken(User user)
        {

            byte[] bytes = Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key"));

            Claim[] claims = new Claim[]
            {
                    new Claim("id", user.Id.ToString()),
            };

            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(bytes), SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                configuration.GetValue<string>("Jwt:Issuer"),
                configuration.GetValue<string>("Jwt:Audience"),
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void SendMail(string to, string token)
        {
            string email = configuration.GetValue<string>("Mail:MailAddress");
            string host = configuration.GetValue<string>("Mail:SmtpHost");
            int port = configuration.GetValue<int>("Mail:SmtpPort");
            string password = configuration.GetValue<string>("Mail:MailPassword");
            string url = "https://" + Request.Host.Value + "/account/confirm?token=" + token;

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = host,
                Port = port,
                Credentials = new NetworkCredential(email, password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress(email, "WebScheduler"),
                Subject = "Registration confirmation",
                Body = "<p>Please confirm your email change by <a href='" + url + "'>clicking here</a></p>",
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(to));
            smtpClient.Send(mailMessage);
        }
    }
}

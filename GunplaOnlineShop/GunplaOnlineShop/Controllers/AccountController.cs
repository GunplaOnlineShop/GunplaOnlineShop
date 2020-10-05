using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using GunplaOnlineShop.Data;
using GunplaOnlineShop.Models;
using GunplaOnlineShop.ViewModels;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using MailKit.Security;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GunplaOnlineShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        // GET: /Account/Register
        [HttpPost]
        public async Task<IActionResult> logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, IsAdmin = false};
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //generate email registration token&link
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new {userId = user.Id, token = token},Request.Scheme);

                    await _userManager.AddClaimAsync(user, new Claim("IsAdmin", user.IsAdmin.ToString(), ClaimValueTypes.Boolean));

                    //Create email message
                    var email = new MimeMessage();
                    email.Sender = MailboxAddress.Parse(_configuration["SMTPConfig:Username"]);
                    email.To.Add(MailboxAddress.Parse(model.Email));
                    email.Subject = "WhiteBase Online Shop Password Recovery";
                    email.Body = new TextPart(TextFormat.Plain) { Text = confirmationLink };

                    //send email
                    SmtpClient smtp = new SmtpClient();
                    smtp.Connect(_configuration["SMTPConfig:Host"], Int32.Parse(_configuration["SMTPConfig:Port"]),true);
                    var username = _configuration["SMTPConfig:Username"];
                    var password = _configuration["SMTPConfig:Password"];
                    smtp.Authenticate(username, password);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    ViewBag.ConfirmTitle = "Registration successful";
                    ViewBag.ConfirmMessage = "Before you can Login, please confirm your email, by clicking on the confirmation link we have emailed you";
                    return View("ConfirmMessage");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                //email confirm check
                if (user != null && !user.EmailConfirmed && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet.");
                    return View(model);
                }
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // sync user shopping cart cookie data if there are any
                    List<AddItemViewModel> ShoppingCartLineItems = new List<AddItemViewModel>();
                    if (HttpContext.Request.Cookies["GunplaShopShoppingCart"] != null)
                    {
                        // extract items from cookie if exists
                        string shoppingCartCookie = HttpContext.Request.Cookies["GunplaShopShoppingCart"];
                        // deserialize the cookie
                        ShoppingCartLineItems = JsonSerializer.Deserialize<List<AddItemViewModel>>(shoppingCartCookie);
                    }
                    var customer = await _userManager.FindByNameAsync(model.Email);

                    foreach (var shoppingCartLineItem in ShoppingCartLineItems)
                    {
                        var item = await _context.Items.FindAsync(shoppingCartLineItem.ItemId);
                        if (item == null || shoppingCartLineItem.Quantity > item.Quantity)
                        {
                            continue;
                        }
                        var scli = _context.ShoppingCartLineItems.Where(li => li.CustomerId == customer.Id && li.ItemId == shoppingCartLineItem.ItemId).FirstOrDefault();
                        if (scli != null)
                        {
                            scli.Quantity += shoppingCartLineItem.Quantity;
                        }
                        else
                        {
                            _context.ShoppingCartLineItems.Add(new ShoppingCartLineItem
                            {
                                ItemId = shoppingCartLineItem.ItemId,
                                Quantity = shoppingCartLineItem.Quantity,
                                CustomerId = customer.Id
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                    HttpContext.Response.Cookies.Delete("GunplaShopShoppingCart");


                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        { 
            return View(); 
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var flag = await _userManager.IsEmailConfirmedAsync(user);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                        new {email = model.Email,token = token},Request.Scheme);
                    
                    //Create email message
                    var email = new MimeMessage();
                    email.Sender = MailboxAddress.Parse(_configuration["SMTPConfig:Username"]);
                    email.To.Add(MailboxAddress.Parse(model.Email));
                    email.Subject = "WhiteBase Online Shop Password Recovery";
                    email.Body = new TextPart(TextFormat.Plain) { Text = passwordResetLink };

                    //send email
                    SmtpClient smtp = new SmtpClient();
                    smtp.Connect(_configuration["SMTPConfig:Host"],Int32.Parse(_configuration["SMTPConfig:Port"]),true);
                    var username = _configuration["SMTPConfig:Username"];
                    var password = _configuration["SMTPConfig:Password"];
                    smtp.Authenticate(username,password);
                    smtp.Send(email);
                    smtp.Disconnect(true);

                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }


    }
}

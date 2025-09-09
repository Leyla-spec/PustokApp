using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using PustokApp.Models;
using PustokApp.ViewModels;

namespace PustokApp.Controllers
{
    public class AccountController
        (UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager)
        : Controller
    {

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVm userLoginVm, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(userLoginVm);
            }
            AppUser user = await userManager.FindByNameAsync(userLoginVm.Username);
            if (user == null)
            {
                user = await userManager.FindByEmailAsync(userLoginVm.Username);
                if (user == null)
                {
                    ModelState.AddModelError("", "Username or password is incorrect");
                    return View(userLoginVm);
                }
            }
            var passwordValid = await userManager.CheckPasswordAsync(user, userLoginVm.Password);
            if (!passwordValid)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View(userLoginVm);
            }
            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "You are not allowed to login here.");
                return View(userLoginVm);

            }
            var result = await signInManager.PasswordSignInAsync(user, userLoginVm.Password, userLoginVm.RememberMe, true);
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please confirm your email address.");
                return View(userLoginVm);
            }
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account is locked. Please try again later.");
                return View(userLoginVm);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View(userLoginVm);
            }
            if (ReturnUrl is null)
                return RedirectToAction("Index", "Home");
            return Redirect(ReturnUrl);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVm userRegisterVm)
        {
            if (!ModelState.IsValid)
            {
                return View(userRegisterVm);
            }
            var user = await userManager.FindByNameAsync(userRegisterVm.Username);
            if (user != null)
            {
                ModelState.AddModelError("Username", "This username is already taken.");
                return View(userRegisterVm);
            }
            user = new AppUser
            {
                UserName = userRegisterVm.Username,
                FullName = userRegisterVm.FullName,
                Email = userRegisterVm.Email
            };
            var result = await userManager.CreateAsync(user, userRegisterVm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(userRegisterVm);
            }
            await userManager.AddToRoleAsync(user, "Member");

            //email

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { email = user.Email, token = token }, Request.Scheme);

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("eliyevaleyla232323@gmail.com"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Test Email Subject";
            email.Body = new TextPart(TextFormat.Html) { Text = $"<a href ='{confirmationLink}'>Confirm Email</a>" };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("eliyevaleyla232323@gmail.com", "hotj fmrl qhyp onnq");
            smtp.Send(email);
            smtp.Disconnect(true);
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (email == null || token == null) return BadRequest();
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();
            if(!await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", token))
                return Content("Token is not valid");
            var result = await userManager.ConfirmEmailAsync(user, token);
            await userManager.UpdateSecurityStampAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            await signInManager.SignInAsync(user, true);
            return RedirectToAction("Login", "Account");
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UserProfile() 
        {
            UserProfileVm userProfileVm = new UserProfileVm();
            var users = await userManager.FindByNameAsync(User.Identity.Name);
            userProfileVm.UserProfileUpdateVm = new UserProfileUpdateVm
            {
                FullName = users.FullName,
                Email = users.Email,
                UserName = users.UserName
            };
            return View(userProfileVm);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm forgotPasswordVm)
        {
            if(!ModelState.IsValid)
            {
                return View(forgotPasswordVm);
            }
            var user = await userManager.FindByEmailAsync(forgotPasswordVm.Email);
            if(user == null)
            {
                ModelState.AddModelError("Email", "There is no user with this email address.");
                return View(forgotPasswordVm);
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { email = forgotPasswordVm.Email, token = token }, Request.Scheme);
            
            return View();
        }
    }
}

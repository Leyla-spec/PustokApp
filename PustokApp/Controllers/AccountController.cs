using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using PustokApp.Models;
using PustokApp.Services;
using PustokApp.ViewModels;

namespace PustokApp.Controllers
{

    public class AccountController
        (UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        EmailService emailService
        )
        : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginVm userLoginVm, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(userLoginVm);
            }
            AppUser user = await userManager.FindByNameAsync(userLoginVm.UserNameOrEmail);
            if (user == null)
            {
                user = await userManager.FindByEmailAsync(userLoginVm.UserNameOrEmail);
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
            using StreamReader reader = new StreamReader("wwwroot/templates/EmailConfirmTemplate.html");
            
            string html = reader.ReadToEnd();
            html = html.Replace("{{link}}", confirmationLink);
            html = html.Replace("{{username}}", user.UserName);
            emailService.Send(user.Email, "Confirm your email", html);

            return RedirectToAction("Login", "Account");

        }
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (email == null || token == null) return BadRequest();
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();
            if (!await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", token))
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
        public async Task<IActionResult> UserProfile(string tab = "dashboard")
        {
            ViewBag.Tab = tab;
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

        [HttpPost]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> UserProfile(UserProfileUpdateVm userProfileUpdateVm)
        {
            ViewBag.Tab = "profile";
            if (!ModelState.IsValid)
                return View(userProfileUpdateVm);
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Login", "Account");
            var isExistUserName = await userManager.FindByNameAsync(userProfileUpdateVm.UserName);
            if (isExistUserName != null && isExistUserName.Id != user.Id)
            {
                ModelState.AddModelError("UserName", "This username is already taken.");
                return View(new UserProfileVm { UserProfileUpdateVm = userProfileUpdateVm });
            }
            var isExistEmail = await userManager.FindByEmailAsync(userProfileUpdateVm.Email);
            if (isExistEmail != null && isExistEmail.Id != user.Id)
            {
                ModelState.AddModelError("Email", "This email is already taken.");
                return View(new UserProfileVm { UserProfileUpdateVm = userProfileUpdateVm });
            }
            user.FullName = userProfileUpdateVm.FullName;
            user.UserName = userProfileUpdateVm.UserName;
            user.Email = userProfileUpdateVm.Email;
            if (!string.IsNullOrWhiteSpace(userProfileUpdateVm.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(userProfileUpdateVm.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required to set new password.");
                    return View(new UserProfileVm { UserProfileUpdateVm = userProfileUpdateVm });
                }
                var isCurrentPasswordValid = await userManager.CheckPasswordAsync(user, userProfileUpdateVm.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                    return View(new UserProfileVm { UserProfileUpdateVm = userProfileUpdateVm });
                }
                if (userProfileUpdateVm.NewPassword != userProfileUpdateVm.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return View(new UserProfileVm { UserProfileUpdateVm = userProfileUpdateVm });
                }
                var isSamePassword = await userManager.CheckPasswordAsync(user, userProfileUpdateVm.NewPassword);
                if (isSamePassword)
                {
                    ModelState.AddModelError("NewPassword", "New password cannot be the same as the current password.");
                    return View(new UserProfileVm { UserProfileUpdateVm = userProfileUpdateVm });
                }
                var result = await userManager.ChangePasswordAsync(user, userProfileUpdateVm.CurrentPassword, userProfileUpdateVm.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(new UserProfileVm { UserProfileUpdateVm = userProfileUpdateVm });
                }

            }
            var identityResult = await userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(new UserProfileVm { UserProfileUpdateVm = userProfileUpdateVm });
            }
            return RedirectToAction("UserProfile", "Account", new { tab = "profile" });
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm forgotPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordVm);
            }
            var user = await userManager.FindByEmailAsync(forgotPasswordVm.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "There is no user with this email address.");
                return View(forgotPasswordVm);
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { email = forgotPasswordVm.Email, token = token }, Request.Scheme);

            using StreamReader reader = new StreamReader("wwwroot/templates/resetPassword.html");
                string html = reader.ReadToEnd();
                html = html.Replace("{{link}}", resetLink);
                html = html.Replace("{{username}}", user.UserName);
            
           emailService.Send(user.Email, "Reset your password", html);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ResetPassword(string email, string token)
        {
            ResetPasswordVm resetPasswordVm = new ResetPasswordVm
            {
                Email = email,
                Token = token
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVm resetPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVm);
            }
            var user = await userManager.FindByEmailAsync(resetPasswordVm.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "There is no user with this email address.");
                return View(resetPasswordVm);
            }
            var result = await userManager.ResetPasswordAsync(user, resetPasswordVm.Token, resetPasswordVm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordVm);
            }
            await userManager.UpdateSecurityStampAsync(user);
            return RedirectToAction("Login", "Account");
        }
    }
}

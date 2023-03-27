// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using activities.Extensions;
using activities.Models;
using activities.Pages;
using activities.Repository.Activities;
using activities.Repository.Countries;
using activities.Repository.Languages;
using activities.Repository.UserProfil;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.CodeAnalysis.Host;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace activities.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUserProfileReposiotry _profileRepository;
        private readonly ICountriesRepository _countriesSerivce;
        private readonly IActivitiesRepository _activitiesRepository;
        private readonly ILanguagesRepository _languagesRepository;
        private readonly IWebHostEnvironment _environment;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUserProfileReposiotry profileReposiotry,
            ICountriesRepository countriesSerivce,
            IActivitiesRepository activitiesRepository,
            ILanguagesRepository languagesRepository,
            IWebHostEnvironment Environment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _profileRepository = profileReposiotry;
            _countriesSerivce = countriesSerivce;
            _activitiesRepository = activitiesRepository;
            _languagesRepository = languagesRepository;
            _environment = Environment;
        }


        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public Country[] countries;
        public Language[] languages;
        public Activity[] activities;

        private async Task LoadResources()
        {
            await _countriesSerivce.InitData(GetBrowserLanguage.GetLanguageFromHeader(HttpContext));
            countries = _countriesSerivce.GetAll().ToArray();
            languages = _languagesRepository.GetAll().ToArray();
            activities = _activitiesRepository.GetAll().ToArray();
        }
        public async Task OnGetAsync(string returnUrl = null)
        {
            await LoadResources();
            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var userId = await _userManager.GetUserIdAsync(user);
                    UserProfile profile = null;
                    try
                    {
                        int UsersCount = await _profileRepository.CountUsers();

                        profile = await CreateProfileAsync(userId, UsersCount);
                        if (UsersCount == 0)
                        {
                            //the first user is the admin by default
                            var role = new IdentityRole { Id = "1", Name = "Admin"};
                            await _roleManager.CreateAsync(role);
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }
                    }
                    catch (Exception ex)
                    {
                        profile = null;
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }

                    if (profile != null)
                    {

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        await _userManager.DeleteAsync(user);
                    }
                    //var code = await _userManager.GenerateEmailConfimartionTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            await LoadResources();

            // If we got this far, something failed, redisplay form
            return Page();
        }
        private async Task<UserProfile> CreateProfileAsync(string userId, int userCount)
        {
            string base64Image = "";
            if (Input.Photo != null)
            {
                base64Image = await GenerateThumbnail.GetReducedImageBase64(Input.Photo);
            }
            return await _profileRepository.Add(new UserProfile
            {
                UserId = userId,
                About = Input.About,
                Approval = userCount == 0 ? 1 :0,
                ApprovalMessage = "",
                Available = Input.Available,
                Base64Photo = base64Image,
                City = Input.City,
                IdActivity = Input.IdActivity,
                IdCountry = Input.IdCountry,
                IdLanguage = Input.IdLanguage,
                Member = userCount == 0 ? 1 :0,
                Name = Input.Name
            });
        }
        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}

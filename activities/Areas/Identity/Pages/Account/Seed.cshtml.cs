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
using activities.Models;
using activities.Pages;
using activities.Repository.Activities;
using activities.Repository.UserProfil;
using activities.Resources;
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
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace activities.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class SeedModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly IUserProfileReposiotry _profileRepository;
        private readonly IActivitiesRepository _activitiesRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IStringLocalizer<Countries> _countriesLocalizer;
        private readonly IStringLocalizer<Languages> _languagesLocalizer;

        public SeedModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            IUserProfileReposiotry profileReposiotry,
            IActivitiesRepository activitiesRepository,
            IWebHostEnvironment Environment,
            IStringLocalizer<Countries> CountriesLocalizer,
            IStringLocalizer<Languages> LanguagesLocalizer)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _emailSender = emailSender;
            _profileRepository = profileReposiotry;
            _activitiesRepository = activitiesRepository;
            _environment = Environment;
            _countriesLocalizer = CountriesLocalizer;
            _languagesLocalizer = LanguagesLocalizer;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public Country[] countries;
        public Language[] languages;
        public Activity[] activities;

        private async Task LoadResources()
        {
           
            countries = _countriesLocalizer.GetAllStrings().Select( s=> new Country { Id=int.Parse(s.Name),Name=s.Value}).ToArray();
            languages = _languagesLocalizer.GetAllStrings().Select(s => new Language { Id = int.Parse(s.Name), Name = s.Value }).ToArray();
            activities = _activitiesRepository.GetAll().ToArray();
        }
        public async Task OnGetAsync()
        {
            await LoadResources();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadResources();
            int currentUsers = await _profileRepository.CountUsers();
            int[] CountriesIDs = countries.Select(s => s.Id).ToArray();
            int[] LanguagesIDs = languages.Select(s => s.Id).ToArray();
            int[] ActivitiesIDs = activities.Select(s => s.Id).ToArray();
            for (int i=1; i <= 100; i++)
            {
                Random random = new Random();

                IdentityUser user = CreateUser();

                await _userStore.SetUserNameAsync(user, "test_"+(currentUsers+i)+"@test.com", CancellationToken.None);
                await _emailStore.SetEmailAsync(user, "test_" + (currentUsers + i) + "@test.com", CancellationToken.None);
                var result = await _userManager.CreateAsync(user, "Test@123");
                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    await _profileRepository.Add(new UserProfile
                    {
                        UserId = userId,
                        About = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                        Approval = 0,
                        ApprovalMessage = "",
                        Available = random.Next(0, 2) == 0,
                        Base64Photo = "",
                        City = "test",
                        IdActivity = ActivitiesIDs[random.Next(0, activities.Length - 1)],
                        IdCountry = CountriesIDs[random.Next(0, countries.Length - 1)],
                        IdLanguage = LanguagesIDs[random.Next(0, languages.Length - 1)],                      
                        Member = 0,
                        Name = "test_"+ (currentUsers + i),
                        Other=""
                    });
                }
            }
            StatusMessage = "Seeding complete";
            return Page();
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

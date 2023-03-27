// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using activities.Extensions;
using activities.Models;
using activities.Pages;
using activities.Repository.Activities;
using activities.Repository.Countries;
using activities.Repository.Languages;
using activities.Repository.UserProfil;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace activities.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly IUserProfileReposiotry _profileRepository;
        private readonly ICountriesRepository _countriesSerivce;
        private readonly IActivitiesRepository _activitiesRepository;
        private readonly ILanguagesRepository _languagesRepository;
        private readonly IWebHostEnvironment _environment;
        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
                        IUserProfileReposiotry profileReposiotry,
            ICountriesRepository countriesSerivce,
            IActivitiesRepository activitiesRepository,
            ILanguagesRepository languagesRepository,
            IWebHostEnvironment Environment
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profileRepository = profileReposiotry;
            _countriesSerivce = countriesSerivce;
            _activitiesRepository = activitiesRepository;
            _languagesRepository = languagesRepository;
            _environment = Environment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

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
        private async Task loadProfile(string userId)
        {

            var profile = await _profileRepository.GetByUserId(userId);
            if (profile == null)
            {
                Input = new InputModel();
            }
            else
            {
                string baseImage = await _profileRepository.GetPhotoByUserId(userId);
                Input = new InputModel
                {
                    About = profile.About,
                    Approval = profile.Approval == 0 ? "Under Review" : profile.Approval == 1 ? "Yes" : "No",
                    Available = profile.Available,
                    Base64Photo = baseImage,
                    City = profile.City,
                    IdActivity = profile.IdActivity,
                    IdCountry = profile.IdCountry,
                    IdLanguage = profile.IdLanguage,
                    Member = profile.Member == 1 ? "Yes" : "No",
                    Name = profile.Name,
                    ApprovalMessage = profile.Approval == 2 ? profile.ApprovalMessage : ""
                };
            }

            await LoadResources();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            Username = await _userManager.GetUserNameAsync(user);
            await loadProfile(userId);
            return Page();
        }
        private async Task<UserProfile> MapInputToProfile()
        {
            string base64Image = "";
            if (Input.Photo != null)
            {
                base64Image = await GenerateThumbnail.GetReducedImageBase64(Input.Photo);
            }
            return new UserProfile
            {
                About = Input.About,
                Approval = 0,
                Available = Input.Available,
                Base64Photo = "data:image/png;base64, "+ base64Image,
                City = Input.City,
                IdActivity = Input.IdActivity,
                IdCountry = Input.IdCountry,
                IdLanguage = Input.IdLanguage,
                Name = Input.Name
            };
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            Username = await _userManager.GetUserNameAsync(user);
            if (!ModelState.IsValid)
            {
                await loadProfile(userId);
                return Page();
            }

            UserProfile profile =await MapInputToProfile();


            try
            {
                profile = await _profileRepository.UserEdit(userId,profile);
            }
            catch (Exception ex)
            {
                profile = null;
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}

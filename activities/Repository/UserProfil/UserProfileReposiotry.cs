﻿using activities.Data;
using activities.Models;
using Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace activities.Repository.UserProfil
{
    public class UserProfileReposiotry : IUserProfileReposiotry
    {
        private readonly ApplicationDbContext _db;

        public UserProfileReposiotry(ApplicationDbContext db)
        {
            _db = db;
        }
        private string MapRessource(int id,Dictionary<string, string> ressources)
        {
            try
            {
                return ressources[id.ToString()];
            }
            catch
            {
                return "NotFound_" + id;
            }
        }
        public async Task<IEnumerable<StatsModel>> GetStats(string GroupBy, Dictionary<string, string> ressources)
        {
            ;
            switch (GroupBy)
            {
                case "Country":
                    var statsCountries =await _db.UserProfiles.GroupBy(x => x.IdCountry).Select(s => new StatsModel
                    {
                        id = s.Key, 
                        total = s.Count(), 
                        review = s.Where(x => x.Approval == 0).Count(),
                        approved = s.Where(x => x.Approval == 1).Count(), 
                        notapproved = s.Where(x => x.Approval == 2).Count(), 
                        member = s.Where(x => x.Member == 1).Count(), 
                        available = s.Where(x => x.Available).Count()
                    }).ToListAsync(); 
                    foreach(var s in statsCountries)
                    {
                        s.name = MapRessource(s.id, ressources);
                    }
                    return statsCountries.OrderBy(o=>o.name);
                case "Language":
                    var statsLanguges =await _db.UserProfiles.GroupBy(x => x.IdLanguage).Select(s => new StatsModel
                    {
                        id = s.Key,
                        total = s.Count(),
                        review = s.Where(x => x.Approval == 0).Count(),
                        approved = s.Where(x => x.Approval == 1).Count(),
                        notapproved = s.Where(x => x.Approval == 2).Count(),
                        member = s.Where(x => x.Member == 1).Count(),
                        available = s.Where(x => x.Available).Count()
                    }).ToListAsync();
                    foreach (var s in statsLanguges)
                    {
                        s.name = MapRessource(s.id, ressources);
                    }
                    return statsLanguges.OrderBy(o => o.name); ;
                default:
                    return new List<StatsModel>();
            }
        }
        public async Task<int[]> GetProfilesActivites()
        {
            return await _db.UserProfiles.Distinct().Select(s => s.IdActivity).ToArrayAsync();
        }
        public async Task<string[]> GetProfilesCountries()
        {
            return await _db.UserProfiles.Distinct().Select(s => s.IdCountry.ToString()).ToArrayAsync();
        }
        public async Task<string[]> GetProfilesLanguages(int idCOuntry)
        {
            return await _db.UserProfiles.Where(p => idCOuntry == 0 ? true : p.IdCountry == idCOuntry).Distinct().Select(s => s.IdLanguage.ToString()).ToArrayAsync();
        }
        public async Task<int> CountUsers()
        {
            return await _db.UserProfiles.CountAsync();
        }
        public async Task<string> GetPhotoByUserId(string userId)
        {
            try
            {
                var photo = await _db.UserProfiles.Select(s => new { id = s.UserId, photo = s.Base64Photo }).Where(p => p.id == userId).FirstOrDefaultAsync();
                if (photo == null)
                {
                    return "NULL";
                }
                return string.IsNullOrEmpty(photo.photo) ? "NULL" : photo.photo;
            }
            catch (Exception ex)
            {
                return "NULL";
            }

        }
        public async Task<UserProfile> GetByUserId(string userId)
        {
            return await _db.UserProfiles.Select(s => new UserProfile
            {
                UserId = s.UserId,
                Name = s.Name,
                About = s.About,
                Approval = s.Approval,
                ApprovalMessage = s.ApprovalMessage,
                Available = s.Available,
                City = s.City,
                Id = s.Id,
                IdActivity = s.IdActivity,
                IdCountry = s.IdCountry,
                IdLanguage = s.IdLanguage,
                Member = s.Member,
                Other = s.Other
            }).Where(p => p.UserId == userId).FirstOrDefaultAsync();
        }
        public async Task<int> CountSearchProfiles(SearchModel model)
        {
            return await _db.UserProfiles.Where(p =>
                 (model.IdCountry == 0 ? true : p.IdCountry == model.IdCountry) &&
                 (model.IdActivity == 0 ? true : p.IdActivity == model.IdActivity) &&
                 (model.IdLanguage == 0 ? true : p.IdLanguage == model.IdLanguage) &&
                 (model.Approval == -1 ? true : p.Approval == model.Approval) &&
                 (model.Member == -1 ? true : p.Member == model.Member) &&
                 (model.Available == -1 ? true : p.Available == (model.Available == 1)) &&
                 (string.IsNullOrEmpty(model.City) ? true : p.City.ToLower().Contains(model.City.ToLower()))
             ).CountAsync();
        }
        public async Task<UserProfile[]> SearchProfiles(int pageNumber, SearchModel model)
        {
            return await _db.UserProfiles
                .Select(s => new UserProfile
                {
                    UserId = s.UserId,
                    Name = s.Name,
                    Approval = s.Approval,
                    ApprovalMessage = s.ApprovalMessage,
                    Available = s.Available,
                    City = s.City,
                    Id = s.Id,
                    IdActivity = s.IdActivity,
                    IdCountry = s.IdCountry,
                    IdLanguage = s.IdLanguage,
                    Member = s.Member,
                    Base64Photo = s.Base64Photo,
                })
                .Where(p =>
                    (model.IdCountry == 0 ? true : p.IdCountry == model.IdCountry) &&
                    (model.IdActivity == 0 ? true : p.IdActivity == model.IdActivity) &&
                    (model.IdLanguage == 0 ? true : p.IdLanguage == model.IdLanguage) &&
                    (model.Approval == -1 ? true : p.Approval == model.Approval) &&
                    (model.Member == -1 ? true : p.Member == model.Member) &&
                    (model.Available == -1 ? true : p.Available == (model.Available == 1)) &&
                    (string.IsNullOrEmpty(model.City) ? true : p.City.ToLower().Contains(model.City.ToLower()))
                )
                .Skip((pageNumber - 1) * model.PageSize)
                .Take(model.PageSize)
                .OrderBy(o => o.Name).ToArrayAsync();
        }
        public async Task<UserProfile> Add(UserProfile profile)
        {
            _db.UserProfiles.Add(profile);
            await _db.SaveChangesAsync();
            return profile;
        }
        public async Task<UserProfile> UserEdit(string userId, UserProfile profile)
        {
            var userProfile = await _db.UserProfiles.Where(p => p.UserId == userId).FirstOrDefaultAsync();
            if (userProfile == null)
            {
                throw new Exception("Profile not found");
            }

            userProfile.About = profile.About;
            userProfile.Approval = profile.Approval;
            userProfile.Other = profile.Other;
            userProfile.Available = profile.Available;
            if (!string.IsNullOrEmpty(profile.Base64Photo))
            {
                userProfile.Base64Photo = profile.Base64Photo;
            }
            if (!string.IsNullOrEmpty(profile.ApprovalMessage))
            {
                userProfile.ApprovalMessage = profile.ApprovalMessage;
            }
            userProfile.City = profile.City;
            userProfile.IdActivity = profile.IdActivity;
            userProfile.IdCountry = profile.IdCountry;
            userProfile.IdLanguage = profile.IdLanguage;
            userProfile.Member = profile.Member;
            userProfile.Name = profile.Name;

            await _db.SaveChangesAsync();
            return userProfile;
        }
        public async Task<bool> Delete(string userId)
        {
            var userProfile = await _db.UserProfiles.FindAsync();
            _db.UserProfiles.Remove(userProfile);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

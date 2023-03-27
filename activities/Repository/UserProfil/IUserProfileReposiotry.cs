﻿using activities.Models;

namespace activities.Repository.UserProfil
{
    public interface IUserProfileReposiotry
    {
        Task<string> GetPhotoByUserId(string userId);
        Task<int> CountUsers();
        Task<UserProfile> Add(UserProfile profile);
        Task<bool> Delete(string userId);
        Task<UserProfile> UserEdit(string userId, UserProfile profile);
        Task<UserProfile> AdminEdit(string userId, ApprovalModel profile);
        Task<UserProfile> GetByUserId(string userId);
        Task<UserProfile[]> SearchProfiles(int pageNumber, SearchModel searchModel);
        Task<int[]> GetProfilesActivites();
        Task<int[]> GetProfilesCountries();
        Task<int[]> GetProfilesLanguages(int idCountry);
        Task<int> CountSearchProfiles(SearchModel model);
    }
}
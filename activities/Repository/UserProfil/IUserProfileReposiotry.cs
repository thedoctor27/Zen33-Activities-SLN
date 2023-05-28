using activities.Models;

namespace activities.Repository.UserProfil
{
    public interface IUserProfileReposiotry
    {
        Task<List<UserProfile>> Last20Registred();
        Task<IEnumerable<StatsModel>> GetStats(string GroupBy, Dictionary<string, string> ressources);
        Task<string> GetPhotoByUserId(string userId);
        Task<int> CountUsers();
        Task<UserProfile> Add(UserProfile profile);
        Task<bool> Delete(string userId);
        Task<UserProfile> UserEdit(string userId, UserProfile profile);
        Task<UserProfile> GetByUserId(string userId);
        Task<UserProfile[]> SearchProfiles(int pageNumber, SearchModel searchModel);
        Task<int[]> GetProfilesActivites();
        Task<string[]> GetProfilesCountries();
        Task<string[]> GetProfilesLanguages(int idCountry);
        Task<int> CountSearchProfiles(SearchModel model);
    }
}
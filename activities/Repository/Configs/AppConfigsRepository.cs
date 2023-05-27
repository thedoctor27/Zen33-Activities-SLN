using activities.Data;
using activities.Models;
using Microsoft.EntityFrameworkCore;

namespace activities.Repository.Configs
{
    public class AppConfigsRepository : IAppConfigsRepository
    {
        private readonly ApplicationDbContext _db;

        public AppConfigsRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task SetTestsUserMode(bool TestUserEnable)
        {
            var config = await _db.AppConfigs.FirstOrDefaultAsync();
            config.EnableTestUser = TestUserEnable;
            await _db.SaveChangesAsync();
        }
        public async Task<bool> GetTestsUserMode()
        {
            var config = await _db.AppConfigs.FirstOrDefaultAsync();
            if(config == null) {
                await InitConfig();
                return false;
            }
            return config.EnableTestUser;
        }
        private async Task InitConfig()
        {
            var config =await _db.AppConfigs.FirstOrDefaultAsync();
            if (config == null)
            {
                _db.AppConfigs.Add(new AppConfig
                {
                    EnableTestUser = false,
                });
                _db.SaveChanges();
            }
        }
    }
}

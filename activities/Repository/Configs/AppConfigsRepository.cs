using activities.Data;
using activities.Models;
using Microsoft.EntityFrameworkCore;

namespace activities.Repository.Configs
{
    public class AppConfigsRepository : IAppConfigsRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

        public AppConfigsRepository(IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public async Task SetTestsUserMode(bool TestUserEnable)
        {
            using (var _db = _dbFactory.CreateDbContext())
            {
                var config = await _db.AppConfigs.FirstOrDefaultAsync();
                config.EnableTestUser = TestUserEnable;
                await _db.SaveChangesAsync();
            }

        }
        public async Task<bool> GetTestsUserMode()
        {
            using (var _db = _dbFactory.CreateDbContext())
            {
                var config = await _db.AppConfigs.FirstOrDefaultAsync();
                if (config == null)
                {
                    await InitConfig();
                    return false;
                }
                return config.EnableTestUser;
            }

        }
        private async Task InitConfig()
        {
            using (var _db = _dbFactory.CreateDbContext())
            {
                var config = await _db.AppConfigs.FirstOrDefaultAsync();
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
}

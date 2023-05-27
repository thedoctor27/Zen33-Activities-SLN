namespace activities.Repository.Configs
{
    public interface IAppConfigsRepository
    {
        Task<bool> GetTestsUserMode();
        Task SetTestsUserMode(bool TestUserEnable);
    }
}
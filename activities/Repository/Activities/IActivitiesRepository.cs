using activities.Models;

namespace activities.Repository.Activities
{
    public interface IActivitiesRepository
    {
        IEnumerable<Activity> GetAll();
        Activity GetById(int id);
        IEnumerable<Activity> GetByPage(int pageNumber, int pageSize);
    }
}
using activities.Data;
using activities.Models;
using Microsoft.EntityFrameworkCore;

namespace activities.Repository.Activities
{
    public class ActivitiesRepository : IActivitiesRepository
    {
        private IEnumerable<Activity> Activities = new List<Activity> {
            new Activity{ Id = 1,Name = "Activity 1"},
            new Activity{ Id = 2,Name = "Activity 2"},
            new Activity{ Id = 3,Name = "Activity 3"},
            new Activity{ Id = 4,Name = "Activity 4"},
            new Activity{ Id = 5,Name = "Activity 5"},
            new Activity{ Id = 6,Name = "Activity 6"},
        };
        public ActivitiesRepository()
        {
        }
        public IEnumerable<Activity> GetByPage(int pageNumber, int pageSize)
        {
            return Activities
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(o => o.Name);
        }
        public IEnumerable<Activity> GetAll()
        {
            return Activities.OrderBy(o => o.Name);
        }
        public Activity GetById(int id)
        {
            var act = Activities.Where(a => a.Id == id).FirstOrDefault();
            if(act == null)
            {
                return new Activity { Id = 0, Name = "NotFound_"+id };
            }
            return act;
        }
    }
}

using activities.Models;

namespace activities.Repository.Languages
{
    public interface ILanguagesRepository
    {
        IEnumerable<Language> GetAll();
        Language GetById(int id);
        IEnumerable<Language> GetByPage(int pageNumber, int pageSize);
    }
}
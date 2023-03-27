using activities.Models;

namespace activities.Repository.Countries
{
    public interface ICountriesRepository
    {
        IEnumerable<Country> GetAll();
        Country GetById(int id);
        IEnumerable<Country> GetByPage(int pageNumber, int pageSize);
        Task InitData(string language);
    }
}
using activities.Data;
using activities.Models;
using Microsoft.EntityFrameworkCore;

namespace activities.Repository.Countries
{
    public class CountriesRepository : ICountriesRepository
    {
        private List<Country> Countries;

        public CountriesRepository()
        {

        }
        public async Task InitData(string language)
        {
            string DataFile = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Countries", language+ ".txt");
            if (!File.Exists(DataFile))
            {
                DataFile = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Countries", "EN.txt");
            }
            Countries = new List<Country>();
            using (StreamReader reader = new StreamReader(DataFile))
            {
                int l = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        Countries.Add(new Country
                        {
                            Id = int.Parse(line.Split(" ")[0]),
                            Name = line.Substring(line.IndexOf(" ") + 1)
                        });
                    }
                    catch
                    {
                        throw new Exception("Unable to read line number " + l + ", the line must in this fromat id NAME");
                    }
                    l++;
                }
            }
        }
        public IEnumerable<Country> GetByPage(int pageNumber, int pageSize)
        {
            if(Countries == null) {
                throw new Exception("Countries list has not been initialised");
            }
            return Countries
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(o => o.Name);
        }
        public IEnumerable<Country> GetAll()
        {
            if (Countries == null)
            {
                throw new Exception("Countries list has not been initialised");
            }
            return Countries.OrderBy(o => o.Name);
        }
        public Country GetById(int id)
        {
            if (Countries == null)
            {
                throw new Exception("Countries list has not been initialised");
            }
            return Countries.Where(c => c.Id == id).FirstOrDefault();
        }
    }
}

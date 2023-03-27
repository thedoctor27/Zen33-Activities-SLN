using activities.Data;
using activities.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace activities.Repository.Languages
{
    public class LanguagesRepository : ILanguagesRepository
    {
        private List<Language> Languages;

        public LanguagesRepository()
        {
            string DataFile = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Languages", "data.txt");
            Languages = new List<Language>();
            using (StreamReader reader = new StreamReader(DataFile))
            {
                int l = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    try
                    {
                        Languages.Add(new Language
                        {
                            Id = int.Parse(line.Split(" ")[0]),
                            Name = line.Substring(line.IndexOf(" ") + 1)
                        });
                    }
                    catch
                    {
                        throw new Exception("Unable to read line number " + l + ", the line must in this fromat id iso_lang NAME");
                    }
                    l++;
                }
            }
        }
        public IEnumerable<Language> GetByPage(int pageNumber, int pageSize)
        {
            if (Languages == null)
            {
                throw new Exception("Languages list has not been initialised");
            }
            return Languages
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(o => o.Name);
        }
        public IEnumerable<Language> GetAll()
        {
            if (Languages == null)
            {
                throw new Exception("Languages list has not been initialised");
            }
            return Languages.OrderBy(o => o.Name);
        }
        public Language GetById(int id)
        {
            if (Languages == null)
            {
                throw new Exception("Languages list has not been initialised");
            }
            return Languages.Where(c => c.Id == id).FirstOrDefault();
        }
    }
}

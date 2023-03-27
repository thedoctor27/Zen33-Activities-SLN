using System.ComponentModel.DataAnnotations;

namespace activities.Models
{
    public class Language
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
    }
}

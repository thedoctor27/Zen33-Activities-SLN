using System.ComponentModel.DataAnnotations;

namespace activities.Models
{
    public class Activity
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}

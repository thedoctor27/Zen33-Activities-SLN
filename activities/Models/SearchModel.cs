namespace activities.Models
{
    public class SearchModel
    {
        public int IdCountry { get; set; }
        public int IdLanguage { get; set; }
        public int IdActivity { get; set; }
        public string City { get; set; }
        public int PageSize { get; set; }
        public int Approval { get; set; }
        public int Member { get; set; }
        public int Available { get; set; }
    }
}

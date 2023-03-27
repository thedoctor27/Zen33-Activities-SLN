using System.ComponentModel.DataAnnotations;

namespace activities.Models
{
    public class ApprovalModel
    {
        public int Approval { get; set; }
        public int Member { get; set; }


        [MaxLength(200)]
        public string? ApprovalMessage { get; set; }
    }
}

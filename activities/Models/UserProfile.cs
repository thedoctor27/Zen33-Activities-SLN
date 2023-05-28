using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace activities.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(100)]

        public string City { get; set; }
        [MaxLength(2000)]
        public string About { get; set; }

        [MaxLength(65)]
        public string Other { get; set; }

        [AllowNull]
        public string Base64Photo { get; set; }

        public int Approval { get; set; }
        public int Member { get; set; }


        [MaxLength(200)]
        public string ApprovalMessage { get; set; }
        public bool Available { get; set; }
        public int IdActivity { get; set; }
        public int IdCountry { get; set; }
        public int IdLanguage { get; set; }

        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Test { get; set; }
        public virtual IdentityUser User { get; set; }

    }
}

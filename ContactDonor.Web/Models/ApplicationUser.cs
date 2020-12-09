using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactDonor.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string BloodGroup { get; set; }
        public string City { get; set; }
    }
}

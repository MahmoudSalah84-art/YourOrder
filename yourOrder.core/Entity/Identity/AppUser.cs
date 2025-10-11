using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace yourOrder.Core.Entity.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public  ICollection<Address> Addresses { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.Identity;

namespace yourOrder.Core.Interfaces
{
    internal interface ITokenService
    {
        string CreateToken(AppUser user, IList<string> roles);
    }
}

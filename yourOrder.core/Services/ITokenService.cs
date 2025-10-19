using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Entity.Identity;

namespace yourOrder.Core.Services
{
    public interface ITokenService
    {

        string CreateToken(AppUser user, IList<string > roles);
        RefreshToken GenerateRefreshToken();
    }
}

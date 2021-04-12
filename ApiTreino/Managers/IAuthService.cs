using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using ApiTreino.Models;

namespace ApiTreino.Managers
{
    public interface IAuthService
    {
        string SecretKey { set; get; }

        bool IsTokenValid(string token);

        string GenerateToken(UserInfoViewModel model);

        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
using Business.Authentication.Models;
using Data.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Authentication.Interfaces
{
    public interface IJwtService
    {
        AuthenticateResponse GenerateJwtToken(User user, int validTime);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Authentication.Interfaces
{
    public interface IJwtSettings
    {
        string Secret { get; }
        string Audience { get; }
        string Issuer { get; }
        int Seconds { get; }
    }
}

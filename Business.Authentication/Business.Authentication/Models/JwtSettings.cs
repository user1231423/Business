using Business.Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Authentication.Models
{
    public class JwtSettings : IJwtSettings
    {
        public string Secret { get; set; }
    }
}

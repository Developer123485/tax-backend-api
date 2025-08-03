using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class AppSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
    }
    public class Smtp
    {
        public string From { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
}

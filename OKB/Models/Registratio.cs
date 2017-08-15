using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OKB.Models
{
    public class Registratio
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string  Password { get; set; }
        public string  ConfirmPassword { get; set; }
        public int Mobile { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
    }
}
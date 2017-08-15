using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OKB.Models
{
    public class ULogin
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
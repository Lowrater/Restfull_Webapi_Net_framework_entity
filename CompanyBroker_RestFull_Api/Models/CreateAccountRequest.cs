using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class CreateAccountRequest
    {
        public int CompanyId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
    }
}
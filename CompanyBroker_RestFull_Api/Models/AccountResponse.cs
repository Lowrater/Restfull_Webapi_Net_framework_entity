using CompanyBroker.DBSData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    /// <summary>
    /// Account model response, to ensure not to parse sensitive data like passwords
    /// </summary>
    public class AccountResponse
    {
        public int CompanyId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public AccountResponse(CompanyAccount account)
        {
            CompanyId = account.CompanyId;
            Email = account.Email;
            Username = account.Username;
            Active = account.Active;
        }
    }
}
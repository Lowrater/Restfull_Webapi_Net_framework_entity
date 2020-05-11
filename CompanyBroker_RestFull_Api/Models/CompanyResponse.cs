using CompanyBroker.DBSData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class CompanyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public CompanyResponse(Company company)
        {
            Id = company.Id;
            Name = company.Name;
            Active = company.Active;
        }
    }
}
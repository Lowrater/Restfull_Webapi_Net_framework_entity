using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class CompanyChangeBalanceRequest
    {
        public int companyId { get; set; }
        public decimal price { get; set; }
        public bool increaseBalance { get; set; }

    }
}
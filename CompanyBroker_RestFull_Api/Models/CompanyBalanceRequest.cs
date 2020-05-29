using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class CompanyBalanceRequest
    {
        public int companyId { get; set; }
        public decimal priceAmount { get; set; }
        public bool increase { get; set; }

    }
}
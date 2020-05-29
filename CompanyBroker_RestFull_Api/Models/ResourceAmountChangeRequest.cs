using CompanyBroker.DBSData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class ResourceAmountChangeRequest
    {
        public CompanyResource companyResource { get; set; }
        public bool increase { get; set; }
    }
}
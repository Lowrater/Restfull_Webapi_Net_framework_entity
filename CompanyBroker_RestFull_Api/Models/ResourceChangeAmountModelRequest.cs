using CompanyBroker.DBSData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class ResourceChangeAmountModelRequest
    {
        public bool IncreaseAmount { get; set; }
        public int companyId { get; set; }
        public int resourceId { get; set; }
    }
}
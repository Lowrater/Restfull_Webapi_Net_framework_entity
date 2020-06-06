using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class ResourceDescriptionModel
    {
        public int ResourceId { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class CollectionFilterRequest
    {
        public IEnumerable<string> CompanyChoices { get; set; }
        public IEnumerable<string> ProductTypeChoices { get; set; }
        public IEnumerable<string> ProductNameChoices { get; set; }
        public string SearchWord { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Models
{
    public class CollectionFilterRequest
    {
        public int[] CompanyChoices { get; set; }
        public string[] ProductTypeChoices { get; set; }
        public string[] ProductNameChoices { get; set; }

        public string SearchWord { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal HigestPrice { get; set; }

    }
}
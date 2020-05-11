using CompanyBroker.DBSData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CompanyBroker_RestFull_Api.Controllers
{
    public class ResourcesController : ApiController
    {

        [HttpGet]
        public async Task<IEnumerable<CompanyResource>> Get()
        {
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                //-- Fetches the account list
                return await entitys.CompanyResources.ToListAsync();
            }

        }
    }
}

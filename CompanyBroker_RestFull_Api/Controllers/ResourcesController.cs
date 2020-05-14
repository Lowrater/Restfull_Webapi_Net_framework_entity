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
        /// <summary>
        /// Fetches all 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Fetches all resources based by CompanyId
        /// </summary>
        /// <returns></returns>
        [Route("api/GetResourcesByCompanyId")]
        [HttpGet]
        public async Task<IEnumerable<CompanyResource>> GetResourcesByCompanyId(int companyId)
        {
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                //-- Fetches the account list
                return await entitys.CompanyResources.Where(c => c.CompanyId == companyId).ToListAsync();
            }
        }

        /// <summary>
        /// Fetches a single product type by a specific type
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CompanyResource> Get(string productType)
        {
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                var responseResource = await entitys.CompanyResources.FirstOrDefaultAsync(p => p.ProductType == productType);

                return new CompanyResource()
                {
                    ProductName = responseResource.ProductName,
                    ResourceId = responseResource.ResourceId,
                    ProductType = responseResource.ProductType,
                    Price = responseResource.Price,
                    Amount = responseResource.Amount,
                    Active = responseResource.Active,
                    CompanyId = responseResource.CompanyId
                };
            }
        }

        /// <summary>
        /// Fetches a single product type by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CompanyResource> Get(int id)
        {
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                var responseResource = await entitys.CompanyResources.FirstOrDefaultAsync(p => p.ResourceId == id);

               return new CompanyResource()
                {
                    ProductName = responseResource.ProductName,
                    ResourceId = responseResource.ResourceId,
                    ProductType = responseResource.ProductType,
                    Price = responseResource.Price,
                    Amount = responseResource.Amount,
                    Active = responseResource.Active,
                    CompanyId = responseResource.CompanyId
                };
            }
        }


        /// <summary>
        /// Fetches all product types from a list
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        [Route("api/GetProductAllTypes")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetProductAllTypes()
        {
            //-- creates a new list
            var list = new List<string>();
            //-- opens an connection to the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                //-- fetches all resources
                var responseList = await entitys.CompanyResources.ToListAsync();

                //-- loops the content
                foreach (var product in responseList)
                {
                    //-- Checks if the list contains the product
                    if(!list.Contains(product.ProductType))
                    {
                        //-- adds the product type
                        list.Add(product.ProductType);
                    }
                }
            }
            //-- returns the list
            return list;
        }

        /// <summary>
        /// Fetches all product names based on product type. Needs an List of product types
        /// </summary>
        /// <param name="productname"></param>
        /// <returns></returns>
        [Route("api/GetAllProductNamesByTypes")]
        [HttpGet]
        public async Task<IEnumerable<string>> GetAllProductNamesByTypes(IEnumerable<string> productTypes)
        {
            //-- creates a new list
            var list = new List<string>();
            //-- connecting to the database 
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                //-- loops the recieved result list
                foreach (string type in productTypes)
                {
                    //-- loops fetches all products based on the type
                    var responseList = await entitys.CompanyResources.Where(p => p.ProductType == type).ToListAsync();
                    //-- loops the responseList for products based on the type
                    foreach (var product in responseList)
                    {
                        //-- adds it to the list
                        list.Add(product.ProductName);
                    }
                }
            }
            //-- returns the list
            return list;
        }
    }
}

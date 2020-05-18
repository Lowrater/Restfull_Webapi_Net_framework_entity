using CompanyBroker.DBSData;
using CompanyBroker_RestFull_Api.Addons;
using CompanyBroker_RestFull_Api.Models;
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
        /// Fetches all resources based by one CompanyId
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


        [Route("api/GetResourcesByListFilters")]
        [HttpPost]
        public async Task<IEnumerable<CompanyResource>> GetResourcesByListFilters(CollectionlistRequest collectionListRequest)
        {
            //-- The new resource list
            var resourceList = new List<CompanyResource>();

            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {              
                resourceList = await entitys.CompanyResources.ToListAsync();
            }

            //-- Filtering the list depending on the inputs from the collectionListRequest
            if (collectionListRequest.ProductNameChoices.Count() != 0)
            {
                resourceList = resourceList.Where(r => collectionListRequest.ProductNameChoices.Any(pn => pn == r.ProductName)).ToList();
            }

            if (collectionListRequest.ProductTypeChoices.Count() != 0)
            {
                resourceList = resourceList.Where(r => collectionListRequest.ProductTypeChoices.Any(pt => pt == r.ProductType)).ToList();
            }

            if(collectionListRequest.CompanyChoices.Count() != 0)
            {
                resourceList = resourceList.Where(r => collectionListRequest.CompanyChoices.Any(cc => cc.Substring(0,1).ConvertToInt() == r.CompanyId)).ToList();
            };

            //-- return the filtered list
            return resourceList;
        }



        /// <summary>
        /// Fetches all resources based by multiple CompanyId
        /// </summary>
        /// <returns></returns>
        [Route("api/GetResourcesByCompanyIds")]
        [HttpPost]
        public async Task<IEnumerable<CompanyResource>> GetResourcesByCompanyId(IEnumerable<int> companyId)
        {
            var resourceList = new List<CompanyResource>();
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                foreach (int id in companyId)
                {
                    var responseList = await entitys.CompanyResources.Where(c => c.CompanyId == id).ToListAsync();

                    foreach (var resource in responseList)
                    {
                        if(!resourceList.Contains(resource))
                        {
                            resourceList.Add(resource);
                        }
                    }
                }
            }
            //-- Returns the list the account list
            return resourceList;
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
        [HttpPost]
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

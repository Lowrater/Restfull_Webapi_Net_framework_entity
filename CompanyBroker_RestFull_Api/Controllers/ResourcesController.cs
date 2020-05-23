using CompanyBroker.DBSData;
using CompanyBroker_RestFull_Api.Addons;
using CompanyBroker_RestFull_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
        public async Task<IList<CompanyResource>> Get()
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
        public async Task<IList<CompanyResource>> GetResourcesByCompanyId(int companyId)
        {
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                //-- Fetches the account list
                return await entitys.CompanyResources.Where(c => c.CompanyId == companyId).ToListAsync();
            }
        }

        /// <summary>
        /// Fetches all resources based on filters
        /// </summary>
        /// <param name="collectionFilterRequest"></param>
        /// <returns></returns>
        [Route("api/GetResourcesByListFilters")]
        [HttpPost]
        public async Task<IList<CompanyResource>> GetResourcesByListFilters(CollectionFilterRequest collectionFilterRequest)
        {
            //-- The new resource list
            var resourceList = new List<CompanyResource>();
            //-- Values from the CollectionFilterRequest
            var productNames = collectionFilterRequest.ProductNameChoices;
            var productTypes = collectionFilterRequest.ProductTypeChoices;
            var companyIds = collectionFilterRequest.CompanyChoices;
            var searchWords = collectionFilterRequest.SearchWord;
            var LowestPrice = collectionFilterRequest.LowestPriceChoice;
            var HigestPirce = collectionFilterRequest.HigestPriceChoice;
            var BulkBuy = collectionFilterRequest.BulkChoice;
            var PartnersOnly = collectionFilterRequest.Partners_OnlyChoice;

            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                //-- Fetching the results. - As queryAble, lets the database struggle with the filtering than the application
                var results = entitys.CompanyResources.AsQueryable();

                //-- Filtering the list depending on the inputs from the collectionListRequest, and checking if they have any value
                if (productNames.Any())
                {
                    results = results.Where(r => productNames.Contains(r.ProductName));
                }

                if (productTypes.Any())
                {
                    results = results.Where(r => productTypes.Contains(r.ProductType));
                }

                if (companyIds.Any())
                {
                    results = results.Where(r => companyIds.Contains(r.CompanyId));
                }

                if (LowestPrice != 0)
                {
                    results = results.Where(r => LowestPrice < r.Price);
                }

                if (HigestPirce != 0)
                {
                    results = results.Where(r => HigestPirce > r.Price);
                }

                //-- word filtering
                if (!string.IsNullOrWhiteSpace(searchWords))
                {
                    //-- Splitting the word by spaces, into array for propper filtering
                    var Words = searchWords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //-- Filtering the results based on the array of words
                    results = results.Where(r => Words.Any(s => r.ProductName.ToLower().Contains(s) || r.ProductType.ToLower().Contains(s)));
                }

                resourceList = await results.ToListAsync();
            }

            //-- return the filtered list
            return resourceList;
        }

        /// <summary>
        /// Fetches all resources based by multiple CompanyId
        /// </summary>
        /// <returns></returns>
        [Route("api/GetResourcesByCompanyIds")]
        [HttpGet]
        public async Task<IList<CompanyResource>> GetResourcesByCompanyId(IEnumerable<int> companyId)
        {
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                return await entitys.CompanyResources.Where(r => companyId.Any(i => i == r.CompanyId)).ToListAsync();
            }
        }

        /// <summary>
        /// Fetches a list of resources based on a single search word
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IList<CompanyResource>> Get(string searchWord)
        {
            //-- opens an connection to the database with the entity 
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                //-- Fetches the collection based on the words in product name and type
                return await entitys.CompanyResources.Where(r => r.ProductName.ToLower().Contains(searchWord.ToLower()) || r.ProductType.ToLower().Contains(searchWord.ToLower())).ToListAsync();
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
        public async Task<List<string>> GetProductAllTypesAsync()
        {
            ////-- opens an connection to the database
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
                //-- fetches all resources, for product types only, and only want one of each.
                return await entitys.CompanyResources.Select(pt => pt.ProductType).Distinct().ToListAsync();
            }
        }

        /// <summary>
        /// Fetches all product names based on product type. Needs an List of product types
        /// </summary>
        /// <param name="productname"></param>
        /// <returns></returns>
        [Route("api/GetAllProductNamesByTypes")]
        [HttpPost]
        public async Task<IList<string>> GetAllProductNamesByTypes(IEnumerable<string> productTypes)
        {
            //-- connecting to the database 
            using (var entitys = new CompanyBrokerResourcesEntities())
            {
               return await entitys.CompanyResources.Where(p => productTypes.Any(t => t == p.ProductType)).Select(x => x.ProductName).ToListAsync();
            }

        }
    }
}

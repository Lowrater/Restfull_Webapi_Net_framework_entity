using CompanyBroker.DBSData;
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
    public class ResourceDescriptionController : ApiController
    {

        #region Get methods
        /// <summary>
        /// Fetches the resource description based on the Resource ID
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResourceDescription> Get(int resourceId)
        {
            using (var entity = new CompanyBrokerResourceDescriptionEntities())
            {
                return await entity.ResourceDescriptions.FirstOrDefaultAsync(r => r.ResourceId == resourceId);
            }
        }

        #endregion


        #region Post methods

        /// <summary>
        /// Adds and description object (row) to the table
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> Post (ResourceDescriptionModel description)
        {
            using (var entity = new CompanyBrokerResourceDescriptionEntities())
            {
                if(description != null)
                {
                    //-- Creates new resource based on the description
                    var desc = new ResourceDescription()
                    {
                        ResourceId = description.ResourceId,
                        Description = description.Description
                    };
                    //-- adds it to the database
                    entity.ResourceDescriptions.Add(desc);
                    await entity.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion


        #region Put Methods
        /// <summary>
        /// Updates an existing product description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> Put (ResourceDescriptionModel description)
        {
            using (var entity = new CompanyBrokerResourceDescriptionEntities())
            {
                if(description != null)
                {
                    //-- Finds the resource
                    var desc = entity.ResourceDescriptions.Where(a => a.ResourceId == description.ResourceId).Single<ResourceDescription>();
                    //-- Changes the fetched resource object
                    desc.Description = description.Description;
                    //-- Tells the entity framework, that we made a change on the fetched object.
                    entity.Entry(desc).State = EntityState.Modified;
                    //-- Trys to apply the changes to the database
                    await entity.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion


    }
}

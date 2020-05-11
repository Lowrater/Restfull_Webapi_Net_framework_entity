using CompanyBroker.DBSData;
using CompanyBroker_RestFull_Api.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace CompanyBroker_RestFull_Api.Controllers
{
    /// <summary>
    /// Company controller, handling all the data from the company table.
    /// </summary>
    public class CompaniesController : ApiController
    {
        /// <summary>
        /// Returns all companies from the database in a list, through a company model without the balance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<CompanyResponse>> Get()
        {
            //-- Creates new list with content of CompanyResponse
            var companyList = new List<CompanyResponse>();
            //-- Uses the CompanyBrokerCompaniesEntities to connect to the database
            using (var entitys = new CompanyBrokerCompaniesEntities())
            {
                //-- Fetches all companies 
                var companies = await entitys.Companies.ToListAsync();
                //-- Loops the results
                foreach (Company company in companies)
                {
                    //-- Adds the restult through the model, to remove sensitive data
                    companyList.Add(new CompanyResponse(company));
                }
            }
            //-- returns the list
            return companyList;
        }

        /// <summary>
        /// Returns an company based on a ID number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CompanyResponse> Get(int id)
        {
            //-- sets an CompanyResponse
            CompanyResponse companyResponse;
            //-- Uses the CompanyBrokerCompaniesEntities to connect to the database
            using (var entitys = new CompanyBrokerCompaniesEntities())
            {
                //-- Fetches an company based on the Id user has entered
                var company = await entitys.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);
                //-- creates the new companyResponse based on the information fetched
                companyResponse = new CompanyResponse(company);
            }
            //-- Returns the results
            return companyResponse;
        }
    }
}

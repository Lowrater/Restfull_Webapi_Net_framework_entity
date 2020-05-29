using CompanyBroker.DBSData;
using CompanyBroker_RestFull_Api.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;

namespace CompanyBroker_RestFull_Api.Controllers
{
    /// <summary>
    /// Company controller, handling all the data from the company table.
    /// </summary>
    public class CompaniesController : ApiController
    {

        #region Get Methods
        /// <summary>
        /// Returns all companies from the database in a list, through a company model without the balance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IList<CompanyResponse>> Get()
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

        /// <summary>
        /// Returns an company based on a ID number
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<CompanyResponse> Get(string companyName)
        {
            //-- sets an CompanyResponse
            CompanyResponse companyResponse;
            //-- Uses the CompanyBrokerCompaniesEntities to connect to the database
            using (var entitys = new CompanyBrokerCompaniesEntities())
            {
                //-- Fetches an company based on the Id user has entered
                var company = await entitys.Companies.FirstOrDefaultAsync(c => c.CompanyName == companyName);
                //-- creates the new companyResponse based on the information fetched
                companyResponse = new CompanyResponse(company);
            }
            //-- Returns the results
            return companyResponse;
        }


        #endregion


        #region Post Methods
        /// <summary>
        /// Creates an company
        /// </summary>
        /// <returns></returns>
        [Route("api/CreateCompany")]
        [HttpPost]
        public async Task<bool> CreateCompany(CreateCompanyRequest createCompanyRequest)
        {
            //-- the result wheter or not the creation was successfull
            bool resultProcess = false;

            //-- checks if the company request is null
            if(createCompanyRequest != null)
            {
                //-- new instance of a Company
                var company = new Company()
                {
                    CompanyName = createCompanyRequest.CompanyName,
                    CompanyBalance = 0,
                    Active = true
                };

                //-- Connecting to the database using the CompaniesEntities
                using (var entitys = new CompanyBrokerCompaniesEntities())
                {
                    //-- adds a new company to the Companies table
                    entitys.Companies.Add(company);
                    //-- Saves the changes to the database
                    await entitys.SaveChangesAsync();
                    //-- Sets the result process to true
                    resultProcess = true;
                }
            }
            //-- returns the result
            return resultProcess;
        }

        #endregion


        #region Put methods

        /// <summary>
        /// Updates the Company balance by an amount
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> IncreaseCompanyBalance(int companyId, decimal amount)
        {
            using (var entity = new CompanyBrokerCompaniesEntities())
            {
                //-- Fetches an company based on the CompanyId 
                var company = entity.Companies.Where(c => c.CompanyId == companyId).Single<Company>();

                if(company != null)
                {
                    //-- Changes the values
                    company.CompanyBalance = company.CompanyBalance + amount;
                    //-- Tells the framework that there has been an change
                    entity.Entry(company).State = EntityState.Modified;
                    //-- Saves the changes
                    await entity.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }

            }
        }


        /// <summary>
        /// Decreases the companyBalance by an amount
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> DecreaseCompanyBalance(int companyId, decimal amount)
        {
            using (var entity = new CompanyBrokerCompaniesEntities())
            {
                //-- Fetches an company based on the CompanyId 
                var company = entity.Companies.Where(c => c.CompanyId == companyId).Single<Company>();

                if (company.CompanyBalance > 0)
                {
                    //-- Changes the values
                    company.CompanyBalance = company.CompanyBalance - amount;
                    //-- Tells the framework that there has been an change
                    entity.Entry(company).State = EntityState.Modified;
                    //-- Saves the changes
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

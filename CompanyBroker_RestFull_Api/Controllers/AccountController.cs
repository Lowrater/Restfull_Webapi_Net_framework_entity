using CompanyBroker.DBSData;
using CompanyBroker_RestFull_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CompanyBroker_RestFull_Api.Controllers
{
    /// <summary>
    /// Account controller, which handles all the data and informations about the accounts stored in the database
    /// </summary>
    public class AccountController : ApiController
    {

        private readonly Random random = new Random();

        private byte[] GetHash(string s, byte[] salt)
        {
            using (var ha = HashAlgorithm.Create("SHA256"))
                return ha.ComputeHash(salt.Concat(Encoding.UTF8.GetBytes(s)).ToArray());
        }

        private byte[] GenerateSalt(int size)
        {
            var salt = new byte[size];
            random.NextBytes(salt);
            return salt;
        }

        /// <summary>
        /// Verifys the login and returns a bool
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [Route("api/VerifyLogin")]
        [HttpPost]
        public async Task<bool> VerifyLogin(LoginRequest loginRequest)
        {
            //-- LoginResult to store result wheter or not the user exists
            bool loginResult = false;
            //-- Uses the account entities to log on the database
            using (var entitys = new CompanyBrokerAccountEntities())
            {
                //-- fetches the user
                var user = await entitys.CompanyAccounts.Where(a => a.Username == loginRequest.Username).SingleOrDefaultAsync();
                //-- checks the response of it exists
                if (user != null)
                {
                    //-- sets the results depending on the password matching
                    loginResult = GetHash(loginRequest.Password, user.PasswordSalt).SequenceEqual(user.PasswordHash);
                }
            }
            //-- returns the results
            return loginResult;
        }

        /// <summary>
        /// Fetches all accounts, through a model to not contain sensitive data like passwords.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IList<AccountResponse>> Get()
        {
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerAccountEntities())
            {
                return (await entitys.CompanyAccounts.ToListAsync()).Select(a => new AccountResponse(a)).ToList();               
            }
        }


        /// <summary>
        /// Fetches all accounts based on companyId, through a model to not contain sensitive data like passwords.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IList<AccountResponse>> Get(int companyId)
        {
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerAccountEntities())
            {
                //-- Fetches the account list
                return (await entitys.CompanyAccounts.ToListAsync()).Select(a => new AccountResponse(a)).Where(c => c.CompanyId == companyId).ToList();
            }
        }


        /// <summary>
        /// Fetches one account based on a ID number, and returns through an model to not contain sensitive data like passwords
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AccountResponse> Get(string userName)
        {
            //-- sets an accountResponse
            AccountResponse accountResponse;
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerAccountEntities())
            {
                //-- fetches the account based on the ID the users has requested
                var responseData = await entitys.CompanyAccounts.FirstOrDefaultAsync(c => c.Username == userName);
                //-- Creates a new accountResponse and parsing the information to remove sensitive data
                accountResponse = new AccountResponse(responseData);
            }
            //-- Returns the response
            return accountResponse;
        }


        /// <summary>
        /// Creates an account from the content recieved from the user / application, to an existing company
        /// </summary>
        /// <param name="accountRequest"></param>
        /// <returns></returns>
        [Route("api/CreateAccount")]
        [HttpPost]
        public async Task<bool> CreateAccount(CreateAccountRequest accountRequest)
        {
            bool resultProcess = false;
            //-- generating the salt
            var salt = GenerateSalt(32);

            if(accountRequest != null)
            {
                //-- Creates the new account
                var user = new CompanyAccount
                {
                    CompanyId = accountRequest.CompanyId,
                    Username = accountRequest.Username,
                    Email = accountRequest.Email,
                    PasswordSalt = salt,
                    PasswordHash = GetHash(accountRequest.Password, salt),
                    Active = accountRequest.Active
                };

                //-- Connects to the database using the entity
                using (var entitys = new CompanyBrokerAccountEntities())
                {
                    //-- adds a new user to the CompanyAccounts table
                    entitys.CompanyAccounts.Add(user);
                    //-- Saves the changes to the database
                    await entitys.SaveChangesAsync();

                    resultProcess = true;
                }
            }

            //-- Returns the user wished to be created
            return resultProcess;
        }
    }
}

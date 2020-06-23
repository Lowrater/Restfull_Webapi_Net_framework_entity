using CompanyBroker.DBSData;
using CompanyBroker_RestFull_Api.Models;
using Newtonsoft.Json;
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

        #region Post Methods


        /// <summary>
        /// Creates an account from the content recieved from the user / application, to an existing company
        /// </summary>
        /// <param name="accountRequest"></param>
        /// <returns></returns>
        [Route("api/CreateAccount")]
        [HttpPost]
        public async Task<bool> CreateAccount(AccountRequest accountRequest)
        {
            bool resultProcess = false;
            //-- generating the salt
            var salt = GenerateSalt(32);

            if (accountRequest != null)
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

        #endregion

        #region Get Methods


        /// <summary>
        /// Verifys login - in JSon formattet string
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [Route("api/VerifyLogin")]
        [HttpGet]
        public async Task<AccountResponse> VerifyLogin(string Username, string Password)
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                //-- Deserialize the JSon string
                var _usernameJS = JsonConvert.DeserializeObject<string>(Username);
                var _passwordJS = JsonConvert.DeserializeObject<string>(Password);
                //-- Decode the encoding base64 string
                var _username = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(_usernameJS));
                var _password = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(_passwordJS));

                //-- Uses the account entities to log on the database
                using (var entitys = new CompanyBrokerAccountEntities())
                {
                    //-- fetches the user
                    var user = await entitys.CompanyAccounts.Where(a => a.Username == _username).SingleOrDefaultAsync();
                    //-- checks the response of it exists
                    if (user != null)
                    {
                        //-- sets the results depending on the password matching
                        var loginResult = GetHash(_password, user.PasswordSalt).SequenceEqual(user.PasswordHash);

                        if (loginResult != false)
                        {
                            return new AccountResponse(user);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
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
        /// Fetches one account based on username and company id and returns through an model to not contain sensitive data like passwords
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AccountResponse> Get(string userName, int companyId)
        {
            //-- sets an accountResponse
            AccountResponse accountResponse;
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerAccountEntities())
            {
                //-- fetches the account based on the ID the users has requested
                var responseData = await entitys.CompanyAccounts.FirstOrDefaultAsync(c => c.Username == userName && c.CompanyId == companyId);
                //-- Creates a new accountResponse and parsing the information to remove sensitive data
                accountResponse = new AccountResponse(responseData);
            }
            //-- Returns the response
            return accountResponse;
        }

        #endregion

        #region Put Methods

        /// <summary>
        /// Updates the user account informations
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> PUT(AccountRequest AccountAPIModel)
        {
            //-- Getting access to the 
            using (var entity = new CompanyBrokerAccountEntities())
            {
                //-- fetches the account
                var responsData =  entity.CompanyAccounts.Where(a => a.CompanyId == AccountAPIModel.CompanyId && a.Username == AccountAPIModel.Username).Single<CompanyAccount>();
                //-- checks the account
                if(responsData != null)
                {
                    //-- sets the new informations
                    responsData.Email = AccountAPIModel.Email;
                    responsData.Active = AccountAPIModel.Active;
                    responsData.Username = AccountAPIModel.Username;

                    //-- checks if password has been changed
                    if (!string.IsNullOrEmpty(AccountAPIModel.Password))
                    {
                        //-- creates new salt
                        var newSalt = GenerateSalt(32);
                        //-- sets new password informations
                        responsData.PasswordHash = GetHash(AccountAPIModel.Password, newSalt);
                        responsData.PasswordSalt = newSalt;
                    }
                    //-- Sets the data entry and sate
                    entity.Entry(responsData).State = EntityState.Modified;
                    //-- saves the data
                    await entity.SaveChangesAsync();
                    //-- returns
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region Delete methods

        /// <summary>
        /// Deletes an account based on company id and username
        /// </summary>
        /// <param name="AccountRequest"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<bool> Delete(int companyId, string username)
        {
            //-- Connects to the database
            using (var entity = new CompanyBrokerAccountEntities())
            {
                //-- fetches an account based on the informations
                var account = entity.CompanyAccounts.Where(a => a.CompanyId == companyId && a.Username == username).Single<CompanyAccount>();
                //-- checks the account
                if(account != null)
                {
                    //-- removes the account
                    entity.CompanyAccounts.Remove(account);
                    //-- informs of the state
                    entity.Entry(account).State = EntityState.Deleted;
                    //-- saves the state
                    await entity.SaveChangesAsync();
                    //-- return
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

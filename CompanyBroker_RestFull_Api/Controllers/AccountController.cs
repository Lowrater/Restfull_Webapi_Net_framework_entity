﻿using CompanyBroker.DBSData;
using CompanyBroker_RestFull_Api.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace CompanyBroker_RestFull_Api.Controllers
{
    /// <summary>
    /// Account controller, which handles all the data and informations about the accounts stored in the database
    /// </summary>
    public class AccountController : ApiController
    {
        /// <summary>
        /// Fetches all accounts, through a model to not contain sensitive data like passwords.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<AccountResponse>> Get()
        {
            //-- Creates an list type of accountResponse
            var accountList = new List<AccountResponse>();
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerAccountEntity())
            {
                //-- Fetches the account list
                var info = await entitys.Accounts.ToListAsync();
                //-- Fetches all account
                foreach (Account account in info)
                {
                    //-- Adds all the accounts, throgh the accountResponse to remove sensitive data
                    accountList.Add(new AccountResponse(account));
                }
            }
            //-- returns the list
            return accountList;
        }


        /// <summary>
        /// Fetches one account based on a ID number, and returns through an model to not contain sensitive data like passwords
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AccountResponse> Get(int id)
        {
            //-- sets an accountResponse
            AccountResponse accountResponse;
            //-- Uses the CompanyBrokeraccountEntity to access the database
            using (var entitys = new CompanyBrokerAccountEntity())
            {
                //-- fetches the account based on the ID the users has requested
                var info = await entitys.Accounts.FirstOrDefaultAsync(c => c.Id == id);
                //-- Creates a new accountResponse and parsing the information to remove sensitive data
                accountResponse = new AccountResponse(info);
            }
            //-- Returns the response
            return accountResponse;
        }
    }
}
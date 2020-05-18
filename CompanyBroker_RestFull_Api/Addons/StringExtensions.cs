using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompanyBroker_RestFull_Api.Addons
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts string to int, if it contains an number
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int ConvertToInt(this string text)
        {
            return Int32.Parse(text);
        }
    }
}
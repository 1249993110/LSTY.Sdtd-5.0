using IceCoffee.Common;
using System;

namespace LSTY.Sdtd.PatronsMod.ExceptionCatch
{
    /// <summary>
    /// WebApi exception
    /// </summary>
    public class WebApiException : CustomExceptionBase
    {
        /// <summary>
        /// PatronsModException
        /// </summary>
        /// <param name="message"></param>
        public WebApiException(string message) : base(message)
        {
        }
        /// <summary>
        /// PatronsModException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public WebApiException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

using IceCoffee.Common;
using System;

namespace LSTY.Sdtd.PatronsMod.ExceptionCatch
{
    /// <summary>
    /// PatronsMod exception
    /// </summary>
    public class PatronsModException : CustomExceptionBase
    {
        /// <summary>
        /// PatronsModException
        /// </summary>
        /// <param name="message"></param>
        public PatronsModException(string message) : base(message)
        {
        }
        /// <summary>
        /// PatronsModException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public PatronsModException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
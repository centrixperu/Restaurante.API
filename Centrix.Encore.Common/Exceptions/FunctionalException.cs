using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Centrix.Encore.Common.Exceptions
{
    [Serializable()]
    public class FunctionalException : Exception, ISerializable
    {
        public string TransactionId { get; }
        public int FuntionalCode { get; }
        public dynamic Data { get; set; }

        public FunctionalException(int status, string message) : base(message)
        {
            this.FuntionalCode = status;
            this.TransactionId = DateTime.Now.ToString(Constants.DateTimeFormats.DD_MM_YYYY_HH_MM_SS_FFF);
        }
        public FunctionalException(int status, string message, dynamic data) : base(message)
        {
            this.FuntionalCode = status;
            this.TransactionId = DateTime.Now.ToString(Constants.DateTimeFormats.DD_MM_YYYY_HH_MM_SS_FFF);
            this.Data = data;
        }
        public FunctionalException(string message) : base(message)
        {
            this.TransactionId = DateTime.Now.ToString(Constants.DateTimeFormats.DD_MM_YYYY_HH_MM_SS_FFF);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Box.Common
{
    public class BoxLogicException : System.Exception
    {

        public int HttpCode { get; private set;}

        public BoxLogicException(string message) : base(message)
        {                     
            HttpCode = 400;
        }

        public BoxLogicException(string message, string details, int httpCode = 400) : base(message)
        {                     
            Details = details;
            HttpCode = httpCode;
        }

        public BoxLogicException(string message, int httpCode) : base(message)
        {                                 
            HttpCode = httpCode;
        }

        public string Details { get; private set; }
    }
}

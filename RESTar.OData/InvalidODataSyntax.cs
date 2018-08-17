using System;
using RESTar.Results;

namespace RESTar.OData
{
    internal class InvalidODataSyntax : BadRequest
    {
        public InvalidODataSyntax(ErrorCodes errorCode, string info, Exception ie = null) : base(errorCode, info, ie) { }
    }
}
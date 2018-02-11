using RESTar.Internal;
using RESTar.Results.Fail.BadRequest;

namespace RESTar.OData
{
    internal class UnsupportedODataVersion : BadRequest
    {
        internal UnsupportedODataVersion() : base(ErrorCodes.NotCompliantWithProtocol,
            "Unsupported OData protocol version. Supported protocol version: 4.0") { }
    }
}
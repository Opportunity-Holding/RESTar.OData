using System.Collections.Generic;
using System.Linq;
using static RESTar.MetadataLevel;

namespace RESTar.OData
{
    [RESTar(Methods.GET, GETAvailableToAll = true)]
    public class ServiceDocument : ISelector<ServiceDocument>
    {
        public string name { get; private set; }
        public string kind { get; private set; }
        public string url { get; private set; }

        public IEnumerable<ServiceDocument> Select(IRequest<ServiceDocument> request) => Metadata
            .Get(OnlyResources)
            .EntityResources
            .Select(resource => new ServiceDocument
            {
                kind = "EntitySet",
                name = resource.Name,
                url = resource.Name
            });
    }
}
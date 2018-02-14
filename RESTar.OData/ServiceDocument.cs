using System.Collections.Generic;
using System.Linq;
using static RESTar.MetadataLevel;

namespace RESTar.OData
{
    /// <inheritdoc />
    /// <summary>
    /// This resource lists all the available resources of the OData service.
    /// </summary>
    [RESTar(Methods.GET, GETAvailableToAll = true)]
    public class ServiceDocument : ISelector<ServiceDocument>
    {
        /// <summary>
        /// The name of the resource
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// The resource kind, for example "EntitySet"
        /// </summary>
        public string kind { get; private set; }

        /// <summary>
        /// The URL to the resource, for example "User"
        /// </summary>
        public string url { get; private set; }

        /// <inheritdoc />
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
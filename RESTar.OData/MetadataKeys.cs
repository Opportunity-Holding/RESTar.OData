namespace RESTar.OData
{
    /// <summary>
    /// The available metadata keys in the RESTar implementation of OData
    /// </summary>
    public enum MetadataKeys
    {
        /// <summary>
        /// @odata.context | Describes the context of the returned entities
        /// </summary>
        context,

        /// <summary>
        /// @odata.count | The number of entities returned
        /// </summary>
        count,

        /// <summary>
        /// @odata.nextLink | A relative link to the next page of entities
        /// </summary>
        nextLink
    }
}
namespace RESTar.OData
{
    /// <summary>
    /// Describes the metadata level for an OData request
    /// </summary>
    public enum MetaDataLevel
    {
        /// <summary>
        /// Only nextLink and count
        /// </summary>
        None,

        /// <summary>
        /// Include minimal metadata: context, count, nextLink
        /// </summary>
        Minimal,

        /// <summary>
        /// Include all metadata
        /// </summary>
        All
    }
}
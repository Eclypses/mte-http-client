namespace Eclypses.MteHttpClient.Shared
{
    public class MteProtectionOptions
    {
        /// <summary>
        /// If true, the destinaiton Url (Route) is encoded for a specific request.
        /// </summary>
        public bool? ShouldProtectUrl { get; set; } = null;
        /// <summary>
        /// Based on this enum, specific headers will be encoded for this specific request
        /// into the x-mte-relay-eh header (Note Content-type is always encoded).
        /// </summary>
        public RelayHeaderDisposition HeaderDisposition { get; set; } = RelayHeaderDisposition.Unknown;
        /// <summary>
        /// If HeaderDisposition is EncodeListOfHeaders, then this is 
        /// the list that will be encoded for this specific request.
        /// NOTE: the Content-Type header is always encoded.
        /// </summary>
        public List<string>? HeadersToEncode { get; set; } = null;    
    }
}

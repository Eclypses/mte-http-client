namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// All data required for a unique Mte-Relay endpoint.
    /// </summary>
    public class MteRelayEndpoint
    {
        /// <summary>
        /// The identifier used for the actual named HttpClient.
        /// </summary>
        public string? HttpClientRelayName { get; set; }
        /// <summary>
        /// The base address of the Mte-Relay for this instance
        /// </summary>
        public string? MteRelayUrl { get; set; }
        /// <summary>
        /// The actual named HttpClient that communicates with the Mte-Relay
        /// for this endpoint.
        /// </summary>
        public HttpClient? mteRelayClient { get; set; }
        /// <summary>
        /// The Authentication Header to add to an Http Request.
        /// </summary>
        public MteAuthenticationHeaderValue AuthenticationHeader { get; set; } = new MteAuthenticationHeaderValue();
        /// <summary>
        /// The Mte version of DefaultRequestHeaders - these are preserved and added to each request.
        /// </summary>
        public Dictionary<string, string>? MteDefaultRequestHeaders { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// The identifier that the Relay server returns upon initial contact.
        /// </summary>
        public string? MteRelayClientIdentifier { get; set; }
        /// <summary>
        /// The comma separated header that accompanies every request to the MteRelay.
        /// </summary>
        public X_MteRelay_Header XmteRelayHeader { get; set; } = new X_MteRelay_Header();
        /// <summary>
        /// The number of retry attempts for this endpoint when the transaction fails.
        /// </summary>
        public int RetryAttempts { get; set; } = 0;
        /// <summary>
        /// Used to round-robin through the list of pair ids.
        /// </summary>
        public int PairIdIdx { get; set; } = 0;
        /// <summary>
        /// Identifies which MteState instance should be used for
        /// a connection out of the list of paired Mtes.
        /// </summary>
        public List<string>? PairIds { get; set; } = new List<string>();
        /// <summary>
        /// If you enter a route here, a GET request is proxied
        /// through so that you can verify if your API is alive.
        /// </summary>
        public string? ApiEchoRoute { get; set; }
        /// <summary>
        /// If you enter a value here, the returned message
        /// from your echo GET must include this text as part
        /// of the returned message.
        /// </summary>
        public string? ApiEchoString { get; set; }
        /// <summary>
        /// Once an endpoint is paired with the MteRelay, this is set to true.
        /// </summary>
        public bool IsPaired { get; set; } = false;  
        /// <summary>
        /// Once an endpoint has been echoed, this is set to true.
        /// </summary>
        public bool IsVerified { get; set; } = false;
        public MteRelayEndpoint() { }

        /// <summary>
        /// Resets the internal state of this specific endpoint.
        /// </summary>
        public void Reset()
        {
            IsPaired = false;
            IsVerified = false;
            PairIdIdx = 0;
            PairIds!.Clear();
            MteRelayClientIdentifier = string.Empty;
        }
    }
}

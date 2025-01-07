namespace Eclypses.MteHttpClient
{
    /// <summary>
    /// Exception thrown when an MteRelay specific Http status code is returned.
    /// </summary>
    public class MteRelayException : ApplicationException
    {
        /// <summary>
        /// Constructs a new MteRelayException
        /// </summary>
        /// <param name="message">The message that is passed to the ApplicationException.</param>
        /// <param name="inner">The optional inner exception.</param>
        public MteRelayException(string? message, Exception? inner = null) : base(message, inner) { }
        /// <summary>
        /// The HttpStatusCode returned from the response.
        /// </summary>
        public int HttpStatusCode { get; set; }
        /// <summary>
        /// The actual error message associated with the specified status code.
        /// </summary>
        public string? HttpReasonPhrase { get; set; }
        /// <summary>
        /// The PairId of the MTE Pairs that the client was sending to the MTE Relay proxy server.
        /// </summary>
        public string? PairId { get; set; }
        /// <summary>
        /// This is the original content before it is converted to an MteRelay
        /// content. It is needed in case of a retry.
        /// </summary>
        public HttpContent? OriginalContent { get; set; }
        /// <summary>
        /// This is the original request route path.  In case we need to retry
        /// we must know this (because it may have been encoded).
        /// </summary>
        public string? OriginalRoute { get; set; }
    }
}

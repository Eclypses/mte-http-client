namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// This is the exchange model with the MteRelay
    /// proxy server when pairing is required.
    /// </summary>
    internal class MtePairRequestModel
    {
        /// <summary>
        /// The pair id that identifies this specific MTE pairing.
        /// </summary>
        public string? PairId { get; set; }
        /// <summary>
        /// The public key for the MteEncoder.
        /// </summary>
        public string? EncoderPublicKey { get; set; }
        /// <summary>
        /// The personalization string for the MteEncoder.
        /// </summary>
        public string? EncoderPersonalizationStr { get; set; }    
        /// <summary>
        /// The public key for the MteDecoder.
        /// </summary>
        public string? DecoderPublicKey { get; set; }
        /// <summary>
        /// The personalization string for the MteDecoder.
        /// </summary>
        public string? DecoderPersonalizationStr { get; set; }

        /// <summary>
        /// Sets all of the string properties to null.
        /// </summary>
        public void Clear()
        {
            PairId = null;
            EncoderPublicKey = null;
            EncoderPersonalizationStr = null;
            DecoderPublicKey = null;
            DecoderPersonalizationStr = null;
        }
    }
}

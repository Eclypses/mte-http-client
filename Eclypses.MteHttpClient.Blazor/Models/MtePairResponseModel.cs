namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// Return model from pairing with the Kyber relay server.
    /// </summary>
    public class MtePairResponseModel
    {
        /// <summary>
        /// The pair id that identifies this specific MTE pairing.
        /// </summary>
        public string? PairId { get; set; }
        /// <summary>
        /// The encrypted secret for the MteEncoder.
        /// </summary>
        public string? EncoderSecret { get; set; }   
        /// <summary>
        /// The nonce value for the MteEncoder.
        /// </summary>
        public string? EncoderNonce { get; set; }
        /// <summary>
        /// The encrypted secret for the MteDecoder.
        /// </summary>
        public string? DecoderSecret { get; set; }    
        /// <summary>
        /// The nonce value for the MteDecoder.
        /// </summary>
        public string? DecoderNonce { get; set; }
        /// <summary>
        /// Sets all of the string properties to null.
        /// </summary>
        public void Clear()
        {
            PairId = null;
            EncoderNonce = null;
            EncoderSecret = null;
            DecoderNonce = null;
            DecoderSecret = null;
        }
    }
}

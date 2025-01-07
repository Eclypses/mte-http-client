namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// The Kyber generated shared secrets.
    /// </summary>
    public class KyberSecrets
    {
        /// <summary>
        /// The shared secret for the encoder.
        /// </summary>
        public string? EncoderSecret { get; set; }
        /// <summary>
        /// The shared secret for the decoder.
        /// </summary>
        public string? DecoderSecret { get; set; }
        /// <summary>
        /// The shared secret return code for the encoder.
        /// </summary>
        public int Erc { get; set; }
        /// <summary>
        /// The shared secret return code for the decoder.
        /// </summary>
        public int Drc { get; set; }
        /// <summary>
        /// Resets this object.
        /// </summary>
        public void Clear()
        {
            EncoderSecret = null;
            DecoderSecret = null;
            Erc = 0;
            Drc = 0;
        }
    }
}

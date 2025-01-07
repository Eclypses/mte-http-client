namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// Public keys returned from java script Kkyber.
    /// </summary>
    public class KyberKeys
    {
        /// <summary>
        /// The kyber encoder public key.
        /// </summary>
        public string? EncoderPublicKey { get; set; }
        /// <summary>
        /// The kyber decoder public key.
        /// </summary>
        public string? DecoderPublicKey { get; set; }
        /// <summary>
        /// The kyber encoder return code.
        /// </summary>
        public int Erc { get; set; }
        /// <summary>
        /// The kyber decoder return code.
        /// </summary>
        public int Drc { get; set; }
    }
}

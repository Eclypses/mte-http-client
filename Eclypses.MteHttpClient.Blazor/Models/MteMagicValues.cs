namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// The three values needed to initialize the MTE.
    /// </summary>
    public class MteMagicValues
    {
        /// <summary>
        /// The personalization string - should be unique to this session.
        /// </summary>
        public string? PersonalizationString { get; set; }
        /// <summary>
        /// Entropy which must be kept secret.
        /// </summary>
        public byte[]? Entropy { get; set; }
        /// <summary>
        /// Used to further isolate the MTE - should be unique
        /// and should be a string that can parse to a Uint64.
        /// Java script requires this to be a string, but dotNet
        /// prefers a Uint64 so it must be parseable.
        /// </summary>
        public string Nonce { get; set; } = string.Empty;    
    }
}

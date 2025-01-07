namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// Encapsulates the state of an encoder or a decoder
    /// based on the StateType.
    /// </summary>
    public class MteState
    {
        /// <summary>
        /// Determines what type of state this is.
        /// </summary>
        public enum MteType { Encoder, Decoder }

        /// <summary>
        /// The type of MTE object that this state is associated with.
        /// </summary>
        public MteType StateType { get; set; }

        /// <summary>
        /// The identifier for this MtePair
        /// </summary>
        public string? PairId { get; set; }

        /// <summary>
        /// Base-64 version of the MTE State for a single instance.
        /// </summary>
        public string? B64State { get; set; }
    }
}

using System.Text;

namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// Determines if we are using MTE or MKE.
    /// </summary>
    public enum EncodeType
    {
        MTE = 0,
        MKE = 1
    }

    /// <summary>
    /// The x-mte-relay header is a comma separated list
    /// of relay parameters - this class manages it.
    /// </summary>
    public class X_MteRelay_Header
    {
        /// <summary>
        /// The ClientId is returned from the Mte-Pair route.
        /// </summary>
        public string? ClientId { get; set; }
        /// <summary>
        /// Unique identifier for the Encoder / Decoder pair
        /// that is pooled for performance.
        /// </summary>
        public string? PairId { get; set; }
        /// <summary>
        /// Determines the Eclypses encoding type.
        /// </summary>
        public EncodeType EncodeType { get; set; } = EncodeType.MKE;
        /// <summary>
        /// If true, the eventual endpoint url is encoded.
        /// </summary>
        public bool UrlIsEncoded { get; set; }
        /// <summary>
        /// If true, there is an encoded header to send to the relay.
        /// </summary>
        public bool HeadersAreEncoded { get; set; }
        /// <summary>
        /// If true, the body of the request is coded.  This must be
        /// false for a GET request.
        /// </summary>
        public bool BodyIsEncoded { get; set; }
        public X_MteRelay_Header() { }
        /// <summary>
        /// Deconstructs a x-mte-relay header into this class.
        /// </summary>
        /// <param name="mteRelayHeader">The x-mte-relay header value.</param>
        public X_MteRelay_Header(string mteRelayHeader)
        {
            string[] s = mteRelayHeader.Split(',');
            if (s.Length > 0) { ClientId = s[0]; }
            if (s.Length > 1) { PairId = s[1]; }
            if (s.Length > 2) { EncodeType = (EncodeType)int.Parse(s[2]); }
            if (s.Length > 3) { if (int.Parse(s[3]) == 1) UrlIsEncoded = true; else UrlIsEncoded = false; }
            if (s.Length > 4) { if (int.Parse(s[4]) == 1) HeadersAreEncoded = true; else HeadersAreEncoded = false; }
            if (s.Length > 5) { if (int.Parse(s[5]) == 1) BodyIsEncoded = true; else BodyIsEncoded = false; }
        }
        /// <summary>
        /// Returns a string of the x-mte-relay header to transmit to the Mte-Relay.
        /// </summary>
        /// <returns>Value for the x-mte-relay header.</returns>
        public override string ToString()
        {
            string value = $"{ClientId},{PairId},{(int)EncodeType},{UrlIsEncoded: 1 ? 0},{HeadersAreEncoded: 1 ? 0},{BodyIsEncoded: 1 ? 0}";
            StringBuilder sb = new StringBuilder($"{ClientId},{PairId},{(int)EncodeType},");
            if (UrlIsEncoded) sb.Append("1,"); else sb.Append("0,");
            if (HeadersAreEncoded) sb.Append("1,"); else sb.Append("0,");
            if (BodyIsEncoded) sb.Append('1'); else sb.Append('0');
            return sb.ToString();
        }
    }
}

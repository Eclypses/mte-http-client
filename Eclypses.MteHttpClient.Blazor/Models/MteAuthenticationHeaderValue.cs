namespace Eclypses.MteHttpClient.Models
{
    /// <summary>
    /// Information to set the Authorization Header
    /// in the MteHttpClient.
    /// </summary>
    public class MteAuthenticationHeaderValue
    {          
        public MteAuthenticationHeaderValue() { }

        /// <summary>
        /// The Authentication Scheme - Bearer or Basic.
        /// </summary>
        public string Scheme { get; set; } = string.Empty;
        /// <summary>
        /// The Authentication Value - JWT or B-64 of UserId / Password
        /// </summary>
        public string Value { get; set; } = string.Empty;
        /// <summary>
        /// Sets the data for the Authentication header.
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="value"></param>
        public void Set(string scheme, string value)
        {
            Scheme = scheme;
            Value = value;
        }
    }
}

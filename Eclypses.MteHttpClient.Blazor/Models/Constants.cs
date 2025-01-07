namespace Eclypses.MteHttpClient.Models
{
    public class Constants
    {
        /// <summary>
        /// 5
        /// </summary>
        internal static int DEFAULT_NUMBER_OF_MTE_INSTANCES = 5;
        /// <summary>
        /// 5
        /// </summary>
        internal static int DEFAULT_NUMBER_OF_PAIRED_STATES = 5;
        /// <summary>
        /// content-type
        /// </summary>
        internal static string STR_CONTENT_TYPE_HEADER = "content-type";
        /// <summary>
        /// content-disposition
        /// </summary>
        internal static string STR_CONTENT_DISPOSITION_HEADER = "content-disposition";
        /// <summary>
        /// content-length
        /// </summary>
        internal static string STR_CONTENT_LENGTH_HEADER = "content-length";
        /// <summary>
        /// no-content
        /// </summary>
        internal static string STR_CONTENT_NO_CONTENT = "no-content";
        /// <summary>
        /// x-mte-relay-eh
        /// </summary>
        internal static string MTE_RELAY_ENCODED_HEADER = "x-mte-relay-eh";
        /// <summary>
        /// x-mte-relay
        /// </summary>
        internal static string MTE_RELAY_HEADER = "x-mte-relay";
        /// <summary>
        /// Authorization
        /// </summary>
        public static string AUTHORIZATION_HEADER = "Authorization";
        /// <summary>
        /// The path where the mterelay-helper.js is stored for the NuGet package.
        /// </summary>
        public static string NUGET_CONTENT_ROOT = "./_content/Eclypses.MteHttpClient.Blazor.AWS";
        /// <summary>
        /// The path where the mterelay-helper.js is stored when testing locally.
        /// </summary>
        public static string LOCAL_TEST_CONTENT_ROOT = "./";
        /// <summary>
        /// The display session item file - used by the MTE SDR
        /// to identify a category of items to store in session storage.
        /// </summary>
        public const string DISPLAY_SESSION_ITEM_FILE = "ObjectStates";
        /// <summary>
        /// Number of times to re-pair and re-try when an MteRelay error is returned
        /// currently this is '1'.
        /// </summary>
        public static int MAX_PROXY_RETRY_ATTEMPTS = 1;   
        /// <summary>
        /// The following Mime types are considered as text.
        /// </summary>
        private static List<string> TextContentTypes = new List<string> {
            "json",
            "text",
            "xml",
            "javascript",
            "urlencoded",
        };
        /// <summary>
        /// Dictionary of status codes and descriptors for Mte-Relay errors.
        /// </summary>
        private static Dictionary<int, string> RelayErrors = new Dictionary<int, string>();
        /// <summary>
        /// Checks the incoming content type to determine if it is a "text" type.
        /// </summary>
        /// <param name="contentType">Content type to check</param>
        /// <returns>true if this content type in in the TextContentTypes list.</returns>
        internal static bool CheckForText(string contentType)
        {
            foreach (string s in TextContentTypes)
            {
                if (contentType.Contains(s, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        /// <summary>
        /// Converts a MteRelay "56x" return code to a string message
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        public static string ParseRelayRC(int rc)
        {
            if (RelayErrors is null || RelayErrors.Count == 0)
            {
                LoadRelayErrors();
            }
            return RelayErrors![rc];
        }

        /// <summary>
        /// Indicates the depth of the client conversation reset.
        /// </summary>
        public enum ConversationReset
        {
            //
            // No reset is required.
            //
            NoResetNeeded = 0,
            //
            // Create new pairs, but keep client id.
            //
            ResetTheEndpointPairs = 1,
            //
            // Create entire new client id due to compromised client or reseed reached.
            //
            ResetTheClient = 2,
        }

        /// <summary>
        /// These return codes indicate that the entire client id must be replaced vs just replacing the pairs.
        /// </summary>
        public static List<int> CatastrophicRelayReturnCodes = new() { 566 };

        /// <summary>
        /// Loads the text explanations of the MteRelay specific return codes.
        /// </summary>
        private static void LoadRelayErrors()
        {
            RelayErrors.Add(559, "Repair is required.");
            RelayErrors.Add(560, "State not found.");
            RelayErrors.Add(561, "Failed to encode.");
            RelayErrors.Add(562, "Failed to decode.");
            RelayErrors.Add(563, "Failed to get state from encoder or decoder.");
            RelayErrors.Add(564, "DRBG reseed is required.");
            RelayErrors.Add(565, "MTE status was not successful.");
            RelayErrors.Add(566, "Invalid Client ID header.");
            RelayErrors.Add(567, "Failed to save decoder stateid.");
            RelayErrors.Add(568, "Failed to save encoder stateid.");
            RelayErrors.Add(569, "Missing required header.");
        }
    }
}

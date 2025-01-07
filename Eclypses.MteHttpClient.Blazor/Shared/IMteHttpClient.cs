using Eclypses.MteHttpClient.Models;

namespace Eclypses.MteHttpClient.Shared
{

    public enum RelayHeaderDisposition
    {
        /// <summary>
        /// We do not know the dispoisiton.
        /// </summary>
        Unknown,
        /// <summary>
        /// Do not encode any headers on the request.
        /// </summary>
        EncodeNoHeaders,
        /// <summary>
        /// Encode all of the headers on the request.
        /// </summary>
        EncodeAllHeaders,
        /// <summary>
        /// Encode only a list of specific headers on the request.
        /// </summary>
        EncodeListOfHeaders
    }

    public interface IMteHttpClient
    {
        /// <summary>
        /// Initializes the Mte-Relay environment for a specific relayIdentifier.
        /// </summary>       
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="shouldReset">If true, this endpoint will be reset.</param>
        /// <returns>true if successful.</returns>
        Task<bool> InitializeAsync(string relayIdentifier, Constants.ConversationReset shouldReset = Constants.ConversationReset.NoResetNeeded);

        /// <summary>
        /// Initializes the Mte-Relay enviornment for a specific Mte-Relay endpoint.
        /// </summary>       
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="shouldReset">If true, this endpoint will be reset.</param>
        /// <returns>true if successful.</returns>
        Task<bool> InitializeAsync(MteRelayEndpoint endpoint, Constants.ConversationReset shouldReset = Constants.ConversationReset.NoResetNeeded);

        /// <summary>
        /// May be used as a convenience method to add an Authentication header for a specific relay identifier.
        /// </summary>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="scheme">The authentication scheme such as 'basic' or 'bearer'.</param>
        /// <param name="value">The actual value for the authenication token.</param>
        void SetAuthenticationHeader(string relayIdentifier, string scheme, string value);

        /// <summary>
        /// May be used as a convenience method to add an Authentication header for
        /// the first (and possibly only) relay identifier in your list.
        /// </summary>
        /// <param name="scheme">The authentication scheme such as 'basic' or 'bearer'.</param>
        /// <param name="value">The actual value for the authenication token.</param>
        void SetAuthenticationHeader(string scheme, string value);

        /// <summary>
        /// A convenience method to set request headers for a specific relay identifier.  These will be included
        /// in each and every request. If this already exists, the value is replaced. If the value is empty,
        /// this header is removed.
        /// </summary>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="key">The key for this speicific header.</param>
        /// <param name="value">The value for this specific header.</param>
        void SetDefaultRequestHeader(string relayIdentifier, string key, string value = "");

        /// <summary>
        /// A convenience method to set request headers for a specific relay identifier.  These will be included
        /// in each and every request. If this already exists, the value is replaced. If the value is empty,
        /// this header is removed.
        /// </summary>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="key">The key for this speicific header.</param>
        /// <param name="value">The value for this specific header.</param>
        void SetDefaultRequestHeader(string key, string value = "");

        Task<HttpResponseMessage> MteDeleteAsync(
             string route,
             Dictionary<string, string>? headers = null,
             string? relayIdentifier = null,
             MteProtectionOptions? protectionOptions = null,
             int deleteAttempts = 0);

        /// <summary>
        /// Asynchronously GETS a payload from your API protecting it with the Eclypses MKE.
        /// </summary>
        /// <param name="route">The route you wish to GET from on your API.</param>
        /// <param name="headers">Any headers that you wish to include in your GET request.</param>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="protectionOptions">Optional override options to manage protection of URL and Headers.</param>
        /// <param name="getAttempts">Number of times you wish to retry if a GET fails.</param>
        /// <returns>HttpResponseMessage from your GET request.</returns>
        Task<HttpResponseMessage> MteGetAsync(
            string route,
            Dictionary<string, string>? headers = null,
            string? relayIdentifier = null,
            MteProtectionOptions? protectionOptions = null,
            int getAttempts = 0);

        /// <summary>
        /// Asynchronously GETS a byte array payload from your API protecting it with the Eclypses MKE.
        /// </summary>
        /// <param name="route">The route you wish to GET from on your API.</param>
        /// <param name="headers">Any headers that you wish to include in your GET request.</param>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="protectionOptions">Optional override options to manage protection of URL and Headers.</param>
        /// <param name="getAttempts">Number of times you wish to retry if a GET fails.</param>
        /// <returns>byte array of the returned content from your GET request.</returns>
        Task<byte[]> MteGetByteArrayAsync(
            string route,
            Dictionary<string, string>? headers = null,
            string? relayIdentifier = null,
            MteProtectionOptions? protectionOptions = null,
            int getAttempts = 0);

        /// <summary>
        /// Not implemented at this time.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="headers"></param>
        /// <returns>Throws a NotSupportedException.</returns>
        Task<Stream> MteGetStreamAsync(
            string route,
            Dictionary<string, string>? headers = null,
            string? relayIdentifier = "",
            MteProtectionOptions? protectionOptions = null,
            int getAttempts = 0);

        /// <summary>
        /// Asynchronously GETS a string payload from your API protecting it with the Eclypses MKE.
        /// </summary>
        /// <param name="route">The route you wish to GET from on your API.</param>
        /// <param name="headers">Any headers that you wish to include in your GET request.</param>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="protectionOptions">Optional override options to manage protection of URL and Headers.</param>
        /// <param name="getAttempts">Number of times you wish to retry if a GET fails.</param>
        /// <returns>String of the returned content from your GET request.</returns>
        Task<string> MteGetStringAsync(
            string route,
            Dictionary<string, string>? headers = null,
            string? relayIdentifier = null,
            MteProtectionOptions? protectionOptions = null,
            int getAttempts = 0);

        /// <summary>
        /// Asynchronously PATCH an HttpContent payload to your API protecting it with the Eclypses MKE.
        /// </summary>
        /// <param name="route">The route you wish to PATCH to on your API.</param>
        /// <param name="content">HttpContent for your PATCH request.</param>
        /// <param name="headers">Any headers that you wish to include in your PATCH request.</param>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="protectionOptions">Optional override options to manage protection of URL and Headers.</param>
        /// <param name="patchAttempts">Number of times you wish to retry if a PATCH fails.</param>
        /// <returns>HttpResponseMessage from your PATCH request.</returns>
        Task<HttpResponseMessage> MtePatchAsync(
            string route,
            HttpContent content,
            Dictionary<string, string>? headers = null,
            string? relayIdentifier = "",
            MteProtectionOptions? protectionOptions = null,
            int patchAttempts = 0);

        /// <summary>
        /// Asynchronously POST an HttpContent payload to your API protecting it with the Eclypses MKE.
        /// </summary>
        /// <param name="route">The route you wish to POST to on your API.</param>
        /// <param name="content">HttpContent for your POST request.</param>
        /// <param name="headers">Any headers that you wish to include in your POST request.</param>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="protectionOptions">Optional override options to manage protection of URL and Headers.</param>
        /// <param name="postAttempts">Number of times you wish to retry if a POST fails.</param>
        /// <returns>HttpResponseMessage from your POST request.</returns>
        Task<HttpResponseMessage> MtePostAsync(
            string route,
            HttpContent? content,
            Dictionary<string, string>? headers = null,
            string? relayIdentifier = "",
            MteProtectionOptions? protectionOptions = null,
            int postAttempts = 0);

        /// <summary>
        /// Asynchronously PUT an HttpContent payload to your API protecting it with the Eclypses MKE.
        /// </summary>
        /// <param name="route">The route you wish to PUT to on your API.</param>
        /// <param name="content">HttpContent for your PUT request.</param>
        /// <param name="headers">Any headers that you wish to include in your PUT request.</param>
        /// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
        /// <param name="protectionOptions">Optional override options to manage protection of URL and Headers.</param>
        /// <param name="postAttempts">Number of times you wish to retry if a PUT fails.</param>
        /// <returns>HttpResponseMessage from your PUT request.</returns>
        Task<HttpResponseMessage> MtePutAsync(
            string route,
            HttpContent? content,
            Dictionary<string, string>? headers = null,
            string? relayIdentifier = "",
            MteProtectionOptions? protectionOptions = null,
            int putAttempts = 0);
    }
}

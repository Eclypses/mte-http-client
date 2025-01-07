using Eclypses.MteHttpClient.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Eclypses.MteHttpClient
{
    public class KyberHelper : IKyberHelper, IAsyncDisposable
    {
        /// <summary>
        /// Lazily creates the actual JS module for Kyber.
        /// </summary>
        private readonly Lazy<Task<IJSObjectReference>> _moduleTaskForKyber;
        /// <summary>
        /// Creates an instance of the KyberHelper class.
        /// </summary>
        /// <param name="jsRuntime">The .Net java script runtime.</param>
        /// <param name="mteRelayOptions">The options to determine the root of the JS module.</param>
        public KyberHelper(IJSRuntime jsRuntime, MteRelayOptions mteRelayOptions)
        {
            //
            // Determine the location of the JS library.
            //
            string jsPath = Constants.NUGET_CONTENT_ROOT;
            if (mteRelayOptions.IsTestingLocal)
            {
                jsPath = Constants.LOCAL_TEST_CONTENT_ROOT;
            }
            string mteHelper = $"{jsPath.TrimEnd('/')}/mterelay-helper.js";
            //
            // Load the actual java script library into a module.
            //
            _moduleTaskForKyber = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", mteHelper).AsTask());
        }
        /// <summary>
        /// Gets a pair of public keys for the designated pair.
        /// </summary>
        /// <param name="pairId">The identifier for this pair of keys.</param>
        /// <returns>The two public keys for the designated pair id.</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<KyberKeys> GetPublicKeysFromKyberAsync(string pairId)
        {
            try
            {
                //
                // Create the actual JS module for Kyber
                // and get the KyberKeyPairs.
                //
                var module = await _moduleTaskForKyber.Value;
                var kyberPublicKeys = await module.InvokeAsync<JsonElement>("makeKyberKeyPairs", pairId);
                //
                // Serialize the returned json and return it to the caller.
                //
                KyberKeys? publicKeyPairs = JsonSerializer.Deserialize<KyberKeys>(kyberPublicKeys);
                return (publicKeyPairs!);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception calling JavaScript 'makeKyberKeyPair' to create a key pair.", ex);
            }
        }
        /// <summary>
        /// Computes the shared secrets to use for the Encoder and Decoder entropy.
        /// </summary>
        /// <param name="pairId">The identifier for this pair of secrets.</param>
        /// <param name="encoderEncryptedData">Encrypted encoder secret from the peer.</param>
        /// <param name="decoderEncryptedData">Encrypted decoder secret from the peer.</param>
        /// <returns>The two secrets for the designated pair id.</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<KyberSecrets> ComputeSharedSecretsFromKyberAsync(string pairId, string encoderEncryptedData, string decoderEncryptedData)
        {
            try
            {
                //
                // Create the actual JS module for Kyber
                // and get the KyberSecrets.
                //
                var module = await _moduleTaskForKyber.Value;
                var kyberSecrets = await module.InvokeAsync<JsonElement>("makeKyberSecrets", pairId, encoderEncryptedData, decoderEncryptedData);
                //
                // Serialize the returned json and return it to the caller.
                //
                KyberSecrets? entropyPair = JsonSerializer.Deserialize<KyberSecrets>(kyberSecrets);
                return entropyPair!;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception calling JavaScript 'makeKyberSecrets' to create entropyPair.", ex);
            }
        }
        /// <summary>
        /// Disposes the Kyber objects.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            if (_moduleTaskForKyber.Value != null)
            {
                var module = await _moduleTaskForKyber.Value;
                await module.DisposeAsync();
            }
        }
    }
}

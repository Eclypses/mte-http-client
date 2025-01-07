using Eclypses.MteHttpClient.Models;

namespace Eclypses.MteHttpClient
{
    /// <summary>
    /// Contract for the Kyber Helper that wraps the JS methods for Kyber.
    /// </summary>
    public interface IKyberHelper
    {
        /// <summary>
        /// Returns the public keys for a decoder and encoder
        /// that are generated from Kyber.
        /// </summary>
        /// <param name="pairId">The specific pooled pair that these keys are for.</param>
        /// <returns>Object with two public keys to transmit to the server.</returns>
        Task<KyberKeys> GetPublicKeysFromKyberAsync(string pairId);
        /// <summary>
        /// Computes the shared secret to use for Entropy for 
        /// the specifiec Encoder / Decoder pair.
        /// </summary>
        /// <param name="pairId">The specific pooled pair that these secrets are for.</param>
        /// <param name="encoderEncryptedData">The peer's encoder encrypted secret.</param>
        /// <param name="decoderEncryptedData">The peer's decoder encrypted secret.</param>
        /// <returns></returns>
        Task<KyberSecrets> ComputeSharedSecretsFromKyberAsync(string pairId, string encoderEncryptedData, string decoderEncryptedData);

        ValueTask DisposeAsync();
    }
}
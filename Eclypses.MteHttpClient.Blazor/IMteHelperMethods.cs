using Eclypses.MteHttpClient.Models;
using Microsoft.JSInterop;


namespace Eclypses.MteHttpClient
{
    /// <summary>
    /// Contract for the actual Mte wrapper. 
    /// Blazor requires async (it calls into WASM).
    /// </summary>
    public interface IMteHelperMethods
    {
        /// <summary>
        /// If the MtePools have been set up, this number indicates how many
        /// were set.  If Zero or less, there are none set up.
        /// </summary>
        int EstablishedMtePoolCount { get; }

        /// <summary>
        /// Since constructors cannot issue async calls, this method is
        /// available for initial setup of the WASM module.  Once the
        /// environment is set, it then sets the license in MteBase
        /// and adds empty encoders and decoders to a concurrent bag.
        /// </summary>
        Task SetupRuntimeAsync();

        /// <summary>
        /// Adds an empty decoder to the internal pool of decoders.
        /// </summary>
        Task AddAnEmptyDecoderAsync();

        /// <summary>
        /// Adds an empty encoder to the internal pool of encoders.
        /// </summary>
        Task AddAnEmptyEncoderAsync();

        /// <summary>
        /// Retrieves an encoder from the internal pool of encoders.
        /// If the pool is empty, a new one is created and added.
        /// </summary>
        /// <returns>An MteEncoder ready to be hydrated,</returns>
        Task<IJSObjectReference> GetAnEncoderAsync();

        /// <summary>
        /// Retrieves a decoder from the internal pool of decoders.
        /// If the pool is empty, a new one is created and added.
        /// </summary>
        /// <returns>An MteEncoder ready to be hydrated,</returns>
        Task<IJSObjectReference> GetADecoderAsync();

        /// <summary>
        /// Puts an encoder into the internal pool of encoders.
        /// </summary>
        /// <param name="encoder">The encoder to put into the pool.</param>
        void PutTheEncoder(IJSObjectReference encoder);

        /// <summary>
        /// Puts a decoder into the internal pool of decoders.
        /// </summary>
        /// <param name="decoder">An MteDecoder to put into the pool.</param>
        void PutTheDecoderAsync(IJSObjectReference decoder);

        /// <summary>
        /// Initializes a decoder with the three magic values for pairing with the server.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to initialize.</param>
        /// <param name="magicValues">The three pairing values that match the proxy server.</param>
        /// <returns>MteStatus indicating success or failure.</returns>
        Task<MteStatus> InitializeDecoderAsync(IJSObjectReference mkeDecoder, MteMagicValues magicValues);

        /// <summary>
        /// Initializes an encoder with the three magic values for pairing with the server.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to initialize.</param>
        /// <param name="magicValues">The three pairing values that match the proxy server.</param>
        /// <returns>MteStatus indicating success or failure.</returns>
        Task<MteStatus> InitializeEncoderAsync(IJSObjectReference mkeEncoder, MteMagicValues magicValues);

        /// <summary>
        /// Restores the internal state of a decoder with the base-64 version of the state.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to refresh with the state.</param>
        /// <param name="state">The base-64 string of the state retrieved from the internal collection.</param>
        /// <returns>MteStatus indicating success or failure.</returns>
        Task<MteStatus> RestoreDecoderStateAsync(IJSObjectReference mkeDecoder, string state);

        /// <summary>
        /// Restores the internal state of an encoder with the base-64 version of the state.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to refresh with the state.</param>
        /// <param name="state">The base-64 string of the state retrieved from the internal collection.</param>
        /// <returns>MteStatus indicating success or failure.</returns>
        Task<MteStatus> RestoreEncoderStateAsync(IJSObjectReference mkeEncoder, string state);

        /// <summary>
        /// Retrieves the internal state of a decoder.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to retrieve the state from.</param>
        /// <returns>Base-64 string of the internal decoder state.</returns>
        Task<string> RetrieveDecoderStateAsync(IJSObjectReference mkeDecoder);

        /// <summary>
        /// Retrieves the internal state of an encoder.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to retrieve the state from.</param>
        /// <returns>Base-64 string of the internal encoder state.</returns>
        Task<string> RetrieveEncoderStateAsync(IJSObjectReference mkeEncoder);

        /// <summary>
        /// Uses MTE to decode a base-64 string of an encoded payload.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to use for the decode method.</param>
        /// <param name="payload">Base-64 string of an encoded payload.</param>
        /// <returns>Clear string representation of the decoded payload.</returns>
        Task<string> DecodeToStringAsync(IJSObjectReference mkeDecoder, string payload);

        /// <summary>
        /// Uses MTE to decode a byte array of an encoded payload.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to use for the decode method.</param>
        /// <param name="payload">An encoded byte array.</param>
        /// <returns>Clear byte array representation of the decoded payload.</returns>
        Task<byte[]> DecodeToByteArrayAsync(IJSObjectReference mkeDecoder, byte[] payload);

        /// <summary>
        /// Uses MTE to encode a string.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to use for the encode method.</param>
        /// <param name="payload">A string to encode.</param>
        /// <returns>Base-64 reprsentation of the encoded string.</returns>
        Task<string> EncodeToStringAsync(IJSObjectReference mkeEncoder, string payload);

        /// <summary>
        /// Uses MTE to encode a byte array, returning a byte array.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to use for the encode method.</param>
        /// <param name="payload">A byte array to encode.</param>
        /// <returns>Encoded byte array of the original data.</returns>
        Task<byte[]> EncodeAsync(IJSObjectReference mkeEncoder, byte[] payload);

        /// <summary>
        /// Uses MTE to encode a string, returning a byte array.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to use for the encode method.</param>
        /// <param name="payload">A string to encode.</param>
        /// <returns>Encoded byte array of the original data.</returns>
        Task<byte[]> EncodeAsync(IJSObjectReference mkeEncoder, string payload);

        Task<string> GetStatusDescription(MteStatus status);
        Task<MteStatus> StartEncrypt(IJSObjectReference mkeEncoder);
        Task<byte[]> EncryptChunk(IJSObjectReference mkeEncoder, byte[] clearBytes);
        Task<(MteStatus, byte[])> FinishEncrypt(IJSObjectReference mkeEncoder);
        Task<MteStatus> StartDecrypt(IJSObjectReference mkeDecoder);
        Task<byte[]> DecryptChunk(IJSObjectReference mkeDecoder, byte[] encryptedBytes);
        Task<(MteStatus, byte[])> FinishDecrypt(IJSObjectReference mkeDecoder);
    }
}

using Eclypses.MteHttpClient.Models;
using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Eclypses.MteHttpClient
{
    public class MteHelperMethods : IMteHelperMethods
    {
        /// <summary>
        /// Property to show how many Mte engines are pooled.
        /// </summary>
        public int EstablishedMtePoolCount { get { return _mkeDecoderBag != null ? _mkeDecoderBag!.Count : -1; } }
        /// <summary>
        /// Collections of Encoders and Decoders that are reused with the paired MteStates.
        /// </summary>
        private static ConcurrentBag<IJSObjectReference> _mkeDecoderBag = new ConcurrentBag<IJSObjectReference>();
        private static ConcurrentBag<IJSObjectReference> _mkeEncoderBag = new ConcurrentBag<IJSObjectReference>();

        /// <summary>
        /// The runtime MTE Relay options from appsettings.json
        /// </summary>
        private readonly MteRelayOptions _mteRelayOptions;
        /// <summary>
        /// Lazily creates the actual JS module for MTE.
        /// </summary>
        private readonly Lazy<Task<IJSObjectReference>> _moduleTaskForMTE;

        #region Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MteHelperMethods(IJSRuntime jsRuntime, MteRelayOptions mteRelayOptions)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _mteRelayOptions = mteRelayOptions;
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
            _moduleTaskForMTE = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", mteHelper).AsTask());
        } 
        #endregion

        #region SetupRuntimeAsync
        /// <summary>
        /// Establishes the link to mte-helper.js and instantiates the MteWasm
        /// module (the actual MTE). It then sets the license for this
        /// specific licensee and builds the initial pools of encoders and decoders.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task SetupRuntimeAsync()
        {
            try
            {
                var module = await _moduleTaskForMTE.Value;
                if (!await module.InvokeAsync<bool>("instantiateMteWasm"))
                {
                    throw new ApplicationException("Could not create the MteWasm (mte.ts) module.");
                }
                await module.InvokeVoidAsync("initLicense", _mteRelayOptions.LicensedCompany, _mteRelayOptions.LicenseKey);

                string version = await module.InvokeAsync<string>("getVersion");

                // Console.WriteLine($"MTE Version: {version}");
                if (_mkeDecoderBag.Count == 0)
                {
                    for (int i = 0; i < _mteRelayOptions.NumberOfCachedMtePairs; i++)
                    {
                        await AddAnEmptyDecoderAsync();
                        await AddAnEmptyEncoderAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not setup the java script interop runtime", ex);
            }
        }
        #endregion

        #region AddAnEmptyDecoderAsync
        /// <summary>
        /// Creates an empty MTE Decoder and adds it to
        /// the "bag" for reuse as needed.
        /// </summary>
        /// <returns>Completed Task</returns>
        public async Task AddAnEmptyDecoderAsync()
        {
            try
            {
                var module = await _moduleTaskForMTE.Value;
                var decoder = await module.InvokeAsync<IJSObjectReference>("makeAnEmptyDecoder");
                //
                // Create a Decoder object and add it to the bag.
                //                
                _mkeDecoderBag.Add(decoder);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create and add an MTE Decoder to the internal collection.", ex);
            }
        }
        #endregion

        #region AddAnEmptyEncoderAsync
        /// <summary>
        /// Creates an empty MTE Encoder and adds it to
        /// the "bag" for reuse as needed.
        /// </summary>
        /// <returns>Completed Task</returns>
        public async Task AddAnEmptyEncoderAsync()
        {
            try
            {
                var module = await _moduleTaskForMTE.Value;
                var encoder = await module.InvokeAsync<IJSObjectReference>("makeAnEmptyEncoder");
                //
                // Create an Encoder object and add it to the bag.
                //
                _mkeEncoderBag.Add(encoder);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create and add an MTE Encoder to the internal collection.", ex);
            }
        }
        #endregion

        #region GetAnEncoderAsync
        /// <summary>
        /// Returns an MTE Encoder from the bag, if
        /// none are available, it adds one.
        /// </summary>
        /// <returns>JS Object reference to an MTE decoder.</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<IJSObjectReference> GetAnEncoderAsync()
        {
            try
            {
                if (_mkeEncoderBag.IsEmpty)
                {
                    await AddAnEmptyEncoderAsync();
                }
                if (_mkeEncoderBag.TryTake(out IJSObjectReference? encoder))
                {
                    return encoder;
                }
                throw new ApplicationException("Could not retrieve an encoder from the collection.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region GetADecoderAsync
        /// <summary>
        /// Returns an MTE Decoder from the bag, if
        /// none are available, it adds one.
        /// </summary>
        /// <returns>JS Object reference to an MTE decoder.</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<IJSObjectReference> GetADecoderAsync()
        {
            try
            {
                if (_mkeDecoderBag.IsEmpty)
                {
                    await AddAnEmptyDecoderAsync();
                }
                if (_mkeDecoderBag.TryTake(out IJSObjectReference? decoder))
                {
                    return decoder;
                }
                throw new ApplicationException("Could not retrieve a decoder from the collection.");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region PutTheEncoderAsync
        /// <summary>
        /// Puts an encoder into the "bag" for later reuse.
        /// </summary>
        /// <param name="encoder">The JS Object reference to an encoder.</param>
        /// <returns>Completed Task</returns>
        public void PutTheEncoder(IJSObjectReference encoder)
        {
            _mkeEncoderBag.Add(encoder);
        }
        #endregion

        #region PutTheDecoderAsync
        /// <summary>
        /// Puts a decoder into the "bag" for later reuse.
        /// </summary>
        /// <param name="decoder">The JS Object reference to a decoder.</param>
        /// <returns>Completed Task</returns>
        public void PutTheDecoderAsync(IJSObjectReference decoder)
        {
            _mkeDecoderBag.Add(decoder);
        }
        #endregion

        #region InitializeDecoderAsync
        /// <summary>
        /// Initializes a decoder with the nonce, entropy, and personalization string.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to initialize.</param>
        /// <param name="magicValues">Object with the three initialization values.</param>
        /// <returns>MteStatus</returns>
        public async Task<MteStatus> InitializeDecoderAsync(IJSObjectReference mkeDecoder, MteMagicValues magicValues)
        {
            try
            {
                var module = await _moduleTaskForMTE.Value;
                MteStatus status = await module.InvokeAsync<MteStatus>("initializeDecoder", mkeDecoder, magicValues.Entropy, magicValues.Nonce, magicValues.PersonalizationString);
                return status;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region InitializeEncoderAsync
        /// <summary>
        /// Initializes an decoder with the nonce, entropy, and personalization string.
        /// </summary>
        /// <param name="mkeEncoder">The decoder to initialize.</param>
        /// <param name="magicValues">Object with the three initialization values.</param>
        /// <returns>MteStatus</returns>
        public async Task<MteStatus> InitializeEncoderAsync(IJSObjectReference mkeEncoder, MteMagicValues magicValues)
        {
            try
            {
                var module = await _moduleTaskForMTE.Value;
                MteStatus status = await module.InvokeAsync<MteStatus>("initializeEncoder", mkeEncoder, magicValues.Entropy, magicValues.Nonce, magicValues.PersonalizationString);
                return status;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region RestoreDecoderStateAsync
        /// <summary>
        /// Restores (sets) the state of a decoder with the B-64 representation of an MteState.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to restore.</param>
        /// <param name="state">Base-64 representation of the state.</param>
        /// <returns>MteStatus</returns>
        public async Task<MteStatus> RestoreDecoderStateAsync(IJSObjectReference mkeDecoder, string state)
        {
            var module = await _moduleTaskForMTE.Value;
            return await module.InvokeAsync<MteStatus>("restoreDecoderState", mkeDecoder, state);
        }
        #endregion

        #region RestoreEncoderStateAsync
        /// <summary>
        /// Restores (sets) the state of an encoder with the B-64 representation of an MteState.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to restore.</param>
        /// <param name="state">Base-64 representation of the state.</param>
        /// <returns>MteStatus</returns>
        public async Task<MteStatus> RestoreEncoderStateAsync(IJSObjectReference mkeEncoder, string state)
        {
            var module = await _moduleTaskForMTE.Value;
            return await module.InvokeAsync<MteStatus>("restoreEncoderState", mkeEncoder, state);
        }
        #endregion

        #region RetrieveDecoderStateAsync
        /// <summary>
        /// Retrieves (gets) the state of a decoder and returns it as a B-64 string.
        /// </summary>
        /// <param name="mkeDecoder">The decoder that you wish to work with.</param>
        /// <returns>Base-64 version of the current state.</returns>
        public async Task<string> RetrieveDecoderStateAsync(IJSObjectReference mkeDecoder)
        {
            var module = await _moduleTaskForMTE.Value;
            var theState = await module.InvokeAsync<string>("retrieveDecoderState", mkeDecoder);
            return theState;
        }
        #endregion

        #region RetrieveEncoderStateAsync
        /// <summary>
        /// Retrieves (gets) the state of an encoder and returns it as a B-64 string.
        /// </summary>
        /// <param name="mkeEncoder">The encoder that you wish to work with.</param>
        /// <returns>Base-64 version of the current state.</returns>
        public async Task<string> RetrieveEncoderStateAsync(IJSObjectReference mkeEncoder)
        {
            var module = await _moduleTaskForMTE.Value;
            var theState = await module.InvokeAsync<string>("retrieveEncoderState", mkeEncoder);
            return theState;
        }
        #endregion

        #region DecodeToStringAsync
        /// <summary>
        /// Decodes a B-64 string of an encoded payload.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to use (must match the partner's encoder).</param>
        /// <param name="payload">Base-64 version of an encoded payload.</param>
        /// <returns>The decoded string.</returns>
        public async Task<string> DecodeToStringAsync(IJSObjectReference mkeDecoder, string payload)
        {
            var module = await _moduleTaskForMTE.Value;
            return await module.InvokeAsync<string>("decodeToString", mkeDecoder, payload);
        }
        #endregion

        #region DecodeToByteArrayAsync
        /// <summary>
        /// Decodes a byte array of an encoded payload.
        /// </summary>
        /// <param name="mkeDecoder">The decoder to use (must match the partner's encoder).</param>
        /// <param name="payload">Byte array containing an encoded payload.</param>
        /// <returns>The decoded byte array.</returns>
        public async Task<byte[]> DecodeToByteArrayAsync(IJSObjectReference mkeDecoder, byte[] payload)
        {
            var module = await _moduleTaskForMTE.Value;
            return await module.InvokeAsync<byte[]>("decodeToByteArray", mkeDecoder, payload);
        }
        #endregion

        #region EncodeToStringAsync
        /// <summary>
        /// Encodes a string returning a B-64 version of the encoded encryptedBytes.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to use (must match the partner's decoder).</param>
        /// <param name="payload">A string to encode.</param>
        /// <returns>Base-64 version of the encoded result.</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<string> EncodeToStringAsync(IJSObjectReference mkeEncoder, string payload)
        {
            var module = await _moduleTaskForMTE.Value;
            JsonElement returnValue = await module.InvokeAsync<JsonElement>("encodeToString", mkeEncoder, payload);
            var status = returnValue.GetProperty("status").GetInt32();
            if (status != (int)MteStatus.mte_status_success)
            {
                throw new ApplicationException($"Error encoding string for Mte-Relay: {status}");
            }
            string? str = returnValue.GetProperty("str").GetString();
            return str!;
        }
        #endregion

        #region EncodeAsync - byte[]
        /// <summary>
        /// Encodes a byte array returning an array of the encoded bytes.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to use (must match the partner's decoder).</param>
        /// <param name="payload">A byte array to encode.</param>
        /// <returns>The encoded result.</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<byte[]> EncodeAsync(IJSObjectReference mkeEncoder, byte[] payload)
        {
            var module = await _moduleTaskForMTE.Value;
            var returnValue = await module.InvokeAsync<JsonElement>("encode", mkeEncoder, payload);
            var status = returnValue.GetProperty("status").GetInt32();
            if (status != (int)MteStatus.mte_status_success)
            {
                throw new ApplicationException($"Error encoding byte[] for Mte-Relay: {status}");
            }
            string? str = returnValue.GetProperty("str").GetString();
            byte[] arr = Convert.FromBase64String(str!);
            return arr;
        }
        #endregion

        #region EncodeAsync - string
        /// <summary>
        /// Encodes a string returning an array of the encoded bytes.
        /// </summary>
        /// <param name="mkeEncoder">The encoder to use (must match the partner's decoder).</param>
        /// <param name="payload">A string to encode.</param>
        /// <returns>The encoded result.</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<byte[]> EncodeAsync(IJSObjectReference mkeEncoder, string payload)
        {
            var module = await _moduleTaskForMTE.Value;
            var returnValue = await module.InvokeAsync<JsonElement>("encode", mkeEncoder, payload);
            var status = returnValue.GetProperty("status").GetInt32();
            if (status != (int)MteStatus.mte_status_success)
            {
                throw new ApplicationException($"Error encoding string to byte[] for Mte-Relay: {status}");
            }
            string? str = returnValue.GetProperty("str").GetString();
            byte[] arr = Convert.FromBase64String(str!);
            return arr;
        }
        #endregion

        #region GetStatusDescription
        public async Task<string> GetStatusDescription(MteStatus status)
        {
            var module = await _moduleTaskForMTE.Value;
            string returnValue = await module.InvokeAsync<string>("getStatusDescription", status);
            return returnValue;
        }
        #endregion

        #region StartEncrypt
        public async Task<MteStatus> StartEncrypt(IJSObjectReference mkeEncoder)
        {
            var module = await _moduleTaskForMTE.Value;
            var status = await module.InvokeAsync<Int32>("startEncrypt", mkeEncoder);           
            if (status != (int)MteStatus.mte_status_success)
            {
                throw new ApplicationException($"Error starting the encryptor for Mte-Relay: {status}");
            }
            return (MteStatus)status;
        }
        #endregion

        #region EncryptChunk
        public async Task<byte[]> EncryptChunk(IJSObjectReference mkeEncoder, byte[] clearBytes)
        {
            var module = await _moduleTaskForMTE.Value;
            var str = await module.InvokeAsync<string>("encryptChunk", mkeEncoder, clearBytes);            
            byte[] arr = Convert.FromBase64String(str!);
            return arr;
        }
        #endregion

        #region FinishEncrypt
        public async Task<(MteStatus,byte[])> FinishEncrypt(IJSObjectReference mkeEncoder)
        {
            var module = await _moduleTaskForMTE.Value;
            var returnValue = await module.InvokeAsync<JsonElement>("finishEncrypt", mkeEncoder);            
            var status = returnValue.GetProperty("status").GetInt32();
            if (status != (int)MteStatus.mte_status_success)
            {
                throw new ApplicationException($"Error encoding string to byte[] for Mte-Relay: {status}");
            }
            string? str = returnValue.GetProperty("str").GetString();
            byte[] arr = Convert.FromBase64String(str!);
            return ((MteStatus)status, arr);
        }
        #endregion

        #region StartDecrypt
        public async Task<MteStatus> StartDecrypt(IJSObjectReference mkeDecoder)
        {
            var module = await (_moduleTaskForMTE.Value);
            var status = await module.InvokeAsync<Int32>("startDecrypt", mkeDecoder);
            if (status != (int)MteStatus.mte_status_success)
            {
                throw new ApplicationException($"Error starting the decryptor for Mte-Relay: {status}");
            }
            return (MteStatus)status;
        }
        #endregion

        #region DecryptChunk
        public async Task<byte[]> DecryptChunk(IJSObjectReference mkeDecoder, byte[] encryptedBytes)
        {
            var module = await _moduleTaskForMTE.Value;
            var str = await module.InvokeAsync<string>("decryptChunk", mkeDecoder, encryptedBytes);
            byte[] arr = Convert.FromBase64String(str!);
            return arr;
        }
        #endregion

        #region FinishDecrypt
        public async Task<(MteStatus, byte[])> FinishDecrypt(IJSObjectReference mkeDecoder)
        {
            var module = await _moduleTaskForMTE.Value;
            var returnValue = await module.InvokeAsync<JsonElement>("finishDecrypt", mkeDecoder);
            var status = returnValue.GetProperty("status").GetInt32();
            if (status != (int)MteStatus.mte_status_success)
            {
                throw new ApplicationException($"Error encoding string to byte[] for Mte-Relay: {status}");
            }
            string? str = returnValue.GetProperty("str").GetString();
            byte[] arr = Convert.FromBase64String(str!);
            return ((MteStatus)status, arr);
        }
        #endregion
    }
}

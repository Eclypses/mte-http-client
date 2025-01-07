using Eclypses.MteHttpClient.Models;
using Microsoft.JSInterop;

namespace Eclypses.MteHttpClient
{
    public class KyberFactory : IKyberFactory
    {
        /// <summary>
        /// The java script runtime interop object.
        /// </summary>
        private readonly IJSRuntime _jsRuntime;
        private readonly MteRelayOptions _relayOptions;
        public KyberFactory(IJSRuntime jsRuntime, MteRelayOptions mteRelayOptions)
        {
            _jsRuntime = jsRuntime;
            _relayOptions = mteRelayOptions;
        }
        /// <summary>
        /// Creates an instance of the Kyber Helper for a single pair.
        /// </summary>
        /// <returns>An instance of the Kyber Helper.</returns>
        public IKyberHelper Create()
        {
            return new KyberHelper(_jsRuntime, _relayOptions);
        }
    }
}

using Eclypses.MteHttpClient.Shared;

namespace Eclypses.MteHttpClient.Models
{


    /// <summary>
    /// Options to affect behavior of the Mte-Relay client.
    /// </summary>
    public class MteRelayOptions : MteProtectionOptions
    {
        /// <summary>
        /// If you have configured multiple endpoints that your app communicates with
        /// they are kept in this list.  There will always be at least one.
        /// </summary>
        public List<MteRelayEndpoint>? Endpoints { get; set; } = new List<MteRelayEndpoint>();
        /// <summary>
        /// When this is false, the mterelay-helper.js is loaded from NuGet,
        /// otherwise it is a project reference - used when we need to test the 
        /// MteHttpClient (NuGet does not give this the chance).
        /// </summary>
        public bool IsTestingLocal { get; set; } = false;
        /// <summary>
        /// The company that your MTE is licensed to.
        /// </summary>
        public string? LicensedCompany { get; set; }
        /// <summary>
        /// The MTE license key assigned to you.
        /// </summary>
        public string? LicenseKey { get; set; }
        /// <summary>
        /// The number of MTE Pairs that you wish to work with.
        /// More pairs increases throughput and aids with
        /// asynchronous requests. This is generally higher
        /// than the number of Mte Instances created. It's
        /// default is set in the Constants object.
        /// </summary>
        public int NumberOfConcurrentMteStates { get; set; }
        /// <summary>
        /// The number of MTE pairs to create at initial pairing time.
        /// These are re-hydrated with the "PairId" states before
        /// use. They are pooled and available for concurrent requests.
        /// If they are all "Busy", then a new one is added to the pool.
        /// </summary>
        public int NumberOfCachedMtePairs { get; set; }
        
    }
}

using Eclypses.MteHttpClient.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eclypses.MteHttpClient.Shared
{
    /// <summary>
    /// An extension method for IServiceCollection
    /// to register all items required for the MteHttpClient.
    /// </summary>
    public static class MteHttpExtensions
    {
        /// <summary>
        /// Registers all objects and services required for the MteHttpClient
        /// </summary>
        /// <param name="services">The IServicecollection interface.</param>
        /// <param name="config">The IConfiguration interface.</param>
        /// <returns>IServiceCollection to allow chaining.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public static IServiceCollection UseMteHttp(this IServiceCollection services, IConfiguration config, bool isDevelopment = false)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            //
            // Get the mteRelay section from the appsettings.json file.
            //
            MteRelayOptions? mteRelayOptions = config.GetSection("mteRelay").Get<MteRelayOptions>();
            if (mteRelayOptions is null)
            {
                throw new ApplicationException("Your appsettings must include a properly configured mteRelay section.");
            }
            //
            // Check for required settings.
            //
            if (mteRelayOptions.Endpoints is null || mteRelayOptions.Endpoints.Count < 1)
            {
                throw new ApplicationException("Your appsettings MUST contain the list of identifers and urls for the MteRelay server endpoints.");
            }
            foreach (var endpoint in mteRelayOptions.Endpoints)
            {
                if (string.IsNullOrWhiteSpace(endpoint.HttpClientRelayName))
                {
                    throw new ApplicationException("Your appsettings MUST include an HttpClientRelayName for each endpoint you wish to use.");
                }
                if (string.IsNullOrWhiteSpace(endpoint.MteRelayUrl))
                {
                    throw new ApplicationException("Your appsettings MUST include an MteRelayUrl for each endpoint you wish to use.");
                }
            }

            //
            // For AWS, the license key is hard-coded in the JavaScript file.
            //
            //if (string.IsNullOrWhiteSpace(mteRelayOptions.LicensedCompany))
            //{
            //    throw new ApplicationException("Your appsettings MUST contain the Licensed Company for the Eclypses Mte.");
            //}
            //if (string.IsNullOrWhiteSpace(mteRelayOptions.LicenseKey))
            //{
            //    throw new ApplicationException("Your appsettings MUST contain your License Key for the Eclypses Mte.");
            //}
            //
            // Set default values if missing.
            //
            if (mteRelayOptions.NumberOfCachedMtePairs < 1)
            {
                mteRelayOptions.NumberOfCachedMtePairs = Constants.DEFAULT_NUMBER_OF_MTE_INSTANCES;
            }
            if (mteRelayOptions.NumberOfConcurrentMteStates < 1)
            {
                mteRelayOptions.NumberOfConcurrentMteStates = Constants.DEFAULT_NUMBER_OF_PAIRED_STATES;
            }
            if (mteRelayOptions.HeaderDisposition == 0)
            {
                mteRelayOptions.HeaderDisposition = RelayHeaderDisposition.EncodeNoHeaders;
            }
            //
            // Parse the headers to encode if found in the config.
            //
            GetListOfHeadersToEncode(config, mteRelayOptions);
            //
            // Add the MteRelayOptions object to the services collection
            // for injection later as needed.
            //
            services.AddSingleton(mteRelayOptions);
            //
            // Add the classes that implement the actual methods
            // needed to communicate with the Mte-Relay proxy.
            //
            services.AddHttpClient(); // This does a TryAdd for the HttpClientFactory - so it is safe to use here.
            services.AddSingleton<IMteHttpClient, MteHttpClient>();
            services.AddSingleton<IKyberFactory, KyberFactory>();
            services.AddSingleton<IMteHelperMethods, MteHelperMethods>();

            return services;
        }
        /// <summary>
        /// If the HeadersToEncode contains a list of headers,
        /// add them to the mteRelayOptions object
        /// </summary>
        /// <param name="config"></param>
        /// <param name="mteRelayOptions"></param>
        private static void GetListOfHeadersToEncode(IConfiguration config, MteRelayOptions mteRelayOptions)
        {
            if (mteRelayOptions.HeaderDisposition == RelayHeaderDisposition.EncodeListOfHeaders)
            {
                string? headersToEncodeList = config["mteRelay:HeadersToEncode"];
                if (!string.IsNullOrWhiteSpace(headersToEncodeList))
                {
                    //
                    // The HeadersToEncode are a pipe delimited list
                    // so parse them into the list.
                    //
                    string[] s = headersToEncodeList.Split('|');
                    if (s is not null)
                    {
                        foreach (var item in s)
                        {
                            mteRelayOptions.HeadersToEncode!.Add(item);
                        }
                    }
                }
            }
        }
    }
}

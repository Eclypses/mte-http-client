# Eclypses.MteHttpClient
The Eclypses MTE-Relay is a proxy server that allows you to deploy secure communications between the front end of your application
and your backend API. Eclypses has a java script based component for traditional browser applications, and with the MteHttpClient
it now offers a Blazor component that can be utilized in a .Net 8.0 or higher Blazor Webassembly front end application.  
If your application uses Blazor Server, this is not applicable since traffic between your server and
your web browser is *rendered html* travelling over a web-socket.  You have no code actually running
in your browser, it is all rendered at the server and pushed down to your browser.

## Getting Started
A Blazor application is a SPA application that runs in all modern browsers. Communicating with the API that serves up the data for
your application is accomplished by using the standard *Microsoft.HttpClient*. The *Eclypses.MteHttpClient* is functionally
equivalent to the *Microsoft.HttpClient* for communicating with an API server.  
It utilizes the *Eclypses MTE* to protect each and every payload that is exchanged with your API. This is available as a NuGet
package *Eclypses.MteHttpClient*. This is used with a licensed WASM based bundle of the actual *Eclypses MTE*.  The WASM bundle is available
to all customers of Eclypses and is downloaded from *https://developers.eclypses.com*, the Eclypses Development Portal.  
Once you are licensed and have downloaded the MTE library, to incorporate it into your browser application involves the following steps.
### Install the NuGet package
Install the NuGet package named *Eclypses.MteHttpClient* from nuget.org.  
This includes the *Eclypses.MteHttpClient.Blazor.dll* which contains the JSInterop calls to the actual MTE WASM library as
well as JSInterop calls to the Kyber handshake routines.
### Install the java script files
Obtain the *Mte.ts* file from the Eclypses development portal that is licensed to your organization.
Install this into the root folder of your Blazor application (*wwwroot*). The NuGet package looks for it in that folder.
### Configure your *appsettings*
You need to add a section to your *appsettings.json* file which must be in your *wwwroot*.  If you do not have an *appsettings* file,
create one.  
You must add a section named *mteRelay* with the following required entries:
- *Endpoints* This is an array of endpoints that your app needs to communicate with. See the section below to understand how this is used.
  - *RelayIdentifier* This is the identifying string that tells the request where to go.
  - *MteRelayUrl* This is the url of your *MTE-Relay* proxy server for the designated endpoint.
- *LicensedCompany* This is the company name associated with your MTE library.
- *LicenseKey* This is the actual license key that you obtained when you licensed the MTE library.
- *NumberOfConcurrentMteStates* This informs the MteHttpClient how many concurrent active MTE pairs are to be maintained.
- *NumberOfPooledMtePairs* This informs the MteHttpClient how many MTE pairs are initially constructed. 
- *ShouldProtectUrl* This informs the *MteHttpClient* that the full route should be encoded prior to sending a request to the *Mte-Relay*.
- *HeaderDisposition* This is the scheme you wish to use for protecting your Http headers. Possible values are:
	- *EncodeNoHeaders* None of your Http headers will be encoded except for *Content-type*.
	- *EncodeAllHeaders* All of your Http headers will be encoded.
	- *EncodeListOfHeaders* Only specific Http headers will be encoded.
- *HeadersToEncode* This is a pipe-delimited list of your Http headers that you wish to encode if you have specified *EncodeListOfHeaders*.  

Optionally, you may include the following in the same *Endpoints* section of your *mteRelay*.

- *ApiEchoRoute* This is an optional route on your API server that can be used to verify that your API is available.
If this is ommitted, then no pre-check is done to verify that the API is available.
- *ApiEchoString* This is an optional value that must be contained within the return from your echo route. If it is not present,
it indicates that your API is not responding properly. If this is blank, then any echoed response is considered to be proof that the API is available.

A sample section is included below:
``` json
 "mteRelay": {
    "Endpoints":  [
      {
        "RelayIdentifier": "API1", // A unique "endpoint" identifier that directs which API processes the message.
        "MteRelayUrl": "http://localhost:8080" // URL of the MteRelay proxy server that listens to the RelayIdentifier
        "ApiEchoRoute": "/api/echo/hello", // When the verification service is started, a GET request for this route will be sent to your API server.
        "ApiEchoString": "API1 is Alive!" // The API should return this as part of its echoed response to ensure that we can securely round trip    
      },
      {
        "RelayIdentifier": "API2", // A unique "endpoint" identifier that directs which API processes the message.
        "MteRelayUrl": "http://localhost:8081" // URL of the MteRelay proxy server that listens to the RelayIdentifier
        "ApiEchoRoute": "/api2/echo/hello", // When the verification service is started, a GET request for this route will be sent to your API server.
        "ApiEchoString": "API2 is Alive!" // The API should return this as part of its echoed response to ensure that we can securely round trip    
      }
    ],
    "LicensedCompany": "MyCompany", // MTE License info
    "LicenseKey": "MyLicenseKey", // MTE license Key
    "NumberOfConcurrentMteStates": 7, // The number of concurrent paired MTEs to work with.
    "NumberOfPooledMtePairs": 5, // The number of MtePairs that are pooled for use.
    "ShouldProtectUrl": true, // If true the route to your API will be protected by MTE.
    "HeaderDisposition": "EncodeListOfHeaders", //EncodeAllHeaders, EncodeNoHeaders, EncodeListOfHeaders (include pipe delimited HeadersToEncode)
    "HeadersToEncode": "Authorization|x-MyCustomHeader"    
  }
```
### Include the MteHttpClient
In your *Program.cs* file of your Blazor application, inform the MteHttpClient that you wish to use it.  
Since the extension method to configure the *Mte-Relay* services is an extension of the *IServiceCollection*, the arguments must be properties of your *builder*. The actual code follows:
``` c#
using Eclypses.MteHttpClient.Shared;

//
// Create an MteHttpClient that will direct all traffic through the MteRelay
// server which is configured as a proxy to the application's API. The settings
// must be in an "mteRelay" section in your appsettings.json file.
// 
builder.Services.UseMteHttp(builder.Configuration);
```
## Usage
Once the installation of the *Eclypses.MteHttpClient* is completed and configured, anywhere in your code that you need to communicate with your API, you use the MTE versions of the standard HttpClient async calls.  
The namespace that you must include in any page or module that uses these methods is:
``` c#
using Eclypses.MteHttpClient.Shared;
```
The specific methods that are supported include the following:
### *InitializeAsync*
This is an optional method to allow you to initialize the *MTE Environment* prior to its usage. It returns *true* if successful which can allow your application to ensure everything is ready
prior to continuing.  If you do not call this optional method, the first time you use the *MteHttpClient*, the environment will call if for you.  

You may wish to call this prior to displaying your first page (like *login*), so in your *OnInitializedAsync* method of your first page, include this:
``` c#
private string _error;
[Inject] IMteHttpClient? _mteHttpClient { get; set; }

if (await _mteHttpClient.InitializeAsync())
{
    Console.WriteLine("The MteRelay has been paired and initialized - Application is ready to go!");
}
else
{
    _error = "Could not pair with the MteRelay - perhaps the relay service is not running.";
}
```
### *MteGetAsync*
``` c#
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
```
#### Examples
Following are some examples for a standard *GET* request.  Other requests
follow this same pattern.
``` c#
using Eclypses.MteHttpClient.Shared; // Include the namespace for the MteHttpClient objects.
. . . . . . . 
private readonly IMteHttpClient _mteHttpClient; // Inject an MteHttpClient into this class.
. . . . . . . 
// Get some data from the "api/getdata" route.
httpResponseMessage = await _mteHttpClient.MteGetAsync("api/getdata");
// Get some data from the "api/getdata" route of the connection named "MyAPI".
httpResponseMessage = await _mteHttpClient.MteGetAsync("api/getdata", relayIdentifier:"MyAPI");
// Get some data from the "api/getdata" route of the connection named "MyAPI" and retry 3 times before failing.
httpResponseMessage = await _mteHttpClient.MteGetAsync("api/getdata", relayIdentifier:"MyAPI", getAttempts:3);
// Get some data from the "api/getdata" route leaving the route in plain text.
var po = new MteProtectionOptions { ShouldProtectUrl=false };
httpResponseMessage = await _mteHttpClient.MteGetAsync("api/getdata", protectionOptions: po);
```
### *MteGetByteArrayAsync*
``` c#
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
```
### *MteGetStringAsync*
``` c#
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
```

### *MtePostAsync*
``` c#
/// <summary>
/// Asynchronously POST an HttpContent payload to your API protecting it with the Eclypses MKE.
/// </summary>
/// <param name="route">The route you wish to POST to on your API.</param>
/// <param name="headers">Any headers that you wish to include in your POST request.</param>
/// <param name="content">HttpContent for your POST request.</param>
/// <param name="protectionOptions">Optional override options to manage protection of URL and Headers.</param>
/// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
/// <param name="postAttempts">Number of times you wish to retry if a POST fails.</param>
/// <returns>HttpResponseMessage from your POST request.</returns>
Task<HttpResponseMessage> MtePostAsync(
    string route, 
    HttpContent? content, 
    Dictionary<string, string>? headers = null, 
    string? relayIdentifier = "",
    MteProtectionOptions? protectionOptions = null,
    int postAttempts = 0);
```
### *MtePutAsync*
``` c#
/// <summary>
/// Asynchronously PUT an HttpContent payload to your API protecting it with the Eclypses MKE.
/// </summary>
/// <param name="route">The route you wish to PUT to on your API.</param>
/// <param name="headers">Any headers that you wish to include in your PUT request.</param>
/// <param name="content">HttpContent for your PUT request.</param>
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
```
### *MtePatchAsync*
``` c#
/// <summary>
/// Asynchronously PATCH an HttpContent payload to your API protecting it with the Eclypses MKE.
/// </summary>
/// <param name="route">The route you wish to PATCH to on your API.</param>
/// <param name="headers">Any headers that you wish to include in your PATCH request.</param>
/// <param name="content">HttpContent for your PATCH request.</param>
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
```
### MteProtectionOptions
The *appsettings.json* file in your *wwwroot* folder sets the default protection options for your web page.
However, you may wish to override these for a specific *MteHttpRequest*.  
An optional override object can be included in each individual request by including
an instance of the *MteProtectionOptions*.  This class is detailed below:
``` c#
public class MteProtectionOptions
{
    /// <summary>
    /// If true, the destinaiton Url (Route) is encoded for a specific request.
    /// </summary>
    public bool? ShouldProtectUrl { get; set; } = null; 
    /// <summary>
    /// Based on this enum, specific headers will be encoded for this specific request
    /// into the x-mte-relay-eh header (Note Content-type is always encoded).
    /// </summary>
    public RelayHeaderDisposition HeaderDisposition { get; set; } = RelayHeaderDisposition.Unknown;
    /// <summary>
    /// If HeaderDisposition is EncodeListOfHeaders, then this is 
    /// the list that will be encoded for this specific request.
    /// NOTE: the Content-Type header is always encoded.
    /// </summary>
    public List<string>? HeadersToEncode { get; set; } = null;    
}
```


### *SetAuthenticationHeader*
This is a method of the *MteHttpClient* that creates a standard *AuthenticationHeader* to be sent through the *Mte-Relay* proxy to your application. It is protected based on your configured *HeaderDisposition*. There is an overload that allows you to not identify the specific endpoint identifier.  If that is not present, the first identifier in your configured list is used.
``` c#
/// <summary>
/// May be used as a convenience method to add an Authentication header for a specific relay identifier.
/// </summary>
/// <param name="relayIdentifier">The identifier for the endpoint you configured in your appsettings.</param>
/// <param name="scheme">The authentication scheme such as 'basic' or 'bearer'.</param>
/// <param name="value">The actual value for the authentication token.</param>
void SetAuthenticationHeader(string relayIdentifier, string scheme, string value);
/// <summary>
/// May be used as a convenience method to add an Authentication header for
/// the first (and possibly only) relay identifier in your list.
/// </summary>
/// <param name="scheme">The authentication scheme such as 'basic' or 'bearer'.</param>
/// <param name="value">The actual value for the authentication token.</param>
void SetAuthenticationHeader(string scheme, string value);
``` 
A code snippet to use this with a Jwt returned from an authentication route follows:
``` c#
_mteHttpClient.SetAuthenticationHeader("API1", "bearer", "someAuthenticationJWTReturnedFromMyAPI1");
```
### *SetDefaultRequestHeader*
A standard *HttpClient* has a property to set Default Request Headers that are included in every request. The *SetDefaultRequestHeader* method functions in much the same way. Any header added in this way is included in every Http request to the Mte-Relay for the specified endpoint identifier and subsequently to your API.  Is is protected based on your configured *HeaderDisposition*.
``` c#
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
/// <param name="key">The key for this speicific header.</param>
/// <param name="value">The value for this specific header.</param>
void SetDefaultRequestHeader(string key, string value = "");
```
``` c#
_mteHttpClient.SetDefaultRequestHeader("API2", "x-custom-header", "This is a default header for all requests for API2");
```
### *Endpoints*
Your *Blazor* application may communicate with multiple endpoints which *Blazor* identifies through different *BaseAddress* properties. The *Mte-Relay* is paired with a single base address, so if your application requires multiple API endpoints, you must use multiple *Mte-Relay* services.  The *Microsoft.HttpClient* allows for *named* HttpClients which are identified with a simple string.  The *Eclypses.MteHttpClient* uses the named clients as its actual connection, so the *relayIdentifier* is the actual name of the specific client associated with the *Mte-Relay* that services the specific API.  
In your *appsettings.json* file of your *Blazor* application, the array of endpoints designate which APIs you wish to send your requests to. 
If you leave the *relayIdentifier* blank on your actual request, the first (and possibly only) *API* that you configured will receive the request. So, if you only have one *API* that your application communicates with, put that single entry in your *Endpoints* array in the *appsettings* and leave the *relayIdentifier* blank in your method calls.
## Additional documentation
This is the Blazor implementation of the client side of your secure system. It communicates
to your API by proxy through the Mte-Relay server. Documentation
describing the Mte-Relay component is found at:  
https://docs.eclypses.com/docs/current/implementation-helpers/mte-relay/overview  
It can be installed from:  
https://www.npmjs.com/package/mte-relay-server  
If you wish to use the java script version of the client, see this link:  
https://www.npmjs.com/package/mte-relay-browser  
Further information regarding named clients is found at:  
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0#named-clients


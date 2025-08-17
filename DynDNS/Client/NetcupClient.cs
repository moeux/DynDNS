using System.Net.Http.Json;
using System.Text.Json;
using DynDNS.Models.Dns;
using DynDNS.Models.Request;
using DynDNS.Models.Response;

namespace DynDNS.Client;

public static class NetcupClient
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri(Environment.GetEnvironmentVariable("NETCUP_BASE_ADDRESS") ??
                              throw new ArgumentNullException())
    };

    private static readonly Dictionary<string, string> AuthDictionary = new()
    {
        ["customernumber"] = Environment.GetEnvironmentVariable("NETCUP_CUSTOMER_NUMBER") ??
                             throw new ArgumentNullException(),
        ["apikey"] = Environment.GetEnvironmentVariable("NETCUP_API_KEY") ?? throw new ArgumentNullException(),
        ["apipassword"] = Environment.GetEnvironmentVariable("NETCUP_API_PASSWORD") ?? throw new ArgumentNullException()
    };

    public static async Task LoginAsync()
    {
        var loginRequest = new LoginRequest
        {
            Parameters =
            {
                ["customernumber"] = AuthDictionary["customernumber"],
                ["apikey"] = AuthDictionary["apikey"],
                ["apipassword"] = AuthDictionary["apipassword"]
            }
        };

        var response = await Client.PostAsJsonAsync(string.Empty, loginRequest, JsonSerializerOptions.Web);

        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<Response>();

        if (loginResponse?.Status != Status.Success) throw new HttpRequestException("Login unsuccessful");

        AuthDictionary["apisessionid"] = loginResponse.Data["apisessionid"];

        Console.WriteLine(loginResponse.LongMessage ?? loginResponse.ShortMessage);
    }

    public static async Task LogoutAsync()
    {
        var logoutRequest = new LogoutRequest
        {
            Parameters =
            {
                ["customernumber"] = AuthDictionary["customernumber"],
                ["apikey"] = AuthDictionary["apikey"],
                ["apisessionid"] = AuthDictionary["apisessionid"]
            }
        };

        var response = await Client.PostAsJsonAsync(string.Empty, logoutRequest, JsonSerializerOptions.Web);

        response.EnsureSuccessStatusCode();

        var logoutResponse = await response.Content.ReadFromJsonAsync<Response>();

        if (logoutResponse?.Status != Status.Success) throw new HttpRequestException("Logout unsuccessful");

        Console.WriteLine(logoutResponse.LongMessage ?? logoutResponse.ShortMessage);
    }

    public static async Task UpdateDnsRecordsAsync(string domainName, params DnsRecord[] dnsRecords)
    {
        if (dnsRecords.Any(dnsRecord =>
                string.IsNullOrWhiteSpace(dnsRecord.Priority) && dnsRecord.Type == RecordType.MX))
            throw new ArgumentException("MX DNS records must have priority set");

        var dnsRecordSet = new Dictionary<string, object>
        {
            ["dnsrecords"] = dnsRecords
        };
        var updateDnsRecordsRequest = new UpdateDnsRecordsRequest
        {
            Parameters =
            {
                ["domainname"] = domainName,
                ["dnsrecordset"] = dnsRecordSet,
                ["customernumber"] = AuthDictionary["customernumber"],
                ["apikey"] = AuthDictionary["apikey"],
                ["apisessionid"] = AuthDictionary["apisessionid"]
            }
        };

        var response = await Client.PostAsJsonAsync(string.Empty, updateDnsRecordsRequest, JsonSerializerOptions.Web);

        response.EnsureSuccessStatusCode();

        var updateDnsRecordsResponse = await response.Content.ReadFromJsonAsync<Response>();

        if (updateDnsRecordsResponse?.Status != Status.Success)
            throw new HttpRequestException("UpdateDnsRecords unsuccessful");

        Console.WriteLine(updateDnsRecordsResponse.LongMessage ?? updateDnsRecordsResponse.ShortMessage);
    }

    public static async Task<string> GetIpAddress()
    {
        return await Client.GetStringAsync("https://api.ipify.org");
    }
}